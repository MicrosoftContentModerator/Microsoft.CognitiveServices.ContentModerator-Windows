using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator.Contract.Image;
using Microsoft.CognitiveServices.ContentModerator.Contract.Text;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public interface IModeratorClient
    {
        /// <summary>
        /// Call Evaluate Image, to determine whether the image violates any policy based
        ///  on multiple ratings.
        /// </summary>
        /// <param name="cacheImage"></param>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <returns></returns>
        Task<EvaluateImageResult> EvaluateImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage);

        /// <summary>
        /// Call Evaluate Image, to determine whether the image violates any policy based
        ///  on multiple ratings.
        /// </summary>
        /// <param name="cacheImage"></param>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <returns></returns>
        Task<EvaluateImageResult> EvaluateImageAsync(Stream content,
            bool cacheImage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <returns></returns>
        Task<DetectFacesResult> DetectFacesImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <returns></returns>
        Task<DetectFacesResult> DetectFacesImageAsync(Stream content,
            bool cacheImage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <param name="enhanced"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        Task<OcrImageResult> OCRImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage, bool enhanced = true, string language = "eng");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <param name="enhanced"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        Task<OcrImageResult> OCRImageAsync(Stream content,
            bool cacheImage, bool enhanced = true, string language = "eng");

        /// <summary>
        /// Match an image against the images url provided.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <param name="listid"></param>
        /// <returns></returns>
        Task<MatchResult> MatchImageAsync(string content, DataRepresentationType dataRepresentationType,
            bool cacheImage, string listid);

        /// <summary>
        /// Match an image against the images stream provided.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataRepresentationType"></param>
        /// <param name="cacheImage"></param>
        /// <param name="listid"></param>
        /// <returns></returns>
        Task<MatchResult> MatchImageAsync(Stream content,
            bool cacheImage, string listid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mediaType"></param>
        /// <param name="language"></param>
        /// <param name="autocorrect"></param>
        /// <param name="urls"></param>
        /// <param name="pii"></param>
        /// <param name="listIds"></param>
        /// <returns></returns>
        Task<ScreenTextResult> ScreenTextAsync(string content, Constants.MediaType mediaType, string language,
            bool autocorrect, bool urls,
            bool pii, string listIds);

        /// <summary>
        /// Identifies languange of the content passed
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        Task<IdentifyLanguageResult> IdentifyLanguageAsync(string content, Constants.MediaType mediaType);

    }
}
