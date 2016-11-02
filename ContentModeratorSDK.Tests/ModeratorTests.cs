using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.CognitiveServices.ContentModerator;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace ContentModeratorSDK.Test
{
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Summary description for ImageTests
    /// </summary>
    [TestClass]
    public class ModeratorTests
    {
        private readonly ModeratorClient client = null;
        private readonly string testImageContent = @"Content\sample.jpg";
        private readonly string testImageContentUrl = @"https://hashblobsm2.blob.core.windows.net/testimages/PNGOCR.png";
        private readonly string matchImageContent = @"Content\img_300.jpg";
        private readonly string matchImageContentUrl = @"https://moderatorsampleimages.blob.core.windows.net/samples/img_300.jpg";
        private readonly string sampleScreenText =
            @"The <a href='www.bunnies.com'>qu!ck</a> brown  <a href='b.suspiciousdomain.com'>f0x</a> jumps over the lzay dog www.benign.net. test@gmail.com 425-559-1777 10.1.2.3 13706 ne 10th st, bellevue, WA 98005";

        private readonly string defaultLanguage = "eng";
        private readonly bool isCache = true;
        private readonly string listId = "Your List Id";
        public ModeratorTests()
        {
            this.client = new ModeratorClient(ConfigurationManager.AppSettings["subscriptionkey"]);

        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod()]
        [DataSource("ImageEvalute")]
        [TestCategory("Data Driven")]
        public async Task EvaluateImageTestDataBasedAsync()
        {
            string type = this.TestContext.DataRow["DataType"].ToString();
            bool isEnabled = bool.Parse(this.TestContext.DataRow["Enabled"].ToString());
            var expectSizeException = bool.Parse(this.TestContext.DataRow["SizeException"].ToString());


            if (!isEnabled)
            {
                return;
            }

            EvaluateImageResult result = null;

            try
            {
                switch (type)
                {
                    case "url":
                        {
                            result =
                                await
                                    this.client.EvaluateImageAsync(
                                        this.TestContext.DataRow["DataVal"].ToString(),
                                        DataRepresentationType.Url,
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()));
                            break;
                        }
                    case "raw":
                        {
                            var webClient = new WebClient();
                            byte[] imageBytes = webClient.DownloadData(this.TestContext.DataRow["DataVal"].ToString());

                            result =
                                await
                                    this.client.EvaluateImageAsync(
                                        new MemoryStream(imageBytes),
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()));
                            break;
                        }
                }

                if(expectSizeException)
                {
                    Assert.Fail("Image Size Expection was expected!!");
                }


                Assert.IsNotNull(result);

                if (bool.Parse(this.TestContext.DataRow["Cache"].ToString()))
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(result.CacheID), "Expected CacheId, Response: {0}", JsonConvert.SerializeObject(result));
                }
                else
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(result.CacheID), "CacheId NOT EXPECTED, Response: {0}", JsonConvert.SerializeObject(result));
                }


                Assert.IsTrue(result.AdultClassificationScore > 0, "Expected higher than 0 Adult Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
                Assert.IsTrue(result.RacyClassificationScore > 0, "Expected higher than 0 Racy Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
                Assert.AreEqual(result.IsImageAdultClassified, bool.Parse(this.TestContext.DataRow["IsAdult"].ToString()), "Image should not have been classified as Adult, Response: {0}", JsonConvert.SerializeObject(result));
                Assert.AreEqual(result.IsImageRacyClassified, bool.Parse(this.TestContext.DataRow["IsRacy"].ToString()), "Image should not have been classified as Racy, Response: {0}", JsonConvert.SerializeObject(result));

            }
            catch (Microsoft.CognitiveServices.ContentModerator.ClientException ex)
            {
                if((ex.Error.Message != "Image Size Error") || !expectSizeException)
                {
                    throw;
                }
            }
        }

         [TestMethod()]
        [DataSource("DetectFaces")]
        [TestCategory("Data Driven")]
        
        public async Task DetectFacesTestDataBasedAsync()
        {
            string type = this.TestContext.DataRow["DataType"].ToString();
            bool isEnabled = bool.Parse(this.TestContext.DataRow["Enabled"].ToString());
            var expectSizeException = bool.Parse(this.TestContext.DataRow["SizeException"].ToString());

            if (!isEnabled)
            {
                return;
            }

            DetectFacesResult result = null;

            try
            {
                switch (type)
                {
                    case "url":
                        {
                            result =
                                await
                                    this.client.DetectFacesImageAsync(
                                        this.TestContext.DataRow["DataVal"].ToString(),
                                        DataRepresentationType.Url,
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()));
                            break;
                        }
                    case "raw":
                        {
                            var webClient = new WebClient();
                            byte[] imageBytes = webClient.DownloadData(this.TestContext.DataRow["DataVal"].ToString());

                            result =
                                await
                                    this.client.DetectFacesImageAsync(
                                        new MemoryStream(imageBytes),
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()));
                            break;
                        }
                }


                if(expectSizeException)
                {
                    Assert.Fail("Image Size Expection was expected!!");
                }
                Assert.IsNotNull(result);
                if (bool.Parse(this.TestContext.DataRow["Cache"].ToString()))
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(result.CacheID), "Expected CacheId, Response: {0}", JsonConvert.SerializeObject(result));
                }
                else
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(result.CacheID), "CacheId NOT EXPECTED, Response: {0}", JsonConvert.SerializeObject(result));
                }

                var expectedFaceCount = int.Parse(this.TestContext.DataRow["FaceCount"].ToString());
                Assert.IsTrue(result.Count == expectedFaceCount, "Face count is less than 1, Response: {0}", JsonConvert.SerializeObject(result));

                if (expectedFaceCount > 0)
                {
                    Assert.IsNotNull(
                        result.Faces,
                        "No Faces detected, Response: {0}",
                        JsonConvert.SerializeObject(result));
                    Assert.IsTrue(
                        result.Faces.Length > 0,
                        "No Faces detected, Response: {0}",
                        JsonConvert.SerializeObject(result));
                }
                else
                {
                    Assert.IsTrue(
                        result.Faces == null || result.Faces.Length == 0,
                        "Faces array should be empty: {0}",
                        JsonConvert.SerializeObject(result));
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

        [TestMethod()]
        [DataSource("OCRImage")]
        [TestCategory("Data Driven")]
        public async Task OCRImageTestDataBasedAsync()
        {
            string type = this.TestContext.DataRow["DataType"].ToString();
            bool isEnabled = bool.Parse(this.TestContext.DataRow["Enabled"].ToString());
            var expectSizeException = bool.Parse(this.TestContext.DataRow["SizeException"].ToString());
            var expectUnknownException = bool.Parse(this.TestContext.DataRow["UnknownException"].ToString());

            if (!isEnabled)
            {
                return;
            }

            OcrImageResult result = null;

            try
            {
                switch (type)
                {
                    case "url":
                        {
                            result =
                                await
                                    this.client.OCRImageAsync(
                                        this.TestContext.DataRow["DataVal"].ToString(),
                                        DataRepresentationType.Url,
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()),
                                        bool.Parse(this.TestContext.DataRow["Enhanced"].ToString()),
                                        this.TestContext.DataRow["Language"].ToString());
                            break;
                        }
                    case "raw":
                        {
                            var webClient = new WebClient();
                            byte[] imageBytes = webClient.DownloadData(this.TestContext.DataRow["DataVal"].ToString());

                            result =
                                await
                                    this.client.OCRImageAsync(
                                        new MemoryStream(imageBytes),
                                        bool.Parse(this.TestContext.DataRow["Cache"].ToString()),
                                        bool.Parse(this.TestContext.DataRow["Enhanced"].ToString()),
                                        this.TestContext.DataRow["Language"].ToString());
                            break;
                        }
                }


                if(expectSizeException || expectUnknownException)
                {
                    Assert.Fail("Image Expection was expected!!");
                }
                Assert.IsNotNull(result);

                if (bool.Parse(this.TestContext.DataRow["Cache"].ToString()))
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(result.CacheID), "Expected CacheId, Response: {0}", JsonConvert.SerializeObject(result));
                }
                else
                {
                    Assert.IsTrue(string.IsNullOrWhiteSpace(result.CacheID), "CacheId NOT EXPECTED, Response: {0}", JsonConvert.SerializeObject(result));
                }




                var textExpected = bool.Parse(this.TestContext.DataRow["TextExpected"].ToString());
                string expectedText = this.TestContext.DataRow["ExpectedText"].ToString();
                var isEnhanced = bool.Parse(this.TestContext.DataRow["Enhanced"].ToString());

                expectedText = expectedText.Replace(@"\r\n", Environment.NewLine);
                

                if (textExpected)
                {
                    Assert.IsTrue(
                        result.Text == expectedText,
                        "Unexpected Text: {0}",
                        JsonConvert.SerializeObject(result));

                    if(isEnhanced)
                    Assert.IsTrue(
                        result.Candidates.Count > 0,
                        "Expected Candidates, Response: {0}",
                        JsonConvert.SerializeObject(result));
                    else
                    {
                    Assert.IsTrue(
                        result.Candidates.Count == 0,
                        "Did not expect Candidates, Response: {0}",
                        JsonConvert.SerializeObject(result));
                    }

                }
            }
            catch (Microsoft.CognitiveServices.ContentModerator.ClientException ex)
            {
                
                if(expectSizeException)
                {
                    Assert.AreEqual("Image Size Error",ex.Error.Message);
                }
                if(expectUnknownException)
                {
                    Assert.AreEqual("Unknown Server Error.",ex.Error.Message);
                }
            }
        }


        #region EvaluateImage
        [TestMethod]
        [TestCategory("EvaluateImage")]
        public void EvaluateImageUrl()
        {
            var result = this.EvaluateImage();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AdultClassificationScore > 0, "Expected higher than 0 Adult Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(result.RacyClassificationScore > 0, "Expected higher than 0 Racy Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageAdultClassified, "Image should not have been classified as Adult, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageRacyClassified, "Image should not have been classified as Racy, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("EvaluateImage")]
        public void EvaluateImageStream()
        {
            EvaluateImageResult result = null;
            
            using (Stream stream = new FileStream(this.testImageContent, FileMode.Open, FileAccess.Read))
            {
                var moderate = this.client.EvaluateImageAsync(stream, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AdultClassificationScore > 0, "Expected higher than 0 Adult Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(result.RacyClassificationScore > 0, "Expected higher than 0 Racy Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageAdultClassified, "Image should not have been classified as Adult, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageRacyClassified, "Image should not have been classified as Racy, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("EvaluateImage")]
        public void EvaluateImageCache()
        {
            EvaluateImageResult result = null;

            result = this.EvaluateImage();

            Assert.IsNotNull(result.CacheID);

            if (!string.IsNullOrWhiteSpace(result.CacheID))
            {
                var moderate = this.client.EvaluateImageAsync(result.CacheID, DataRepresentationType.Cacheid, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AdultClassificationScore > 0, "Expected higher than 0 Adult Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(result.RacyClassificationScore > 0, "Expected higher than 0 Racy Classification Score value for test image, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageAdultClassified, "Image should not have been classified as Adult, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsTrue(!result.IsImageRacyClassified, "Image should not have been classified as Racy, Response: {0}", JsonConvert.SerializeObject(result));
        }

        #endregion

        #region DetectFaces
        [TestMethod]
        [TestCategory("DetectFaces")]
        public void DetectFacesImageUrl()
        {
            var result = this.DetectFacesImage();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Face count is less than 1, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Faces, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("DetectFaces")]
        public void DetectFacesImageStream()
        {
            DetectFacesResult result = null;

            using (Stream stream = new FileStream(this.testImageContent, FileMode.Open, FileAccess.Read))
            {
                var moderate = this.client.DetectFacesImageAsync(stream, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Face count is less than 1, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Faces, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("DetectFaces")]
        public void DetectFacesImageCache()
        {
            DetectFacesResult result = null;

            result = this.DetectFacesImage();

            Assert.IsNotNull(result.CacheID);

            if (!string.IsNullOrWhiteSpace(result.CacheID))
            {
                var moderate = this.client.DetectFacesImageAsync(result.CacheID, DataRepresentationType.Cacheid, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Face count is less than 1, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Faces, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }
        #endregion

        #region OCR
        [TestMethod]
        [TestCategory("OCR")]
        public void OCRImageUrl()
        {
            var result = this.OCRImage();
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Text), "Text not found, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Candidates, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("OCR")]
        public void OCRImageStream()
        {
            OcrImageResult result = null;

            using (Stream stream = new FileStream(this.testImageContent, FileMode.Open, FileAccess.Read))
            {
                var moderate = this.client.OCRImageAsync(stream, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Text), "Text not found, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Candidates, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("OCR")]
        public void OCRImageCache()
        {
            OcrImageResult result = null;

            result = this.OCRImage();

            Assert.IsNotNull(result.CacheID);

            if (!string.IsNullOrWhiteSpace(result.CacheID))
            {
                var moderate = this.client.OCRImageAsync(result.CacheID, DataRepresentationType.Cacheid, this.isCache);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Text), "Text not found, Response: {0}", JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result.Candidates, "No Faces detected, Response: {0}", JsonConvert.SerializeObject(result));
        }
        #endregion

        #region Match Image
        [TestMethod]
        [TestCategory("Match")]
        public void MatchImageUrl()
        {
            var result = this.MatchImage();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsMatch);
            Assert.IsNotNull(result.Matches);
            Assert.IsTrue(result.Matches.Count > 0, "No matches found, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("Match")]
        public void MatchImageStream()
        {
            MatchResult result = null;

            using (Stream stream = new FileStream(this.matchImageContent, FileMode.Open, FileAccess.Read))
            {
                var moderate = this.client.MatchImageAsync(stream,  this.isCache, this.listId);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsMatch);
            Assert.IsNotNull(result.Matches);
            Assert.IsTrue(result.Matches.Count > 0, "No matches found, Response: {0}", JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        [TestCategory("Match")]
        public void MatchImageCache()
        {
            MatchResult result = null;

            result = this.MatchImage();

            Assert.IsNotNull(result.CacheID);

            if (!string.IsNullOrWhiteSpace(result.CacheID))
            {
                var moderate = this.client.MatchImageAsync(result.CacheID, DataRepresentationType.Cacheid, this.isCache, this.listId);
                result = moderate.Result;
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsMatch);
            Assert.IsNotNull(result.Matches);
            Assert.IsTrue(result.Matches.Count > 0, "No matches found, Response: {0}", JsonConvert.SerializeObject(result));
        }
        #endregion

        

        #region Text

        [TestMethod]
        [TestCategory("Text")]
        public void ScreenText()
        {
            var moderate = this.client.ScreenTextAsync(this.sampleScreenText, Constants.MediaType.Plain, this.defaultLanguage, false,
                true, true, string.Empty);
            var result = moderate.Result;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Status != null &&  result.Status.Code == "3000");
            Assert.IsNotNull(result.PII);
            Assert.IsTrue(result.PII.Email != null && result.PII.Email.Count > 0);
            Assert.IsTrue(result.PII.Address != null && result.PII.Address.Count > 0);
            Assert.IsTrue(result.PII.Phone != null && result.PII.Phone.Count > 0);
            Assert.IsTrue(result.PII.IPA != null && result.PII.IPA.Count > 0);

        }

        [TestMethod]
        [TestCategory("Text")]
        public void IdentifyLanguage()
        {
            var moderate = this.client.IdentifyLanguageAsync(this.sampleScreenText, Constants.MediaType.Plain);
            var result = moderate.Result;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Status != null && result.Status.Code == "3000");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.DetectedLanguage));
        }
        #endregion

        #region private methods

        #region Image
        private EvaluateImageResult EvaluateImage()
        {
            var moderate = this.client.EvaluateImageAsync(this.testImageContentUrl, DataRepresentationType.Url, this.isCache);
            var result = moderate.Result;
            return result;
        }

        private DetectFacesResult DetectFacesImage()
        {
            var moderate = this.client.DetectFacesImageAsync(this.testImageContentUrl, DataRepresentationType.Url, this.isCache);
            var result = moderate.Result;
            return result;
        }

        private OcrImageResult OCRImage()
        {
            var moderate = this.client.OCRImageAsync(this.testImageContentUrl, DataRepresentationType.Url, this.isCache, true);
            var result = moderate.Result;
            return result;
        }

        private MatchResult MatchImage()
        {
            var moderate = this.client.MatchImageAsync(this.matchImageContentUrl, DataRepresentationType.Url, this.isCache, this.listId);
            var result = moderate.Result;
            return result;
        }
        #endregion

        

        #endregion

    }
}
