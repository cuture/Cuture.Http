﻿using System;
using System.Net.Http;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="HttpMessageInvoker"/> 工厂
    /// </summary>
    public interface IHttpMessageInvokerFactory : IDisposable
    {
        #region 方法

        /// <summary>
        /// 获取 <paramref name="request"/> 对应使用的 <see cref="HttpMessageInvoker"/>
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        HttpMessageInvoker GetInvoker(IHttpRequest request);

        #endregion 方法
    }
}