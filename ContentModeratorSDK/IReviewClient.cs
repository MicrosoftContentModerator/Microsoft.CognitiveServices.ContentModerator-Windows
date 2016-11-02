using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Review;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public interface IReviewClient
    {
        #region Review Operations
        /// <summary>
        /// Get review details for the review Id.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        Task<ReviewResponse> GetReview(string teamName, string reviewId);

        /// <summary>
        /// Creates review.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="reviewRequests"></param>
        /// <returns></returns>
        Task<string[]> CreateReview(string teamName, List<ReviewRequest> reviewRequests);

        #endregion

        #region Job Operations
        /// <summary>
        /// Get job details for the job Id.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<JobDetailsResult> GetJobDetails(string teamName, string jobId);

        /// <summary>
        /// Creates a new Job based on url.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="workFlowName"></param>
        /// <param name="callBackEndpoint"></param>
        /// <returns></returns>
        Task<CreateJobResult> CreateJob(string teamName, string content, ContentType contentType, string contentId, string workFlowName,
            string callBackEndpoint);

        /// <summary>
        /// Creates a new Job based on image stream.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="workFlowName"></param>
        /// <param name="callBackEndpoint"></param>
        /// <returns></returns>
        Task<CreateJobResult> CreateJob(string teamName, Stream content, ContentType contentType, string contentId, string workFlowName,
            string callBackEndpoint);

        #endregion

        #region WorkFlow Operations

        /// <summary>
        /// Get all workflows of team.
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        Task<List<WorkFlowItem>> GetAllWorkflows(string teamName);

        /// <summary>
        /// Get workflow details.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="workFlowName"></param>
        /// <returns></returns>
        Task<WorkFlowItem> GetWorkflow(string teamName, string workFlowName);

        /// <summary>
        /// Create new work flow.
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="workFlowName"></param>
        /// <param name="description"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> CreateWorkFlow(string teamName, string workFlowName, string description, Expression expression);

        #endregion
    }
}
