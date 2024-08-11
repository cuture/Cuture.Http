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
        #region Private 字段

        private static readonly Random s_random = new Random();

        #endregion Private 字段

        #region Public 方法

        public IConfiguration Configuration { get; }

        #endregion Public 方法

        #region Public 构造函数

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion Public 构造函数

        #region Public 方法

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var data = Resource.Data;

            app.Use(async (context, next) =>
            {
                var cancellationToken = context.RequestAborted;

                if (context.Request.Path == "/")
                {
                    await context.Response.WriteAsync(Resource.Index, cancellationToken);
                }
                else if (context.Request.Path == "/data.dat")
                {
                    context.Response.ContentType = "application/data";
                    context.Response.ContentLength = data.Length;

                    var md = new Memory<byte>(data);
                    var maxSpanLength = 51200;
                    var minSpanLength = maxSpanLength / 2;

                    var maxRandomIndex = data.Length - maxSpanLength;
                    var index = 0;

                    for (; index < maxRandomIndex;)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
                        var randomSize = s_random.Next(minSpanLength, maxSpanLength);
                        await context.Response.BodyWriter.WriteAsync(md.Slice(index, randomSize), cancellationToken);
                        index += randomSize;
                    }
                    await context.Response.BodyWriter.WriteAsync(md.Slice(index, data.Length - index), cancellationToken);
                }
                else
                {
                    if (context.Request.Method == "CUSTOM") //�Զ���Method
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
            services.AddControllers().AddNewtonsoftJson();
        }

        #endregion Public 方法
    }
}
