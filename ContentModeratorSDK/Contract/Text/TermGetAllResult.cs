using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    public class TermGetAllResult
    {
        public TermsData Data { get; set; }
    }

    public class TermsData : Result
    {
        public string Language { get; set; }

        public List<TermObject> Terms { get; set; } 
    }

    public class TermObject
    {
        public string Term { get; set; }
    }
}

