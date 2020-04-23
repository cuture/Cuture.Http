# Cuture.Http

源于自用的http请求辅助库整理, 基于System.Net.Http.HttpClient做了一些简单封装和拓展方法, 用于进行http请求; 目标框架为```.NetStandard2.0```;

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

### 其它工具拓展等

```C#
"https://dotnet.microsoft.com/".EncodeBase64();
//aHR0cHM6Ly9kb3RuZXQubWljcm9zb2Z0LmNvbS8=
"aHR0cHM6Ly9kb3RuZXQubWljcm9zb2Z0LmNvbS8=".DecodeBase64();
//https://dotnet.microsoft.com/

"keyword关键词".UrlEncode();
//keyword%e5%85%b3%e9%94%ae%e8%af%8d
"keyword%e5%85%b3%e9%94%ae%e8%af%8d".UrlDecode();
//keyword关键词

UserAgents.RandomUserAgent();
//随机的UA

var cookie = "lang=en-US; Path=/; Max-Age=2147483647 i_like_gogs=d38e69bb16e9080d; Path=/; HttpOnly _csrf=Zxnf2GNhwYoZUONx6ylflfFS0CI6MTU3ODExNzU2NzU4MDM0NjEzMg%3D%3D; Path=/; Expires=Sun, 05 Jan 2020 05:59:27 GMT; HttpOnly";
CookieUtility.Clean(cookie);
//lang=en-US; i_like_gogs=d38e69bb16e9080d; _csrf=Zxnf2GNhwYoZUONx6ylflfFS0CI6MTU3ODExNzU2NzU4MDM0NjEzMg%3D%3D;
```