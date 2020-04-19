using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    /// <summary>
    /// host webapi服务的测试基类
    /// </summary>
    public abstract class WebServerHostTestBase
    {
        #region 字段

        protected IHost ServerHost = null;

        #endregion 字段

        #region 方法

        [TestCleanup]
        public virtual async Task CleanupAsync()
        {
            if (TestServer.HostByTestHost)
            {
                await ServerHost?.StopAsync();
            }
            HttpRequestOptions.DefaultTurboClientFactory.Clear();
        }

        [TestInitialize]
        public virtual async Task InitAsync()
        {
            if (TestServer.HostByTestHost)
            {
                ServerHost = await TestServer.CreateHostBuilder(new string[0]).StartAsync();
            }
            HttpRequestOptions.DefaultConnectionLimit = 200;
        }

        /// <summary>
        /// 获取一个大小为 <paramref name="count"/> 的数组
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected int[] Array(int count) => new int[count];

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
            if (requestCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(requestCount));
            }

            if (getRequestFunc is null)
            {
                throw new ArgumentNullException(nameof(getRequestFunc));
            }

            if (assertAction is null)
            {
                throw new ArgumentNullException(nameof(assertAction));
            }

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
            if (requestCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(requestCount));
            }

            if (getRequestFunc is null)
            {
                throw new ArgumentNullException(nameof(getRequestFunc));
            }

            if (assertAction is null)
            {
                throw new ArgumentNullException(nameof(assertAction));
            }

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

        #endregion 方法
    }
}