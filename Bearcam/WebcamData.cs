using System;
using Foundation;
using UIKit;

namespace Bearcam
{
    public struct WebcamData
    {
        public string Title { get; set; }
        public NSUrl Url { get; set; }
        public UIImage Thumbnail { get; set; }
    }
}

