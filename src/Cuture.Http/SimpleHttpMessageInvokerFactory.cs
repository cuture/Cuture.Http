using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http;

/// <summary>
/// 简单实现的 <see cref="IHttpMessageInvokerFactory"/>
/// </summary>
public sealed class SimpleHttpMessageInvokerFactory : IHttpMessageInvokerFactory
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
    private SortedQueue<CreatedTimeTagedObject<WeakReference<HttpClient>>>? _clientReleaseQueue;

    /// <summary>
    /// HoldClient的集合
    /// </summary>
    private HashSet<HttpClient>? _holdedClients;

    /// <summary>
    /// 是否已释放
    /// </summary>
    private bool _isDisposed;

    private CancellationTokenSource? _releaseTokenSource;

    #region Client

    /// <summary>
    /// 默认的 <see cref="HttpClient"/>
    /// </summary>
    private WeakReference<HttpClient> _client = new(null!);

    /// <summary>
    /// 禁用Proxy的 <see cref="HttpClient"/>
    /// </summary>
    private WeakReference<HttpClient> _directlyClient = new(null!);

    #endregion Client

    #region Client Dictionary

    /// <summary>
    /// 有代理信息的 <see cref="HttpClient"/>
    /// </summary>
    private Lazy<ConcurrentDictionary<int, WeakReference<HttpClient>>> _proxyClients = new();

    /// 有代理信息的 <see cref="HttpClient"/>
    public ConcurrentDictionary<int, WeakReference<HttpClient>> ProxyClients { get => _proxyClients.Value; }

    #endregion Client Dictionary

    #endregion 字段

    #region Public 构造函数

    /// <summary>
    /// 简单实现的 <see cref="IHttpMessageInvokerFactory"/>
    /// </summary>
    /// <param name="holdClient">保持内部所有 <see cref="HttpClient"/> 的引用，不被GC回收
    /// <para/>
    /// (非永久持有，以 holdTimeSpan 参数控制最短持有时长，使其仍被释放，以应对DNS变更)</param>
    /// <param name="holdTimeSpan">保持引用的时长，不传递时为默认值 <see cref="DefaultHoldMinute" /></param>
    public SimpleHttpMessageInvokerFactory(bool holdClient = true, TimeSpan? holdTimeSpan = null)
    {
        _isHoldClient = holdClient;
        if (holdClient)
        {
            _holdedClients = new HashSet<HttpClient>();
            _clientReleaseQueue = new SortedQueue<CreatedTimeTagedObject<WeakReference<HttpClient>>>();
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

    /// <inheritdoc/>
    public HttpMessageInvoker GetInvoker(IHttpRequest request)
    {
        CheckDisposed();

        HttpClient httpClient;

        if (request.DisableProxy)
        {
            httpClient = GetClientInWeakReference(_directlyClient, () => HttpClientUtil.CreateProxyDisabledDefaultClient());
        }
        else
        {
            if (request.Proxy is null)
            {
                httpClient = GetClientInWeakReference(_client, () => HttpClientUtil.CreateDefaultClient());
            }
            else
            {
                httpClient = GetProxyClientInWeakReferenceDictionary(request, ProxyClients, proxy => HttpClientUtil.CreateProxyedClient(proxy));
            }
        }
        return httpClient;
    }

    /// <summary>
    /// 销毁 <see cref="HttpClient"/> 弱引用列表
    /// </summary>
    /// <param name="httpClients"></param>
    private static void DisposeClients(IEnumerable<WeakReference<HttpClient>> httpClients)
    {
        foreach (var item in httpClients)
        {
            ReleaseWeakReferenceClient(item);
        }
    }

    /// <summary>
    /// 从弱引用中释放 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="httpClientWR"></param>
    private static void ReleaseWeakReferenceClient(WeakReference<HttpClient> httpClientWR)
    {
        if (httpClientWR.TryGetTarget(out var client))
        {
            Debug.WriteLine($"Dispose {nameof(HttpClient)}:{client.GetHashCode()}");

            httpClientWR.SetTarget(null!);
            client.Dispose();
        }
    }

    private void CancelAutoReleaseTask()
    {
        if (_releaseTokenSource != null)
        {
            try
            {
                _releaseTokenSource.Cancel();
            }
            catch { }
            finally
            {
                _releaseTokenSource.Dispose();
            }
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
    /// 从弱引用中获取 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="weakReference"></param>
    /// <param name="createFunc"></param>
    /// <returns></returns>
    private HttpClient GetClientInWeakReference(WeakReference<HttpClient> weakReference,
                                                Func<HttpClient> createFunc)
    {
        if (!weakReference.TryGetTarget(out HttpClient? httpClient))
        {
            lock (weakReference)
            {
                if (!weakReference.TryGetTarget(out httpClient))
                {
                    httpClient = createFunc();

                    Debug.WriteLine($"new HttpClient:{httpClient.GetHashCode()}");

                    weakReference.SetTarget(httpClient);

                    HoldClient(weakReference);
                }
            }
        }

        return httpClient;
    }

    /// <summary>
    /// 从弱引用字典中获取 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="weakReferenceDictionary"></param>
    /// <param name="createFunc"></param>
    /// <returns></returns>
    private HttpClient GetProxyClientInWeakReferenceDictionary(IHttpRequest request,
                                                               ConcurrentDictionary<int, WeakReference<HttpClient>> weakReferenceDictionary,
                                                               Func<IWebProxy, HttpClient> createFunc)
    {
        HttpClient client;

        //HACK 此处是否可能有问题?
        var proxy = request.Proxy!;
        int proxyHash = -1261813833;

        if (proxy is WebProxy webProxy)
        {
            proxyHash = proxyHash * -1521134295 + webProxy.Address!.OriginalString.GetHashCode(StringComparison.Ordinal);
        }
        else
        {
            var proxyUri = proxy.GetProxy(request.RequestUri);
            if (proxyUri is null)
            {
                return GetClientInWeakReference(_client, () => HttpClientUtil.CreateDefaultClient());
            }
            proxyHash = proxyHash * -1521134295 + proxyUri.OriginalString.GetHashCode(StringComparison.Ordinal);
        }

        if (proxy.Credentials?.GetCredential(request.RequestUri, string.Empty) is NetworkCredential credential)
        {
            proxyHash = proxyHash * -1521134295 + credential.UserName.GetHashCode(StringComparison.Ordinal);
            proxyHash = proxyHash * -1521134295 + credential.Password.GetHashCode(StringComparison.Ordinal);
            proxyHash = proxyHash * -1521134295 + credential.Domain.GetHashCode(StringComparison.Ordinal);
        }

        if (!weakReferenceDictionary.TryGetValue(proxyHash, out WeakReference<HttpClient>? clientWR))
        {
            lock (weakReferenceDictionary)
            {
                if (!weakReferenceDictionary.TryGetValue(proxyHash, out clientWR))
                {
                    clientWR = new WeakReference<HttpClient>(null!);
                    client = GetClientInWeakReference(clientWR, () => createFunc(proxy));

                    Debug.WriteLine($"new HttpClient:{client.GetHashCode()} ProxyHash:{proxyHash}");

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
    private void HoldClient(WeakReference<HttpClient> clientWR)
    {
        if (_isHoldClient)
        {
            if (clientWR.TryGetTarget(out var client))
            {
                lock (_clientReleaseQueue!)
                {
                    _holdedClients!.Add(client);
                    _clientReleaseQueue.Enqueue(new CreatedTimeTagedObject<WeakReference<HttpClient>>(clientWR, DateTime.UtcNow));
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
                if (_clientReleaseQueue!.Peek() is CreatedTimeTagedObject<WeakReference<HttpClient>> next)
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
