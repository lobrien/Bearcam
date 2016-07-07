using System;
using AVFoundation;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Bearcam
{
    public partial class ViewController : UIViewController
    {
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.

            var baseUrl = NSUrl.FromString("https://www.youtube.com/watch?v=gtUucAcQoaE");
            var vs = YoutubeParser.VideosForUrl (baseUrl);

            var src = vs["live"];
            var asset = AVAsset.FromUrl(src);
            var playerItem = new AVPlayerItem(asset);
            var player = new AVPlayer (playerItem);
            var playerLayer = AVPlayerLayer.FromPlayer (player);
            var frame = new CGRect (0, 0, this.View.Frame.Width, this.View.Frame.Height);
            playerLayer.Frame = frame;
            this.View.Layer.AddSublayer (playerLayer);
            player.Play ();

        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


