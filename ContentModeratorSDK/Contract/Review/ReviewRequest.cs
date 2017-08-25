using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{
    public class ReviewRequest
    {
        public string Content { get; set; }
        public string ContentId { get; set; }
        public string CallbackEndpoint { get; set; }
        public ReviewStatus Status { get; set; }

        public IList<KeyValue> Metadata { get; set; }
        public ContentType Type { get; set; }

    }
    public class VideoReviewRequest : ReviewRequest
    {
        public long TimeScale { get; set; }
        public List<VideoFrame> VideoFrames { get; set; }
        public string Comments { get; set; }
    }
    
    public enum ReviewStatus
    {
        Pending = 1,
        Complete = 2,
        UnPublished = 3
    }
}
