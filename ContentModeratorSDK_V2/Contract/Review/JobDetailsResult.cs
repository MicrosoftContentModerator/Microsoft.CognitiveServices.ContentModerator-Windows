using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{
    public class JobDetailsResult
    {
        public JobDetailsResult()
        {
            this.ResultMetaData = new List<KeyValue>();
        }
        public string Id { get; set; }
        public string TeamName { get; set; }
        public JobStatus Status { get; set; }
        public string WorkflowId { get; set; }
        public string CallBackEndpoint { get; set; }
        public string ReviewId { get; set; }
        public List<KeyValue> ResultMetaData { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobStatus
    {
        Pending = 1,
        Complete = 2,
        Failed = 3
    }
}
