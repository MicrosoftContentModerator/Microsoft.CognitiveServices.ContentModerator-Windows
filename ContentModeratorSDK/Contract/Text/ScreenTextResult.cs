using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    /// <summary>
    /// Result from screening a text, containing details regarding the Match
    /// </summary>
    public class ScreenTextResult : Result
    {
        public string OriginalText;
        public string NormalizedText;
        public string Misrepresentation;
        public string Language;
        public MatchTerm[] Terms;
        public MatchUrl[] Urls;
        public PII PII;
        public string ContentId;
        public bool IsMatch;
        public MatchDetails MatchDetails;
    }
}
