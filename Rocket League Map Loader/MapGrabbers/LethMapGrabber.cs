using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Rocket_League_Map_Loader.Models;
using System.Web;

namespace Rocket_League_Map_Loader.MapGrabbers
{
    public class LethMapGrabber
    {
        private List<Map> Maps { get; set; } = new List<Map>();

        public List<Map> GetLethamyrsMaps()
        {
            var url = "https://lethamyr.com/mymaps";
            Maps = new List<Map>();
            GetNextPage(url);

            foreach(var map in Maps)
                ParseMapPage(map);

            return Maps.Distinct().ToList();
        }

        public List<Map> GetCommunityMaps()
        {
            var url = "https://lethamyr.com/community-maps-1";
            Maps = new List<Map>();
            GetNextPage(url);

            foreach (var map in Maps)
                ParseMapPage(map);

            return Maps.Distinct().ToList();
        }

        private string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private void GetNextPage(string url)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Get(url));

            var articles = htmlDoc.DocumentNode.Descendants("article");

            foreach(HtmlNode article in articles)
            {
                var map = ParseArticle(article);

                if (map == null)
                    continue;
                
                if (Maps.All(x => x.Name != map.Name))
                    Maps.Add(map);
            }

            var olderNodes = htmlDoc.DocumentNode.Descendants().Where(x => x.HasClass("older"));

            if(!olderNodes.Any())
                return;

            var nextNode = olderNodes?.LastOrDefault();

            if(nextNode != null && nextNode.ChildNodes != null && nextNode.ChildNodes?.Count > 1)
            {
                var nextHref = nextNode.ChildNodes[1].GetAttributeValue("href", null);

                if (nextHref != null)
                    GetNextPage($"https://lethamyr.com{nextHref}");
            }
        }

        private Map ParseArticle(HtmlNode article)
        {
            var map = new Map();

            var titleNode = article.Descendants("h1").FirstOrDefault();

            if(titleNode == null)
                return null;

            map.Name = titleNode.ChildNodes[1].InnerText.Replace("\n", "").Trim();

            var alreadyDownloadedMap = AppState.DownloadedMaps.SingleOrDefault(x => x.Name == map.Name);

            if (alreadyDownloadedMap != null)
                return alreadyDownloadedMap;

            var imageNode = article.Descendants("img").FirstOrDefault();

            var imageSrc = imageNode?.GetAttributeValue("data-src", null);

            if (imageSrc != null)
            {
                var wc = new WebClient();
                var stream = wc.OpenRead(imageSrc);

                map.Image = new BitmapImage();
                map.Image.BeginInit();
                map.Image.StreamSource = stream;
                map.Image.EndInit();
            }

            var mapHref = titleNode.ChildNodes[1].GetAttributeValue("href", null);
            map.Webpage = $"https://lethamyr.com{mapHref}";

            var blogCategoryListNode = article.Descendants().FirstOrDefault(x => x.HasClass("blog-categories-list"));

            if(blogCategoryListNode != null)
            {
                foreach (HtmlNode blogCategoryNode in blogCategoryListNode.Descendants("a"))
                    map.BlogCategories.Add(blogCategoryNode.InnerText);
            }

            var descriptionNode = article.Descendants().First(x => x.HasClass("blog-excerpt-wrapper"));
            map.ShortDescription = descriptionNode.ChildNodes.First().InnerText;

            return map;
        }

        private void ParseMapPage(Map map)
        {
            if(map.Webpage == null)
                return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Get(map.Webpage));

            var downloadNode = htmlDoc.DocumentNode.Descendants("a").First(x => x.InnerText.Contains("Download"));
            var downloadHref = downloadNode?.GetAttributeValue("href", null);
            var googleDriveId = downloadHref.Split('/')[5];
            var directDownloadLink = $"https://docs.google.com/uc?export=download&id={googleDriveId}";
            map.DownloadLink = directDownloadLink;

            var authorNode = htmlDoc.DocumentNode.Descendants("meta").First(x => x.GetAttributeValue("itemprop", null) == "author");
            var shortDescriptionNode = htmlDoc.DocumentNode.Descendants("meta").First(x => x.GetAttributeValue("itemprop", null) == "description");
            var datePublishedNode = htmlDoc.DocumentNode.Descendants("meta").First(x => x.GetAttributeValue("itemprop", null) == "datePublished");

            map.Info = new MapInfo
            {
                author = authorNode.GetAttributeValue("content", null),
                desc = shortDescriptionNode.GetAttributeValue("content", null),
            };

            DateTime.TryParse(datePublishedNode.GetAttributeValue("content", string.Empty), out var datePublished);
            map.DatePublished = datePublished;

            var article = htmlDoc.DocumentNode.Descendants("article").First();
            var dataBlocks = article.Descendants().Where(x => x.HasClass("sqs-block-content"));

            foreach(var dataBlock in dataBlocks)
            {
                var header = dataBlock.Descendants("h3").FirstOrDefault()?.InnerText;
                var text = dataBlock.Descendants("p").FirstOrDefault()?.InnerText;

                if(header != null && header == "Description")
                    map.LongDescription = text;

                if(header != null && header == "Recommended Settings")
                    map.RecommendedSettings = text;
            }

            var embedBlock = article.Descendants().FirstOrDefault(x => x.HasClass("embed-block"));
            var json = embedBlock?.GetAttributeValue("data-block-json", null);

            if(json != null)
            {
                var embedData = JsonConvert.DeserializeObject<EmbedData>(System.Net.WebUtility.HtmlDecode(json));
                map.YoutubeEmbed = embedData;
                map.YoutubeUrl = embedData.url;
            }

            var tagNodes = article.Descendants().Where(x => x.HasClass("blog-item-tag-wrapper"));

            foreach(var tagNode in tagNodes)
            {
                var tagAnchor = tagNode.ChildNodes.FirstOrDefault();

                if (tagAnchor != null)
                    map.Tags.Add(tagAnchor.InnerText); 
            }
        }
    }
}
