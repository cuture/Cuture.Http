using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Internal;

namespace Cuture.Http
{
    /// <summary>
    /// 简单实现的 <see cref="IHttpTurboClientFactory"/>
    /// </summary>
    public sealed class SimpleHttpTurboClientFactory : IHttpTurboClientFactory
    {
        #region 字段

        /// <summary>
        /// 默认持有客户端的时长
        /// </summary>
        public const int DefaultHoldMinute = 5;

        private readonly TimeSpan? _holdTimeSpan;

        /// <summary>
        /// 是否保持所有Client的引用
        /// </summary>
        private readonly bool _isHoldClient;

        /// <summary>
        /// client释放队列
        /// </summary>
        private SortedQueue<CreatedTimeTagedObject<WeakReference<IHttpTurboClient>>>? _clientReleaseQueue;

        /// <summary>
        /// HoldClient的集合
        /// </summary>
        private HashSet<IHttpTurboClient>? _holdedClients;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed;

        private CancellationTokenSource? _releaseTokenSource;

        #region Client

        /// <summary>
        /// 默认的 <see cref="IHttpTurboClient"/>
        /// </summary>
        private WeakReference<IHttpTurboClient> _client = new WeakReference<IHttpTurboClient>(null!);

        /// <summary>
        /// 禁用Proxy的 <see cref="IHttpTurboClient"/>
        /// </summary>
        private WeakReference<IHttpTurboClient> _directlyClient = new WeakReference<IHttpTurboClient>(null!);

        #endregion Client

        #region Client Dictionary

        /// <summary>
        /// 有代理信息的 <see cref="IHttpTurboClient"/>
        /// </summary>
        private Lazy<ConcurrentDictionary<int, WeakReference<IHttpTurboClient>>> _proxyClients = new Lazy<ConcurrentDictionary<int, WeakReference<IHttpTurboClient>>>();

        /// 有代理信息的 <see cref="IHttpTurboClient"/>
        public ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> ProxyClients { get => _proxyClients.Value; }

        #endregion Client Dictionary

        #endregion 字段

        #region Public 构造函数

        /// <summary>
        /// 简单实现的 <see cref="IHttpTurboClientFactory"/>
        /// </summary>
        /// <param name="holdClient">保持内部所有 <see cref="IHttpTurboClient"/> 的引用，不被GC回收
        /// <para/>
        /// (非永久持有，以 holdTimeSpan 参数控制最短持有时长，使其仍被释放，以应对DNS变更)</param>
        /// <param name="holdTimeSpan">保持引用的时长，不传递时为默认值 <see cref="DefaultHoldMinute" /></param>
        public SimpleHttpTurboClientFactory(bool holdClient = true, TimeSpan? holdTimeSpan = null)
        {
            _isHoldClient = holdClient;
            if (holdClient)
            {
                _holdedClients = new HashSet<IHttpTurboClient>();
                _clientReleaseQueue = new SortedQueue<CreatedTimeTagedObject<WeakReference<IHttpTurboClient>>>();
                _holdTimeSpan = holdTimeSpan.HasValue
                                    ? holdTimeSpan.Value.TotalSeconds > 0
                                        ? holdTimeSpan
                                        : throw new ArgumentOutOfRangeException(nameof(holdTimeSpan))
                                    : TimeSpan.FromMinutes(DefaultHoldMinute);

                InitAutoReleaseTask();
            }
        }

        #endregion Public 构造函数

        #region 方法

        /// <summary>
        /// 清空所有的Client缓存
        /// </summary>
        public void Clear()
        {
            CheckDisposed();

            ReleaseWeakReferenceClient(_client);
            ReleaseWeakReferenceClient(_directlyClient);

            if (_proxyClients.IsValueCreated)
            {
                var proxyClients = ProxyClients.Values;

                ProxyClients.Clear();

                DisposeClients(proxyClients);
            }

            if (_clientReleaseQueue != null)
            {
                lock (_clientReleaseQueue)
                {
                    _clientReleaseQueue.Clear();
                    _holdedClients?.Clear();
                }
            }
        }

