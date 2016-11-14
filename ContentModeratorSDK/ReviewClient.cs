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
    public class ReviewClient : ClientBase,  IReviewClient
    {
        /// <summary>
        /// AD tenent url
        /// </summary>
        private const string DEFAULT_AD_TENANT_URL = "https://login.microsoftonline.com/contentmoderatorprod.onmicrosoft.com/oauth2/token";

        /// <summary>
        /// AD tenent url
        /// </summary>
        private string DEFAULT_AD_REVIEW_SVC_URL = Uri.EscapeUriString("https://api.contentmoderator.cognitive.microsoft.com/review");
        

        /// <summary>
        /// The client Id
        /// </summary>
        private string _clientId;

        /// <summary>
        /// The client secret
        /// </summary>
        private string _clientSecret;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public ReviewClient(string subscriptionKey, string clientId, string clientSecret) : this(subscriptionKey, "https://westus.api.cognitive.microsoft.com/contentmoderator/review/v1.0", clientId, clientSecret) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public ReviewClient(string subscriptionKey, string apiRoot, string clientId, string clientSecret)
        {
            this.ApiRoot = apiRoot?.TrimEnd('/');
            this.SubscriptionKey = subscriptionKey;
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

            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            requestUrl.Append(string.Concat("subscription-key=", this.SubscriptionKey, "&"));
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

            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }

            requestUrl.Append(string.Concat("subscription-key=", this.SubscriptionKey, "&"));
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

        
    }
}
