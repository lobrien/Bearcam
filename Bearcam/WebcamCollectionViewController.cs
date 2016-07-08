// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace Bearcam
{
    static class WebcamThumbnail
    {
        private static UIImage unknown = UIImage.FromFile ("bear_thumbnail.jpeg");
        public static UIImage Unknown 
        {
            get 
            {
                return unknown;
            }
        }
    }

    public class EventArgsT<T> : EventArgs
    {
        public T Value { get; }

        public EventArgsT (T v)
        {
            Value = v;
        }
    }



    class WebcamCollectionViewSource : UICollectionViewSource
    {
        List<WebcamData> webcams = new List<WebcamData> ();

        public WebcamCollectionViewSource ()
        {
            //var url = YoutubeParser.VideosForUrl (NSUrl.FromString ("https://www.youtube.com/watch?v=gtUucAcQoaE")) ["live"];
            //webcams.Add (new WebcamData { Thumbnail = WebcamThumbnail.Unknown, Title = "Bears", Url = url });

            //Bea cam
            var wc = YoutubeParser.WebcamForUrl(NSUrl.FromString ("https://www.youtube.com/watch?v=gtUucAcQoaE"));
            webcams.Add (wc);
            //Albatross cam
            webcams.Add (YoutubeParser.WebcamForUrl (NSUrl.FromString ("https://www.youtube.com/watch?v=X7Rs9B72-pU")));
            //Jelly cam
            webcams.Add (YoutubeParser.WebcamForUrl (NSUrl.FromString ("https://www.youtube.com/watch?v=ceP_zvyM3A4")));
        }

        public override nint NumberOfSections (UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            return webcams.Count;
        }

        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (WebcamCell)collectionView.DequeueReusableCell ("WebcamCellID", indexPath);
            var webcam = webcams [indexPath.Row];
            cell.Thumbnail.Image = webcam.Thumbnail;
            //Have to do this lazily, so that Thumbnail != null
            //cell.Thumbnail.AdjustsImageWhenAncestorFocused = true;
            cell.Title.Text = webcam.Title;
            cell.Url = webcam.Url;

            return cell;
        }
   }

    class WebcamCollectionViewDelegate : UICollectionViewDelegate
    {
        public override bool CanFocusItem (UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        public override void DidUpdateFocus (UICollectionView collectionView, UICollectionViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
        {
            
        }

        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            CellSelected (this, new EventArgsT<nint>(indexPath.Item));
        }

        public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
        {
        }

        public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
        {
        }

        public override NSIndexPath GetIndexPathForPreferredFocusedView (UICollectionView collectionView)
        {
            return NSIndexPath.Create (new [] { 0, 0 });
        }

        public event EventHandler<EventArgsT<nint>> CellSelected = delegate { };
    }


	public partial class WebcamCollectionViewController : UICollectionViewController
	{
        WebcamCollectionViewSource collectionSource = new WebcamCollectionViewSource ();
    
		public WebcamCollectionViewController (IntPtr handle) : base (handle)
		{
            
		}

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            this.CollectionView.Source = collectionSource;
            var webcamDelegate = new WebcamCollectionViewDelegate ();
            this.CollectionView.Delegate = webcamDelegate;
            this.CollectionView.RemembersLastFocusedIndexPath = true;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            switch (segue.Identifier)
            {
            case "DisplayWebcam":
                var webcamCell = (WebcamCell)sender;

                var webcamVC = (ViewController) segue.DestinationViewController;
                webcamVC.ActivateWebcam (webcamCell.Url);
                break;
            default:
                //Another segue
                break;
            }
        }

    }
}