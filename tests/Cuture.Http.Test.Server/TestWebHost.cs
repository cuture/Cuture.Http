using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Cuture.Http.Test.Server
{
    public class TestWebHost
    {
        #region Private 字段

        public static bool HostByTestHost = true;

        public static string TestHost = $"http://{Resource.ServerAddress}:{Resource.ServerPort}";

        #endregion Private 字段

        #region Public 方法

        public static IHostBuilder CreateHostBuilder(string[] args, bool useTestServer) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (useTestServer)
                    {
                        webBuilder.UseTestServer();
                    }
                    webBuilder.UseStartup<Startup>()
                              .UseUrls(TestHost);
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args, false).Build().Run();
        }

        #endregion Public 方法
    }
}
