# Microsoft Content Moderator API: Windows Client Library
This repo contains the Windows client library & sample for the Microsoft Content Moderator API, an offering within [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services), formerly known as Project Oxford.
* [Learn about the Content Moderator](https://www.microsoft.com/cognitive-services/en-us/content-moderator)
* [Read the documentation](https://www.microsoft.com/cognitive-services/en-us/content-moderator/documentation)
* [Find more SDKs & Samples](https://www.microsoft.com/cognitive-services/en-us/SDK-Sample?api=content%20moderator)


## The Client Library
The client library is a thin C\# client wrapper for the Content Moderator API.

### Build the Library
 1. Starting in the folder where you clone the repository (this folder)

 2. In a git command line tool, type `git submodule init` (or do this through a UI)

 3. Pull in the shared Windows code by calling `git submodule update`

 4. Start Microsoft Visual Studio 2015 and select `File > Open > Project/Solution`.
 
 5. Double-click the Visual Studio 2015 Solution (.sln) file.

 6. Press Ctrl+Shift+B, or select `Build > Build Solution`.

### Run the tests
After the build is complete, press F5 to run the sample.

First, you must obtain a Content Moderator API subscription key by [following the instructions on our website](<https://www.microsoft.com/cognitive-services/en-us/content-moderator/documentation/quickstart>).

Microsoft will receive the images you upload and may use them to improve the Content Moderator
API and related services. By submitting an image, you confirm you have consent from everyone in it.

## Contributing
We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can. Learn more about how you can help on our [Contribution Rules & Guidelines](</CONTRIBUTING.md>). 

You can reach out to us anytime with questions and suggestions using our communities below: 
 - **Feedback & feature requests:** [Cognitive Services UserVoice Forum](<https://cognitive.uservoice.com>)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## License
All Microsoft Cognitive Services SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).


## Developer Code of Conduct
Developers using Cognitive Services, including this client library & sample, are expected to follow the “Developer Code of Conduct for Microsoft Cognitive Services”, found at [http://go.microsoft.com/fwlink/?LinkId=698895](http://go.microsoft.com/fwlink/?LinkId=698895).
