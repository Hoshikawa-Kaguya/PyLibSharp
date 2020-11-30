using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
    /// <summary>
    /// 设置 HTTP 请求的基本参数。
    /// </summary>
    public class ReqParams
    {
        /// <summary>
        /// <para>设置 HTTP 请求中的默认头部。</para>
        /// <para>若此参数并不包含你想要设置的头部，请改用 <see langword="CustomHeader"></see> 参数去设置。</para>
        /// </summary>
        public Dictionary<HttpRequestHeader, string> Header { get; set; } = new Dictionary<HttpRequestHeader, string>();

        /// <summary>
        /// 设置传输过程中使用的代理。
        /// </summary>
        public WebProxy ProxyToUse { get; set; }

        /// <summary>
        /// 设置传输时采用的 Cookie 容器。
        /// </summary>
        public CookieContainer Cookies { get; set; } = new CookieContainer();

        /// <summary>
        /// 当要设置 <see langword="Header"></see> 中的参数所不包含的默认头部或任何自定义头部时，请使用本参数设置。
        /// </summary>
        public Dictionary<string, string> CustomHeader { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// <para>设置要传递给服务器的请求参数。</para>
        /// <para>当 HTTP 动作为 <see langword="GET"></see> 时，将智能附加至 URL 。</para>
        /// <para>其他 HTTP 动作时，将写入传输流，此时请设置 <see langword="PostParamsType"></see> 以明确如何传输数据。</para>
        /// <para>其他 HTTP 动作时，此参数优先级不如 <see langword="PostContent"></see> 高，若设置了 <see langword="PostContent"></see> 参数，此参数将被忽略。</para>
        /// </summary>
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>设置要 Post 至服务器的 HTTP 内容（如有必要需手动设置 <see langword="HttpContent"></see> 的 MediaType 字符串，如嫌麻烦请继续使用 <see langword="PostRawData"></see>、<see langword="PostJson"></see>、<see langword="PostMultiPart"></see> 参数，这些参数会自动设置 ContentType ）。</para>
        /// <para>若同时设置了 <see langword="PostRawData"></see>、<see langword="PostJson"></see>、<see langword="PostMultiPart"></see>，此参数优先级最高，将使用本参数指定的方式进行 Post ；</para>
        /// <para>在未设置 <see langword="PostParamsType"></see> 参数时，将自动修改 <see langword="PostParamsType"></see> 的值。</para>
        /// </summary>
        public HttpContent PostContent { get; set; }

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>建议改用 <see langword="PostContent"></see> 参数传递 HttpContent；</para>
        /// <para>设置要 Post 至服务器的原始字节序列。</para>
        /// <para>若同时设置了 <see langword="PostContent"></see>、<see langword="PostRawData"></see>、<see langword="PostJson"></see>、<see langword="PostMultiPart"></see>，<see langword="PostContent"></see> 参数优先级最高，将使用 <see langword="PostContent"></see> 指定的方式进行 Post ；</para>
        /// <para>在未设置 <see langword="PostParamsType"></see> 参数时，将自动修改 <see langword="PostParamsType"></see> 的值。</para>
        /// </summary>
        public byte[] PostRawData { get; set; }

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>建议改用 <see langword="PostContent"></see> 参数传递 HttpContent；</para>
        /// <para>设置要 Post 至服务器的 Json 数据。</para>
        /// <para>若同时设置了 <see langword="PostContent"></see>、<see langword="PostRawData"></see>、<see langword="PostJson"></see>、<see langword="PostMultiPart"></see>，<see langword="PostContent"></see> 参数优先级最高，将使用 <see langword="PostContent"></see> 指定的方式进行 Post ；</para>
        /// <para>在未设置 <see langword="PostParamsType"></see> 参数时，将自动修改 <see langword="PostParamsType"></see> 的值。</para>
        /// </summary>
        public object PostJson { get; set; }

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>建议改用 <see langword="PostContent"></see> 参数传递 HttpContent；</para>
        /// <para>设置要 Post 到服务器的 MultiPart 数据。</para>
        /// <para>若同时设置了 <see langword="PostContent"></see>、<see langword="PostRawData"></see>、<see langword="PostJson"></see>、<see langword="PostMultiPart"></see>，<see langword="PostContent"></see> 参数优先级最高，将使用 <see langword="PostContent"></see> 指定的方式进行 Post ；</para>
        /// <para>在未设置 <see langword="PostParamsType"></see> 参数时，将自动修改 <see langword="PostParamsType"></see> 的值。</para>
        /// </summary>
        public MultipartFormDataContent PostMultiPart { get; set; }

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>若 Post 传输的是字符串，该参数用来决定将采用的编码（对 <see langword="PostContent"></see> 参数中设置的编码无影响）。</para>
        /// </summary>
        public Encoding PostEncoding { get; set; } = new System.Text.UTF8Encoding(false);

        /// <summary>
        /// <para>（GET 动作中此参数将被忽略）</para>
        /// <para>设置 Post 采用的传输方式。</para>
        /// </summary>
        public PostType PostParamsType { get; set; } = PostType.none;

        /// <summary>
        /// <para>设置是否采用自定义错误捕捉器。</para>
        /// <para>若设置为true，请务必同时设置 <see cref="T:PyLibSharp.Requests.Requests" /> 类的 <see langword="ReqExceptionHandler"></see> 参数。</para>
        /// </summary>
        public bool UseHandler { get; set; } = false;

        /// <summary>
        /// 设置是否在结果中转储原始字节流。
        /// </summary>
        public bool IsStream { get; set; } = false;

        /// <summary>
        /// <para>设置当请求 HTML 时，是否使用 HTML 头部中的 <see langword="meta"></see> 标签自动获取编码。</para>
        /// <para>如设为 <see langword="true"></see>，将覆盖 HTTP 响应头中的编码设置。</para>
        /// </summary>
        public bool IsUseHtmlMetaEncoding { get; set; } = true;

        /// <summary>
        /// 设置当 HTTP 响应码不正常时，是否抛出异常。
        /// </summary>
        public bool IsThrowErrorForStatusCode { get; set; } = true;

        /// <summary>
        /// 设置当 HTTP 响应超时时，是否抛出异常。
        /// </summary>
        public bool IsThrowErrorForTimeout { get; set; } = true;

        /// <summary>
        /// 是否检查 HTTPS 证书的合法性，默认检查，以避免中间人等不安全因素
        /// <para>如非必要请不要设为false</para>
        /// </summary>
        public bool isCheckSSLCert { get; set; } = true;

        /// <summary>
        /// 设置 HTTP 连接等待的超时时间（单位毫秒/ms）。
        /// </summary>
        public int Timeout { get; set; } = 1500;

        /// <summary>
        /// 设置读取 HTTP 响应的缓冲区大小（单位字节/byte）。
        /// </summary>
        public int ReadBufferSize { get; set; } = 1024;
    }

    /// <summary>
    /// 设置当 Post 时数据的传输方式。
    /// </summary>
    public enum PostType
    {
        /// <summary>
        /// 采用 HttpContent 封装 Post 有效载荷。
        /// </summary>
        http_content,

        /// <summary>
        /// 要传输的是 Json 数据（将自动设置 ContentType ）
        /// </summary>
        json,

        /// <summary>
        /// 要传输的是 WWW 表单数据（将自动进行 URL 编码以及设置 ContentType ）
        /// </summary>
        x_www_form_urlencoded,

        /// <summary>
        /// 要传输的是 Multipart FormData 数据（将自动设置 ContentType ）
        /// </summary>
        form_data,

        /// <summary>
        /// 要传输的是原始字节序列数据（建议手动设置 ContentType ）
        /// </summary>
        raw,

        /// <summary>
        /// 默认（不 Post 数据）
        /// </summary>
        none
    }

    /// <summary>
    /// 若要让字典支持重复的键，请实例化本类并传入到字典的构造函数中。
    /// </summary>
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

    /// <summary>
    /// 储存 HTTP 响应的基本信息。
    /// </summary>
    public class ReqResponse : IEnumerable<string>
    {
        /// <summary>
        /// 获取 HTTP 响应转储的原始字节流。
        /// </summary>
        /// <returns>HTTP 响应转储的原始字节流</returns>
        public MemoryStream RawStream { get; }

        /// <summary>
        /// 获取 HTTP 响应的 Cookie 容器。
        /// </summary>
        /// <returns>HTTP 响应的 Cookie 容器</returns>
        public CookieContainer Cookies { get; }

        /// <summary>
        /// 获取 HTTP 响应的纯文本（将使用 <see langword="Encode"></see> 参数所代表的编码进行解码）
        /// </summary>
        /// <returns>HTTP 响应纯文本</returns>
        public string Text => Encode.GetString(RawStream.ToArray());

        /// <summary>
        /// 获取 HTTP 响应的原始字节序列。
        /// </summary>
        /// <returns>HTTP 响应的原始字节序列</returns>
        public byte[] Content => RawStream.ToArray();

        /// <summary>
        /// 获取 HTTP 响应的 ContentType。
        /// </summary>
        /// <returns>HTTP 响应的ContentType</returns>
        public string ContentType { get; }

        /// <summary>
        /// <para>获取 HTTP 响应使用的编码；</para>
        /// <para>或设置当解码 <see langword="Text"></see> 参数或执行 Json() 函数时要采用的编码。</para>
        /// </summary>
        public Encoding Encode { get; set; }

        /// <summary>
        /// 获取 HTTP 响应码。
        /// </summary>
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

        /// <summary>
        /// 将结果中的 Text（将使用 <see langword="Encode"></see> 参数所代表的编码进行解码）解析为 <see langword="Json"></see> 的 JObject。
        /// </summary>
        /// <returns>解析后的 JObject 对象</returns>
        public JObject Json()
        {
            try
            {
                // if (!ContentType.Contains("application/json"))
                // {
                //     throw new WarningException("HTTP 响应中的 Content-Type 并非 JSON 格式，响应的数据有可能并不是 JSON");
                // }
                try
                {
                    return (JObject) JsonConvert.DeserializeObject(Text);
                }
                catch
                {
                    return JArray.Parse(Text ?? "[]")[0].ToObject<JObject>();
                }
            }
            catch (Exception ex)
            {
                throw new ReqResponseParseException("JSON 解析出错，请确保响应为 JSON 格式: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 将结果中的 Text（将使用 <see langword="Encode"></see> 参数所代表的编码进行解码）解析为指定类型的对象。
        /// </summary>
        /// <returns>解析后的对象</returns>
        public T ToObject<T>()
        {
            try
            {
                // if (!ContentType.Contains("application/json")) 
                // {
                //     throw new WarningException("HTTP 响应中的 Content-Type 并非 JSON 格式，响应的数据有可能并不是 JSON");
                // }

                try
                {
                    return JsonConvert.DeserializeObject<T>(Text);
                }
                catch
                {
                    return JArray.Parse(Text ?? "[]")[0].ToObject<T>();
                }
            }
            catch (Exception ex)
            {
                throw new
                    ReqResponseParseException("JSON 解析出错，无法解析为类型：" + typeof(T) + "。请确保响应为 JSON 格式及与类型匹配: " + ex.Message,
                                              ex);
            }
        }

        /// <summary>
        /// 获取 HTTP 响应的纯文本（将使用 <see langword="Encode"></see> 参数所代表的编码进行解码）
        /// </summary>
        /// <returns>HTTP 响应纯文本</returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// 获取返回值文本每一行的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            bool isCrLf = Text.Contains("\r");
            foreach (string line in Regex.Split(Text, isCrLf ? "\r\n" : "\n"))
            {
                yield return line;
            }
        }

        /// <summary>
        /// 获取返回值文本每一行的迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            bool isCrLf = Text.Contains("\r");
            foreach (string line in Regex.Split(Text, isCrLf ? "\r\n" : "\n"))
            {
                yield return line;
            }
        }
    }

    #region 自定义错误部分

    /// <summary>
    /// HTTP 中 URL解析、请求或响应中可能出现的错误类型。
    /// </summary>
    public enum ErrorType
    {
        ArgumentNull,
        HTTPStatusCodeError,
        HTTPRequestTimeout,
        HTTPRequestHeaderError,
        UrlParseError,
        UserCancelled,
        Other,
        HTTPRequestError
    }

    class ReqRequestException : ApplicationException
    {
        public  string    Error;
        public  ErrorType ErrType;
        private Exception innerException;

        public ReqRequestException()
        {
        }

        public ReqRequestException(string msg, ErrorType errType) : base(msg)
        {
            this.Error   = msg;
            this.ErrType = errType;
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
        public  ErrorType ErrType;
        private Exception innerException;

        public ReqResponseException()
        {
        }

        public ReqResponseException(string msg, ErrorType errType) : base(msg)
        {
            this.Error   = msg;
            this.ErrType = errType;
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
        public  ErrorType ErrType;
        private Exception innerException;

        public ReqResponseParseException()
        {
        }

        public ReqResponseParseException(string msg, ErrorType errType) : base(msg)
        {
            this.Error   = msg;
            this.ErrType = errType;
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
        public  ErrorType ErrType;
        private Exception innerException;

        public ReqUrlException()
        {
        }

        public ReqUrlException(string msg, ErrorType errType) : base(msg)
        {
            this.Error   = msg;
            this.ErrType = errType;
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
        public  ErrorType ErrType;
        private Exception innerException;

        public ReqHeaderException()
        {
        }

        public ReqHeaderException(string msg, ErrorType errType) : base(msg)
        {
            this.Error   = msg;
            this.ErrType = errType;
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
            public ErrorType          ErrType            { get; set; }
        }

        public static ReqResponse XHR(string XHRData)
        {
            return XHRBase(XHRData, new ReqParams()).Result;
        }

        public static async Task<ReqResponse> XHRAsync(string XHRData)
        {
            return await XHRBase(XHRData, new ReqParams());
        }

        public static ReqResponse XHR(string XHRData, ReqParams Params)
        {
            return XHRBase(XHRData, Params).Result;
        }

        public static async Task<ReqResponse> XHRAsync(string XHRData, ReqParams Params)
        {
            return await XHRBase(XHRData, Params);
        }

        private static async Task<ReqResponse> XHRBase(string XHRData, ReqParams Params)
        {
            List<string> HeaderAndData =
                XHRData.Split(new string[] {"\r\n\r\n"}, 2, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> linesOfXHR =
                HeaderAndData[0].Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (Params.UseHandler)
            {
                if (ReqExceptionHandler == null)
                    throw new ArgumentNullException(nameof(ReqExceptionHandler),
                                                    new Exception("若要使用自定义错误处理函数，请先对事件 ReqExceptionHandler 增加处理函数。"));
            }
            else
            {
                if (ReqExceptionHandler != null)
                    Params.UseHandler = true;
            }

            if (!linesOfXHR.Any())
            {
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqUrlException("XHR 格式有误：应至少有1行",
                                                      new Exception()), ErrorType.UrlParseError);
            }

            string HTTPFirst = linesOfXHR.First();
            linesOfXHR.RemoveAt(0);
            string method = "";
            string URL    = "";
            try
            {
                var firstLine = HTTPFirst.Trim().Replace("  ", "").Split(new[] {' '});
                method = firstLine[0];
                URL    = firstLine[1];
                string HTTPProtocal = firstLine[2]; //忽略
            }
            catch (Exception ex)
            {
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqUrlException("XHR 格式有误：第一行格式有误",
                                                      ex), ErrorType.UrlParseError);
            }


            Dictionary<string, string>            headerAndKey        = new Dictionary<string, string>();
            Dictionary<HttpRequestHeader, string> defaultHeaderAndKey = new Dictionary<HttpRequestHeader, string>();
            string                                host                = "";
            //解析每一行 XHR
            linesOfXHR.ForEach(i =>
                               {
                                   string currLine = (i.EndsWith("\r") ? i.Trim().TrimEnd('\r') : i.Trim());
                                   string key      = currLine;
                                   string value    = "";
                                   if (currLine.Contains(":"))
                                   {
                                       key   = currLine.Split(new string[] {":"}, 2, StringSplitOptions.None)[0].Trim();
                                       value = currLine.Split(new string[] {":"}, 2, StringSplitOptions.None)[1].Trim();
                                   }

                                   if (key.ToLower() == "host")
                                   {
                                       host = value;
                                   }
                                   else if (key.ToLower() == "content-length")
                                   {
                                   }
                                   else if (key.ToLower() == "accept-encoding")
                                   {
                                   }
                                   //如果是预先定义的HTTP头部
                                   else if (Enum.IsDefined(typeof(HttpRequestHeader), key.Replace("-", "")))
                                   {
                                       defaultHeaderAndKey
                                           .Add((HttpRequestHeader) Enum.Parse(typeof(HttpRequestHeader), key.Replace("-", ""), true),
                                                value);
                                       headerAndKey.Remove(key);
                                   }
                                   else
                                   {
                                       //如果是自定义HTTP头部
                                       headerAndKey.Add(key, value);
                                   }
                               });


            Params.Header       = defaultHeaderAndKey;
            Params.CustomHeader = headerAndKey;

            if (host == "")
            {
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqUrlException("XHR 格式有误：未指定目标服务器 Host",
                                                      new Exception()), ErrorType.UrlParseError);
            }


            URL = "http://" + host + URL;

            if (method.ToUpper() != "GET" && HeaderAndData.Count > 1)
            {
                if (defaultHeaderAndKey.ContainsKey(HttpRequestHeader.ContentType))
                {
                    if (defaultHeaderAndKey[HttpRequestHeader.ContentType].Contains("charset="))
                    {
                        Params.PostEncoding =
                            Encoding.GetEncoding(defaultHeaderAndKey[HttpRequestHeader.ContentType]
                                                     .Split(new string[] {"charset="}, StringSplitOptions.None)[1]);
                    }

                    if (Params.PostParamsType == PostType.none)
                    {
                        if (defaultHeaderAndKey[HttpRequestHeader.ContentType].ToLower()
                                                                              .Contains("application/x-www-form-urlencoded")
                        )
                        {
                            Params.PostParamsType = PostType.x_www_form_urlencoded;
                        }

                        if (defaultHeaderAndKey[HttpRequestHeader.ContentType].ToLower()
                                                                              .Contains("multipart/form-data")
                        )
                        {
                            Params.PostParamsType = PostType.form_data;
                        }

                        if (defaultHeaderAndKey[HttpRequestHeader.ContentType].ToLower()
                                                                              .Contains("application/json")
                        )
                        {
                            Params.PostParamsType = PostType.json;
                        }
                    }
                }

                switch (Params.PostParamsType)
                {
                    case PostType.raw:
                        Params.PostRawData = Params.PostEncoding.GetBytes(HeaderAndData[1]);
                        break;
                    case PostType.x_www_form_urlencoded:
                        defaultHeaderAndKey[HttpRequestHeader.ContentType] =
                            "application/x-www-form-urlencoded;charset=" + Params.PostEncoding.WebName;
                        Params.PostRawData    = Params.PostEncoding.GetBytes(HeaderAndData[1]);
                        Params.PostParamsType = PostType.raw;
                        break;
                    case PostType.form_data:
                        Params.PostRawData    = Params.PostEncoding.GetBytes(HeaderAndData[1]);
                        Params.PostParamsType = PostType.raw;
                        break;
                    case PostType.json:
                        defaultHeaderAndKey[HttpRequestHeader.ContentType] =
                            "application/json;charset=" + Params.PostEncoding.WebName;
                        Params.PostJson = HeaderAndData[1];
                        break;
                    default:
                        if (defaultHeaderAndKey.ContainsKey(HttpRequestHeader.ContentType))
                        {
                            if (defaultHeaderAndKey[HttpRequestHeader.ContentType].ToLower().Contains("applition/json"))
                            {
                                Params.PostJson = HeaderAndData[1];
                            }
                            else
                            {
                                Params.PostRawData = Params.PostEncoding.GetBytes(HeaderAndData[1]);
                            }
                        }
                        else
                        {
                            Params.PostRawData = Params.PostEncoding.GetBytes(HeaderAndData[1]);
                        }

                        break;
                }
            }

            return await RequestBase(URL, method, Params, new CancellationTokenSource());
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
                throw new ArgumentNullException(nameof(Url), new Exception("URL 不可为空"));
            }

            //检查错误捕捉器合法性
            if (Params.UseHandler)
            {
                if (ReqExceptionHandler == null)
                    throw new ArgumentNullException(nameof(ReqExceptionHandler),
                                                    new Exception("若要使用自定义错误处理函数，请先对事件 ReqExceptionHandler 增加处理函数。"));
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
            //自动判断 Post 类型
            if (Params.PostParamsType == PostType.none)
            {
                if (Params.PostContent != null)
                {
                    Params.PostParamsType = PostType.http_content;
                }
                else if (!string.IsNullOrEmpty(paramStr))
                {
                    Params.PostParamsType = PostType.x_www_form_urlencoded;
                }
                else
                {
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
            }

            try
            {
                //解析 Url，产生必要的报错
                Uri urlToSend = new Uri(Url);


                if (Method == "GET")
                {
                    //如果是 GET 请求，需要拼接参数到 URL 上
                    var urlParsed = Url.Contains("?") ? Url.Split('?')[0] : Url;
                    if (paramStr == "")
                    {
                        if (Url.Contains("?"))
                        {
                            urlParsed += "?" + Url.Split('?')[1];
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
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqUrlException("构造 URL 时发生错误，请检查 URL 格式和请求参数",
                                                      ex), ErrorType.UrlParseError);
            }


            request.Method  = Method;
            request.Timeout = Params.Timeout;
            request.Proxy   = Params.ProxyToUse;

            //是否检查 SSL 证书
            if (!Params.isCheckSSLCert)
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (i, j, k, l) => true;
            }

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

                if (Method != "GET")
                {
                    //附加 Cookies
                    var           cookieList = Utils.GetAllCookies(Params.Cookies);
                    StringBuilder sb         = new StringBuilder();
                    //必须手动拼接，否则 POST 等请求带不出 Cookies
                    foreach (Cookie o in cookieList)
                    {
                        sb.Append(o.Name + "=" + o.Value + ";");
                    }

                    Params.Header.Add(HttpRequestHeader.Cookie, sb.ToString());
                }

                //读取参数指定的头部，正确写入 request 属性
                foreach (KeyValuePair<HttpRequestHeader, string> header in Params.Header)
                {
                    switch (header.Key)
                    {
                        case HttpRequestHeader.Accept:
                            request.Accept = header.Value;
                            break;
                        case HttpRequestHeader.Connection:
                            //request.Connection = header.Value;
                            if (header.Value == "keep-alive")
                            {
                                request.KeepAlive = true;
                            }
                            else
                            {
                                request.KeepAlive = false;
                            }

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
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqHeaderException("构造 HTTP 头部时发生错误",
                                                         ex), ErrorType.HTTPRequestHeaderError);
            }


            Stream myResponseStream = null;
            //StreamReader myStreamReader   = null;
            int bufferSize = Params.ReadBufferSize;

            Encoding        responseEncoding        = Encoding.UTF8;
            MemoryStream    responseStream          = new MemoryStream();
            HttpStatusCode  responseStatusCode      = 0;
            string          responseContentType     = "";
            CookieContainer responseCookieContainer = Params.Cookies;

            //POST 数据写入
            if (Method == "POST" || Method == "PUT")
            {
                using (Stream stream = request.GetRequestStream())
                {
                    //判断 POST 类型
                    switch (Params.PostParamsType)
                    {
                        case PostType.http_content:
                            if (Params.PostContent == null)
                            {
                                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                      ReqHeaderException("以 HttpContent 类型 POST 时，HttpContent 参数未设置或为空",
                                                                         new ArgumentNullException(nameof(Params))),
                                                  ErrorType.ArgumentNull);
                            }

                            request.ContentType = Params.PostContent.Headers.ContentType + ";charset=" +
                                                  Params.PostContent.Headers.ContentEncoding;

                            await Params.PostContent.CopyToAsync(stream);

                            break;
                        case PostType.x_www_form_urlencoded:
                            if (string.IsNullOrEmpty(paramStr))
                            {
                                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                      ReqHeaderException("以 application/x-www-form-urlencoded 类型 POST 时，Params 参数未设置或为空",
                                                                         new ArgumentNullException(nameof(Params))),
                                                  ErrorType.ArgumentNull);
                            }

                            request.ContentType =
                                "application/x-www-form-urlencoded;charset=" + Params.PostEncoding.WebName;
                            byte[] data = Params.PostEncoding.GetBytes(paramStr.ToString());

                            await stream.WriteAsync(data, 0, data.Length);


                            break;
                        case PostType.form_data:
                            if (Params.PostMultiPart is null)
                            {
                                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                      ReqHeaderException("以 multipart/formdata 类型 POST 时，PostMultiPart 参数未设置或为空",
                                                                         new ArgumentNullException("PostMultiPart")),
                                                  ErrorType.ArgumentNull);
                            }

                            var dat  = Params.PostMultiPart;
                            var task = dat.ReadAsByteArrayAsync();
                            request.ContentType   = dat.Headers.ContentType.ToString();
                            request.ContentLength = dat.Headers.ContentLength.Value;


                            await stream.WriteAsync(task.Result, 0, task.Result.Length);


                            break;
                        case PostType.raw:
                            if (Params.PostRawData is null || Params.PostRawData.Length == 0)
                            {
                                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                      ReqHeaderException("以 Raw 类型 POST 时，PostRawData 参数未设置或为空",
                                                                         new ArgumentNullException("PostRawData")),
                                                  ErrorType.ArgumentNull);
                            }


                            await stream.WriteAsync(Params.PostRawData, 0, Params.PostRawData.Length);


                            break;
                        case PostType.json:
                            if (Params.PostJson == null)
                            {
                                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                      ReqHeaderException("以 Json 类型 POST 时，PostJson 参数未设置或为空",
                                                                         new ArgumentNullException("PostJson")),
                                                  ErrorType.ArgumentNull);
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


                            await stream.WriteAsync(jsonData, 0, jsonData.Length);


                            break;
                    }
                }
            }


            try
            {
                request.CookieContainer = Params.Cookies;
                HttpWebResponse response = null;
                try
                {
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
                    if (responseTask.IsFaulted)
                    {
                        //出错继续抛出，若是WebException则仍可以继续，其他Exception由再外层的try-catch捕获
                        throw responseTask.Exception.InnerException;
                    }

                    if (responseTask.IsCanceled)
                    {
                        // HandleError(Params.UseHandler, ReqExceptionHandler, new
                        //                 ReqRequestException("用户主动取消 HTTP 请求", ErrorType.UserCancelled), ErrorType.UserCancelled);

                        return new ReqResponse(new MemoryStream(), Params.Cookies, "", new UTF8Encoding(), 0);
                    }

                    response = (HttpWebResponse) responseTask.Result;
                }
                catch (WebException ex)
                {
                    //状态码错误
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        if (Params.IsThrowErrorForStatusCode)
                        {
                            Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                  ReqResponseException("HTTP 状态码指示请求发生错误，状态为：" +
                                                                       (int) ((HttpWebResponse) ex
                                                                           .Response).StatusCode +
                                                                       " "                       +
                                                                       ((HttpWebResponse) ex
                                                                           .Response).StatusCode,
                                                                       ErrorType
                                                                           .HTTPStatusCodeError),
                                              ErrorType.HTTPStatusCodeError);
                        }
                    }
                    //超时
                    else if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        if (Params.IsThrowErrorForTimeout)
                        {
                            Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                                  ReqResponseException("HTTP 请求超时",
                                                                       ErrorType
                                                                           .HTTPRequestTimeout),
                                              ErrorType.HTTPRequestTimeout);
                        }

                        return new ReqResponse(new MemoryStream(), Params.Cookies, "", new UTF8Encoding(), 0);
                    }
                    //其他未知错误
                    else
                    {
                        Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                              ReqResponseException("HTTP 请求时发生错误", ex),
                                          ErrorType.HTTPRequestError);
                    }

                    response = (HttpWebResponse) ex.Response;
                }

                //确保报错后有Response
                if (response is null)
                {
                    return new ReqResponse(new MemoryStream(), Params.Cookies, "", new UTF8Encoding(), 0);
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
                    await (myResponseStream ?? throw new ReqResponseException("请求无响应", ErrorType.HTTPRequestTimeout))
                        .ReadAsync(buffer, 0, bufferSize);

                while (count > 0)
                {
                    responseStream.Write(buffer, 0, count);
                    count = await myResponseStream.ReadAsync(buffer, 0, bufferSize);
                }

                //编码自动判断
                if (response.ContentEncoding != "" && !(response.ContentEncoding is null))
                {
                    responseEncoding = Encoding.GetEncoding(response.ContentEncoding.ToLower());
                }
                else if (response.CharacterSet != "" && !(response.CharacterSet is null) &&
                         response.ContentType.Contains("charset"))
                {
                    responseEncoding = Encoding.GetEncoding(response.CharacterSet.ToLower() ??
                                                            throw new ReqResponseException("请求无响应",
                                                                ErrorType.HTTPRequestTimeout));
                }
                else
                {
                    responseEncoding = Encoding.UTF8;
                }

                //通过HTML头部的Meta tag判断编码
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
                Utils.HandleError(Params.UseHandler, ReqExceptionHandler, new
                                      ReqResponseException("HTTP 请求或解析响应时发生未知错误", ex),
                                  ErrorType.Other);
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