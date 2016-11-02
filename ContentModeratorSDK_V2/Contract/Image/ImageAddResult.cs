using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    public class ImageAddResult : Result
    {
        /// <summary>
        /// Detailed information from image add action
        /// </summary>
        public AdvancedInfo[] AdditionalInfo;

        /// <summary>
        /// Id of added image
        /// </summary>
        public string ContentId;
    }
}
