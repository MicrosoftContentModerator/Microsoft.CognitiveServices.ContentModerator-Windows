using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Microsoft.CognitiveServices.ContentModerator.Contract.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class ModeratorClient : IModeratorClient
    {
        /// <summary>
        /// The service host
        /// </summary>
        private const string DEFAULT_API_ROOT = "https://westus.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0";

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
        public ModeratorClient(string subscriptionKey) : this(subscriptionKey, DEFAULT_API_ROOT) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        public ModeratorClient(string subscriptionKey, string apiRoot)
        {
            _apiRoot = apiRoot?.TrimEnd('/');
            _subscriptionKey = subscriptionKey;
        }

        #region EvaluateImage
        public async Task<EvaluateImageResult> EvaluateImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage)
        {
            
            dynamic evaluateImageRequest = new ExpandoObject();
            evaluateImageRequest.DataRepresentation = dataRepresentationType.ToString();
            evaluateImageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            return await InvokeImageModeratorAsync<ExpandoObject, EvaluateImageResult>(evaluateImageRequest, Constants.Operations.Evaluate.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<EvaluateImageResult> EvaluateImageAsync(Stream content, bool cacheImage)
        {
            var imageType = ModeratorHelper.GetImageFormat(content);
            if (imageType.Equals(ModeratorHelper.ImageFormat.unknown))
            {
                throw new Exception($"Image type: {imageType} not supported");
            }
            content.Position = 0;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            return await InvokeImageModeratorAsync<Stream, EvaluateImageResult>(content, Constants.Operations.Evaluate.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region DetectFaces
        public async Task<DetectFacesResult> DetectFacesImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage)
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return await InvokeImageModeratorAsync<ExpandoObject, DetectFacesResult>(imageRequest, Constants.Operations.FindFaces.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<DetectFacesResult> DetectFacesImageAsync(Stream content, bool cacheImage)
        {
            var imageType = ModeratorHelper.GetImageFormat(content);
            if (imageType.Equals(ModeratorHelper.ImageFormat.unknown))
            {
                throw new Exception($"Image type: {imageType} not supported");
            }
            content.Position = 0;
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return await InvokeImageModeratorAsync<Stream, DetectFacesResult>(content, Constants.Operations.FindFaces.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region OCR
        public async Task<OcrImageResult> OCRImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage, bool enhanced = true, string language = "eng")
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            metaData.Add(new KeyValue()
            {
                Key = "enhanced",
                Value = enhanced.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });

            return
                await
                    InvokeImageModeratorAsync<ExpandoObject, OcrImageResult>(imageRequest,
                        Constants.Operations.OCR.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<OcrImageResult> OCRImageAsync(Stream content, bool cacheImage, bool enhanced = true, string language = "eng")
        {
            var imageType = ModeratorHelper.GetImageFormat(content);
            if (imageType.Equals(ModeratorHelper.ImageFormat.unknown))
            {
                throw new Exception($"Image type: {imageType} not supported");
            }
            content.Position = 0;
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            metaData.Add(new KeyValue()
            {
                Key = "enhanced",
                Value = enhanced.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });

            return await InvokeImageModeratorAsync<Stream, OcrImageResult>(content, Constants.Operations.OCR.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region Match
        public async Task<MatchResult> MatchImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage, string listid)
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            metaData.Add(new KeyValue()
            {
                Key = "listid",
                Value = listid
            });

            return
                await
                    InvokeImageModeratorAsync<ExpandoObject, MatchResult>(imageRequest,
                        Constants.Operations.Match.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<MatchResult> MatchImageAsync(Stream content, bool cacheImage, string listid)
        {
            var imageType = ModeratorHelper.GetImageFormat(content);
            if (imageType.Equals(ModeratorHelper.ImageFormat.unknown))
            {
                throw new Exception($"Image type: {imageType} not supported");
            }
            content.Position = 0;
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });
            metaData.Add(new KeyValue()
            {
                Key = "listid",
                Value = listid
            });

            return await InvokeImageModeratorAsync<Stream, MatchResult>(content, Constants.Operations.Match.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region Text

        public async Task<ScreenTextResult> ScreenTextAsync(string content, Constants.MediaType mediaType,
            string language, bool autocorrect, bool urls, bool pii, string listIds)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });
            metaData.Add(new KeyValue()
            {
                Key = "autocorrect",
                Value = autocorrect.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "urls",
                Value = urls.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "PII",
                Value = pii.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "listId",
                Value = listIds
            });

            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeTextModeratorAsync<string, ScreenTextResult>(content, Constants.Operations.Screen.ToString(),
                        metaData, mediaType).ConfigureAwait(false);
        }


        public async Task<IdentifyLanguageResult> IdentifyLanguageAsync(string content, Constants.MediaType mediaType)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = _subscriptionKey
            });

            return
                await
                    InvokeTextModeratorAsync<string, IdentifyLanguageResult>(content, Constants.Operations.DetectLanguage.ToString(),
                        metaData, mediaType).ConfigureAwait(false);
        }
        #endregion

        #region private methods

        private async Task<S> InvokeImageModeratorAsync<T, S>(dynamic imageRequest, string operation, List<KeyValue> metaData)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, $"/ProcessImage/{operation}?"));
            //var requestUrl = string.Concat(_apiRoot, $"/Image/{operation}?", $"CacheImage={cacheImage}&",
            //    $"subscription-key={_subscriptionKey}");
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.SendAsync<T, S>("POST", imageRequest, request)
                        .ConfigureAwait(false);
        }

        private async Task<S> InvokeTextModeratorAsync<T, S>(dynamic textRequest, string operation, List<KeyValue> metaData, Constants.MediaType mediaType)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, $"/ProcessText/{operation}?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());
            request.ContentType = ModeratorHelper.GetEnumDescription(mediaType);

            return
                await
                    this.SendAsync<T, S>("POST", textRequest, request)
                        .ConfigureAwait(false);
        }

        #endregion

        #region the json client

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
                    if(string.IsNullOrWhiteSpace(request.ContentType))
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

                var asyncState = new WebRequestAsyncState()
                {
                    RequestBytes = this.SerializeRequestBody(requestBody),
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
