using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{
    public class KeyValue
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentType
    {
        Image = 1,
        Text = 2,
        Video = 3
    }
}
