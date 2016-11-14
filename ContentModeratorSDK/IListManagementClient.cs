using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;

namespace Microsoft.CognitiveServices.ContentModerator
{
    using System.IO;
    using Microsoft.CognitiveServices.ContentModerator.Contract;
    using Microsoft.CognitiveServices.ContentModerator.Contract.Text;

    public interface IListManagementClient
    {
        #region Image API

        #region Image
        /// <summary>
        /// Add an image into the Image list
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="listId"></param>
        /// <param name="tag"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        Task<ImageAddResult> ImageAddAsync(string content, DataRepresentationType dataRepresentationType, string listId, string tag,
            string label);

        Task<ImageAddResult> ImageAddAsync(Stream content, string listId, string tag,
            string label);

        /// <summary>
        /// Delete an image inside a listId.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task<string> ImageDeleteAsync(string listId, string imageId);

        /// <summary>
        /// Deletes/Reset all images in a listId
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<string> ImageDeleteAllAsync(string listId);

        /// <summary>
        /// Get List of all Image Ids in a list Id.
        /// </summary>
        /// <returns></returns>
        Task<ImageListResult> ImageGetAllIdsAsync(string listId);

        #endregion

        #region ImageList
        /// <summary>
        /// Refresh index for a listId.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<ImageRefreshIndexResult> ImageListRefreshIndexAsync(string listId);

        /// <summary>
        /// Get all the content lists
        /// </summary>
        /// <returns></returns>
        Task<List<ListItemResult>> ImageListGetAllAsync();

        /// <summary>
        /// Creates new list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="listMetaData"></param>
        /// <returns></returns>
        Task<ListItemResult> ImageListCreateAsync(string name, string description, Dictionary<string, string> listMetaData);

        /// <summary>
        /// Updates the list details
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="listMetaData"></param>
        /// <returns></returns>
        Task<ListItemResult> ImageListUpdateAsync(string listId, string name, string description, Dictionary<string, string> listMetaData);

        /// <summary>
        /// Get ListId details.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<ListItemResult> ImageListDetailAsync(string listId);

        /// <summary>
        /// Deletes and image list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<string> ImageListDeleteAsync(string listId);

        #endregion

        #endregion


        #region Text

        #region Term Management
        Task<string> TermAddAsync(string listId, string term, string language);

        Task<string> TermDeleteAsync(string listId, string term, string language);

        Task<string> TermDeleteAllAsync(string listId, string language);
        
        Task<TermGetAllResult> TermGetAllTermsAsync(string listId, string language);

        #endregion

        #region Term Lists

        Task<string> TermListRefreshIndexAsync(string listId, string language);

        /// <summary>
        /// Get all the content lists
        /// </summary>
        /// <returns></returns>
        Task<List<ListItemResult>> TermListGetAllAsync();

        /// <summary>
        /// Creates new list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="listMetaData"></param>
        /// <returns></returns>
        Task<ListItemResult> TermListCreateAsync(string name, string description, Dictionary<string, string> listMetaData);

        /// <summary>
        /// Updates the list details
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="listMetaData"></param>
        /// <returns></returns>
        Task<ListItemResult> TermListUpdateAsync(string listId, string name, string description, Dictionary<string, string> listMetaData);

        /// <summary>
        /// Get ListId details.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<ListItemResult> TermListDetailAsync(string listId);

        /// <summary>
        /// Deletes and term list.
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<string> TermListDeleteAsync(string listId);



        #endregion


        #endregion
    }
}
