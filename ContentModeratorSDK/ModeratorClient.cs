using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Microsoft.CognitiveServices.ContentModerator.Contract.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class ModeratorClient : ClientBase, IModeratorClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public ModeratorClient(string subscriptionKey) : this(subscriptionKey, "https://westus.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModeratorClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        public ModeratorClient(string subscriptionKey, string apiRoot)
        {
            this.ApiRoot = apiRoot?.TrimEnd('/');
            this.SubscriptionKey = subscriptionKey;
        }

        #region EvaluateImage
        public async Task<EvaluateImageResult> EvaluateImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage)
        {
            
            dynamic evaluateImageRequest = new ExpandoObject();
            evaluateImageRequest.DataRepresentation = dataRepresentationType.ToString();
            evaluateImageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = this.SubscriptionKey
            });
            return await InvokeImageModeratorAsync<ExpandoObject, EvaluateImageResult>(evaluateImageRequest, Constants.Operations.Evaluate.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<EvaluateImageResult> EvaluateImageAsync(Stream content, bool cacheImage)
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
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = this.SubscriptionKey
            });
            return await this.InvokeImageModeratorAsync<Stream, EvaluateImageResult>(content, Constants.Operations.Evaluate.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region DetectFaces
        public async Task<DetectFacesResult> DetectFacesImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage)
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "subscription-key",
                Value = this.SubscriptionKey
            });

            return await InvokeImageModeratorAsync<ExpandoObject, DetectFacesResult>(imageRequest, Constants.Operations.FindFaces.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<DetectFacesResult> DetectFacesImageAsync(Stream content, bool cacheImage)
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
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });

            return await this.InvokeImageModeratorAsync<Stream, DetectFacesResult>(content, Constants.Operations.FindFaces.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region OCR
        public async Task<OcrImageResult> OCRImageAsync(string content, DataRepresentationType dataRepresentationType, bool cacheImage, bool enhanced = true, string language = "eng")
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "enhanced",
                Value = enhanced.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });

            return
                await
                    InvokeImageModeratorAsync<ExpandoObject, OcrImageResult>(imageRequest,
                        Constants.Operations.OCR.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<OcrImageResult> OCRImageAsync(Stream content, bool cacheImage, bool enhanced = true, string language = "eng")
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
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "enhanced",
                Value = enhanced.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });

            return await this.InvokeImageModeratorAsync<Stream, OcrImageResult>(content, Constants.Operations.OCR.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region Match
        public async Task<MatchResult> MatchImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage, string listid)
        {

            dynamic imageRequest = new ExpandoObject();
            imageRequest.DataRepresentation = dataRepresentationType.ToString();
            imageRequest.Value = content;

            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
            
            metaData.Add(new KeyValue()
            {
                Key = "listid",
                Value = listid
            });

            return
                await
                    InvokeImageModeratorAsync<ExpandoObject, MatchResult>(imageRequest,
                        Constants.Operations.Match.ToString(), metaData).ConfigureAwait(false);
        }

        public async Task<MatchResult> MatchImageAsync(Stream content, bool cacheImage, string listid)
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
                Key = "CacheImage",
                Value = cacheImage.ToString()
            });
           
            metaData.Add(new KeyValue()
            {
                Key = "listid",
                Value = listid
            });

            return await this.InvokeImageModeratorAsync<Stream, MatchResult>(content, Constants.Operations.Match.ToString(), metaData).ConfigureAwait(false);
        }

        #endregion

        #region Text

        public async Task<ScreenTextResult> ScreenTextAsync(string content, Constants.MediaType mediaType,
            string language, bool autocorrect, bool urls, bool pii, string listIds)
        {
            List<KeyValue> metaData = new List<KeyValue>();
            metaData.Add(new KeyValue()
            {
                Key = "language",
                Value = language
            });
            metaData.Add(new KeyValue()
            {
                Key = "autocorrect",
                Value = autocorrect.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "urls",
                Value = urls.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "PII",
                Value = pii.ToString()
            });
            metaData.Add(new KeyValue()
            {
                Key = "listId",
                Value = listIds
            });

           

            return
                await this.InvokeTextModeratorAsync<string, ScreenTextResult>(content, Constants.Operations.Screen.ToString(),
                        mediaType,metaData);
        }


        public async Task<IdentifyLanguageResult> IdentifyLanguageAsync(string content, Constants.MediaType mediaType)
        {
            return
                await this.InvokeTextModeratorAsync<string, IdentifyLanguageResult>(content, Constants.Operations.DetectLanguage.ToString(),
                        mediaType);
        }
        #endregion

        #region private methods

        private async Task<S> InvokeImageModeratorAsync<T, S>(dynamic imageRequest, string operation, List<KeyValue> metaData = null)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, $"/ProcessImage/{operation}?"));

            if(metaData != null)
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());

            return
                await
                    this.SendAsync<T, S>("POST", imageRequest, request)
                        .ConfigureAwait(false);
        }

        private async Task<S> InvokeTextModeratorAsync<T, S>(dynamic textRequest, string operation, Constants.MediaType mediaType, List<KeyValue> metaData = null)
        {
            StringBuilder requestUrl = new StringBuilder(string.Concat(this.ApiRoot, $"/ProcessText/{operation}?"));

            if(metaData != null)
            foreach (var k in metaData)
            {
                requestUrl.Append(string.Concat(k.Key, "=", k.Value));
                requestUrl.Append("&");
            }
            var request = WebRequest.Create(requestUrl.ToString());
            request.ContentType = ModeratorHelper.GetEnumDescription(mediaType);

            return
                await
                    this.SendAsync<T, S>("POST", textRequest, request)
                        .ConfigureAwait(false);
        }

        #endregion

       
    }
}
