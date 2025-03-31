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
            HyperlinkURL = new Uri("https://www.youtube.com/watch?v=###########");
            HyperlinkText = "Watch the Video";
            SpoilerTitle = "Video Transcript";
            SpoilerText = "Default Spoiler Text";
            // Note these are also 'set' under the function newToolStripMenuItem_Click() in Form1.cs
        }
    }
}
