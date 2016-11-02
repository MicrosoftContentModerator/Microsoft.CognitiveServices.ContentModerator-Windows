using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Cache;
using Microsoft.CognitiveServices.ContentModerator;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentModeratorSDK.Test
{
    using System.Configuration;

    /// <summary>
    /// Summary description for ImageListManagement
    /// </summary>
    [TestClass]
    public class ListManagementTests
    {
        private readonly ListManagementClient client = null;
        private readonly string testImageContentUrl = @"https://moderatorsampleimages.blob.core.windows.net/samples/sample.jpg";
        public ListManagementTests()
        {
            this.client = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"], ConfigurationManager.AppSettings["listmanagmentrooturl"]);
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Image API

        #region Image
        [TestMethod]
        public void ImageAddUrl()
        {
            var result = this.ImageAdd();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAllImageIds()
        {
            var moderate = this.client.ImageGetAllIdsAsync("117");
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteAllImages()
        {
            var moderate = this.client.ImageDeleteAllAsync("26");
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteImage()
        {
            var moderate = this.client.ImageDeleteAsync("26", "169");
            var result = moderate.Result;
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }

        #endregion

        #region ImageList

        [TestMethod]
        public void CreateImageList()
        {
            Dictionary<string, string> listMetaData = new Dictionary<string, string>();
            listMetaData.Add("MyListMData1", "MyListMData1");
            listMetaData.Add("MyListMData2", "MyListMData2");
            var moderate = this.client.ImageListCreateAsync("MyList", "My custom List", listMetaData);
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UpdateImageList()
        {
            Dictionary<string, string> listMetaData = new Dictionary<string, string>();
            listMetaData.Add("MyListMData1", "MyListMData1");
            listMetaData.Add("MyListMData2", "MyListMData2");
            var moderate = this.client.ImageListUpdateAsync("116","MyList", "My custom List", listMetaData);
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RefreshIndex()
        {
            var moderate = this.client.ImageListRefreshIndexAsync("26");
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAllLists()
        {
            var moderate = this.client.ImageListGetAllAsync();
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetListDetails()
        {
            var moderate = this.client.ImageListDetailAsync("113");
            var result = moderate.Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteImageList()
        {
            var moderate = this.client.ImageListDeleteAsync("116");
            var result = moderate.Result;
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }

        #endregion

        #endregion


        #region private methods

        #region Image
        private ImageAddResult ImageAdd()
        {
            var moderate = this.client.ImageAddAsync(this.testImageContentUrl, DataRepresentationType.Url, "117", string.Empty,
                string.Empty);
            var result = moderate.Result;
            return result;
        }
        
        #endregion

        #endregion
    }
}
