using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.CognitiveServices.ContentModerator
{
    using Microsoft.CognitiveServices.ContentModerator.Contract;
    using Microsoft.CognitiveServices.ContentModerator.Contract.Text;

    public class ListManagementClient : IListManagementClient
    {

        /// <summary>
        /// The service host
        /// </summary>
        private const string DEFAULT_API_ROOT = "https://westus.api.cognitive.microsoft.com/contentmoderator/lists/v1.0";

        /// <summary>
        /// The subscription key
        /// </summary>
        private string _subscriptionKey;

        /// <summary>
        /// The root URI for Vision API
        /// </summary>
        private readonly string _apiRoot;

        /// <summary>
        /// Default timeout for calls
        /// </summary>
        private const int DEFAULT_TIMEOUT = 2 * 60 * 1000; // 2 minutes timeout

        /// <summary>
        /// Default timeout for calls, overridable by subclasses
        /// </summary>
        protected virtual int DefaultTimeout => DEFAULT_TIMEOUT;

        /// <summary>
        /// The default resolver
        /// </summary>
        private CamelCasePropertyNamesContractResolver _defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public ListManagementClient(string subscriptionKey) : this(subscriptionKey, DEFAULT_API_ROOT) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        public ListManagementClient(string subscriptionKey, string apiRoot)
        {
            _apiRoot = apiRoot?.TrimEnd('/');
            _subscriptionKey = subscriptionKey;
        }

        #region ImageAPI

        #region Image Add
        public async Task<ImageAddResult> ImageAddAsync(string content, DataRepresentationType dataRepresentationType,
            string listId, string tag,
            string label)
        {
            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "tag",
                Value = tag
            });
            metaData.Add(new KeyValue()
            {
                Key = "label",
                Value = label
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ImageAddResult>(imageRequest,
                        string.Format(Constants.IMAGE_ADD, listId), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ImageAddResult> ImageAddAsync(Stream content, string listId, string tag, string label)
        {
            dynamic imageRequest = new ExpandoObject();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "tag",
                Value = tag
            });
            metaData.Add(new KeyValue()
            {
                Key = "label",
                Value = label
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ImageAddResult>(imageRequest,
                        string.Format(Constants.IMAGE_ADD, listId), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ImageListResult> ImageGetAllIdsAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ImageListResult>(string.Format(Constants.IMAGE_GETALLIDS, listId), Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);

        }

        public async Task<string> ImageDeleteAllAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.IMAGE_DELETEALL, listId),
                        Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> ImageDeleteAsync(string listId, string imageId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            
            return
                await
                    InvokeAsync<string>(string.Format(Constants.IMAGE_DELETE, listId, imageId),
                        Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }
        #endregion
        
        #region ImageLists

        public async Task<ListItemResult> ImageListCreateAsync(string name, string description,
            Dictionary<string, string> listMetaData)
        {
            dynamic imageListRequest = new ExpandoObject();
            imageListRequest.Name = name;
            imageListRequest.Description = description;
            imageListRequest.Metadata = listMetaData;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(imageListRequest,
                        Constants.IMAGELIST_CREATE, Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ListItemResult> ImageListUpdateAsync(string listId, string name, string description,
            Dictionary<string, string> listMetaData)
        {
            dynamic imageListRequest = new ExpandoObject();
            imageListRequest.Name = name;
            imageListRequest.Description = description;
            imageListRequest.Metadata = listMetaData;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(imageListRequest,
                        string.Format(Constants.IMAGELIST_UPDATE, listId), Constants.HttpMethod.PUT, metaData)
                        .ConfigureAwait(false);

        }

        public async Task<ImageRefreshIndexResult> ImageListRefreshIndexAsync(string listId)
        {
            dynamic imageRequest = new ExpandoObject();

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ImageRefreshIndexResult>(imageRequest,
                        string.Format(Constants.IMAGELIST_REFRESHINDEX, listId), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);

        }
        public async Task<List<ListItemResult>> ImageListGetAllAsync()
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<List<ListItemResult>>(Constants.IMAGELIST_GETALL, Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }
        public async Task<ListItemResult> ImageListDetailAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ListItemResult>(string.Format(Constants.IMAGELIST_GETDETAIL, listId),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> ImageListDeleteAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.IMAGELIST_DELETE, listId),
                        Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> TermAddAsync(string listId, string term, string language)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.TERM_ADD, listId, term), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> TermDeleteAsync(string listId, string term, string language)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.TERM_DELETE, listId, term), Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> TermDeleteAllAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.TERM_DELETEALL, listId), Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<TermGetAllResult> TermGetAllTermsAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<TermGetAllResult>(string.Format(Constants.TERM_GETALLIDS, listId), Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> TermListRefreshIndexAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.TERMLIST_REFRESHINDEX, listId), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<List<ListItemResult>> TermListGetAllAsync()
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<List<ListItemResult>>(Constants.TERMLIST_GETALL, Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ListItemResult> TermListCreateAsync(string name, string description, Dictionary<string, string> listMetaData)
        {
           dynamic termlistrequest = new ExpandoObject();
            termlistrequest.Name = name;
            termlistrequest.Description = description;
            termlistrequest.Metadata = listMetaData;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(termlistrequest,
                        Constants.TERMLIST_CREATE, Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ListItemResult> TermListUpdateAsync(string listId, string name, string description, Dictionary<string, string> listMetaData)
        {
            dynamic imageListRequest = new ExpandoObject();
            imageListRequest.Name = name;
            imageListRequest.Description = description;
            imageListRequest.Metadata = listMetaData;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(imageListRequest,
                        string.Format(Constants.TERMLIST_UPDATE, listId), Constants.HttpMethod.PUT, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<ListItemResult> TermListDetailAsync(string listId)
        {
             List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<ListItemResult>(string.Format(Constants.TERMLIST_GETDETAIL, listId),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string> TermListDeleteAsync(string listId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeAsync<string>(string.Format(Constants.TERMLIST_DELETE, listId),
                        Constants.HttpMethod.DELETE, metaData)
                        .ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Private methods

        private async Task<S> InvokeAsync<T, S>(dynamic imageRequest, string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.SendAsync<T, S>(method.ToString(), imageRequest, request)
                        .ConfigureAwait(false);
        }

        private async Task<T> InvokeAsync<T>(string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData)
        {
            //StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, $"/Image/{operationUrl}?"));
            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.GetAsync<T>(method.ToString(), request)
                        .ConfigureAwait(false);
        }
        #endregion

        #region the json client

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="request">The request.</param>
        /// <param name="setHeadersCallback">The set headers callback.</param>
        /// <returns>
        /// The response object.
        /// </returns>
        private async Task<TResponse> GetAsync<TResponse>(string method, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            if (request == null)
            {
                new ArgumentNullException("request");
            }

            try
            {
                request.Method = method;
                if (null == setHeadersCallback)
                {
                    this.SetCommonHeaders(request);
                }
                else
                {
                    setHeadersCallback(request);
                }

                var getResponseAsync = Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);

                await Task.WhenAny(getResponseAsync, Task.Delay(DefaultTimeout)).ConfigureAwait(false);

                //Abort request if timeout has expired
                if (!getResponseAsync.IsCompleted)
                {
                    request.Abort();
                }

                return this.ProcessAsyncResponse<TResponse>(getResponseAsync.Result as HttpWebResponse);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    this.HandleException(e);
                    return true;
                });
                return default(TResponse);
            }
            catch (Exception e)
            {
                this.HandleException(e);
                return default(TResponse);
            }
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="request">The request.</param>
        /// <param name="setHeadersCallback">The set headers callback.</param>
        /// <returns>The response object.</returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        private async Task<TResponse> SendAsync<TRequest, TResponse>(string method, TRequest requestBody, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                request.Method = method;
                if (null == setHeadersCallback)
                {
                    if (string.IsNullOrWhiteSpace(request.ContentType))
                        this.SetCommonHeaders(request);
                }
                else
                {
                    setHeadersCallback(request);
                }

                if (requestBody is Stream)
                {
                    //request.ContentType = "application/octet-stream";
                    request.ContentType = $"image/jpeg";
                }

                var asyncState = new ModeratorClient.WebRequestAsyncState()
                {
                    RequestBytes = this.SerializeRequestBody(requestBody),
                    WebRequest = (HttpWebRequest)request,
                };

                var continueRequestAsyncState = await Task.Factory.FromAsync<Stream>(
                                                    asyncState.WebRequest.BeginGetRequestStream,
                                                    asyncState.WebRequest.EndGetRequestStream,
                                                    asyncState,
                                                    TaskCreationOptions.None).ContinueWith<ModeratorClient.WebRequestAsyncState>(
                                                       task =>
                                                       {
                                                           var requestAsyncState = (ModeratorClient.WebRequestAsyncState)task.AsyncState;
                                                           if (requestBody != null)
                                                           {
                                                               using (var requestStream = task.Result)
                                                               {
                                                                   if (requestBody is Stream)
                                                                   {
                                                                       (requestBody as Stream).CopyTo(requestStream);
                                                                   }
                                                                   else
                                                                   {
                                                                       requestStream.Write(requestAsyncState.RequestBytes, 0, requestAsyncState.RequestBytes.Length);
                                                                   }
                                                               }
                                                           }

                                                           return requestAsyncState;
                                                       }).ConfigureAwait(false);

                var continueWebRequest = continueRequestAsyncState.WebRequest;
                var getResponseAsync = Task.Factory.FromAsync<WebResponse>(
                    continueWebRequest.BeginGetResponse,
                    continueWebRequest.EndGetResponse,
                    continueRequestAsyncState);

                await Task.WhenAny(getResponseAsync, Task.Delay(DefaultTimeout)).ConfigureAwait(false);

                //Abort request if timeout has expired
                if (!getResponseAsync.IsCompleted)
                {
                    request.Abort();
                }

                return this.ProcessAsyncResponse<TResponse>(getResponseAsync.Result as HttpWebResponse);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    this.HandleException(e);
                    return true;
                });
                return default(TResponse);
            }
            catch (Exception e)
            {
                this.HandleException(e);
                return default(TResponse);
            }
        }

        /// <summary>
        /// Processes the asynchronous response.
        /// </summary>
        /// <typeparam name="T">Type of response.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <returns>The response.</returns>
        private T ProcessAsyncResponse<T>(HttpWebResponse webResponse)
        {
            using (webResponse)
            {
                if (webResponse.StatusCode == HttpStatusCode.OK ||
                    webResponse.StatusCode == HttpStatusCode.Accepted ||
                    webResponse.StatusCode == HttpStatusCode.Created)
                {
                    if (webResponse.ContentLength != 0)
                    {
                        using (var stream = webResponse.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                if (webResponse.ContentType == "image/jpeg" ||
                                    webResponse.ContentType == "image/png")
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        return (T)(object)ms.ToArray();
                                    }
                                }
                                else
                                {
                                    string message = string.Empty;
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        message = reader.ReadToEnd();
                                    }

                                    JsonSerializerSettings settings = new JsonSerializerSettings();
                                    settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                                    settings.NullValueHandling = NullValueHandling.Ignore;
                                    settings.ContractResolver = this._defaultResolver;

                                    return JsonConvert.DeserializeObject<T>(message, settings);
                                }
                            }
                        }
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Set request content type.
        /// </summary>
        /// <param name="request">Web request object.</param>
        private void SetCommonHeaders(WebRequest request)
        {
            request.ContentType = "application/json";
        }

        /// <summary>
        /// Serialize the request body to byte array.
        /// </summary>
        /// <typeparam name="T">Type of request object.</typeparam>
        /// <param name="requestBody">Strong typed request object.</param>
        /// <returns>Byte array.</returns>
        private byte[] SerializeRequestBody<T>(T requestBody)
        {
            if (requestBody == null || requestBody is Stream)
            {
                return null;
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.ContractResolver = this._defaultResolver;

                return System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody, settings));
            }
        }

        /// <summary>
        /// Process the exception happened on rest call.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        private void HandleException(Exception exception)
        {
            WebException webException = exception as WebException;
            if (webException != null && webException.Response != null)
            {
                if (webException.Response.ContentType.ToLower().Contains("application/json"))
                {
                    Stream stream = null;

                    try
                    {
                        stream = webException.Response.GetResponseStream();
                        if (stream != null)
                        {
                            string errorObjectString;
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                stream = null;
                                errorObjectString = reader.ReadToEnd();
                            }

                            ClientError errorCollection = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                            if (errorCollection != null)
                            {
                                throw new ClientException
                                {
                                    Error = errorCollection,
                                };
                            }
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Dispose();
                        }
                    }
                }
            }

            throw exception;
        }

        /// <summary>
        /// This class is used to pass on "state" between each Begin/End call
        /// It also carries the user supplied "state" object all the way till
        /// the end where is then hands off the state object to the
        /// WebRequestCallbackState object.
        /// </summary>
        internal class WebRequestAsyncState
        {
            /// <summary>
            /// Gets or sets request bytes of the request parameter for http post.
            /// </summary>
            public byte[] RequestBytes { get; set; }

            /// <summary>
            /// Gets or sets the HttpWebRequest object.
            /// </summary>
            public HttpWebRequest WebRequest { get; set; }

            /// <summary>
            /// Gets or sets the request state object.
            /// </summary>
            public object State { get; set; }
        }


        #endregion
    }
}
