using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace Cuture.Http.Test;

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
    public const string ThroughProxy = "Through-Proxy";
    private readonly ExplicitProxyEndPoint _explicitProxy = new ExplicitProxyEndPoint(IPAddress.Loopback, ProxyPort, false);

    private readonly ProxyServer _proxyServer = new ProxyServer(false, false, false)
    {
        ThreadPoolWorkerThread = 3,
        MaxCachedConnections = 3,
        ReuseSocket = true,
        TcpTimeWaitSeconds = 30,
    };

    public bool IsSystemProxy { get; set; } = false;

    public ProxyAuthenticateInfo SystemProxyInfo { get; set; }

    #endregion 字段

    #region 属性

    public Dictionary<string, ProxyAuthenticateInfo> Authenticates { get; } = new Dictionary<string, ProxyAuthenticateInfo>();

    #endregion 属性

    #region 方法

    public void DisableSystemProxy()
    {
        _proxyServer.DisableSystemProxy(ProxyProtocolType.Http);
        _proxyServer.ProxyBasicAuthenticateFunc = BasicAuthenticate;
        IsSystemProxy = false;
        Debug.WriteLine(nameof(DisableSystemProxy));
    }

    public void SetAsSystemProxy()
    {
        SystemProxyInfo = new ProxyAuthenticateInfo();
        _proxyServer.SetAsSystemProxy(_explicitProxy, ProxyProtocolType.Http);
        _proxyServer.ProxyBasicAuthenticateFunc = null;
        IsSystemProxy = true;
        Debug.WriteLine(nameof(SetAsSystemProxy));
    }

    public void StartProxyServer()
    {
        _proxyServer.AddEndPoint(_explicitProxy);
        _proxyServer.ProxyBasicAuthenticateFunc = BasicAuthenticate;
        _proxyServer.BeforeRequest += BeforeRequest;
        _proxyServer.BeforeResponse += BeforeResponse;
        _proxyServer.Start();
        Debug.WriteLine(nameof(StartProxyServer));
    }

    public void StopProxyServer()
    {
        _proxyServer.Stop();
        Debug.WriteLine(nameof(StopProxyServer));
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
        if (IsSystemProxy)
        {
            if (e.HttpClient.Request.Url.StartsWith(TestWebHost.TestHost))
            {
                lock (SystemProxyInfo)
                {
                    SystemProxyInfo.RequestTime++;
                }
            }
            return Task.CompletedTask;
        }

        if (e.HttpClient.Request.Headers.Headers.ContainsKey("Proxy-Authorization"))
        {
            var authorizationHeader = e.HttpClient.Request.Headers.Headers["Proxy-Authorization"].Value;
            var authorizationBase64String = authorizationHeader.Split(' ')[1];
            var authorization = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationBase64String));
            var authorizations = authorization.Split(':');
            var username = authorizations[0];
            var password = authorizations[1];

            if (Authenticates.ContainsKey(username)
                && Authenticates[username] is ProxyAuthenticateInfo accountInfo
                && password.Equals(accountInfo.Password))
            {
                lock (accountInfo)
                {
                    accountInfo.RequestTime++;
                }
                return Task.CompletedTask;
            }
        }

        e.Respond(new Response() { StatusCode = 401 }, false);

        return Task.CompletedTask;
    }

    private Task BeforeResponse(object sender, SessionEventArgs e)
    {
        e.HttpClient.Response.Headers.AddHeader(ThroughProxy, "1");
        return Task.CompletedTask;
    }

    #endregion 方法
}
