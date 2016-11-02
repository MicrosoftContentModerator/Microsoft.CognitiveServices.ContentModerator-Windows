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
    public class IdentifyLanguageResult : Result
    {
        public string DetectedLanguage;
    }
}
