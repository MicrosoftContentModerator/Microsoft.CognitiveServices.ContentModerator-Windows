using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    public class MatchTerm
    {
        public string Term;
        public int Index;
    }

    public class MatchUrl
    {
        public MatchUrlCategories categories;

        public string URL;
    }
    public class MatchUrlCategories
    {
        public double Adult;
        public double Malware;
        public double Phishing;
    }
}
