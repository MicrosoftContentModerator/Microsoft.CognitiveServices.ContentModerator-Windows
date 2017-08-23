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

    public class ListManagementClient : ClientBase, IListManagementClient
    {
        

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public ListManagementClient(string subscriptionKey) : this(subscriptionKey, "https://westus.api.cognitive.microsoft.com/contentmoderator/lists/v1.0") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        public ListManagementClient(string subscriptionKey, string apiRoot)
        {
            this.ApiRoot = apiRoot?.TrimEnd('/');
            this.SubscriptionKey = subscriptionKey;
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

            var metaData = new List<KeyValue>();
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

            return
                await
                    InvokeAsync<ExpandoObject, ImageAddResult>(
                        imageRequest,
                        string.Format(Constants.IMAGE_ADD, listId),
                        Constants.HttpMethod.POST,
                        metaData);
        }

        public async Task<ImageAddResult> ImageAddAsync(Stream content, string listId, string tag, string label)
        {
            dynamic imageRequest = new ExpandoObject();
            imageRequest.Value = content;

            var metaData = new List<KeyValue>();
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
           

            return
                await
                    InvokeAsync<ExpandoObject, ImageAddResult>(imageRequest,
                        string.Format(Constants.IMAGE_ADD, listId), Constants.HttpMethod.POST, metaData);
        }

        public async Task<ImageListResult> ImageGetAllIdsAsync(string listId)
        {
           return
                await this.InvokeAsync<ImageListResult>(string.Format(Constants.IMAGE_GETALLIDS, listId), Constants.HttpMethod.GET);
        }

        public async Task<string> ImageDeleteAllAsync(string listId)
        {
            return
                await this.InvokeAsync<string>(
                        string.Format(Constants.IMAGE_DELETEALL, listId),
                        Constants.HttpMethod.DELETE);
        }

        public async Task<string> ImageDeleteAsync(string listId, string imageId)
        {   
            return
                await this.InvokeAsync<string>(string.Format(Constants.IMAGE_DELETE, listId, imageId),
                        Constants.HttpMethod.DELETE);
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

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(
                        imageListRequest,
                        Constants.IMAGELIST_CREATE,
                        Constants.HttpMethod.POST);
        }

        public async Task<ListItemResult> ImageListUpdateAsync(string listId, string name, string description,
            Dictionary<string, string> listMetaData)
        {
            dynamic imageListRequest = new ExpandoObject();
            imageListRequest.Name = name;
            imageListRequest.Description = description;
            imageListRequest.Metadata = listMetaData;
            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(
                        imageListRequest,
                        string.Format(Constants.IMAGELIST_UPDATE, listId),
                        Constants.HttpMethod.PUT);

        }

        public async Task<ImageRefreshIndexResult> ImageListRefreshIndexAsync(string listId)
        {
            dynamic imageRequest = new ExpandoObject();
            return
                await
                    InvokeAsync<ExpandoObject, ImageRefreshIndexResult>(imageRequest,
                        string.Format(Constants.IMAGELIST_REFRESHINDEX, listId), Constants.HttpMethod.POST);

        }
        public async Task<List<ListItemResult>> ImageListGetAllAsync()
        {
            return
                await this.InvokeAsync<List<ListItemResult>>(Constants.IMAGELIST_GETALL, Constants.HttpMethod.GET, null);
        }
        public async Task<ListItemResult> ImageListDetailAsync(string listId)
        {
            return
                await this.InvokeAsync<ListItemResult>(string.Format(Constants.IMAGELIST_GETDETAIL, listId),
                        Constants.HttpMethod.GET);
        }

        public async Task<string> ImageListDeleteAsync(string listId)
        {
   
            return
                await this.InvokeAsync<string>(string.Format(Constants.IMAGELIST_DELETE, listId),
                        Constants.HttpMethod.DELETE);
        }

        public async Task<string> TermAddAsync(string listId, string term, string language)
        {
            var metaData = new List<KeyValue>
            {
                new KeyValue()
                {
                    Key = "language",
                    Value = language
                }
            };

            await this.InvokeAsync<string>(
                        string.Format(Constants.TERM_ADD, listId, term),
                        Constants.HttpMethod.POST,
                        metaData);

            return string.Empty;

        }

        public async Task<string> TermDeleteAsync(string listId, string term, string language)
        {
            var metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });


            await
                    this.InvokeAsync<string>(
                        string.Format(Constants.TERM_DELETE, listId, term),
                        Constants.HttpMethod.DELETE,
                        metaData);

            return string.Empty;
        }

        public async Task<string> TermDeleteAllAsync(string listId, string language)
        {
            var metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });

            await this.InvokeAsync<string>(string.Format(Constants.TERM_DELETEALL, listId), Constants.HttpMethod.DELETE, metaData);

            return string.Empty;

        }

        public async Task<TermGetAllResult> TermGetAllTermsAsync(string listId, string language)
        {
            var metaData = new List<KeyValue>
            {
                new KeyValue()
                {
                    Key = "language",
                    Value = language
                }
            };


            return
                await this.InvokeAsync<TermGetAllResult>(string.Format(Constants.TERM_GETALLIDS, listId), Constants.HttpMethod.GET, metaData);
        }

        public async Task<string> TermListRefreshIndexAsync(string listId, string language)
        {
            var metaData = new List<KeyValue>
            {
                new KeyValue()
                {
                    Key = "language",
                    Value = language
                }
            };

            await this.InvokeAsync<string>(string.Format(Constants.TERMLIST_REFRESHINDEX, listId), Constants.HttpMethod.POST,metaData);

            return string.Empty;
        }

        public async Task<List<ListItemResult>> TermListGetAllAsync()
        {
            return
                await this.InvokeAsync<List<ListItemResult>>(Constants.TERMLIST_GETALL, Constants.HttpMethod.GET);
        }

        public async Task<ListItemResult> TermListCreateAsync(string name, string description, Dictionary<string, string> listMetaData)
        {
           dynamic termlistrequest = new ExpandoObject();
            termlistrequest.Name = name;
            termlistrequest.Description = description;
            termlistrequest.Metadata = listMetaData;

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(termlistrequest,
                        Constants.TERMLIST_CREATE, Constants.HttpMethod.POST);
        }

        public async Task<ListItemResult> TermListUpdateAsync(string listId, string name, string description, Dictionary<string, string> listMetaData)
        {
            dynamic imageListRequest = new ExpandoObject();
            imageListRequest.Name = name;
            imageListRequest.Description = description;
            imageListRequest.Metadata = listMetaData;

            return
                await
                    InvokeAsync<ExpandoObject, ListItemResult>(
                        imageListRequest,
                        string.Format(Constants.TERMLIST_UPDATE, listId),
                        Constants.HttpMethod.PUT);
        }

        public async Task<ListItemResult> TermListDetailAsync(string listId)
        {  
            return
                await this.InvokeAsync<ListItemResult>(string.Format(Constants.TERMLIST_GETDETAIL, listId),
                Constants.HttpMethod.GET);
        }

        public async Task<string> TermListDeleteAsync(string listId)
        {   
            return
                await this.InvokeAsync<string>(string.Format(Constants.TERMLIST_DELETE, listId),
                        Constants.HttpMethod.DELETE);
        }

        #endregion

        #endregion

        #region Private methods

        

        


        #endregion
    }
}
