using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using XLabs.Cryptography;
using System.Net;

namespace CYINT.XPlatformNetworking
{
    public class XPlatformNetworkingImplementation
    {
        protected HttpClient _httpClient;
        protected HttpClientHandler _handler;
        protected HttpResponseMessage _response;
        protected Dictionary<string, string> _cookies;
        protected Dictionary<string, string> _headers;
        protected bool _followRedirects;
        protected int _maxResponseContentBufferSize;
        public string _baseUrl;
        
        public XPlatformNetworkingImplementation(string baseUrl)
        {
            _cookies = new Dictionary<string,string>();
            _headers = new Dictionary<string, string>();
            _maxResponseContentBufferSize = 256000;
            _followRedirects = true;
            SetBaseUrl(baseUrl);
        }

        public bool FollowRedirectsAreEnabled()
        {
            return _followRedirects;
        }

        public bool IsNetworkAvailable()
        {
            return DependencyService.Get<IXPlatformNetworking>().IsNetworkAvailable();
        }

        public HttpClient GetHttpClient()
        {
            if(_httpClient == null)                                
                InitializeClient();

            return _httpClient;
        } 

        public void ClearHeaders()
        {
            _headers.Clear();
            InitializeClient();
        }

        public void ClearCookies()
        {
            _cookies.Clear();
            InitializeClient();
        }


        public async Task<byte[]> FetchBinaryData(string path)
        { 
            Uri uri;
            HttpResponseMessage response;
            response = null;
            byte[] file;

            if(IsNetworkAvailable())
            {
                uri = new Uri(string.Format( path, string.Empty));
                response = await GetHttpClient().GetAsync(uri);
                SetResponse(response);

                if( response.IsSuccessStatusCode )
                {
                    file = await response.Content.ReadAsByteArrayAsync();
                    return file; 
                }                                         
            }

            throw new XPlatformNetworkingImplementationException("Network call failed, and no local storage data exists.");
       
        }



        public async Task<string> FetchData(
            string key
            ,string path
            ,bool isPost = false
            ,string postData = null
            ,Encoding encoding = null
            ,string mediaType = "application/x-www-form-urlencoded"
        )
        { 
            Uri uri;
            HttpResponseMessage response;
            string hashedKey;

            hashedKey = MD5.GetMd5String(key);
            response = null;

            if(IsNetworkAvailable())
            {

                encoding = encoding ??  Encoding.UTF8;
                uri = new Uri(string.Format( GetBaseUrl() + path, string.Empty));

                if(isPost)
                {
                    StringContent payload = new StringContent(postData, encoding, mediaType);
                    response = await GetHttpClient().PostAsync(uri, payload);
                }
                else
                {
                    response = await GetHttpClient().GetAsync(uri);
                }

                SetResponse(response);

                if( response.IsSuccessStatusCode || ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    PlaceInLocalStorage(hashedKey, content);
                    return content; 
                }                                         
            }
       
            try
            {
                return await RetrieveFromLocalStorage(hashedKey);
            }
            catch(Exception Ex)
            {                  
                string message = Ex.Message;
                throw new XPlatformNetworkingImplementationException("Network call failed, and no local storage data exists.", response);
            }
       
        }

        public void SetMaxResponseContentBufferSize(int size)
        {
            _maxResponseContentBufferSize = size;
            InitializeClient();
        }

        public int GetMaxResponseContentBufferSize()
        {
            return _maxResponseContentBufferSize;
        }

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        public HttpResponseMessage GetResponse()
        {
            return _response;
        }

        public void DisableFollowRedirect(bool nofollow = true)
        {
            _followRedirects = !nofollow;
            InitializeClient();
        }

        public CookieContainer GetCookieContainer()
        {
            if(_handler != null)
                return _handler.CookieContainer;

            throw new XPlatformNetworkingImplementationException("Handler has not been initialized.");
        }

        public void PlaceInLocalStorage(string key, string content)
        {
            DependencyService.Get<IXPlatformNetworking>().PlaceInLocalStorage(key, content);
        }

        public async Task<string> RetrieveFromLocalStorage(string key)
        {
            return await DependencyService.Get<IXPlatformNetworking>().RetrieveFromLocalStorage(key);
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string GetBaseUrl()
        {
            return _baseUrl;
        }

        public void SetClientHandler(HttpClientHandler handler)
        {
            _handler = handler;
            InitializeClient();
        }

        public void SetCookies(Dictionary<string, string> cookies)
        {
            _cookies = cookies;
            InitializeClient();
        }

        public Dictionary<string,string> GetCookies()
        {
            return _cookies;
        }

        public void AddCookie(string name, string value)
        {
            _cookies.Add(name, value);
            InitializeClient();
        }

        public string GetCookie(string name)
        {
            if(_cookies.ContainsKey(name))
                return _cookies[name];
            
            throw new XPlatformNetworkingImplementationException("Cookie does not exist");
        }

        public bool CookieExists(string name)
        {
            if(_cookies.ContainsKey(name))
                return true;

            return false;
        }

        public bool HeaderExists(string name)
        {
            if(_headers.ContainsKey(name))
                return true;

            return false;
        }

        public void RemoveCookie(string name)
        {
            if(_cookies.ContainsKey(name))
            {
                _cookies.Remove(name);
                InitializeClient();
            }
            else
                throw new XPlatformNetworkingImplementationException("Cookie does not exist");

        }

        public Dictionary<string, string> GetHeaders()
        {
            return _headers;
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            InitializeClient();
        }

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
            InitializeClient();
        }

        public void RemoveHeader(string name)
        {
            if(_headers.ContainsKey(name))
            {
                _headers.Remove(name);
                InitializeClient();
            }
            else
                throw new XPlatformNetworkingImplementationException("Header does not exist.");
        }

        public string GetHeader(string name)
        {
            if(_headers.ContainsKey(name))
                return _headers[name];

            throw new XPlatformNetworkingImplementationException("Header does not exist.");
        }

        public void InitializeClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookieJar = new CookieContainer();                
            
            if(GetCookies().Count > 0)
            {
                foreach ( KeyValuePair<string, string> cookie in GetCookies())
                {
                    cookieJar.Add(new Uri(GetBaseUrl()), new Cookie(cookie.Key, cookie.Value));
                }
            }

            handler.CookieContainer = cookieJar;
            _handler = handler;
            _httpClient = new HttpClient(_handler);

            _httpClient.MaxResponseContentBufferSize = _maxResponseContentBufferSize;
            
            if(GetHeaders().Count > 0)
            {
                foreach (KeyValuePair<string, string> header in GetHeaders())
                {
                    if(!_httpClient.DefaultRequestHeaders.Contains(header.Key))
                        _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public HttpClientHandler GetClientHandler()
        {
            if(_handler == null)            
                InitializeClient();            

            return _handler;
        }

    }

    public class XPlatformNetworkingImplementationException : Exception
    {
        protected HttpResponseMessage _response;
        public XPlatformNetworkingImplementationException (string message, HttpResponseMessage response = null) : base (message)
        {
            SetResponse(response);
        }

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        public HttpResponseMessage GetResponse()
        {
            return _response;
        }
    }



    public class XPlatformNetworkingImplementationNotCachedException : Exception
    {
        protected string _path;
        public XPlatformNetworkingImplementationNotCachedException (string message, string path) : base (message)
        {
            SetPath(path);
        }

        public string GetPath()
        {
            return _path;
        }

        public void SetPath(string path)
        {
            _path = path;
        }
    }
}
