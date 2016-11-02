using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentModeratorSDK.Test
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CognitiveServices.ContentModerator;
    using Microsoft.CognitiveServices.ContentModerator.Contract.Image;

    [TestClass]
    public class ImageListAndMatchScenarios
    {
        private static string listId = "";

        private readonly ListManagementClient imageListclient = null;
        private readonly ModeratorClient moderatorClient = null;
        public ImageListAndMatchScenarios()
        {
            this.imageListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);
            this.moderatorClient = new ModeratorClient(ConfigurationManager.AppSettings["subscriptionkey"]);
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            //Create a new list
            //Get all Lists to verify creation

            var imageListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);
            var listName = Guid.NewGuid().ToString("n");

            Dictionary<string, string> listMetaData = new Dictionary<string, string>();
            listMetaData.Add("MyListMData1", "MyListMData1");
            listMetaData.Add("MyListMData2", "MyListMData2");

            var createRes = imageListclient.ImageListCreateAsync(listName, "This is Unit Test", listMetaData).Result;
            Assert.IsNotNull(createRes);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(createRes.Id));

            listId = createRes.Id;

            var getAllRes = imageListclient.ImageListGetAllAsync().Result;
            Assert.IsNotNull(getAllRes);
            Assert.IsTrue(getAllRes.Count > 0);
            Assert.IsTrue(getAllRes.Exists(e => e.Id == createRes.Id));

            Dictionary<string, string[]> x = new Dictionary<string, string[]>();
            
            

        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            //Delete ALL Images
            //Delete Image List
            //Call get all lists to verify deletion

            var imageListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);

            var updateImageList = imageListclient.ImageListUpdateAsync(listId, "Updated Name", "Updated", null).Result;
            Assert.IsNotNull(updateImageList);

            var deleteAllRes = imageListclient.ImageDeleteAllAsync(listId).Result;
            Assert.IsNotNull(deleteAllRes);

            var deleteImageList = imageListclient.ImageListDeleteAsync(listId).Result;
            Assert.IsNotNull(deleteImageList);


            var getAllRes = imageListclient.ImageListGetAllAsync().Result;
            Assert.IsNotNull(getAllRes);
            Assert.IsTrue(!getAllRes.Exists(e => e.Id == listId));
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        [TestCategory("Data Driven")]
        [DataSource("ImageMatch")]
        public async Task ImageMatchTestsAsync()
        {
            
            //Add Image
            //Refresh Index
            //List all images
            //Call Match - Should Match
            //Delete Image
            //List all images
            //Refresh Index
            //Call Match - Should not Match

            bool isEnabled = bool.Parse(this.TestContext.DataRow["Enabled"].ToString());
            var expectSizeException = bool.Parse(this.TestContext.DataRow["SizeException"].ToString());
            if (!isEnabled)
            {
                return;
            }


            Thread.Sleep(1000);

            var imageUrl = this.TestContext.DataRow["DataVal"].ToString();
            var imageTags = this.TestContext.DataRow["Tag"].ToString();
            var imageLabel = this.TestContext.DataRow["Label"].ToString();

            try
            {
                var addImageRes =
                    await
                        this.imageListclient.ImageAddAsync(
                            imageUrl,
                            DataRepresentationType.Url,
                            listId,
                            imageTags,
                            imageLabel);

               

                Assert.IsNotNull(addImageRes);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(addImageRes.ContentId));

                var refreshIndexRes = await this.imageListclient.ImageListRefreshIndexAsync(listId);
                Assert.IsNotNull(refreshIndexRes);
                Thread.Sleep(10000);


                var listAllImagesRes = await this.imageListclient.ImageGetAllIdsAsync(listId);
                Assert.IsNotNull(listAllImagesRes);
                Assert.IsTrue(listAllImagesRes.ContentIds.Exists(e => e.ToString() == addImageRes.ContentId));


                var matchRes =
                    await this.moderatorClient.MatchImageAsync(imageUrl, DataRepresentationType.Url, false, listId);
                Assert.IsNotNull(matchRes);
                Assert.IsTrue(matchRes.Matches.Exists(m => m.MatchId.ToString() == addImageRes.ContentId));

                var deleteImageRes = await this.imageListclient.ImageDeleteAsync(listId, addImageRes.ContentId);
                Assert.IsNotNull(deleteImageRes);

                var listAllAfterDeleteImagesRes = await this.imageListclient.ImageGetAllIdsAsync(listId);
                Assert.IsNotNull(listAllAfterDeleteImagesRes);
                Assert.IsTrue(
                    !listAllAfterDeleteImagesRes.ContentIds.Exists(e => e.ToString() == addImageRes.ContentId));

                var refreshIndexAfterDeleteRes = await this.imageListclient.ImageListRefreshIndexAsync(listId);
                Assert.IsNotNull(refreshIndexRes);
                Thread.Sleep(10000);

                var matchAfterDeleteRes =
                    await this.moderatorClient.MatchImageAsync(imageUrl, DataRepresentationType.Url, false, listId);
                Assert.IsNotNull(matchAfterDeleteRes);
                Assert.IsTrue(!matchAfterDeleteRes.Matches.Exists(m => m.MatchId.ToString() == addImageRes.ContentId));

                if(expectSizeException)
                {
                    Assert.Fail("Image Size Expection was expected!!");
                }

            }
            catch (Microsoft.CognitiveServices.ContentModerator.ClientException ex)
            {
              
                if((ex.Error.Message != "Image Size Error") || !expectSizeException)
                {
                    throw;
                }
            }

        }
    }
}
