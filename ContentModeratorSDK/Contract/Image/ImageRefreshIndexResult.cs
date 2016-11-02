using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    public class ImageRefreshIndexResult : Result
    {
        /// <summary>
        /// Advanced Information from the request
        /// </summary>
        public AdvancedInfo[] AdvancedInfo;

        /// <summary>
        /// Boolean indicating if the update was succssful
        /// </summary>
        public bool IsUpdateSuccess;

        /// <summary>
        /// Content Source refreshed
        /// </summary>
        public string ContentSourceId;
    }
}
