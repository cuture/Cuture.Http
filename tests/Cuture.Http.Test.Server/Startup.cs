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
        #region �ֶ�

        private static readonly Random s_random = new Random();

        #endregion �ֶ�

        #region ����

        public IConfiguration Configuration { get; }

        #endregion ����

        #region ���캯��

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion ���캯��

        #region ����

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

                if (context.Request.Path == "/")    //��ҳ
                {
                    await context.Response.WriteAsync(Resource.Index, cancellationToken);
                }
                else if (context.Request.Path == "/data.dat")   //�����ļ�����
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

        #endregion ����
    }
}
