using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Review;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class ReviewClient : IReviewClient
    {
        /// <summary>
        /// The service host
        /// </summary>
        private const string DEFAULT_API_ROOT = "https://westus.api.cognitive.microsoft.com/contentmoderator/review/v1.0";

        /// <summary>
        /// AD tenent url
        /// </summary>
        private const string DEFAULT_AD_TENANT_URL = "https://login.microsoftonline.com/contentmoderatorprod.onmicrosoft.com/oauth2/token";

        /// <summary>
        /// AD tenent url
        /// </summary>
        private string DEFAULT_AD_REVIEW_SVC_URL = Uri.EscapeUriString("https://api.contentmoderator.cognitive.microsoft.com/review");

        /// <summary>
        /// The subscription key
        /// </summary>
        private string _subscriptionKey;

        /// <summary>
        /// The root URI for Vision API
        /// </summary>
        private readonly string _apiRoot;

        /// <summary>
        /// The client Id
        /// </summary>
        private string _clientId;

        /// <summary>
        /// The client secret
        /// </summary>
        private string _clientSecret;

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
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public ReviewClient(string subscriptionKey, string clientId, string clientSecret) : this(subscriptionKey, DEFAULT_API_ROOT, clientId, clientSecret) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public ReviewClient(string subscriptionKey, string apiRoot, string clientId, string clientSecret)
        {
            _apiRoot = apiRoot?.TrimEnd('/');
            _subscriptionKey = subscriptionKey;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        #region Review Operations

        public async Task<ReviewResponse> GetReview(string teamName, string reviewId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            return
                await
                    InvokeAsync<ReviewResponse>(string.Format(Constants.GET_REVIEW_DETAILS, teamName, reviewId),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<string[]> CreateReview(string teamName, List<ReviewRequest> reviewRequests)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            return
                await
                    InvokeAsync<List<ReviewRequest>, string[]>(reviewRequests,
                        string.Format(Constants.CREATE_REVIEW, teamName), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        #endregion

        #region Job Operations

        public async Task<JobDetailsResult> GetJobDetails(string teamName, string jobId)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            return
                await
                    InvokeAsync<JobDetailsResult>(string.Format(Constants.GET_JOB_DETAILS, teamName, jobId),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<CreateJobResult> CreateJob(string teamName, string content, ContentType contentType, string contentId,
            string workFlowName, string callBackEndpoint)
        {
            dynamic jobRequest = new ExpandoObject();
            jobRequest.ContentValue = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "ContentType",
                Value = contentType.ToString()
            });

            metaData.Add(new KeyValue()
            {
                Key = "ContentId",
                Value = contentId
            });

            metaData.Add(new KeyValue()
            {
                Key = "WorkflowName",
                Value = workFlowName
            });

            metaData.Add(new KeyValue()
            {
                Key = "CallBackEndpoint",
                Value = callBackEndpoint
            });

            return
                await
                    InvokeAsync<ExpandoObject, CreateJobResult>(jobRequest,
                        string.Format(Constants.CREATE_JOB, teamName), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        public async Task<CreateJobResult> CreateJob(string teamName, Stream content, ContentType contentType,
            string contentId, string workFlowName,
            string callBackEndpoint)
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
                Key = "ContentType",
                Value = contentType.ToString()
            });

            metaData.Add(new KeyValue()
            {
                Key = "ContentId",
                Value = contentId
            });

            metaData.Add(new KeyValue()
            {
                Key = "WorkflowName",
                Value = workFlowName
            });

            metaData.Add(new KeyValue()
            {
                Key = "CallBackEndpoint",
                Value = callBackEndpoint
            });

            return
                await
                    InvokeAsync<Stream, CreateJobResult>(content,
                        string.Format(Constants.CREATE_JOB, teamName), Constants.HttpMethod.POST, metaData)
                        .ConfigureAwait(false);
        }

        #endregion

        #region Workflow Operations

        public async Task<List<WorkFlowItem>> GetAllWorkflows(string teamName)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            List<WorkFlowItem> workFlowList = new List<WorkFlowItem>();

            JArray result =
                await
                    InvokeAsync<JArray>(string.Format(Constants.GET_ALL_TEAM_WORKFLOWS, teamName),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);

            if (result?.Count > 0)
            {
                List<string> connectorNames = new List<string>();
                foreach (var wkf in result)
                {
                    JObject wItem = wkf as JObject;
                    WorkFlowItem item = new WorkFlowItem()
                    {
                        Name = wItem?["Name"].Value<string>(),
                        Description = wItem?["Description"].Value<string>(),
                        Expression = GetExpression(wItem?["Expression"] as JObject, connectorNames),
                    };
                    workFlowList.Add(item);
                }
            }

            return workFlowList;
        }

        public async Task<WorkFlowItem> GetWorkflow(string teamName, string workFlowName)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            WorkFlowItem workFlowItem = null;
            JObject result =
                await
                    InvokeAsync<JObject>(string.Format(Constants.GET_TEAM_WORKFLOW, teamName, workFlowName),
                        Constants.HttpMethod.GET, metaData)
                        .ConfigureAwait(false);

            if (result != null)
            {
                List<string> connectorNames = new List<string>();
                workFlowItem = new WorkFlowItem()
                {
                    Name = result?["Name"].Value<string>(),
                    Description = result?["Description"].Value<string>(),
                    Expression = GetExpression(result?["Expression"] as JObject, connectorNames),
                };
            }
            return workFlowItem;
        }

        public async Task<bool> CreateWorkFlow(string teamName, string workFlowName, string description, Expression expression)
        {
            dynamic workFlowRequest = new ExpandoObject();
            workFlowRequest.Description = description;
            workFlowRequest.Expression = expression;

            await
                InvokeAsync<ExpandoObject, string>(workFlowRequest,
                    string.Format(Constants.CREATE_WORKFLOW, teamName, workFlowName), Constants.HttpMethod.PUT,
                    new List<KeyValue>())
                    .ConfigureAwait(false);

            return true;
        }

        #endregion

        #region Private methods

        private async Task<TokenResult> GetToken()
        {
            string url = DEFAULT_AD_TENANT_URL;
            StringBuilder requestBody = new StringBuilder();
            requestBody.Append(string.Concat("resource=", DEFAULT_AD_REVIEW_SVC_URL));
            requestBody.Append(string.Concat("&client_id=", _clientId));
            requestBody.Append(string.Concat("&client_secret=", _clientSecret));
            requestBody.Append(string.Concat("&grant_type=", "client_credentials"));
            //string requestBody =
            //    "resource=http%3A%2F%2Frvsvc&client_id=2c739b15-a26c-475e-b234-9bb366c2f70a&client_secret=cEKo9ocrj8apgRCgGuhpA96/SWcPUKTh1Z6NF40MBns=&grant_type=client_credentials";
            var request = WebRequest.Create(url);
            return
                await
                    this.SendAsync<string, TokenResult>("POST", requestBody.ToString(), request, headerCallback)
                        .ConfigureAwait(false);
        }
        private async Task<S> InvokeAsync<T, S>(dynamic imageRequest, string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData)
        {
            //Get Token
            var token = await GetToken();

            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            requestUrl.Append(string.Concat("subscription-key=", _subscriptionKey, "&"));
            var request = WebRequest.Create(requestUrl.ToString());
            request.Headers.Add("Authorization", string.Concat("Bearer ", token.access_token));

            return
                await
                    this.SendAsync<T, S>(method.ToString(), imageRequest, request)
                        .ConfigureAwait(false);
        }

        private async Task<T> InvokeAsync<T>(string operationUrl, Constants.HttpMethod method, List<KeyValue> metaData)
        {
            //Get Token
            var token = await GetToken();

            StringBuilder requestUrl = new StringBuilder(string.Concat(_apiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }

            requestUrl.Append(string.Concat("subscription-key=", _subscriptionKey, "&"));
            var request = WebRequest.Create(requestUrl.ToString());

            request.Headers.Add("Authorization", string.Concat("Bearer ", token.access_token));

            return
                await
                    this.GetAsync<T>(method.ToString(), request)
                        .ConfigureAwait(false);
        }

        private void headerCallback(WebRequest request)
        {
            request.ContentType = "application/x-www-form-urlencoded";
        }

        private static Expression GetExpression(JObject obj, List<string> connectorNames)
        {
            Expression result = null;
            if (obj["Type"].Value<string>() == "Combine")
            {
                var res = new Combination();

                res.Left = GetExpression((JObject)obj["Left"], connectorNames);
                res.Right = GetExpression((JObject)obj["Right"], connectorNames);
                res.Combine = (CombineCondition)Enum.Parse(typeof(CombineCondition), obj["Combine"].Value<string>());
                result = res;
            }
            if (obj["Type"].Value<string>() == "Condition")
            {
                result = JsonConvert.DeserializeObject<Condition>(obj.ToString());
                connectorNames.Add((result as Condition).ConnectorName);
            }

            return result;
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
            else if (requestBody is string)
            {
                return System.Text.Encoding.UTF8.GetBytes(requestBody as string);
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                //settings.ContractResolver = this._defaultResolver;

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
