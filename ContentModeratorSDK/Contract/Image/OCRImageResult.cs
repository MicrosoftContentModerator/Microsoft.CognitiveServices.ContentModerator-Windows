using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    public class OcrImageResult : ImageResult
    {
        public OcrImageResult()
        {
            this.Candidates = new List<Candidate>();
        }
        /// <summary>
        /// Array of name value pairs wit specific information about evaluate
        /// </summary>
        public AdvancedInfo[] AdvancedInfo;

        /// <summary>
        /// OCR Result Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// OCR Text Result
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// OCR Result Candidates
        /// </summary>
        public List<Candidate> Candidates { get; set; }

    }

    public class Candidate
    {
        /// <summary>
        /// Text of candidate
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Confidence score of candidate
        /// </summary>
        public int Confidence { get; set; }
    }
}
