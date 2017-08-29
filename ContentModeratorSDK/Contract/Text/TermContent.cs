using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    public class TermContent
    {
       // public List<Term> Delete { get; set; }
        public List<Term> add { get; set; }

    }

    public class Term
    {
        public string LanguageId { get; set; }

        public List<string> Contents { get; set; }
    }
}
