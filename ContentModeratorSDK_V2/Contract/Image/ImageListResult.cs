using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    public class ImageListResult :Result
    {
        public string ContentSource { get; set; }
        public List<long> ContentIds { get; set; }
    }
}
