﻿using System.Diagnostics;

using Cuture.Http.Test.Server;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Cuture.Http.Test;

/// <summary>
/// host webapi服务的测试基类
/// </summary>
public abstract class WebServerHostTestBase
{
    #region 字段

    protected IHost ServerHost = null;

    #endregion 字段

    #region Protected 属性

    protected virtual bool UseTestServer { get; } = true;

    #endregion Protected 属性

    #region 方法

    [TestCleanup]
    public virtual async Task CleanupAsync()
    {
        if (TestWebHost.HostByTestHost)
        {
            await ServerHost?.StopAsync();
        }
        HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool.Dispose();
    }

    [TestInitialize]
    public virtual async Task InitAsync()
    {
        IHttpMessageInvokerPool invokerPool = null;
        if (TestWebHost.HostByTestHost)
        {
            var hostBuilder = TestWebHost.CreateHostBuilder([], UseTestServer);
            ServerHost = await hostBuilder.StartAsync();
            if (UseTestServer)
            {
                invokerPool = new TestHttpMessageInvokerPool(ServerHost.GetTestServer().CreateHandler());
            }
        }

        invokerPool ??= new SimpleHttpMessageInvokerPool();

        HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool = invokerPool;
    }

    /// <summary>
    /// 获取一个大小为 <paramref name="count"/> 的数组
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected static int[] Array(int count) => new int[count];

    /// <summary>
    /// 并行请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestCount">请求总数</param>
    /// <param name="getRequestFunc">获取请求的方法</param>
    /// <param name="assertAction">对每个请求的断言委托</param>
    /// <returns></returns>
    protected virtual async Task ParallelRequestAsync<T>(int requestCount,
                                                         Func<Task<HttpOperationResult<T>>> getRequestFunc,
                                                         Action<HttpOperationResult<T>> assertAction)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(requestCount, 1);
        ArgumentNullException.ThrowIfNull(getRequestFunc);
        ArgumentNullException.ThrowIfNull(assertAction);

        Debug.WriteLine($"Start Request, Count: {requestCount}");

        var sw = Stopwatch.StartNew();

        var tasks = Array(requestCount).Select(m => getRequestFunc()).ToList();

        await Task.WhenAll(tasks);

        sw.Stop();

        Debug.WriteLine($"Total Time: {sw.Elapsed.TotalSeconds} s");

        tasks.ForEach(m =>
        {
            assertAction(m.Result);
        });
    }

    /// <summary>
    /// 并行请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestCount">请求总数</param>
    /// <param name="getRequestFunc">获取请求的方法</param>
    /// <param name="assertAction">对每个请求的断言委托</param>
    /// <returns></returns>
    protected virtual async Task ParallelRequestAsync<T>(int requestCount,
                                                         Func<Task<TextHttpOperationResult<T>>> getRequestFunc,
                                                         Action<TextHttpOperationResult<T>> assertAction)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(requestCount, 1);
        ArgumentNullException.ThrowIfNull(getRequestFunc);
        ArgumentNullException.ThrowIfNull(assertAction);

        Debug.WriteLine($"Start Request, Count: {requestCount}");

        var sw = Stopwatch.StartNew();

        var tasks = Array(requestCount).Select(m => getRequestFunc()).ToList();

        await Task.WhenAll(tasks);

        sw.Stop();

        Debug.WriteLine($"Total Time: {sw.Elapsed.TotalSeconds} s");

        tasks.ForEach(m =>
        {
            assertAction(m.Result);
        });
    }

    protected bool UserAgentEquals(string ua1, string ua2)
    {
        Assert.AreEqual(ua1.Length, ua2.Length);

        for (int i = 0; i < ua1.Length; i++)
        {
            var ua1Char = ua1[i];
            var ua2Char = ua2[i];
            if (ua1Char == ua2Char
                || (ua1Char == ' ' && ua2Char == ',')
                || (ua1Char == ',' && ua2Char == ' '))
            {
                continue;
            }
            return false;
        }
        return true;
    }

    #endregion 方法
}
