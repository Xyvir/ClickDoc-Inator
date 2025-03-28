using System;

namespace Better_Steps_Recorder
{
    public class LinkHeading
    {
        public Uri HyperlinkURL { get; set; }
        public string HyperlinkText { get; set; }
        public string SpoilerTitle { get; set; }
        public string SpoilerText { get; set; }

        public LinkHeading()
        {
            HyperlinkURL = new Uri("http://example.com");
            HyperlinkText = "Default Hyperlink Text";
            SpoilerTitle = "Default Spoiler Title";
            SpoilerText = "Default Spoiler Text";
        }
    }
}
