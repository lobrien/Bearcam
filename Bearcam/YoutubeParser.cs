using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundation;

namespace Bearcam
{
    public static class YoutubeParser
    {
        const string YOUTUBE_INFO_URL = "https://www.youtube.com/get_video_info?video_id=";
        const string USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit(537.4 (KHTML, like Gecko) Chrome/22.0.1229.79 Safari/537.4";


        static Dictionary<string, string> DictionaryFromQueryStringComponents (this string input)
        {
            return Regex.Matches (input, "([^?=&]+)(=([^&]*))?").Cast<Match> ().ToDictionary (x => x.Groups [1].Value, x => x.Groups [3].Value);
        }

        public static string IdForUrl (NSUrl url)
        {
            String id = null;

            if (url.Host == "youtu.be")
            {
                id = url.PathComponents [1];
            }
            else
            {
                if (url.AbsoluteString.IndexOf ("www.youtube.com/embed") == 1)
                {
                    id = url.PathComponents [2];
                }
                else
                {
                    if (url.Host == "youtube.googleapis.com" || url.PathComponents [0] == "www.youtube.com")
                    {
                        id = url.PathComponents [2];
                    }
                    else
                    {
                        id = url.AbsoluteString.DictionaryFromQueryStringComponents()["v"];
                    }
                }
            }

            return id;
        }

        public static Dictionary<string, NSUrl> VideosForId (string id)
        {
            var vs = new Dictionary<string, NSUrl> ();
            if (id == null)
            {
                throw new ArgumentNullException ();
            }
            var uri = YOUTUBE_INFO_URL + id;
            using (var c = new WebClient ())
            {
                c.Headers.Add (HttpRequestHeader.UserAgent, USER_AGENT);
                var resultString = c.DownloadString (uri);
                var dict = resultString.DictionaryFromQueryStringComponents();
                if (dict == null)
                {
                    throw new NullReferenceException ();
                }
                var streamMapArray = dict ["url_encoded_fmt_stream_map"];
                if (streamMapArray.Length > 0)
                {
                    var streamMaps = streamMapArray.Split (new [] { ',' });
                    foreach (var videoEncodedString in streamMaps)
                    {
                        var videoComponents = videoEncodedString.DictionaryFromQueryStringComponents ();
                        var typ = WebUtility.UrlDecode (videoComponents ["type"]);
                        if (!videoComponents.ContainsKey ("stereo3d"))
                        {
                            if (videoComponents.ContainsKey ("itag"))
                            {
                                var signature = videoComponents ["itag"];
                                if (typ.IndexOf ("mp4") > -1)
                                {
                                    var urlComponent = WebUtility.UrlDecode (videoComponents ["url"]);
                                    var url = NSUrl.FromString ($"{urlComponent}&signature={signature}");
                                    var quality = WebUtility.UrlDecode (videoComponents ["quality"]);
                                    vs.Add (quality, url);
                                }
                            }
                        }
                        else
                        {
                            //3D video. FEATURE: figure out streams
                        }
                    }

                }
                else
                {
                    //streamMapArray.Length == 0
                    //Check for live
                    if (dict.ContainsKey ("live_playback") && dict.ContainsKey ("hlsvp"))
                    {
                        vs.Add ("live", NSUrl.FromString (WebUtility.UrlDecode(dict ["hlsvp"])));
                    }
                }
            }


            return vs;
        }

        public static Dictionary<string, NSUrl> VideosForUrl (NSUrl url)
        {
            var id = YoutubeParser.IdForUrl (url);
            var vs = YoutubeParser.VideosForId (id);

            return vs;
        }

        internal static WebcamData WebcamForUrl (NSUrl url)
        {
            var id = YoutubeParser.IdForUrl (url);
            var uri = YOUTUBE_INFO_URL + id;
            using (var c = new WebClient ())
            {
                c.Headers.Add (HttpRequestHeader.UserAgent, USER_AGENT);
                var resultString = c.DownloadString (uri);
                var dict = resultString.DictionaryFromQueryStringComponents ();
                if (dict == null)
                {
                    throw new NullReferenceException ();
                }
                if (dict.ContainsKey ("live_playback") && dict.ContainsKey ("hlsvp"))
                {
                    var src = NSUrl.FromString (WebUtility.UrlDecode (dict ["hlsvp"]));
                    var title = WebUtility.UrlDecode(dict["title"]);
                    var thumbnailUrl = NSUrl.FromString(WebUtility.UrlDecode(dict ["thumbnail_url"]));
                    var thumbnail = WebcamThumbnail.Unknown;
                    if (thumbnailUrl != null)
                    {
                        try
                        {
                            var imgData = NSData.FromUrl (thumbnailUrl);
                            thumbnail = new UIKit.UIImage (imgData);
                        }
                        catch (Exception x)
                        {
                            //Swallow it -- some problem either with delivery or creation
                        }
                    }
                    return new WebcamData { Url = src, Title = title, Thumbnail = thumbnail };
                }
            }
                throw new Exception ();
        }
    }
}

