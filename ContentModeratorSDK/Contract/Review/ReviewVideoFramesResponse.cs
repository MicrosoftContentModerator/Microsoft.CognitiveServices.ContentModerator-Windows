using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{

    public class ReviewVideoFramesResponse
    {
        public List<VideoFrame> VideoFrames { get; set; }
    }
    public class VideoFrame
    {
        public string Timestamp { get; set; }
        public string FrameImage { get; set; }
        public ICollection<KeyValue> Metadata { get; set; }
        public ICollection<KeyValue> ReviewerResultTags { get; set; }
        public byte[] FrameImageBytes { get; set; }
    }
}
