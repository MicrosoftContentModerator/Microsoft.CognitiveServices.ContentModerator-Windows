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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class ReviewClient : ClientBase, IReviewClient
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
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        //public ReviewClient(string subscriptionKey) : this(subscriptionKey, "https://westus.api.cognitive.microsoft.com/contentmoderator/review/v1.0") { }
        public ReviewClient(string subscriptionKey) : this(subscriptionKey, "https://wabashcognitiveservices.azure-api.net/review/v1.0") { }


        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        /// <param name="clientId">The client Id.</param>
        /// <param name="clientSecret">The client secret.</param>
        public ReviewClient(string subscriptionKey, string apiRoot)
        {
            this.ApiRoot = apiRoot?.TrimEnd('/');
            this.SubscriptionKey = subscriptionKey;
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
        public async Task<ReviewVideoFramesResponse> GetVideoFrames(string teamName, string reviewId, string startSeed, string noOfRecords)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            queryParameters.Add(new KeyValue()
            {
                Key = "startSeed",
                Value = startSeed
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "noOfRecords",
                Value = noOfRecords
            });

            return
                await
                    InvokeAsync<ReviewVideoFramesResponse>(string.Format(Constants.GET_VIDEO_FRAMES, teamName, reviewId),
                        Constants.HttpMethod.GET, queryParameters)
                        .ConfigureAwait(false);
        }
        public async Task<string[]> CreateReview(string teamName, List<ReviewRequest> reviewRequests)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            return
                await
                    InvokeAsync<List<ReviewRequest>, string[]>(reviewRequests,
                        string.Format(Constants.CREATE_REVIEW, teamName), Constants.HttpMethod.POST, queryParameters)
                        .ConfigureAwait(false);
        }
        public async Task<bool> AddTranscript(string teamName, string reviewId, byte[] transcript)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            await
                InvokeAsync<byte[], string>(transcript,
                    string.Format(Constants.ADD_TRANSCRIPT, teamName, reviewId), Constants.HttpMethod.PUT, queryParameters)
                    .ConfigureAwait(false);
            return true;
        }
        public async Task<bool> PublishVideoReview(string teamName, string reviewId)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            await
                InvokeAsync<string>(string.Format(Constants.PUBLISH_VIDEO_REVIEW, teamName, reviewId), Constants.HttpMethod.PATCH, queryParameters)
                    .ConfigureAwait(false);
            return true;
        }
        public async Task<bool> AddVideoFrames(string teamName, string reviewId, string framesList,List<VideoFrame> frames, string frameZipPath)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();

            if (!string.IsNullOrWhiteSpace(frameZipPath))
            {
                    var response = ExecuteAddFramesMultipartRequest(teamName, reviewId, framesList, frameZipPath);

            }
            else
            {
                await
                       InvokeAsync<List<ReviewRequest>, string[]>(frames,
                           string.Format(Constants.ADD_VIDEO_FRAMES, teamName), Constants.HttpMethod.POST, queryParameters)
                           .ConfigureAwait(false);
            }
            return true;
        }

        #endregion

        #region Job Operations

        public async Task<JobDetailsResult> GetJobDetails(string teamName, string jobId)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            return
                await
                    InvokeAsync<JobDetailsResult>(string.Format(Constants.GET_JOB_DETAILS, teamName, jobId),
                        Constants.HttpMethod.GET, queryParameters)
                        .ConfigureAwait(false);
        }

        public async Task<CreateJobResult> CreateJob(string teamName, string content, ContentType contentType, string contentId,
            string workFlowName, string callBackEndpoint)
        {
            dynamic jobRequest = new ExpandoObject();
            jobRequest.ContentValue = content;

            List<KeyValue> queryParameters = new List<KeyValue>();
            queryParameters.Add(new KeyValue()
            {
                Key = "ContentType",
                Value = contentType.ToString()
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "ContentId",
                Value = contentId
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "WorkflowName",
                Value = workFlowName
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "CallBackEndpoint",
                Value = callBackEndpoint
            });

            return
                await
                    InvokeAsync<ExpandoObject, CreateJobResult>(jobRequest,
                        string.Format(Constants.CREATE_JOB, teamName), Constants.HttpMethod.POST, queryParameters)
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
            List<KeyValue> queryParameters = new List<KeyValue>();
            queryParameters.Add(new KeyValue()
            {
                Key = "ContentType",
                Value = contentType.ToString()
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "ContentId",
                Value = contentId
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "WorkflowName",
                Value = workFlowName
            });

            queryParameters.Add(new KeyValue()
            {
                Key = "CallBackEndpoint",
                Value = callBackEndpoint
            });

            return
                await
                    InvokeAsync<Stream, CreateJobResult>(content,
                        string.Format(Constants.CREATE_JOB, teamName), Constants.HttpMethod.POST, queryParameters)
                        .ConfigureAwait(false);
        }

        #endregion

        #region Workflow Operations

        public async Task<List<WorkFlowItem>> GetAllWorkflows(string teamName)
        {
            List<KeyValue> queryParameters = new List<KeyValue>();
            List<WorkFlowItem> workFlowList = new List<WorkFlowItem>();

            JArray result =
                await
                    InvokeAsync<JArray>(string.Format(Constants.GET_ALL_TEAM_WORKFLOWS, teamName),
                        Constants.HttpMethod.GET, queryParameters)
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
            List<KeyValue> queryParameters = new List<KeyValue>();
            WorkFlowItem workFlowItem = null;
            JObject result =
                await
                    InvokeAsync<JObject>(string.Format(Constants.GET_TEAM_WORKFLOW, teamName, workFlowName),
                        Constants.HttpMethod.GET, queryParameters)
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
        private async Task<S> InvokeAsync<T, S>(dynamic imageRequest, string operationUrl, Constants.HttpMethod method, List<KeyValue> queryParameters)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));
            foreach (var k in queryParameters)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            requestUrl.Append(string.Concat("subscription-key=", this.SubscriptionKey, "&"));
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.SendAsync<T, S>(method.ToString(), imageRequest, request)
                        .ConfigureAwait(false);
        }

        private async Task<T> InvokeAsync<T>(string operationUrl, Constants.HttpMethod method, List<KeyValue> queryParameters)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, operationUrl, "?"));
            foreach (var k in queryParameters)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }

            requestUrl.Append(string.Concat("subscription-key=", this.SubscriptionKey, "&"));
            var request = WebRequest.Create(requestUrl.ToString());
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



        /// <summary>
        /// Posts frames to add it to the video.
        /// </summary>
        /// <param name="reviewId">reviewID</param>
        /// <param name="reviewFrameList">reviewFrameList</param>
        /// <returns>Response of AddFrames Api call</returns>
        private async Task<HttpResponseMessage> ExecuteAddFramesMultipartRequest(string teamName, string reviewId, string reviewFrameList, string filePath)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);
            var requestUrl = string.Concat(this.ApiRoot, Constants.ADD_VIDEO_FRAMES);
            var uri = string.Format(requestUrl, teamName, reviewId);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
            string frameZipPath = filePath;
            string filename = Path.GetFileName(frameZipPath);
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(reviewFrameList), "FrameMetadata");
            byte[] frameZip = File.ReadAllBytes(frameZipPath);
            var zipContent = new ByteArrayContent(frameZip, 0, frameZip.Length);
            zipContent.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
            form.Add(zipContent, "FrameImageZip", "frameZip.zip");
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.PostAsync(uri, form);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return response;
        }



        #endregion


    }
}
