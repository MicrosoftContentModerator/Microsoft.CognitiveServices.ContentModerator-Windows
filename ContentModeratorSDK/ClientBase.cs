using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator
{
    using System.IO;
    using System.Net;
    using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public abstract class ClientBase
    {

        /// <summary>
        /// The subscription key
        /// </summary>
        protected string SubscriptionKey;

        /// <summary>
        /// The root URI for Vision API
        /// </summary>
        protected string ApiRoot;

        /// <summary>
        /// Default timeout for calls
        /// </summary>
        protected const int DEFAULT_TIMEOUT = 2 * 60 * 1000; // 2 minutes timeout

        /// <summary>
        /// Default timeout for calls, overridable by subclasses
        /// </summary>
        protected virtual int DefaultTimeout => DEFAULT_TIMEOUT;

        /// <summary>
        /// The default resolver
        /// </summary>
        private readonly CamelCasePropertyNamesContractResolver defaultResolver = new CamelCasePropertyNamesContractResolver();


        protected async Task<TS> InvokeAsync<T, TS>(dynamic imageRequest, string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData = null)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));

            if (metaData != null)
            {
                foreach (var k in metaData)
                {
                    requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                    requestUrl.Append("&");
                }
            }
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.SendAsync<T, TS>(method.ToString(), imageRequest, request);
        }

        protected async Task<T> InvokeAsync<T>(string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData = null)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));

            if (metaData != null)
            {
                foreach (var k in metaData)
                {
                    requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                    requestUrl.Append("&");
                }
            }

            var request = WebRequest.Create(requestUrl.ToString());


            if (method != Constants.HttpMethod.GET)
            {
                request.ContentLength = 0;
            }



            return
                await
                    this.GetAsync<T>(method.ToString(), request);
        }




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
        protected async Task<TResponse> GetAsync<TResponse>(string method, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                request.Method = method;
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);

                if (null == setHeadersCallback)
                {
                    this.SetCommonHeaders(request);
                }
                else
                {
                    setHeadersCallback(request);
                }

                var getResponse = Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);

                await Task.WhenAny(getResponse, Task.Delay(this.DefaultTimeout));

                //Abort request if timeout has expired
                if (!getResponse.IsCompleted)
                {
                    request.Abort();
                }

                return this.ProcessAsyncResponse<TResponse>(getResponse.Result as HttpWebResponse);
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
        protected async Task<TResponse> SendAsync<TRequest, TResponse>(string method, TRequest requestBody, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                request.Method = method;
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);

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
                    request.ContentType = $"image/jpeg";
                }

                var asyncState = new WebRequestAsyncState()
                {
                    RequestBytes = this.SerializeRequestBody(requestBody, request.ContentType),
                    WebRequest = (HttpWebRequest)request,
                };

                var continueRequestAsyncState = await Task.Factory.FromAsync<Stream>(
                                                    asyncState.WebRequest.BeginGetRequestStream,
                                                    asyncState.WebRequest.EndGetRequestStream,
                                                    asyncState,
                                                    TaskCreationOptions.None).ContinueWith<WebRequestAsyncState>(
                                                       task =>
                                                       {
                                                           var requestAsyncState = (WebRequestAsyncState)task.AsyncState;
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
                                                       });

                var continueWebRequest = continueRequestAsyncState.WebRequest;
                var getResponse = Task.Factory.FromAsync<WebResponse>(
                    continueWebRequest.BeginGetResponse,
                    continueWebRequest.EndGetResponse,
                    continueRequestAsyncState);

                await Task.WhenAny(getResponse, Task.Delay(this.DefaultTimeout));

                //Abort request if timeout has expired
                if (!getResponse.IsCompleted)
                {
                    request.Abort();
                }

                return this.ProcessAsyncResponse<TResponse>(getResponse.Result as HttpWebResponse);
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
        protected T ProcessAsyncResponse<T>(HttpWebResponse webResponse)
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
                                    string message;
                                    using (var reader = new StreamReader(stream))
                                    {
                                        message = reader.ReadToEnd();
                                    }

                                    JsonSerializerSettings settings = new JsonSerializerSettings();
                                    settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                                    settings.NullValueHandling = NullValueHandling.Ignore;
                                    settings.ContractResolver = this.defaultResolver;

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
        /// Serialize the request body to byte array
        /// </summary>
        /// <typeparam name="T">Type of request object</typeparam>
        /// <param name="requestBody">Strong typed request object</param>
        /// <param name="contentType">Content Type of the Request</param>
        /// <returns>Byte array</returns>
        private byte[] SerializeRequestBody<T>(T requestBody, string contentType)
        {
            byte[] data = null;
            if (requestBody != null && !(requestBody is Stream))
            {
                if (contentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    settings.ContractResolver = this.defaultResolver;
                    data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody, settings));                }                else                {
                    data = System.Text.Encoding.UTF8.GetBytes(requestBody as string);                }
            }

            return data;
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
                        stream?.Dispose();
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
    }
}
