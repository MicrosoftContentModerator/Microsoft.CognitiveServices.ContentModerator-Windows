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

        public IList<KeyValue> Metadata { get; set; }
        public ContentType Type { get; set; }

    }
}
