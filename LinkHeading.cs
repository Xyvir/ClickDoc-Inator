using System;

namespace Better_Steps_Recorder
{
    public class LinkHeading
    {
        public string HyperlinkURL { get; set; }
        public string HyperlinkText { get; set; }
        public string SpoilerTitle { get; set; }
        public string SpoilerText { get; set; }

        public LinkHeading()
        {
            HyperlinkURL = "";
            HyperlinkText = "";
            SpoilerTitle = "";
            SpoilerText = "";
            // Note these are also 'set' under the function newToolStripMenuItem_Click() in Form1.cs
        }
    }
}
