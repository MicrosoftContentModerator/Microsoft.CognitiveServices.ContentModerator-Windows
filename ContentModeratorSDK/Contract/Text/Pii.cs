using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Text
{
    public class PII
    {
        public List<Email> Email { get; set; }
        public List<IPAddress> IPA { get; set; }
        public List<PhoneNumber> Phone { get; set; }
        public List<Address> Address { get; set; }
    }

    public class PIIBase
    {
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class Email : PIIBase
    {
        public string Detected { get; set; }
        public string SubType { get; set; }
    }


    public class IPAddress : PIIBase
    {
        public string SubType { get; set; }
    }

    public class PhoneNumber : PIIBase
    {
        public string CountryCode { get; set; }
    }

    public class Address : PIIBase
    {
    }
}
