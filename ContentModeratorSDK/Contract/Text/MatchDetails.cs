using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    // <summary>
    // Specific information about a match
    // </summary>
    public class MatchDetails
    {
        /// <summary>
        /// Advanced Information from the match
        /// </summary>
        public AdvancedInfo[] AdvancedInfo;

        /// <summary>
        /// The match flags containing details on the match operation
        /// </summary>
        public MatchFlag[] MatchFlags;
    }

    /// <summary>
    /// Name value pair with detailed information on the response
    /// </summary>
    public class AdvancedInfo
    {
        public string Key;
        public string Value;
    }

    /// <summary>
    /// Information about a match including the source ContentId and the actual score
    /// </summary>
    public class MatchFlag
    {
        /// <summary>
        /// Additional details about match
        /// </summary>
        public AdvancedInfo[] AdvancedInfo;

        /// <summary>
        /// Match score. 1 means exact match.
        /// </summary>
        public double Score;

        /// <summary>
        /// Match source ContentId
        /// </summary>
        public string Source;

    }
}
