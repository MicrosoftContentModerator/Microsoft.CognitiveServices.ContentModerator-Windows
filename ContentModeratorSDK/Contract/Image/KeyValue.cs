using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    /// <summary>
    /// Key value pairs for passing additional meta data info.
    /// </summary>
    public class KeyValue
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