        /// <summary>
        /// 销毁相关资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _clientReleaseQueue = null;
            _holdedClients = null;

            ReleaseWeakReferenceClient(_client);
            ReleaseWeakReferenceClient(_directlyClient);

            _client = null!;
            _directlyClient = null!;

            if (_proxyClients.IsValueCreated)
            {
                var proxyClients = ProxyClients;
                _proxyClients = null!;

                DisposeClients(proxyClients.Values);
            }

            CancelAutoReleaseTask();
        }

        /// <summary>
        /// 通过Uri判定获取一个Client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IHttpTurboClient GetTurboClient(IHttpRequest request)
        {
            CheckDisposed();

            IHttpTurboClient turboClient;

            if (request.DisableProxy)
            {
                turboClient = GetClientInWeakReference(_directlyClient, () => new HttpTurboClient(HttpTurboClient.CreateDefaultClientHandler().DisableProxy()));
            }
            else
            {
                if (request.Proxy is null)
                {
                    turboClient = GetClientInWeakReference(_client, () => new HttpTurboClient());
                }
                else
                {
                    turboClient = GetProxyClientInWeakReferenceDictionary(request, ProxyClients, proxy =>
                    {
                        var httpClientHandler = HttpTurboClient.CreateDefaultClientHandler();
                        httpClientHandler.UseProxy = true;
                        httpClientHandler.Proxy = request.Proxy;
                        return new HttpTurboClient(httpClientHandler);
                    });
                }
            }
            return turboClient;
        }

        /// <summary>
        /// 销毁
        /// <see cref="IHttpTurboClient"/>
        /// 弱引用列表
        /// </summary>
        /// <param name="turboClients"></param>
        private static void DisposeClients(IEnumerable<WeakReference<IHttpTurboClient>> turboClients)
        {
            foreach (var item in turboClients)
            {
                ReleaseWeakReferenceClient(item);
            }
        }

        /// <summary>
        /// 从弱引用中释放
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="turboClientWR"></param>
        private static void ReleaseWeakReferenceClient(WeakReference<IHttpTurboClient> turboClientWR)
        {
            if (turboClientWR.TryGetTarget(out var client))
            {
                Debug.WriteLine($"Dispose {nameof(IHttpTurboClient)}:{client.GetHashCode()}");

                turboClientWR.SetTarget(null!);
                client.Dispose();
            }
        }

        private void CancelAutoReleaseTask()
        {
            if (_releaseTokenSource != null)
            {
                _releaseTokenSource.Cancel(true);
                _releaseTokenSource.Dispose();
                _releaseTokenSource = null;
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(string.Empty);
            }
        }

