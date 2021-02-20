namespace Cuture.Http.Extensions
{
    /// <summary>
    /// <see cref="HttpRequestOptions"/>的 Newtonsoft.Json 配置拓展
    /// </summary>
    public static class HttpRequestOptionsNewtonsoftJsonExtensions
    {
        #region 静态构造函数

        static HttpRequestOptionsNewtonsoftJsonExtensions()
        {
            if (HttpRequestOptions.DefaultJsonSerializer is null)
            {
                HttpRequestOptions.DefaultJsonSerializer = new NewtonsoftJsonJsonSerializer();
            }

            if (HttpRequestOptions.DefaultFormDataFormatter is null)
            {
                HttpRequestOptions.DefaultFormDataFormatter = new NewtonsoftJsonFormDataFormatter();
            }
        }

        #endregion 静态构造函数

        #region Public 方法

        /// <summary>
        /// 将基于 Newtonsoft.Json 实现的<see cref="IJsonSerializer"/>、<see cref="IFormDataFormatter"/>设置为<see cref="HttpRequestOptions"/>的全局默认值
        /// </summary>
        public static void SetNewtonsoftJsonAsDefault()
        {
            HttpRequestOptions.Default.UseNewtonsoftJson();

            HttpRequestOptions.DefaultJsonSerializer = new NewtonsoftJsonJsonSerializer();
            HttpRequestOptions.DefaultFormDataFormatter = new NewtonsoftJsonFormDataFormatter();
        }

        /// <summary>
        /// 使用基于 Newtonsoft.Json 实现的序列化器
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static HttpRequestOptions UseNewtonsoftJson(this HttpRequestOptions options)
        {
            options.JsonSerializer = new NewtonsoftJsonJsonSerializer();
            return options;
        }

        #endregion Public 方法
    }
}