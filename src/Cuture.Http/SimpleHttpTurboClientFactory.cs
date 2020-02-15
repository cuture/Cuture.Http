using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// 简单的ClientFactory
    /// </summary>
    public sealed class SimpleHttpTurboClientFactory : IHttpTurboClientFactory
    {
        #region 字段

        /// <summary>
        ///
        /// </summary>
        private ConcurrentBag<IHttpTurboClient> _holdedClients = new ConcurrentBag<IHttpTurboClient>();

        /// <summary>
        /// 是否保持所有Client的引用
        /// </summary>
        private bool _isHoldClient = true;

        #region Client

        /// <summary>
        /// 默认的Client
        /// </summary>
        private WeakReference<IHttpTurboClient> _client = new WeakReference<IHttpTurboClient>(null);

        /// <summary>
        /// 禁用Proxy的Client
        /// </summary>
        private WeakReference<IHttpTurboClient> _directlyClient = new WeakReference<IHttpTurboClient>(null);

        #endregion Client

        #region Client Dictionary

        /// <summary>
        /// 有代理信息的Client
        /// </summary>
        private ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> _proxyClients = new ConcurrentDictionary<int, WeakReference<IHttpTurboClient>>();

        #endregion Client Dictionary

        #endregion 字段

        #region 方法

        /// <summary>
        /// 清空所有的Client缓存
        /// </summary>
        public void Clear()
        {
            var proxyClients = _proxyClients.Values;

            _proxyClients.Clear();

            ReleaseWeakReferenceClient(_client);
            ReleaseWeakReferenceClient(_directlyClient);

            DisposeClients(proxyClients);

            _holdedClients = new ConcurrentBag<IHttpTurboClient>();
        }

        /// <summary>
        /// 销毁相关资源
        /// </summary>
        public void Dispose()
        {
            _holdedClients = null;

            var proxyClients = _proxyClients;

            ReleaseWeakReferenceClient(_client);
            ReleaseWeakReferenceClient(_directlyClient);

            _proxyClients = null;
            _client = null;
            _directlyClient = null;

            DisposeClients(proxyClients.Values);
        }

        /// <summary>
        /// 通过Uri判定获取一个Client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IHttpTurboClient GetTurboClient(IHttpTurboRequest request)
        {
            IHttpTurboClient turboClient = null;

            if (request.DisableProxy)
            {
                turboClient = GetClientInWeakReference(_directlyClient, () => HoldClient(new HttpTurboClient(HttpTurboClient.CreateDefaultClientHandler().DisableProxy())));
            }
            else
            {
                if (request.Proxy is null)
                {
                    turboClient = GetClientInWeakReference(_client, () => HoldClient(new HttpTurboClient()));
                }
                else
                {
                    turboClient = GetProxyClientInWeakReferenceDictionary(request, _proxyClients, proxy =>
                    {
                        var httpClientHandler = HttpTurboClient.CreateDefaultClientHandler();
                        httpClientHandler.UseProxy = true;
                        httpClientHandler.Proxy = request.Proxy;
                        return HoldClient(new HttpTurboClient(httpClientHandler));
                    });
                }
            }
            return turboClient;
        }

        /// <summary>
        /// 保持所有Client的引用,不被释放
        /// </summary>
        /// <param name="hold">是否保持</param>
        public void HoldAllClient(bool hold)
        {
            if (!hold)
            {
                _holdedClients = new ConcurrentBag<IHttpTurboClient>();
            }
            _isHoldClient = hold;
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
        /// 从弱引用中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="weakReference"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private static IHttpTurboClient GetClientInWeakReference(WeakReference<IHttpTurboClient> weakReference,
                                                                 Func<IHttpTurboClient> createFunc)
        {
            if (!weakReference.TryGetTarget(out IHttpTurboClient turboClient))
            {
                lock (weakReference)
                {
                    if (!weakReference.TryGetTarget(out turboClient))
                    {
                        turboClient = createFunc();

                        Debug.WriteLine($"new HttpTurboClient:{turboClient.GetHashCode()}");

                        weakReference.SetTarget(turboClient);
                    }
                }
            }

            return turboClient;
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

                client.Dispose();
            }
            turboClientWR.SetTarget(null);
        }

        /// <summary>
        /// 从弱引用字典中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="weakReferenceDictionary"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private IHttpTurboClient GetProxyClientInWeakReferenceDictionary(IHttpTurboRequest request,
                                                                         ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> weakReferenceDictionary,
                                                                         Func<IWebProxy, IHttpTurboClient> createFunc)
        {
            IHttpTurboClient client = null;

            //HACK 此处是否可能有问题?
            var proxy = request.Proxy;
            int proxyHash = -1261813833;

            if (proxy is WebProxy webProxy)
            {
                proxyHash = proxyHash * -1521134295 + webProxy.Address.OriginalString.GetHashCode();
            }
            else
            {
                var proxyUri = proxy.GetProxy(request.RequestUri);
                if (proxyUri is null)
                {
                    return GetClientInWeakReference(_client, () => HoldClient(new HttpTurboClient()));
                }
                proxyHash = proxyHash * -1521134295 + proxyUri.OriginalString.GetHashCode();
            }

            if (proxy.Credentials?.GetCredential(request.RequestUri, string.Empty) is NetworkCredential credential)
            {
                proxyHash = proxyHash * -1521134295 + credential.UserName.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Password.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Domain.GetHashCode();
            }

            if (!weakReferenceDictionary.TryGetValue(proxyHash, out WeakReference<IHttpTurboClient> clientWR))
            {
                lock (weakReferenceDictionary)
                {
                    if (!weakReferenceDictionary.TryGetValue(proxyHash, out clientWR))
                    {
                        client = createFunc(proxy);
                        clientWR = new WeakReference<IHttpTurboClient>(client);

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
        private IHttpTurboClient HoldClient(IHttpTurboClient client)
        {
            if (_isHoldClient)
            {
                _holdedClients.Add(client);
            }
            return client;
        }

        #endregion 方法
    }
}