        /// <summary>
        /// 从弱引用中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="weakReference"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private IHttpTurboClient GetClientInWeakReference(WeakReference<IHttpTurboClient> weakReference,
                                                          Func<IHttpTurboClient> createFunc)
        {
            if (!weakReference.TryGetTarget(out IHttpTurboClient? turboClient))
            {
                lock (weakReference)
                {
                    if (!weakReference.TryGetTarget(out turboClient))
                    {
                        turboClient = createFunc();

                        Debug.WriteLine($"new HttpTurboClient:{turboClient.GetHashCode()}");

                        weakReference.SetTarget(turboClient);

                        HoldClient(weakReference);
                    }
                }
            }

            return turboClient;
        }

        /// <summary>
        /// 从弱引用字典中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="weakReferenceDictionary"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private IHttpTurboClient GetProxyClientInWeakReferenceDictionary(IHttpRequest request,
                                                                         ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> weakReferenceDictionary,
                                                                         Func<IWebProxy, IHttpTurboClient> createFunc)
        {
            IHttpTurboClient client;

            //HACK 此处是否可能有问题?
            var proxy = request.Proxy!;
            int proxyHash = -1261813833;

            if (proxy is WebProxy webProxy)
            {
                proxyHash = proxyHash * -1521134295 + webProxy.Address!.OriginalString
#if NETCOREAPP
                                                        .GetHashCode(StringComparison.Ordinal);
#else
                                                        .GetHashCode();
#endif
            }
            else
            {
                var proxyUri = proxy.GetProxy(request.RequestUri);
                if (proxyUri is null)
                {
                    return GetClientInWeakReference(_client, () => new HttpTurboClient());
                }
                proxyHash = proxyHash * -1521134295 + proxyUri.OriginalString
#if NETCOREAPP
                                                        .GetHashCode(StringComparison.Ordinal);
#else
                                                        .GetHashCode();
#endif
            }

            if (proxy.Credentials?.GetCredential(request.RequestUri, string.Empty) is NetworkCredential credential)
            {
#if NETCOREAPP
                proxyHash = proxyHash * -1521134295 + credential.UserName.GetHashCode(StringComparison.Ordinal);
                proxyHash = proxyHash * -1521134295 + credential.Password.GetHashCode(StringComparison.Ordinal);
                proxyHash = proxyHash * -1521134295 + credential.Domain.GetHashCode(StringComparison.Ordinal);
#else
                proxyHash = proxyHash * -1521134295 + credential.UserName.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Password.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Domain.GetHashCode();
#endif
            }

            if (!weakReferenceDictionary.TryGetValue(proxyHash, out WeakReference<IHttpTurboClient>? clientWR))
            {
                lock (weakReferenceDictionary)
                {
                    if (!weakReferenceDictionary.TryGetValue(proxyHash, out clientWR))
                    {
                        clientWR = new WeakReference<IHttpTurboClient>(null!);
                        client = GetClientInWeakReference(clientWR, () => createFunc(proxy));

                        Debug.WriteLine($"new HttpTurboClient:{client.GetHashCode()} ProxyHash:{proxyHash}");

                        weakReferenceDictionary.AddOrUpdate(proxyHash, clientWR, (k, oldClientWR) =>
                        {
                            ReleaseWeakReferenceClient(oldClientWR);
                            return clientWR;
                        });
                    }
                    else
                    {
                        client = GetClientInWeakReference(clientWR, () => createFunc(proxy));
                    }
                }
            }
            else
            {
                client = GetClientInWeakReference(clientWR, () => createFunc(proxy));
            }

            return client;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HoldClient(WeakReference<IHttpTurboClient> clientWR)
        {
            if (_isHoldClient)
            {
                if (clientWR.TryGetTarget(out var client))
                {
                    lock (_clientReleaseQueue!)
                    {
                        _holdedClients!.Add(client);
                        _clientReleaseQueue.Enqueue(new CreatedTimeTagedObject<WeakReference<IHttpTurboClient>>(clientWR, DateTime.UtcNow));
                    }
                }
            }
        }

        private void InitAutoReleaseTask()
        {
            if (!_isHoldClient)
            {
                return;
            }

            CancelAutoReleaseTask();

            var tokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var token = tokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    if (_clientReleaseQueue!.Peek() is CreatedTimeTagedObject<WeakReference<IHttpTurboClient>> next)
                    {
                        var now = DateTime.UtcNow;
                        var expire = next.CreatedTime.Add(_holdTimeSpan!.Value);
                        if (expire <= now)
                        {
                            lock (_clientReleaseQueue)
                            {
                                next = _clientReleaseQueue.Dequeue()!;
                            }
                            if (next != null
                                && next.Data.TryGetTarget(out var client))
                            {
                                _holdedClients!.Remove(client);
                                //释放引用，等待GC回收
                                next.Data.SetTarget(null!);
                            }
                        }
                        else
                        {
                            await Task.Delay(expire - now, token).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await Task.Delay(_holdTimeSpan!.Value, token).ConfigureAwait(false);
                    }
                }
            }, tokenSource.Token);

            _releaseTokenSource = tokenSource;
        }

#endregion 方法
    }
}