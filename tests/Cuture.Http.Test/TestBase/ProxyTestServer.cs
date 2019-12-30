using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Cuture.Http.Test
{
    public class ProxyAuthenticateInfo
    {
        #region 属性

        public int ConnectTime { get; set; } = 0;

        public string Password { get; set; } = string.Empty;

        public int RequestTime { get; set; } = 0;

        public string UserName { get; set; } = string.Empty;

        #endregion 属性
    }

    /// <summary>
    /// 代理服务器
    /// </summary>
    public class ProxyTestServer
    {
        #region 字段

        public const int ProxyPort = 3277;
        private readonly ProxyServer _proxyServer = new ProxyServer(false, false, false)
        {
            ThreadPoolWorkerThread = 100,
            MaxCachedConnections = 100,
        };

        #endregion 字段

        #region 属性

        public Dictionary<string, ProxyAuthenticateInfo> Authenticates { get; } = new Dictionary<string, ProxyAuthenticateInfo>();

        #endregion 属性

        #region 方法

        public void StartProxyServer()
        {
            _proxyServer.AddEndPoint(new ExplicitProxyEndPoint(IPAddress.Loopback, ProxyPort, false));
            _proxyServer.ProxyBasicAuthenticateFunc = BasicAuthenticate;
            _proxyServer.BeforeRequest += BeforeRequest;
            _proxyServer.Start();
        }

        public void StopProxyServer()
        {
            _proxyServer.Stop();
        }

        private async Task<bool> BasicAuthenticate(SessionEventArgsBase session, string username, string password)
        {
            await Task.CompletedTask;
            if (Authenticates.TryGetValue(username, out var accountInfo))
            {
                if (password.Equals(accountInfo.Password))
                {
                    lock (accountInfo)
                    {
                        accountInfo.ConnectTime++;
                    }
                    return true;
                }
            }

            Assert.Fail();

            return false;
        }

        private Task BeforeRequest(object sender, SessionEventArgs e)
        {
            var authorizationHeader = e.HttpClient.Request.Headers.Headers["Proxy-Authorization"].Value;
            var authorizationBase64String = authorizationHeader.Split(' ')[1];
            var authorization = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationBase64String));
            var authorizations = authorization.Split(':');
            var username = authorizations[0];
            var password = authorizations[1];

            var accountInfo = Authenticates[username];
            if (password.Equals(accountInfo.Password))
            {
                lock (accountInfo)
                {
                    accountInfo.RequestTime++;
                }
            }

            Assert.Fail();

            return Task.CompletedTask;
        }

        #endregion 方法
    }
}