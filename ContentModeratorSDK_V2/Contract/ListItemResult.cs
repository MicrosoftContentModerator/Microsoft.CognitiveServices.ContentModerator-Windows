namespace Microsoft.CognitiveServices.ContentModerator.Contract
{
    using System.Collections.Generic;

    public class ListItemResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
