namespace Microsoft.CognitiveServices.ContentModerator.Contract.Image
{
    /// <summary>
    /// Base class from a Image service result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Status of the result
        /// </summary>
        public Status Status { get; set; }

        public string TrackingId { get; set; }
        
    }
}