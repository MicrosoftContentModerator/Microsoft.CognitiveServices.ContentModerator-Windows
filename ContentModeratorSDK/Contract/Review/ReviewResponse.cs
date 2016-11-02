using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{
    public class ReviewResponse
    {
        public string ReviewId { get; set; }

        public ICollection<KeyValue> Metadata { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }

        public string ContentId { get; set; }

        public string CallbackEndpoint { get; set; }

        public string Status { get; set; }

        public ICollection<KeyValue> ReviewerResultTags { get; set; }

        public string CreatedBy { get; set; }
    }
}
