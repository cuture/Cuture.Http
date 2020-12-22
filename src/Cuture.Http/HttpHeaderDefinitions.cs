namespace Cuture.Http
{
    /// <summary>
    /// http标头定义
    /// </summary>
    public static class HttpHeaderDefinitions
    {
        //  (^[a-z0-9]+)\s+\d+[ \r\n\t]+([a-z0-9-]+)(.+)
        //  /// <summary>\n/// $2$3\n/// </summary>\npublic const string $1 = "$2";

        #region HttpRequestHeader

        /// <summary>
        /// Accept 标头，指定响应可接受的 MIME 类型。
        /// </summary>
        public const string Accept = "Accept";

        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的字符集。
        /// </summary>
        public const string AcceptCharset = "Accept-Charset";

        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的内容编码。
        /// </summary>
        public const string AcceptEncoding = "Accept-Charset";

        /// <summary>
        /// Accept-Langauge 标头，指定用于响应的首选自然语言。
        /// </summary>
        public const string AcceptLanguage = "Accept-Langauge";

        /// <summary>
        /// Allow 标头，指定支持的 HTTP 方法集。
        /// </summary>
        public const string Allow = "Allow";

        /// <summary>
        /// Authorization 标头，指定客户端提供的以向服务器验证自身身份的凭据。
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// Cache-Control 标头，指定请求/响应链上所有缓存控制机制必须服从的指令。
        /// </summary>
        public const string CacheControl = "Cache-Control";

        /// <summary>
        /// Connection 标头，指定特定连接所需的选项。
        /// </summary>
        public const string Connection = "Connection";

        /// <summary>
        /// Content-Encoding 标头，指定应用到随附的正文数据的编码。
        /// </summary>
        public const string ContentEncoding = "Content-Encoding";

        /// <summary>
        /// Content-Langauge 标头，指定随附的正文数据的自然语言。
        /// </summary>
        public const string ContentLanguage = "Content-Langauge";

        /// <summary>
        /// Content-Length 标头，指定随附的正文数据的长度（以字节为单位）。
        /// </summary>
        public const string ContentLength = "Content-Length";

        /// <summary>
        /// Content-Location 标头，指定可从中获取随附的正文的 URI。
        /// </summary>
        public const string ContentLocation = "Content-Location";

        /// <summary>
        /// Content-MD5 标头，指定随附的正文数据的 MD5 摘要，以便提供端到端消息完整性检查。 由于 MD5 出现冲突问题，Microsoft 建议使用基于 SHA256 或更高版本的安全模型。
        /// </summary>
        public const string ContentMd5 = "Content-MD5";

        /// <summary>
        /// Content-Range 标头，指定完整正文中应用随附的部分正文数据的位置。
        /// </summary>
        public const string ContentRange = "Content-Range";

        /// <summary>
        /// Content-Type 标头，指定随附的正文数据的 MIME 类型。
        /// </summary>
        public const string ContentType = "Content-Type";

        /// <summary>
        /// Cookie 标头，指定向服务器提供的 cookie 数据。
        /// </summary>
        public const string Cookie = "Cookie";

        /// <summary>
        /// Date 标头，指定发出请求的日期和时间。
        /// </summary>
        public const string Date = "Date";

        /// <summary>
        /// Expect 标头，指定客户端所需的特定服务器行为。
        /// </summary>
        public const string Expect = "Expect";

        /// <summary>
        /// Expires 标头，指定日期和时间，在该日期和时间之后随附的正文数据将被视为已过期。
        /// </summary>
        public const string Expires = "Expires";

        /// <summary>
        /// From 标头，指定控制请求的用户代理的用户的 Internet 电子邮件地址。
        /// </summary>
        public const string From = "From";

        /// <summary>
        /// Host 标头，指定要请求的资源的主机名和端口号。
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// If-match 标头，指定仅当客户端所指示的资源的缓存副本是最新的时，才执行所请求的操作。
        /// </summary>
        public const string IfMatch = "If-match";

        /// <summary>
        /// If-Modified-Since 标头，指定仅当自指示的数据和时间之后修改了请求的资源时，才执行请求的操作。
        /// </summary>
        public const string IfModifiedSince = "If-Modified-Since";

        /// <summary>
        /// If-None-Match 标头，指定仅当客户端所指示的资源的缓存副本都不是最新的时，才执行所请求的操作。
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";

        /// <summary>
        /// If-Range 标头，指定如果客户端的缓存副本是最新的，则仅发送指定范围的请求资源。
        /// </summary>
        public const string IfRange = "If-Range";

        /// <summary>
        /// If-Unmodified-Since 标头，指定仅当自指示的数据和时间之后未修改请求的资源时，才执行请求的操作。
        /// </summary>
        public const string IfUnmodifiedSince = "If-Unmodified-Since";

        /// <summary>
        /// Keep-Alive 标头，指定用于保持持久性连接的参数。
        /// </summary>
        public const string KeepAlive = "Keep-Alive";

        /// <summary>
        /// Last-Modified 标头，指定上次修改随附的正文数据的日期和时间。
        /// </summary>
        public const string LastModified = "Last-Modified";

        /// <summary>
        /// Max-Forwards 标头，指定一个整数，表示此请求还可以被转发的次数。
        /// </summary>
        public const string MaxForwards = "Max-Forwards";

        /// <summary>
        /// Pragma 标头，指定特定于实现的指令，这些指令可应用到请求/响应链上的任意代理。
        /// </summary>
        public const string Pragma = "Pragma";

        /// <summary>
        /// Proxy-Authorization 标头，指定客户端提供的以向代理验证自身身份的凭据。
        /// </summary>
        public const string ProxyAuthorization = "Proxy-Authorization";

        /// <summary>
        /// Range 标头，指定代替整个响应返回的客户端请求的响应的子范围。
        /// </summary>
        public const string Range = "Range";

        /// <summary>
        /// Referer 标头，指定可从中获取请求 URI 的资源 URI。
        /// </summary>
        public const string Referer = "Referer";

        /// <summary>
        /// TE 标头，指定响应可接受的传输编码。
        /// </summary>
        public const string Te = "TE";

        /// <summary>
        /// Trailer 标头，指定显示在以成块传输编码方式编码的消息尾部的标头字段。
        /// </summary>
        public const string Trailer = "Trailer";

        /// <summary>
        /// Transfer-Encoding 标头，指定应用到消息正文的转换类型（如果有）。
        /// </summary>
        public const string TransferEncoding = "Transfer-Encoding";

        /// <summary>
        /// Translate 标头，与 WebDAV 功能一起使用的 HTTP 规范的 Microsoft 扩展。
        /// </summary>
        public const string Translate = "Translate";

        /// <summary>
        /// Upgrade 标头，指定客户端支持的其他通信协议。
        /// </summary>
        public const string Upgrade = "Upgrade";

        /// <summary>
        /// User-Agent 标头，指定有关客户端代理的信息。
        /// </summary>
        public const string UserAgent = "User-Agent";

        /// <summary>
        /// Via 标头，指定网关和代理要使用的中间协议。
        /// </summary>
        public const string Via = "Via";

        /// <summary>
        /// Warning 标头，指定消息中可能不会反映的有关消息的状态或转换的其他信息。
        /// </summary>
        public const string Warning = "Warning";

        #endregion HttpRequestHeader

        #region HttpResponseHeader

        /// <summary>
        /// Accept-Ranges 标头，指定服务器接受的范围。
        /// </summary>
        public const string AcceptRanges = "Accept-Ranges";

        /// <summary>
        /// Age 标头，指定自起始服务器生成响应以来的时间长度（以秒为单位）。
        /// </summary>
        public const string Age = "Age";

        /// <summary>
        /// Etag 标头，指定请求的变量的当前值。
        /// </summary>
        public const string ETag = "Etag";

        /// <summary>
        /// Location 标头，指定为获取请求的资源而将客户端重定向到的 URI。
        /// </summary>
        public const string Location = "Location";

        /// <summary>
        /// Proxy-Authenticate 标头，指定客户端必须对代理验证其自身。
        /// </summary>
        public const string ProxyAuthenticate = "Proxy-Authenticate";

        /// <summary>
        /// Retry-After 标头，指定某个时间（以秒为单位）或日期和时间，在此时间之后客户端可以重试其请求。
        /// </summary>
        public const string RetryAfter = "Retry-After";

        /// <summary>
        /// Server 标头，指定关于起始服务器代理的信息。
        /// </summary>
        public const string Server = "Server";

        /// <summary>
        /// Set-Cookie 标头，指定提供给客户端的 Cookie 数据。
        /// </summary>
        public const string SetCookie = "Set-Cookie";

        /// <summary>
        /// Vary 标头，指定用于确定缓存的响应是否为新响应的请求标头。
        /// </summary>
        public const string Vary = "Vary";

        /// <summary>
        /// WWW-Authenticate 标头，指定客户端必须对服务器验证其自身。
        /// </summary>
        public const string WwwAuthenticate = "WWW-Authenticate";

        #endregion HttpResponseHeader
    }
}