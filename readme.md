# Cuture.Http

用于快速进行Http请求的链式拓展方法库。
- 主要为针对`string`和`Uri`对象的拓展方法，及一些Http相关的工具;
- 本质上是对`System.Net.Http.HttpClient`等的封装; 
- 目标框架目前为`.Net5`/`.NetStandard2.0`;

## 如何使用

安装Nuget包
```PowerShell
Install-Package Cuture.Http
```

1. 创建请求
```C#
var request = "http://www.domain.com/api".ToHttpRequest();
```

2. 设置请求
```C#
 request.UseUserAgent(UserAgents.FireFox)
        .AddHeader("header1", "header1Value")
        .UsePost()
        .TimeOut(3000)
        .WithCancellation(token)
        //进行其他的一些请求设置等
        .WithFormContent($"key={value.UrlEncode()}");
```

3. 请求并获取结果
```C#
var response = await request.TryGetAsStringAsync();
Console.WriteLine($"response:{response.Data}");
```
* 请求方法包括直接返回请求结果的方法 `GetAsBytesAsync`、`GetAsJsonAsync`、`GetAsObjectAsync<T>`、`GetAsStringAsync` 和内部吞掉异常的 `TryGetAsBytesAsync`、`TryGetAsJsonAsync`、`TryGetAsObjectAsync<T>`、`TryGetAsStringAsync` ; 
* `GetAsJsonAsync` 将返回以 `Newtonsoft.Json.Linq.JObject.Parse` 转换请求结果后的 `JObject` 对象;
* `GetAsObjectAsync<T>` 将返回以 `Newtonsoft.Json.JsonConvert.DeserializeObject<T>` 反序列化请求结果后的 `T` 对象;

## 使用示例

### 获取网页数据
```C#
var response = await "http://www.baidu.com".ToHttpRequest()
                                            .GetAsStringAsync();
Console.WriteLine(response);
```
### 获取并解析接口数据
```C#
var url = "https://docs.microsoft.com/api/privacy/cookieConsent?locale=zh-cn";
var response = await url.ToHttpRequest()
                        .GetAsJsonAsync();
Console.WriteLine(response["message"]["message"]);
```
### 需要进度的下载
```C#
var url = "https://download.visualstudio.microsoft.com/download/pr/a16689d1-0872-4ef9-a592-406d3038d8f7/cf4f84504385a599f0cb6a5c113ccb34/aspnetcore-runtime-3.1.0-win-x64.exe";
try
{
    using var stream = File.OpenWrite("d:\\runtime.exe");
    await url.ToHttpRequest()
             .DownloadToStreamWithProgressAsync((contentLength, downloaded) =>
             {
                 if (contentLength > 0)
                 {
                     Console.WriteLine($"已下载：{downloaded / 1024} kb，进度 {(((float)downloaded / contentLength) * 100).Value.ToString("F")} %");
                 }
                 else
                 {
                     Console.WriteLine($"已下载：{downloaded / 1024} kb");
                 }
             }, stream, 1024 * 1024);
}
catch (Exception ex)
{
    Console.WriteLine($"下载失败:{ex}");
}
```

### 从原始数据构建请求(.net5 only now)

- 使用从各种抓包工具中复制的原始数据，快速构建等价请求
```C#
var rawBase64Str = "R0VUIGh0dHA6Ly9kZXRlY3Rwb3J0YWwuZmlyZWZveC5jb20vc3VjY2Vzcy50eHQgSFRUUC8xLjENCkhvc3Q6IGRldGVjdHBvcnRhbC5maXJlZm94LmNvbQ0KVXNlci1BZ2VudDogTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NDsgcnY6ODQuMCkgR2Vja28vMjAxMDAxMDEgRmlyZWZveC84NC4wDQpBY2NlcHQ6ICovKg0KQWNjZXB0LUxhbmd1YWdlOiB6aC1DTix6aDtxPTAuOCx6aC1UVztxPTAuNyx6aC1ISztxPTAuNSxlbi1VUztxPTAuMyxlbjtxPTAuMg0KQWNjZXB0LUVuY29kaW5nOiBnemlwLCBkZWZsYXRlDQpDYWNoZS1Db250cm9sOiBuby1jYWNoZQ0KUHJhZ21hOiBuby1jYWNoZQ0KRE5UOiAxDQpDb25uZWN0aW9uOiBrZWVwLWFsaXZlDQoNCg==";
var request = RequestBuildTool.FromRaw(rawBase64Str);
//进行其他的一些请求设置等，覆盖原始的请求设置
var result = await request.TryGetAsStringAsync();
```

- 仅从原始数据中加载指定部分
```C#
//仅读取请求头
request.LoadHeadersFromRaw(rawBase64Str);
//仅读取请求内容
request.LoadContentFromRaw(rawBase64Str);
//读取请求头和内容
request.LoadHeadersAndContentFromRaw(rawBase64Str);
```

### 部分其它工具拓展示例

#### Base64编码
```C#
"https://dotnet.microsoft.com/".EncodeBase64();
//aHR0cHM6Ly9kb3RuZXQubWljcm9zb2Z0LmNvbS8=
"aHR0cHM6Ly9kb3RuZXQubWljcm9zb2Z0LmNvbS8=".DecodeBase64();
//https://dotnet.microsoft.com/
```

#### UrlEncode
```C#
"keyword关键词".UrlEncode();
//keyword%e5%85%b3%e9%94%ae%e8%af%8d
"keyword%e5%85%b3%e9%94%ae%e8%af%8d".UrlDecode();
//keyword关键词
```

#### 随机UA
```C#
UserAgents.RandomUserAgent();
```

#### Cookie字符串清理
```C#
var cookie = "lang=en-US; Path=/; Max-Age=2147483647 i_like_gogs=d38e69bb16e9080d; Path=/; HttpOnly _csrf=Zxnf2GNhwYoZUONx6ylflfFS0CI6MTU3ODExNzU2NzU4MDM0NjEzMg%3D%3D; Path=/; Expires=Sun, 05 Jan 2020 05:59:27 GMT; HttpOnly";
CookieUtility.Clean(cookie);
//lang=en-US; i_like_gogs=d38e69bb16e9080d; _csrf=Zxnf2GNhwYoZUONx6ylflfFS0CI6MTU3ODExNzU2NzU4MDM0NjEzMg%3D%3D;
```