using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class ApiError
    {
        public string Message { get; set; }
        [JsonIgnore]
        public ErrorType ErrorType { get; set; }
        public string TrackingId { get; set; }
        public List<Error> Errors { get; set; }
    }
    public enum ErrorType
    {
        Default,
        Conflict,
        InvalidImage,
        InvalidContentSource,
        NoContentSource,
        MaxImagesExceeded,
        ContentSourceFormatException,
        ContentLengthExceedLimit,
        InvalidRequest,
        InvalidJson,
        LanguageIdMissingRequest,
        InvalidMediaType,
        NonDetectableLanguage
    }

    public class Error
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
