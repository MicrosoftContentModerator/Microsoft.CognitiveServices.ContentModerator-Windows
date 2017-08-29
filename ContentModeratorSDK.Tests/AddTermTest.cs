using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.ContentModerator;
using Microsoft.CognitiveServices.ContentModerator.Contract.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentModeratorSDK.Test
{
    [TestClass]
    public class AddTermTest
    {
        private readonly IListManagementClient _textListclient = null;
        public AddTermTest()
        {
            this._textListclient = new ListManagementClient(ConfigurationManager.AppSettings["subscriptionkey"]);
        }
        [TestMethod]
        public async Task AddTermTestAsync()
        {
            var res = await _textListclient.TermAddAsync("samplelistid", "sdktest", "eng");
        }

        [TestMethod]
        public async Task UpdateTermsBulkTestAsync()
        {
            var contents = new List<string> {"sampleterm"};
            Term tc = new Term
            {
                LanguageId = "eng",
                Contents = contents
            };
            var tcs = new List<Term> {tc};
            var terms = new TermContent {add = tcs};
            var res = await _textListclient.UpdateTermsBulkAsync("samplelistid", terms, "eng");
        }
    }
}
