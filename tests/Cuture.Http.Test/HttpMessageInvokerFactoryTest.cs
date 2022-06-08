using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public abstract class HttpMessageInvokerFactoryTest<T> where T : IHttpMessageInvokerPool
{
    #region Private 字段

    protected const int TypeCount = 6;

    #endregion Private 字段

    #region 字段

    protected T Pool;

    #endregion 字段

    #region 方法

    [TestCleanup]
    public void Cleanup()
    {
        Pool.Dispose();
    }

    [TestMethod]
    public void GetClient()
    {
        var count = 1000;
        var url = "http://127.0.0.1/index";
        var hashSet = new HashSet<int>();

        #region 普通请求

        var request = url.CreateHttpRequest();
        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #region 允许重定向请求

        request = url.CreateHttpRequest().AutoRedirection();

        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #endregion 允许重定向请求

        #endregion 普通请求

        #region 代理请求

        request = url.CreateHttpRequest()
                     .UseProxy("http://127.0.0.1:8000");
        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #region 代理允许重定向请求

        request = url.CreateHttpRequest()
             .UseProxy("http://127.0.0.1:8000")
             .AutoRedirection();

        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #endregion 代理允许重定向请求

        #endregion 代理请求

        #region 有验证的代理请求

        request = url.CreateHttpRequest()
                     .UseProxy(new WebProxy("http://127.0.0.1:8000")
                     {
                         Credentials = new NetworkCredential("proxy_user_name", "proxy_password")
                     });
        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #region 有验证的代理允许重定向请求

        request = url.CreateHttpRequest()
                     .AutoRedirection()
                     .UseProxy(new WebProxy("http://127.0.0.1:8000")
                     {
                         Credentials = new NetworkCredential("proxy_user_name", "proxy_password")
                     });

        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #endregion 有验证的代理允许重定向请求

        #endregion 有验证的代理请求

        #region 有验证的代理请求2

        request = url.CreateHttpRequest()
                     .UseProxy(new WebProxy("http://127.0.0.1:8000")
                     {
                         Credentials = new NetworkCredential("proxy_user_name2", "proxy_password2")
                     });
        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #region 有验证的代理允许重定向请求2

        request = url.CreateHttpRequest()
                     .AutoRedirection()
                     .UseProxy(new WebProxy("http://127.0.0.1:8000")
                     {
                         Credentials = new NetworkCredential("proxy_user_name2", "proxy_password2")
                     });

        for (int i = 0; i < count; i++)
        {
            using var owner = Pool.Rent(request);
            hashSet.Add(owner.Value.GetHashCode());
        }

        #endregion 有验证的代理允许重定向请求2

        #endregion 有验证的代理请求2

        Assert.AreEqual(4, hashSet.Count);
    }

    [TestInitialize]
    public void Init()
    {
        Pool = CreateFactory();
    }

    [TestMethod]
    public void ParallelGetClient()
    {
        for (int type = 0; type < TypeCount; type++)
        {
            var count = InternalParallelGetClient(1_000_000, type);
            Assert.AreEqual(1, count, $"类型：{type}失败");
        }

        var randomCount = InternalParallelGetClient(1_000_000, -1);
        Assert.AreEqual(4, randomCount, $"随机类型失败");
    }

    [TestMethod]
    public void ParallelGetProxyClient()
    {
        int count = 10_000;

        var proxies = new WebProxy[] {
            new WebProxy("http://127.0.0.1:3277"), //1
            new WebProxy("http://127.0.0.1:3277"){ Credentials = new NetworkCredential(){ UserName="anonymous" } }, //1
            new WebProxy("http://127.0.0.2:3277"){ Credentials = new NetworkCredential(){ UserName="anonymous" } }, //1
            new WebProxy("http://127.0.0.3:3277"){ Credentials = new NetworkCredential(){ UserName="anonymous" } }, //1
            new WebProxy("http://127.0.0.1:3277"){ Credentials = new NetworkCredential(){ UserName="uesr1",Password="psw" } },  //1
            new WebProxy("http://127.0.0.1:3277"){ Credentials = new NetworkCredential(){ UserName="uesr2",Password="psw" } },  //1
            new WebProxy("http://127.0.0.2:3277"){ Credentials = new NetworkCredential(){ UserName="uesr1",Password="psw" } },  //1
        };

        //预热，避免并发执行导致创建多个
        for (int i = 0; i < proxies.Length; i++)
        {
            LocalRentInvoker(i % proxies.Length, new());
        }

        var hashSet = new HashSet<object>();

        Parallel.For(0, count, i =>
        {
            LocalRentInvoker(i % proxies.Length, hashSet);
        });

        Assert.AreEqual(7, hashSet.Count);

        void LocalRentInvoker(int index, HashSet<object> hashSet)
        {
            var proxy = proxies[index];
            var request = "http://127.0.0.1/index".CreateHttpRequest().UseProxy(proxy);
            using var owner = Pool.Rent(request);

            lock (hashSet)
            {
                hashSet.Add(owner.Value);
            }
        }
    }

    protected abstract T CreateFactory();

    private int InternalParallelGetClient(int count, int type)
    {
        var urls = new string[] {
            "http://127.0.0.1/index",
            "http://127.0.0.2/index",
            "https://127.0.0.3/index",
            "https://127.0.0.4/index",
            "https://127.0.0.1/index",
            "https://127.0.0.2/index",
        };

        //预热，避免并发执行导致创建多个
        for (int i = 0; i < TypeCount; i++)
        {
            LocalRentInvoker(i % TypeCount, new());
        }

        var hashSet = new HashSet<object>();

        Parallel.For(0, count, i =>
        {
            LocalRentInvoker(type == -1 ? i % TypeCount : type, hashSet);
        });

        return hashSet.Count;

        void LocalRentInvoker(int index, HashSet<object> hashSet)
        {
            var request = urls[index].CreateHttpRequest();
            switch (index)
            {
                case 1:
                    request.UseProxy("http://127.0.0.1:8000");
                    break;

                case 2:
                    request.AutoRedirection();
                    break;

                case 3:
                    request.UseProxy("http://127.0.0.1:8000")
                           .AutoRedirection();
                    break;

                case 4:
                    request.UseProxy(new WebProxy("http://127.0.0.1:8000")
                    {
                        Credentials = new NetworkCredential("proxy_user_name", "proxy_password")
                    });
                    break;

                case 5:
                    request.AutoRedirection()
                           .UseProxy(new WebProxy("http://127.0.0.1:8000")
                           {
                               Credentials = new NetworkCredential("proxy_user_name2", "proxy_password2")
                           });
                    break;

                case 0:
                default:
                    break;
            }
            using var owner = Pool.Rent(request);

            lock (hashSet)
            {
                hashSet.Add(owner.Value);
            }
        }
    }

    #endregion 方法
}
