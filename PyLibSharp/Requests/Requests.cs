using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PyLibSharp.Requests
{
    public class ReqParams
    {
        public Dictionary<HttpRequestHeader, string> Header { get; set; } = new Dictionary<HttpRequestHeader, string>();
        public WebProxy                              ProxyToUse { get; set; }
        public CookieContainer                       Cookies { get; set; } = new CookieContainer();
        public Dictionary<string, string>            CustomHeader { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>            Params { get; set; } = new Dictionary<string, string>();
        public byte[]                                PostRawData { get; set; }
        public object                                PostJson { get; set; }
        public MultipartFormDataContent              PostMultiPart { get; set; }
        public Encoding                              PostEncoding { get; set; } = new System.Text.UTF8Encoding(false);
        public PostType                              PostParamsType { get; set; } = PostType.none;
        public bool                                  UseHandler { get; set; } = false;
        public bool                                  IsStream { get; set; } = false;
        public bool                                  IsUseHtmlMetaEncoding { get; set; } = true;
        public bool                                  IsThrowErrorForStatusCode { get; set; } = true;
        public int                                   Timeout { get; set; } = 500;
        public int                                   ReadBufferSize { get; set; } = 1024;
    }

    public enum PostType
    {
        json,
        x_www_form_urlencoded,
        form_data,
        raw,
        none
    }

    public class ReqRepeatable : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x != y;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    public class ReqResponse
    {
        public MemoryStream    RawStream { get; }
        public CookieContainer Cookies   { get; }
        public string          Text      => Encode.GetString(RawStream.ToArray());

        public byte[] Content => RawStream.ToArray();

        public string ContentType { get; }

        public Encoding       Encode     { get; set; }
        public HttpStatusCode StatusCode { get; }

        public ReqResponse(MemoryStream rawStream, CookieContainer cookies, string contentType, Encoding encode,
                           HttpStatusCode statusCode)
        {
            Cookies     = cookies;
            RawStream   = rawStream;
            ContentType = contentType;
            Encode      = encode;
            StatusCode  = statusCode;
        }

        public JObject Json()
        {
            try
            {
                // if (!ContentType.Contains("application/json"))
                // {
                //     throw new WarningException("HTTP 响应中的 Content-Type 并非 JSON 格式，响应的数据有可能并不是 JSON");
                // }

                return JsonConvert.DeserializeObject(Text) as JObject;
            }
            catch (Exception ex)
            {
                // if (ex.GetType() == typeof(WarningException))
                // {
                //     throw;
                // }

                throw new ReqResponseParseException("JSON 解析出错，请确保响应为 JSON 格式", ex);
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    #region 自定义错误部分

    class ReqRequestException : ApplicationException
    {
        public  string    Error;
        private Exception innerException;

        public ReqRequestException()
        {
        }

        public ReqRequestException(string msg) : base(msg)
        {
            this.Error = msg;
        }

        public ReqRequestException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            Error               = msg;
        }

        public string GetError()
        {
            return Error;
        }
    }

    class ReqResponseException : ApplicationException
    {
        public  string    Error;
        private Exception innerException;

        public ReqResponseException()
        {
        }

        public ReqResponseException(string msg) : base(msg)
        {
            this.Error = msg;
        }

        public ReqResponseException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            Error               = msg;
        }

        public string GetError()
        {
            return Error;
        }
    }


    class ReqResponseParseException : ApplicationException
    {
        public  string    Error;
        private Exception innerException;

        public ReqResponseParseException()
        {
        }

        public ReqResponseParseException(string msg) : base(msg)
        {
            this.Error = msg;
        }

        public ReqResponseParseException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            Error               = msg;
        }

        public string GetError()
        {
            return Error;
        }
    }

    class ReqUrlException : ApplicationException
    {
        public  string    Error;
        private Exception innerException;

        public ReqUrlException()
        {
        }

        public ReqUrlException(string msg) : base(msg)
        {
            this.Error = msg;
        }

        public ReqUrlException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            Error               = msg;
        }

        public string GetError()
        {
            return Error;
        }
    }

    class ReqHeaderException : ApplicationException
    {
        public  string    Error;
        private Exception innerException;

        public ReqHeaderException()
        {
        }

        public ReqHeaderException(string msg) : base(msg)
        {
            this.Error = msg;
        }

        public ReqHeaderException(string msg, Exception innerException) : base(msg, innerException)
        {
            this.innerException = innerException;
            Error               = msg;
        }

        public string GetError()
        {
            return Error;
        }
    }

    #endregion

    public class Requests
    {
        public static event EventHandler<AggregateExceptionArgs> ReqExceptionHandler;

        public class AggregateExceptionArgs : EventArgs
        {
            public AggregateException AggregateException { get; set; }
        }


        public static ReqResponse Get(string Url)
        {
            return RequestBase(Url, "GET", new ReqParams(), new CancellationTokenSource()).Result;
        }

        public static ReqResponse Get(string Url, ReqParams Params)
        {
           return RequestBase(Url, "GET", Params, new CancellationTokenSource()).Result;
        }

        public static ReqResponse Get(string Url, ReqParams Params, CancellationTokenSource CancelFlag)
        {
            return RequestBase(Url, "GET", Params, CancelFlag).Result;
        }

        public static ReqResponse Post(string Url)
        {
            return RequestBase(Url, "POST", new ReqParams(), new CancellationTokenSource()).Result;
        }

        public static ReqResponse Post(string Url, ReqParams Params)
        {
            return RequestBase(Url, "POST", Params, new CancellationTokenSource()).Result;
        }

        public static ReqResponse Post(string Url, ReqParams Params, CancellationTokenSource CancelFlag)
        {
            return RequestBase(Url, "POST", Params, CancelFlag).Result;
        }

        public static async Task<ReqResponse> GetAsync(string Url)
        {
            return await RequestBase(Url, "GET", new ReqParams(), new CancellationTokenSource());
        }

        public static async Task<ReqResponse> GetAsync(string Url, ReqParams Params)
        {
            return await RequestBase(Url, "GET", Params, new CancellationTokenSource());
        }

        public static async Task<ReqResponse> GetAsync(string Url, ReqParams Params, CancellationTokenSource CancelFlag)
        {
            return await RequestBase(Url, "GET", Params, CancelFlag);
        }

        public static async Task<ReqResponse> PostAsync(string Url)
        {
            return await RequestBase(Url, "POST", new ReqParams(), new CancellationTokenSource());
        }

        public static async Task<ReqResponse> PostAsync(string Url, ReqParams Params)
        {
            return await RequestBase(Url, "POST", Params, new CancellationTokenSource());
        }

        public static async Task<ReqResponse> PostAsync(string Url, ReqParams Params,
                                                        CancellationTokenSource CancelFlag)
        {
            return await RequestBase(Url, "POST", Params, CancelFlag);
        }

        public static async Task<ReqResponse> RequestBase(string Url, string Method, ReqParams Params,
                                                          CancellationTokenSource CancelFlag)
        {
            //不能直接使用GB2312，必须先注册
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentNullException(nameof(Url));
            }

            if (Params.UseHandler)
            {
                if (ReqExceptionHandler == null)
                    throw new ArgumentNullException(nameof(ReqExceptionHandler));
            }
            else
            {
                if (ReqExceptionHandler != null)
                    Params.UseHandler = true;
            }

            if (Params is null)
            {
                Params = new ReqParams();
            }

            HttpWebRequest request = null;
            //参数处理部分
            string paramStr =
                String.Join("&",
                            Params.Params.Select(i => HttpUtility.UrlEncode(i.Key) + "=" +
                                                      HttpUtility.UrlEncode(i.Value)));
            if (Params.PostParamsType == PostType.none)
            {
                if (!string.IsNullOrEmpty(paramStr))
                {
                    Params.PostParamsType = PostType.x_www_form_urlencoded;
                }

                if (Params.PostJson != null)
                {
                    Params.PostParamsType = PostType.json;
                }

                if (Params.PostRawData != null && Params.PostRawData.Length != 0)
                {
                    Params.PostParamsType = PostType.raw;
                }

                if (Params.PostMultiPart != null && Params.PostMultiPart.Any())
                {
                    Params.PostParamsType = PostType.form_data;
                }
            }

            try
            {
                Uri urlToSend = new Uri(Url);


                if (Method == "GET")
                {
                    var urlParsed = Url.Contains("?") ? Url.Split('?')[0] : Url;
                    if (paramStr == "")
                    {
                        if (Url.Contains("?"))
                        {
                            urlParsed += Url.Split('?')[1];
                        }
                    }
                    else
                    {
                        paramStr = (urlToSend.Query.StartsWith("?") ? urlToSend.Query : "?" + urlToSend.Query) +
                                   (urlToSend.Query.EndsWith("&")
                                       ? ""
                                       : ((urlToSend.Query != "" && urlToSend.Query != "?" && paramStr != "")
                                           ? "&"
                                           : ""))
                                 + paramStr;
                        urlParsed += ((urlToSend.AbsolutePath == "/" && !Url.EndsWith("/")) ? "/" : "") + paramStr;
                    }

                    request = (HttpWebRequest) WebRequest.Create(urlParsed);
                }
                else
                {
                    request = (HttpWebRequest) WebRequest.Create(urlToSend);
                }
            }
            catch (Exception ex)
            {
                if (Params.UseHandler)
                    ReqExceptionHandler(null,
                                        new AggregateExceptionArgs()
                                        {
                                            AggregateException =
                                                new AggregateException(new
                                                                           ReqUrlException("构造 URL 时发生错误，请检查 URL 格式和请求参数",
                                                                               ex))
                                        });
                else
                    throw new ReqUrlException("构造 URL 时发生错误，请检查 URL 格式和请求参数", ex);
            }

            request.Method  = Method;
            request.Timeout = Params.Timeout;
            request.Proxy   = Params.ProxyToUse;

            try
            {
                //头部处理部分
                //默认头部添加
                if (!Params.Header.ContainsKey(HttpRequestHeader.AcceptLanguage))
                {
                    Params.Header.Add(HttpRequestHeader.AcceptLanguage,
                                      "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
                }

                if (!Params.Header.ContainsKey(HttpRequestHeader.UserAgent))
                {
                    Params.Header.Add(HttpRequestHeader.UserAgent,
                                      "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36 Edg/85.0.564.41");
                }

                if (!Params.Header.ContainsKey(HttpRequestHeader.Accept))
                {
                    Params.Header.Add(HttpRequestHeader.Accept,
                                      "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                }

                foreach (KeyValuePair<HttpRequestHeader, string> header in Params.Header)
                {
                    switch (header.Key)
                    {
                        case HttpRequestHeader.Accept:
                            request.Accept = header.Value;
                            break;
                        case HttpRequestHeader.Connection:
                            request.Connection = header.Value;
                            break;
                        case HttpRequestHeader.ContentLength:
                            if (long.TryParse(header.Value, out long length))
                            {
                                request.ContentLength = length;
                            }

                            break;
                        case HttpRequestHeader.ContentType:
                            request.ContentType = header.Value.Trim().ToLower();
                            break;
                        case HttpRequestHeader.Date:
                            if (DateTime.TryParse(header.Value, out DateTime date))
                            {
                                request.Date = date;
                            }

                            break;
                        case HttpRequestHeader.Expect:
                            request.Expect = header.Value;
                            break;
                        case HttpRequestHeader.Host:
                            request.Host = header.Value;
                            break;
                        case HttpRequestHeader.Referer:
                            request.Referer = header.Value;
                            break;
                        case HttpRequestHeader.UserAgent:
                            request.UserAgent = header.Value;
                            break;
                        default:
                            request.Headers.Add(header.Key, header.Value);
                            break;
                    }
                }

                foreach (KeyValuePair<string, string> header in Params.CustomHeader)
                {
                    request.Headers.Add(HttpUtility.UrlEncode(header.Key), HttpUtility.UrlEncode(header.Value));
                }
            }
            catch (Exception ex)
            {
                if (Params.UseHandler)
                    ReqExceptionHandler(null,
                                        new AggregateExceptionArgs()
                                        {
                                            AggregateException =
                                                new AggregateException(new ReqHeaderException("构造 HTTP 头部时发生错误", ex))
                                        });
                else
                    throw new ReqHeaderException("构造 HTTP 头部时发生错误", ex);
            }


            Stream myResponseStream = null;
            //StreamReader myStreamReader   = null;
            int bufferSize = Params.ReadBufferSize;

            Encoding        responseEncoding        = Encoding.UTF8;
            MemoryStream    responseStream          = new MemoryStream();
            HttpStatusCode  responseStatusCode      = 0;
            string          responseContentType     = "";
            CookieContainer responseCookieContainer = new CookieContainer();

            //POST 数据写入
            if (Method == "POST" || Method == "PUT")
            {
                switch (Params.PostParamsType)
                {
                    case PostType.x_www_form_urlencoded:
                        if (string.IsNullOrEmpty(paramStr))
                        {
                            if (Params.UseHandler)
                                ReqExceptionHandler(null,
                                                    new AggregateExceptionArgs()
                                                    {
                                                        AggregateException =
                                                            new AggregateException(new
                                                                ReqRequestException("以 application/x-www-form-urlencoded 类型 POST 时，Params 参数未设置或为空",
                                                                    new ArgumentNullException(nameof(
                                                                        Params))))
                                                    });
                            else
                                throw new
                                    ReqRequestException("以 application/x-www-form-urlencoded 类型 POST 时，Params 参数未设置或为空",
                                                        new ArgumentNullException(nameof(Params)));
                        }

                        request.ContentType =
                            "application/x-www-form-urlencoded;charset=" + Params.PostEncoding.WebName;
                        byte[] data = Params.PostEncoding.GetBytes(paramStr.ToString());
                        using (Stream stream = request.GetRequestStream())
                        {
                            await stream.WriteAsync(data, 0, data.Length);
                        }

                        break;
                    case PostType.form_data:
                        if (Params.PostMultiPart is null)
                        {
                            if (Params.UseHandler)
                                ReqExceptionHandler(null,
                                                    new AggregateExceptionArgs()
                                                    {
                                                        AggregateException =
                                                            new AggregateException(new
                                                                ReqRequestException("以 multipart/formdata 类型 POST 时，PostMultiPart 参数未设置或为空",
                                                                    new
                                                                        ArgumentNullException("PostMultiPart")))
                                                    });
                            else
                                throw new ReqRequestException("以 multipart/formdata 类型 POST 时，PostMultiPart 参数未设置或为空",
                                                              new ArgumentNullException("PostMultiPart"));
                        }

                        var dat  = Params.PostMultiPart;
                        var task = dat.ReadAsByteArrayAsync();
                        request.ContentType   = dat.Headers.ContentType.ToString();
                        request.ContentLength = dat.Headers.ContentLength.Value;

                        using (Stream stream = request.GetRequestStream())
                        {
                            await stream.WriteAsync(task.Result, 0, task.Result.Length);
                        }

                        break;
                    case PostType.raw:
                        if (Params.PostRawData is null || Params.PostRawData.Length == 0)
                        {
                            if (Params.UseHandler)
                                ReqExceptionHandler(null,
                                                    new AggregateExceptionArgs()
                                                    {
                                                        AggregateException =
                                                            new AggregateException(new
                                                                ReqRequestException("以 application/x-www-form-urlencoded 类型 POST 时，Params 参数未设置或为空",
                                                                    new ArgumentNullException(nameof(
                                                                        Params))))
                                                    });
                            else
                                throw new ReqRequestException("以 Raw 类型 POST 时，PostRawData 参数未设置或为空",
                                                              new ArgumentNullException("RawPostParams"));
                        }

                        using (Stream stream = request.GetRequestStream())
                        {
                            await stream.WriteAsync(Params.PostRawData, 0, Params.PostRawData.Length);
                        }

                        break;
                    case PostType.json:
                        if (Params.PostJson == null)
                        {
                            if (Params.UseHandler)
                                ReqExceptionHandler(null,
                                                    new AggregateExceptionArgs()
                                                    {
                                                        AggregateException =
                                                            new AggregateException(new
                                                                ReqRequestException("以 Json 类型 POST 时，PostJson 参数未设置或为空",
                                                                    new ArgumentNullException("PostJson")))
                                                    });
                            else
                                throw new ReqRequestException("以 Json 类型 POST 时，PostJson 参数未设置或为空",
                                                              new ArgumentNullException("PostJson"));
                        }

                        request.ContentType = "application/json;charset=" + Params.PostEncoding.WebName;
                        byte[] jsonData;
                        if (Params.PostJson is string json)
                        {
                            jsonData = Params.PostEncoding.GetBytes(json);
                        }
                        else
                        {
                            jsonData = Params.PostEncoding.GetBytes(JsonConvert.SerializeObject(Params.PostJson));
                        }


                        using (Stream stream = request.GetRequestStream())
                        {
                            await stream.WriteAsync(jsonData, 0, jsonData.Length);
                        }

                        break;
                }
            }


            try
            {
                request.CookieContainer = Params.Cookies;

                //开始异步请求
                Task<WebResponse> responseTask = request.GetResponseAsync(CancelFlag.Token);
                //如果取消
                if (CancelFlag.IsCancellationRequested)
                {
                    return new ReqResponse(new MemoryStream(), Params.Cookies, "", new UTF8Encoding(), 0);
                }
                else if (await Task.WhenAny(responseTask, Task.Delay(Params.Timeout)) != responseTask)
                {
                    return new ReqResponse(new MemoryStream(), Params.Cookies, "", new UTF8Encoding(), 0);
                }

                //异步请求结果
                HttpWebResponse response = (HttpWebResponse) responseTask.Result;
                if (responseTask.IsFaulted)
                {
                    throw responseTask.Exception;
                }
                if (Params.IsThrowErrorForStatusCode && response.StatusCode != HttpStatusCode.OK               &&
                    response.StatusCode                                     != HttpStatusCode.Accepted         &&
                    response.StatusCode                                     != HttpStatusCode.Continue         &&
                    response.StatusCode                                     != HttpStatusCode.Created          &&
                    response.StatusCode                                     != HttpStatusCode.Found            &&
                    response.StatusCode                                     != HttpStatusCode.Moved            &&
                    response.StatusCode                                     != HttpStatusCode.MovedPermanently &&
                    response.StatusCode                                     != HttpStatusCode.MultipleChoices  &&
                    response.StatusCode                                     != HttpStatusCode.NoContent        &&
                    response.StatusCode                                     != HttpStatusCode.NotModified      &&
                    response.StatusCode                                     != HttpStatusCode.PartialContent   &&
                    response.StatusCode                                     != HttpStatusCode.Redirect         &&
                    response.StatusCode                                     != HttpStatusCode.RedirectKeepVerb &&
                    response.StatusCode                                     != HttpStatusCode.RedirectMethod   &&
                    response.StatusCode                                     != HttpStatusCode.ResetContent)
                {
                    if (Params.UseHandler)
                        ReqExceptionHandler(null,
                                            new AggregateExceptionArgs()
                                            {
                                                AggregateException =
                                                    new
                                                        AggregateException(new
                                                                               ReqRequestException("HTTP 状态码指示请求发生错误，状态码为：" +
                                                                                   response.StatusCode))
                                            });
                    else
                        throw new ReqRequestException("HTTP 状态码指示请求发生错误，状态码为：" + response.StatusCode);
                }

                //获取响应流
                myResponseStream = response.GetResponseStream();

                // myStreamReader =
                //     new StreamReader(myResponseStream ?? throw new ReqResponseException("请求无响应"),
                //                      Encoding.GetEncoding(response.CharacterSet ??
                //                                           throw new ReqResponseException("请求无响应")));
                //
                //流转储
                byte[] buffer = new byte[bufferSize];
                int count =
                    await (myResponseStream ?? throw new ReqResponseException("请求无响应"))
                        .ReadAsync(buffer, 0, bufferSize);
                while (count > 0)
                {
                    responseStream.Write(buffer, 0, count);
                    count = await myResponseStream.ReadAsync(buffer, 0, bufferSize);
                }

                //编码自动判断
                if (response.ContentEncoding != ""&&!(response.ContentEncoding is null))
                {
                    responseEncoding = Encoding.GetEncoding(response.ContentEncoding.ToLower());
                }
                else if (response.CharacterSet != "" && !(response.CharacterSet is null) && response.ContentType.Contains("charset"))
                {
                    responseEncoding = Encoding.GetEncoding(response.CharacterSet.ToLower() ??
                                                            throw new ReqResponseException("请求无响应"));
                }
                else
                {
                    responseEncoding = Encoding.UTF8;
                }

                //通过HTML头部的Meta判断编码
                if (Params.IsUseHtmlMetaEncoding &&
                    response.ContentType.ToLower().IndexOf("text/html", StringComparison.Ordinal) != -1)
                {
                    var CharSetMatch =
                        Regex.Match(responseEncoding.GetString(responseStream.ToArray()),
                                    @"<meta.*?charset=""?([a-z0-9-]+)\b", RegexOptions.IgnoreCase)
                             .Groups;
                    if (CharSetMatch.Count > 1 && CharSetMatch[1].Value != "")
                    {
                        string overrideCharset = CharSetMatch[1].Value;
                        responseEncoding = Encoding.GetEncoding(overrideCharset);
                    }
                }

                //属性添加
                responseStatusCode  = response.StatusCode;
                responseContentType = response.ContentType;
                responseCookieContainer.Add(response.Cookies);
            }
            catch (Exception ex)
            {
                if (Params.UseHandler)
                    ReqExceptionHandler(null,
                                        new AggregateExceptionArgs()
                                        {
                                            AggregateException =
                                                new AggregateException(new ReqResponseException("请求时发生错误", ex))
                                        });
                else
                    throw new ReqResponseException("请求时发生错误", ex);
            }

            //使用Finally将会导致不弹出错误
            // finally
            // {
            //     //myStreamReader?.Close();
            //     myResponseStream?.Close();
            // }
            myResponseStream?.Close();
            return new ReqResponse(responseStream, responseCookieContainer, responseContentType, responseEncoding,
                                   responseStatusCode);
        }
    }
}