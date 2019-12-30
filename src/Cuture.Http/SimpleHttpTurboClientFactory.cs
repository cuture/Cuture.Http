using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// 简单的HttpTurboClientFactory
    /// </summary>
    public sealed class SimpleHttpTurboClientFactory : IHttpTurboClientFactory
    {
        #region 字段

        /// <summary>
        /// 允许自动重定向的代理的HttpTurboClient
        /// </summary>
        private ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> _allowRedirectionProxyTurboClients = new ConcurrentDictionary<int, WeakReference<IHttpTurboClient>>();

        /// <summary>
        /// 允许自动重定向的HttpTurboClient
        /// </summary>
        private WeakReference<IHttpTurboClient> _allowRedirectionTurboClient = new WeakReference<IHttpTurboClient>(null);

        /// <summary>
        ///
        /// </summary>
        private ConcurrentBag<IHttpTurboClient> _holdedClients = new ConcurrentBag<IHttpTurboClient>();

        /// <summary>
        /// 是否保持所有Client的引用
        /// </summary>
        private bool _isHoldClient = true;

        /// <summary>
        /// 代理的HttpTurboClient
        /// </summary>
        private ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> _proxyTurboClients = new ConcurrentDictionary<int, WeakReference<IHttpTurboClient>>();

        /// <summary>
        /// HttpTurboClient
        /// </summary>
        private WeakReference<IHttpTurboClient> _turboClient = new WeakReference<IHttpTurboClient>(null);

        #endregion 字段

        #region 方法

        /// <summary>
        /// 清空所有的HttpClient缓存
        /// </summary>
        public void Clear()
        {
            var allAllowRedirectionProxyTurboClients = _allowRedirectionProxyTurboClients.Values;
            var allProxyTurboClients = _proxyTurboClients.Values;

            _allowRedirectionProxyTurboClients.Clear();
            _proxyTurboClients.Clear();

            ReleaseWeakReferenceClient(_allowRedirectionTurboClient);
            ReleaseWeakReferenceClient(_turboClient);

            DisposeClients(allAllowRedirectionProxyTurboClients);
            DisposeClients(allProxyTurboClients);

            _holdedClients = new ConcurrentBag<IHttpTurboClient>();
        }

        /// <summary>
        /// 销毁相关资源
        /// </summary>
        public void Dispose()
        {
            _holdedClients = null;

            var allowRedirectionProxyTurboClients = _allowRedirectionProxyTurboClients;
            var proxyTurboClients = _proxyTurboClients;

            ReleaseWeakReferenceClient(_allowRedirectionTurboClient);
            ReleaseWeakReferenceClient(_turboClient);

            _allowRedirectionProxyTurboClients = null;
            _allowRedirectionTurboClient = null;
            _proxyTurboClients = null;
            _turboClient = null;

            DisposeClients(allowRedirectionProxyTurboClients.Values);
            DisposeClients(proxyTurboClients.Values);
        }

        /// <summary>
        /// 通过Uri判定获取一个HttpTurboClient
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IHttpTurboClient GetTurboClient(IHttpTurboRequest request)
        {
            IHttpTurboClient turboClient = null;

            if (request.AllowRedirection)
            {
                if (request.Proxy is null)
                {
                    turboClient = GetTurboClientInWeakReference(_allowRedirectionTurboClient, () => HoldClient(new HttpTurboClient(true)));
                }
                else
                {
                    turboClient = GetProxyTurboClientInWeakReferenceDictionary(request, _allowRedirectionProxyTurboClients, proxy =>
                    {
                        var httpClientHandler = HttpTurboClient.CreateDefaultAllowRedirectionClientHandler();
                        httpClientHandler.UseProxy = true;
                        httpClientHandler.Proxy = proxy;
                        return HoldClient(new HttpTurboClient(httpClientHandler));
                    });
                }
            }
            else
            {
                if (request.Proxy is null)
                {
                    turboClient = GetTurboClientInWeakReference(_turboClient, () => HoldClient(new HttpTurboClient(false)));
                }
                else
                {
                    turboClient = GetProxyTurboClientInWeakReferenceDictionary(request, _proxyTurboClients, proxy =>
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
        /// 从弱引用字典中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="weakReferenceDictionary"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private static IHttpTurboClient GetProxyTurboClientInWeakReferenceDictionary(IHttpTurboRequest request,
                                                                                     ConcurrentDictionary<int, WeakReference<IHttpTurboClient>> weakReferenceDictionary,
                                                                                     Func<IWebProxy, IHttpTurboClient> createFunc)
        {
            IHttpTurboClient turboClient = null;

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
                proxyHash = proxyHash * -1521134295 + proxyUri.OriginalString.GetHashCode();
            }

            if (proxy.Credentials?.GetCredential(request.RequestUri, string.Empty) is NetworkCredential credential)
            {
                proxyHash = proxyHash * -1521134295 + credential.UserName.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Password.GetHashCode();
                proxyHash = proxyHash * -1521134295 + credential.Domain.GetHashCode();
            }

#pragma warning disable CA2000 // 丢失范围之前释放对象

            if (!weakReferenceDictionary.TryGetValue(proxyHash, out WeakReference<IHttpTurboClient> turboClientWR))
            {
                lock (weakReferenceDictionary)
                {
                    if (!weakReferenceDictionary.TryGetValue(proxyHash, out turboClientWR))
                    {
                        turboClient = createFunc(proxy);
                        turboClientWR = new WeakReference<IHttpTurboClient>(turboClient);
#if DEBUG
                        Debug.WriteLine($"new HttpTurboClient:{turboClient.GetHashCode()} ProxyHash:{proxyHash} AllowRedirection:{request.AllowRedirection}");
#endif
                        weakReferenceDictionary.AddOrUpdate(proxyHash, turboClientWR, (k, oldClientWR) =>
                        {
                            ReleaseWeakReferenceClient(oldClientWR);
                            return turboClientWR;
                        });
                    }
                    else
                    {
                        turboClient = GetTurboClientInWeakReference(turboClientWR, () => createFunc(proxy));
                    }
                }
            }
            else
            {
                turboClient = GetTurboClientInWeakReference(turboClientWR, () => createFunc(proxy));
            }
#pragma warning restore CA2000 // 丢失范围之前释放对象

            return turboClient;
        }

        /// <summary>
        /// 从弱引用中获取
        /// <see cref="IHttpTurboClient"/>
        /// </summary>
        /// <param name="weakReference"></param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        private static IHttpTurboClient GetTurboClientInWeakReference(WeakReference<IHttpTurboClient> weakReference,
                                                                      Func<IHttpTurboClient> createFunc)
        {
            if (!weakReference.TryGetTarget(out IHttpTurboClient turboClient))
            {
                lock (weakReference)
                {
                    if (!weakReference.TryGetTarget(out turboClient))
                    {
                        turboClient = createFunc();
#if DEBUG
                        Debug.WriteLine($"new HttpTurboClient:{turboClient.GetHashCode()}");
#endif
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
#if DEBUG
                Debug.WriteLine($"Dispose {nameof(IHttpTurboClient)}:{client.GetHashCode()}");
#endif
                client.Dispose();
            }
            turboClientWR.SetTarget(null);
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