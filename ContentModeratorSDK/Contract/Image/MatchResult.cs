using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    public class MatchResult : ImageResult
    {
        /// <summary>
        /// Boolean indicating whether an image is a match
        /// </summary>
        public bool IsMatch;

        /// <summary>
        /// Match details
        /// </summary>
        public List<Match> Matches;
    }

    public class Match
    {
        /// <summary>
        /// Match score. 1 means exact match.
        /// </summary>
        public double Score;

        /// <summary>
        /// Match ID.
        /// </summary>
        public int MatchId;

        /// <summary>
        /// Content Source id for the image
        /// </summary>
        public string Source;

        /// <summary>
        /// All Tag(s) provided while adding the image
        /// </summary>
        public int[] Tags;

        /// <summary>
        /// Label given when image added
        /// </summary>
        public string Label;


    }
}
