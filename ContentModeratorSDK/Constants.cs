using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.ContentModerator
{
    public class Constants
    {
        public enum Operations
        {
            Evaluate,
            FindFaces,
            OCR,
            Match,
            Screen,
            DetectLanguage,

        }

        public enum HttpMethod
        {
            POST,
            GET,
            DELETE,
            PUT
        }

        public enum MediaType
        {
            [Description("text/html")]
            Html,
            [Description("text/markdown")]
            MarkDown,
            [Description("text/plain")]
            Plain,
            [Description("text/xml")]
            Xml,
            [Description("application/json")]
            ApplicationJson,
            [Description("image/jpeg")]
            ImageJpeg,
            [Description("image/png")]
            ImagePng,
        }

        #region Image Urls

        public const string IMAGE_ADD = "/imagelists/{0}/images";
        public const string IMAGE_DELETE = "/imagelists/{0}/images/{1}";
        public const string IMAGE_DELETEALL = "/imagelists/{0}/images";
        public const string IMAGE_GETALLIDS = "/imagelists/{0}/Images";
        #endregion

        #region Image Lists
        public const string IMAGELIST_CREATE = "/imagelists";
        public const string IMAGELIST_UPDATE = "/imagelists/{0}";
        public const string IMAGELIST_REFRESHINDEX = "/imagelists/{0}/RefreshIndex";
        public const string IMAGELIST_GETALL = "/imagelists";
        public const string IMAGELIST_GETDETAIL = "/imagelists/{0}";
        public const string IMAGELIST_DELETE = "/imagelists/{0}";
        #endregion


        #region Term Urls

        public const string TERM_ADD = "/termlists/{0}/terms/{1}";
        public const string TERM_DELETE = "/termlists/{0}/terms/{1}";
        public const string TERM_DELETEALL = "/termlists/{0}/terms";
        public const string TERM_GETALLIDS = "/termlists/{0}/terms";
        #endregion

        #region Term Lists
        public const string TERMLIST_CREATE = "/termlists";
        public const string TERMLIST_UPDATE = "/termlists/{0}";
        public const string TERMLIST_REFRESHINDEX = "/termlists/{0}/RefreshIndex";
        public const string TERMLIST_GETALL = "/termlists";
        public const string TERMLIST_GETDETAIL = "/termlists/{0}";
        public const string TERMLIST_DELETE = "/termlists/{0}";
        #endregion

        #region Review Urls
        public const string GET_REVIEW_DETAILS = "/teams/{0}/reviews/{1}";
        public const string CREATE_REVIEW = "/teams/{0}/reviews";
        public const string GET_JOB_DETAILS = "/teams/{0}/jobs/{1}";
        public const string CREATE_JOB = "/teams/{0}/jobs";
        public const string GET_ALL_TEAM_WORKFLOWS = "/teams/{0}/workflows";
        public const string GET_TEAM_WORKFLOW = "/teams/{0}/workflows/{1}";
        public const string CREATE_WORKFLOW = "/teams/{0}/workflows/{1}";

        #endregion
    }
}
