using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentModeratorSDK.Test
{
    using Microsoft.CognitiveServices.ContentModerator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Configuration;
    using System.Threading;
    using Microsoft.CognitiveServices.ContentModerator.Contract.Image;

    [TestClass]
    public class TextListScreenScenarios
    {
        private static string listId = "";

        private readonly ListManagementClient textListclient = null;
        private readonly ModeratorClient moderatorClient = null;
        public TextListScreenScenarios()
        {
            this.textListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);
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

            var textListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);
            var listName = Guid.NewGuid().ToString("n");

            Dictionary<string, string> listMetaData = new Dictionary<string, string>();
            listMetaData.Add("MyListMData1", "MyListMData1");
            listMetaData.Add("MyListMData2", "MyListMData2");

            var createRes = textListclient.TermListCreateAsync(listName, "This is Unit Test", listMetaData)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(createRes);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(createRes.Id));

            listId = createRes.Id;

            var getAllRes = textListclient.TermListGetAllAsync().ConfigureAwait(false)
                .GetAwaiter().GetResult();
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

            var textListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);

            var updateImageList = textListclient.TermListUpdateAsync(listId, "Updated Name", "Updated", null).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(updateImageList);

            var deleteImageList = textListclient.TermListDeleteAsync(listId).ConfigureAwait(false)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(deleteImageList);


            var getAllRes = textListclient.TermListGetAllAsync().ConfigureAwait(false)
                .GetAwaiter().GetResult();
            Assert.IsNotNull(getAllRes);
            Assert.IsTrue(!getAllRes.Exists(e => e.Id == listId));
        }

        [TestMethod]
        [TestCategory("Data Driven")]
        [DataSource("TermMatch")]
        public async Task TermMatchTestsAsync()
        {
            
            //Add Term
            //Refresh Index
            //List all Terms
            //Call Screen - Should Match
            //Delete Term
            //List all Terms
            //Refresh Index
            //Call Term - Should not Match

            bool isEnabled = bool.Parse(this.TestContext.DataRow["Enabled"].ToString());
            if (!isEnabled)
            {
                return;
            }


            Thread.Sleep(1000);

            //inputs
            var termToAdd = this.TestContext.DataRow["TermToAdd"].ToString();
            var textToScreen = this.TestContext.DataRow["TextToScreen"].ToString();
            var contentType = (Constants.MediaType) Enum.Parse(typeof(Constants.MediaType),this.TestContext.DataRow["ContentType"].ToString());
            var language = this.TestContext.DataRow["Language"].ToString();
            var autoCorrect = bool.Parse(this.TestContext.DataRow["AutoCorrect"].ToString());
            var identifyUrls = bool.Parse(this.TestContext.DataRow["IdentifyUrls"].ToString());
            var detectPii = bool.Parse(this.TestContext.DataRow["Pii"].ToString());

            //Expected output
            var shouldFindMatch = bool.Parse(this.TestContext.DataRow["MatchTerm"].ToString());
            var urlCount = int.Parse(this.TestContext.DataRow["UrlCount"].ToString());
            var emails = int.Parse(this.TestContext.DataRow["Email"].ToString());
            var phones = int.Parse(this.TestContext.DataRow["Phone"].ToString());
            var ipas = int.Parse(this.TestContext.DataRow["IPA"].ToString());
            var addresses = int.Parse(this.TestContext.DataRow["Address"].ToString());
           
            var addTextRes =
                await
                    this.textListclient.TermAddAsync(listId, termToAdd, language);

               

            Assert.IsNotNull(addTextRes);

            var refreshIndexRes = await this.textListclient.TermListRefreshIndexAsync(listId, language);
            Assert.IsNotNull(refreshIndexRes);
            Thread.Sleep(10000);


            var listAllTermsRes = await this.textListclient.TermGetAllTermsAsync(listId, language);
            Assert.IsNotNull(listAllTermsRes);
            Assert.IsTrue(listAllTermsRes.Data.Terms.Exists(e => e.Term == termToAdd));


            var matchRes =
                await this.moderatorClient.ScreenTextAsync(textToScreen, contentType, language,autoCorrect, identifyUrls, detectPii,listId);
            Assert.IsNotNull(matchRes);

            if (shouldFindMatch)
            {
                Assert.IsNotNull(matchRes.Terms);
                Assert.IsTrue(matchRes.Terms.ToList().Exists(m => (m.Term == termToAdd) && (m.ListId == listId)));
            }

            if(urlCount > 0)
            {
                Assert.IsTrue(matchRes.Urls.Length == urlCount );
            }

            if(emails > 0)
            {
                Assert.IsTrue(matchRes.PII.Email.Count == emails);
            }

            if(phones > 0)
            {
                Assert.IsTrue(matchRes.PII.Phone.Count == phones);
            }

            if (ipas > 0)
            {
                Assert.IsTrue(matchRes.PII.IPA.Count == ipas);
            }

            if (addresses > 0)
            {
                Assert.IsTrue(matchRes.PII.Address.Count == addresses);
            }
            var deleteTermRes = await this.textListclient.TermDeleteAsync(listId, termToAdd, language);
            Assert.IsNotNull(deleteTermRes);

            var listAllAfterDeleteTermsRes = await this.textListclient.TermGetAllTermsAsync(listId, language);
            Assert.IsNotNull(listAllAfterDeleteTermsRes);
            Assert.IsTrue(
                !listAllAfterDeleteTermsRes.Data.Terms.Exists(e => e.Term == termToAdd));

            var refreshIndexAfterDeleteRes = await this.textListclient.TermListRefreshIndexAsync(listId, language);
            Assert.IsNotNull(refreshIndexAfterDeleteRes);
            Thread.Sleep(10000);

            var matchAfterDeleteRes =
                await this.moderatorClient.ScreenTextAsync(textToScreen, contentType, language,autoCorrect, identifyUrls, detectPii,listId);
            Assert.IsNotNull(matchAfterDeleteRes);
            if (matchAfterDeleteRes.Terms != null)
            {
                Assert.IsTrue(
                    !matchAfterDeleteRes.Terms.ToList().Exists(m => (m.Term == termToAdd) && (m.ListId == listId)));
            }


        }
    }
}
