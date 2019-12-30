using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cuture.Http.Test.Server
{
    public class Startup
    {
        #region 字段

        private static readonly Random s_random = new Random();

        #endregion 字段

        #region 属性

        public IConfiguration Configuration { get; }

        #endregion 属性

        #region 构造函数

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion 构造函数

        #region 方法

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")    //首页
                {
                    await context.Response.WriteAsync(Resource.Index);
                }
                else if (context.Request.Path == "/data.dat")   //慢速文件下载
                {
                    var data = Resource.Data;

                    context.Response.ContentType = "application/data";
                    context.Response.ContentLength = data.Length;

                    var md = new Memory<byte>(data);
                    var maxSpanLength = 51200;
                    var minSpanLength = maxSpanLength / 2;

                    var maxRandomIndex = data.Length - maxSpanLength;
                    var index = 0;

                    for (; index < maxRandomIndex;)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5));
                        var randomSize = s_random.Next(minSpanLength, maxSpanLength);
                        await context.Response.BodyWriter.WriteAsync(md.Slice(index, randomSize));
                        index += randomSize;
                    }
                    await context.Response.BodyWriter.WriteAsync(md.Slice(index, data.Length - index));
                }
                else
                {
                    if (context.Request.Method == "CUSTOM") //自定义Method
                    {
                        context.Request.Method = "POST";
                    }
                    await next();
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        #endregion 方法
    }
}