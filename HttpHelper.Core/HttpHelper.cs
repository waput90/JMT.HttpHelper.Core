using HttpHelper.Core;
using HttpHelper.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpHelper.Common
{
    public class HttpHelper
    {
        #region Field Properties

        private bool IsKoreanCharSet { get; set; } = false;
        private string BaseUrl { get; set; } = string.Empty;
        private string Route { get; set; } = string.Empty;
        private string Content { get; set; } = string.Empty;
        private string AuthKey { get; set; } = string.Empty;
        private string MediaTypeHeader { get; set; } = MimeType.JSON;
        private string AppType { get; set; } = string.Empty;
        private HttpType? httpType { get; set; } = null;
        public string CustomAuthName { get; set; }
        public string CustomAuthValue { get; set; }
        public List<(string name, string value)> Headers { get; set; }
        public IHttpClientFactory Factory { get; set; } = null;
        public string Token { get; set; } = string.Empty;

        #endregion

        #region Methods
        public HttpHelper()
        {
            // initialize the list
            Headers = new List<(string name, string value)>();
        }
        public HttpHelper SetCustomHeader(string name, string value)
        {
            this.Headers.Add((name, value));
            return this;
        }

        public HttpHelper ToKoreanCharset()
        {
            this.IsKoreanCharSet = true;
            return this;
        }
        public HttpHelper SetBaseUrl(string baseUrl)
        {
            this.BaseUrl = baseUrl;
            return this;
        }

        public HttpHelper SetRoute(string route)
        {
            this.Route = route;
            return this;
        }

        public HttpHelper SetContent(string content)
        {
            this.Content = content;
            return this;
        }

        public HttpHelper SetAuthKey(string authKey)
        {
            this.AuthKey = authKey;
            return this;
        }

        public HttpHelper SetAppType(string appType)
        {
            this.AppType = appType;
            return this;
        }

        public HttpHelper SetMethod(HttpType type)
        {
            this.httpType = type;
            return this;
        }

        public HttpHelper SetMediaTypeHeader(string mediaHeader)
        {
            this.MediaTypeHeader = mediaHeader;
            return this;
        }

        public HttpHelper SetHttpClientFactory(IHttpClientFactory factory)
        {
            this.Factory = factory;
            return this;
        }

        public HttpHelper SetToken(string token)
        {
            this.Token = token;
            return this;
        }

        public async Task<TResponse> RequestDeserialize<TResponse>()
        {
            string request = await Request();

            return string.IsNullOrWhiteSpace(request) ? default : request.DeserializeTo<TResponse>();
        }

        public async Task<string> Request()
        {
            var client = this.Factory is null ? ServiceLocatorHelper.Current
                .GetInstance<IHttpClientFactory>()
                .CreateClient() : this.Factory.CreateClient();

            if (!string.IsNullOrWhiteSpace(this.BaseUrl))
                client.BaseAddress = new Uri(this.BaseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(this.MediaTypeHeader));

            if (!string.IsNullOrWhiteSpace(this.Token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);

            if (!this.Headers.IsNull())
                foreach (var h in Headers)
                    client.DefaultRequestHeaders.Add(h.name, h.value);
            try
            {
                HttpResponseMessage response = null;
                switch (httpType)
                {
                    case HttpType.GET:
                        {
                            response = await client.GetAsync($"{this.BaseUrl}{this.Route}{this.Content}");
                            break;
                        }
                    case HttpType.POST:
                        {
                            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, this.Route)
                            {
                                Content = new StringContent(this.Content, Encoding.UTF8, this.AppType)
                            };
                            response = await client.SendAsync(req);
                            break;
                        }
                    case HttpType.DELETE:
                        {
                            response = await client.DeleteAsync($"{this.BaseUrl}{this.Route}{this.Content}");
                            break;
                        }
                    case HttpType.PATCH:
                        {
                            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Patch, this.Route)
                            {
                                Content = new StringContent(this.Content, Encoding.UTF8, this.AppType)
                            };
                            response = await client.SendAsync(req);
                            break;
                        }
                }
                if (response.Content != null)
                {
                    // for korean charset response
                    if (this.IsKoreanCharSet)
                    {
                        // register encoding
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        Encoding euckr = Encoding.GetEncoding(51949);

                        var newResponse = await response.Content.ReadAsByteArrayAsync();
                        return euckr.GetString(newResponse, 0, newResponse.Length);
                    }
                    else
                        return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
            return null;
        }

        #endregion
    }
}
