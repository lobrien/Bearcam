using System;
using AVFoundation;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Bearcam
{
    public partial class ViewController : UIViewController
    {
        AVAsset asset;
        AVPlayerItem playerItem;
        AVPlayer player;
        AVPlayerLayer playerLayer;

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public void ActivateWebcam (NSUrl url)
        {
            if (playerLayer != null)
            {
                playerLayer.RemoveFromSuperLayer ();
            }
            asset = AVAsset.FromUrl (url);
            playerItem = new AVPlayerItem (asset);
            player = new AVPlayer (playerItem);
            playerLayer = AVPlayerLayer.FromPlayer (player);
            var frame = new CGRect (0, 0, this.View.Frame.Width, this.View.Frame.Height);
            playerLayer.Frame = frame;
            this.View.Layer.AddSublayer (playerLayer);
            player.Play ();

        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.


        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


