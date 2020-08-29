using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace PyLibSharp.Requests
{
    public class ReqParams
    {
        public Dictionary<HttpRequestHeader, string> Header                    { get; set; }
        public WebProxy                              ProxyToUse                { get; set; }
        public CookieContainer                       Cookies                   { get; set; }
        public Dictionary<string, string>            CustomHeader              { get; set; }
        public Dictionary<string, string>            Params                    { get; set; }
        public byte[]                                RawPostParams             { get; set; }
        public string                                PostJson                  { get; set; }
        public MultipartFormDataContent              PostMultiPart             { get; set; }
        public Encoding                              PostEncoding              { get; set; }
        public PostType                              PostParamsType            { get; set; }
        public bool                                  IsStream                  { get; set; }
        public bool                                  IsUseHtmlMetaEncoding     { get; set; }
        public bool                                  IsThrowErrorForStatusCode { get; set; }
        public int                                   Timeout                   { get; set; }
        public int                                   ReadBufferSize            { get; set; }

        public ReqParams()
        {
            Header                    = new Dictionary<HttpRequestHeader, string>();
            Cookies                   = new CookieContainer();
            CustomHeader              = new Dictionary<string, string>();
            Params                    = new Dictionary<string, string>();
            IsStream                  = false;
            IsUseHtmlMetaEncoding     = false;
            IsThrowErrorForStatusCode = true;
            PostEncoding              = new System.Text.UTF8Encoding(false);
            PostParamsType            = PostType.none;
            Timeout                   = 500;
            ReadBufferSize            = 1024;
        }
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

        public Encoding       Encode     { get; }
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
        public static ReqResponse Get(string Url, ReqParams Params = null)
        {
            return RequestBase(Url, "GET", Params).Result;
        }

        public static ReqResponse Post(string Url, ReqParams Params = null)
        {
            return RequestBase(Url, "POST", Params).Result;
        }
        public static async Task<ReqResponse> GetAsync(string Url, ReqParams Params = null)
        {
            return await RequestBase(Url, "GET", Params);
        }

        public static async Task<ReqResponse> PostAsync(string Url, ReqParams Params = null)
        {
            return await RequestBase(Url, "POST", Params);
        }

        public static async Task<ReqResponse> RequestBase(string Url, string Method, ReqParams Params = null)
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentNullException(nameof(Url));
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

                if (!string.IsNullOrEmpty(Params.PostJson))
                {
                    Params.PostParamsType = PostType.json;
                }

                if (Params.RawPostParams != null && Params.RawPostParams.Length != 0)
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
                    paramStr = (urlToSend.Query.StartsWith("?") ? urlToSend.Query : "?" + urlToSend.Query) +
                               (urlToSend.Query.EndsWith("&")
                                   ? ""
                                   : ((urlToSend.Query != "" && urlToSend.Query != "?" && paramStr != "") ? "&" : ""))
                             + paramStr;
                    urlParsed += ((urlToSend.AbsolutePath == "/" && !Url.EndsWith("/")) ? "/" : "") + paramStr;
                    request   =  (HttpWebRequest) WebRequest.Create(urlParsed);
                }
                else
                {
                    request = (HttpWebRequest) WebRequest.Create(urlToSend);
                }
            }
            catch (Exception ex)
            {
                throw new ReqUrlException("构造 URL 时发生错误，请检查 URL 格式和请求参数", ex);
            }

            request.Method  = Method;
            request.Timeout = Params.Timeout;
            request.Proxy   = Params.ProxyToUse;

            try
            {
                //头部处理部分
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
                throw new ReqHeaderException("构造 HTTP 头部时发生错误", ex);
            }


            Stream myResponseStream = null;
            //StreamReader myStreamReader   = null;
            int bufferSize = Params.ReadBufferSize;

            Encoding        responseEncoding;
            MemoryStream    responseStream = new MemoryStream();
            HttpStatusCode  responseStatusCode;
            string          responseContentType     = "";
            CookieContainer responseCookieContainer = new CookieContainer();

            if (Method == "POST" || Method == "PUT")
            {
                switch (Params.PostParamsType)
                {
                    case PostType.x_www_form_urlencoded:
                        if (string.IsNullOrEmpty(paramStr))
                        {
                            throw new
                                ReqRequestException("以 application/x-www-form-urlencoded 类型 POST 时，Params 参数未设置或为空",
                                                    new ArgumentNullException("Params"));
                        }

                        request.ContentType =
                            "application/x-www-form-urlencoded;charset=" + Params.PostEncoding.WebName;
                        byte[] data = Params.PostEncoding.GetBytes(paramStr.ToString());
                        using (Stream stream = request.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }

                        break;
                    case PostType.form_data:
                        if (Params.PostMultiPart is null)
                        {
                            throw new ReqRequestException("以 multipart/formdata 类型 POST 时，PostMultiPart 参数未设置或为空",
                                                          new ArgumentNullException("PostMultiPart"));
                        }

                        var dat  = Params.PostMultiPart;
                        var task = dat.ReadAsByteArrayAsync();
                        request.ContentType   = dat.Headers.ContentType.ToString();
                        request.ContentLength = dat.Headers.ContentLength.Value;

                        using (Stream stream = request.GetRequestStream())
                        {
                            stream.Write(task.Result, 0, task.Result.Length);
                        }

                        break;
                    case PostType.raw:
                        if (Params.RawPostParams is null || Params.RawPostParams.Length == 0)
                        {
                            throw new ReqRequestException("以 Raw 类型 POST 时，RawPostParams 参数未设置或为空",
                                                          new ArgumentNullException("RawPostParams"));
                        }

                        using (Stream stream = request.GetRequestStream())
                        {
                            stream.Write(Params.RawPostParams, 0, Params.RawPostParams.Length);
                        }

                        break;
                    case PostType.json:
                        if (string.IsNullOrEmpty(Params.PostJson))
                        {
                            throw new ReqRequestException("以 Json 类型 POST 时，PostJson 参数未设置或为空",
                                                          new ArgumentNullException("PostJson"));
                        }

                        request.ContentType = "application/json;charset=" + Params.PostEncoding.WebName;
                        byte[] jsonData = Params.PostEncoding.GetBytes(Params.PostJson.ToString());
                        using (Stream stream = request.GetRequestStream())
                        {
                            stream.Write(jsonData, 0, jsonData.Length);
                        }

                        break;
                }
            }


            try
            {
                request.CookieContainer = Params.Cookies;
                HttpWebResponse response = (HttpWebResponse) (await request.GetResponseAsync());
               
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
                    throw new ReqRequestException("HTTP 状态码指示请求发生错误，状态码为：" + response.StatusCode);
                }

                myResponseStream = response.GetResponseStream();
                // myStreamReader =
                //     new StreamReader(myResponseStream ?? throw new ReqResponseException("请求无响应"),
                //                      Encoding.GetEncoding(response.CharacterSet ??
                //                                           throw new ReqResponseException("请求无响应")));
                //

                byte[] buffer = new byte[bufferSize];
                int count =
                    (myResponseStream ?? throw new ReqResponseException("请求无响应")).Read(buffer, 0, bufferSize);
                while (count > 0)
                {
                    responseStream.Write(buffer, 0, count);
                    count = myResponseStream.Read(buffer, 0, bufferSize);
                }

                if (response.CharacterSet != "")
                {
                    responseEncoding = Encoding.GetEncoding(response.CharacterSet ??
                                                            throw new ReqResponseException("请求无响应"));
                }
                else if (response.ContentEncoding != "")
                {
                    responseEncoding = Encoding.GetEncoding(response.ContentEncoding);
                }
                else
                {
                    responseEncoding = Encoding.Default;
                }

                responseStatusCode  = response.StatusCode;
                responseContentType = response.ContentType;
                responseCookieContainer.Add(response.Cookies);

                //通过HTML头部的Meta判断编码
                if (Params.IsUseHtmlMetaEncoding && response.ContentType.ToLower().IndexOf("text/html") != -1)
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
            }
            catch (Exception ex)
            {
                throw new ReqResponseException("请求时发生错误", ex);
            }
            finally
            {
                //myStreamReader?.Close();
                myResponseStream?.Close();
            }

            return new ReqResponse(responseStream, responseCookieContainer, responseContentType, responseEncoding,
                                   responseStatusCode);
        }
    }
}