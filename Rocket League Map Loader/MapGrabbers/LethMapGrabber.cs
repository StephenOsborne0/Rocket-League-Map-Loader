using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RL_Map_Loader.Extensions;
using RL_Map_Loader.Models;

namespace RL_Map_Loader.MapGrabbers
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

            var documentNode = htmlDoc.DocumentNode;
            var articles = documentNode.Descendants("article");

            foreach(HtmlNode article in articles)
            {
                var map = ParseArticle(article);

                if (map == null)
                    continue;
                
                if (Maps.All(x => x.Name != map.Name))
                    Maps.Add(map);
            }

            var nextNode = documentNode.GetLastDescendantByClass("older");
            var nextHref = nextNode?.GetSecondChild()?.GetAttributeValue("href", null);

            if (nextHref != null)
                GetNextPage($"https://lethamyr.com{nextHref}");
        }

        private Map ParseArticle(HtmlNode article)
        {
            var map = new Map();
            var titleNode = article.GetFirstDescendantByTagName("h1");

            if(titleNode == null)
                return null;

            map.Name = titleNode.GetSecondChild().InnerText.Replace("\n", "").Trim();
            var mapCacheFileName = Path.Combine(AppState.MapCacheDirectory, $"{map.Name}.json");

            if(File.Exists(mapCacheFileName))
                return Map.Load(mapCacheFileName);

            var imageSrc = article.GetFirstDescendantByTagName("img")?.GetAttributeValue("data-src", null);

            if (imageSrc != null)
            {
                var wc = new WebClient();
                var stream = wc.OpenRead(imageSrc);

                map.Image = new BitmapImage();
                map.Image.BeginInit();
                map.Image.StreamSource = stream;
                //map.SaveImageSource(AppState.MapCacheDirectory, $"{map.Name}.bin");
                map.Image.EndInit();
            }

            var mapHref = titleNode.GetSecondChild().GetAttributeValue("href", null);
            map.Webpage = $"https://lethamyr.com{mapHref}";

            var blogCategoryListNode = article.GetFirstDescendantByClass("blog-categories-list");

            if(blogCategoryListNode != null)
            {
                foreach (HtmlNode blogCategoryNode in blogCategoryListNode.Descendants("a"))
                    map.BlogCategories.Add(blogCategoryNode.InnerText);
            }

            var descriptionNode = article.GetFirstDescendantByClass("blog-excerpt-wrapper");
            map.ShortDescription = descriptionNode.ChildNodes.First().InnerText;

            return map;
        }

        private void ParseMapPage(Map map)
        {
            if(map.Webpage == null)
                return;

            var mapCacheFileName = Path.Combine(AppState.MapCacheDirectory, $"{map.Name}.json");

            if (File.Exists(mapCacheFileName))
                return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Get(map.Webpage));

            var documentNode = htmlDoc.DocumentNode;

            var downloadHref = documentNode.GetFirstDescendantByTagNameWithInnerText("a", "Download")?.GetAttributeValue("href", null);
            map.GoogleDriveId = downloadHref.Split('/')[5];

            var authorNode = documentNode.GetFirstDescendantOfTypeWithAttribute("meta", "itemprop", "author");
            var shortDescriptionNode = documentNode.GetFirstDescendantOfTypeWithAttribute("meta", "itemprop", "description");
            var datePublishedNode = documentNode.GetFirstDescendantOfTypeWithAttribute("meta", "itemprop", "datePublished");

            map.Info = new MapInfo
            {
                author = authorNode.GetAttributeValue("content", null),
                desc = shortDescriptionNode.GetAttributeValue("content", null),
            };

            DateTime.TryParse(datePublishedNode.GetAttributeValue("content", string.Empty), out var datePublished);
            map.DatePublished = datePublished;

            var article = documentNode.GetFirstDescendantByTagName("article");
            var dataBlocks = article.GetDescendantsByClass("sqs-block-content");

            if(dataBlocks != null)
            {
                foreach (var dataBlock in dataBlocks)
                {
                    var header = dataBlock.GetFirstDescendantByTagName("h3")?.InnerText;
                    var text = dataBlock.GetFirstDescendantByTagName("p")?.InnerText;

                    if (header != null && header == "Description")
                        map.LongDescription = text;

                    if (header != null && header == "Recommended Settings")
                        map.RecommendedSettings = text;
                }
            }

            var youtubeEmbedJson = article.GetFirstDescendantByClass("embed-block")?.GetAttributeValue("data-block-json", null);

            if(youtubeEmbedJson != null)
            {
                var embedData = JsonConvert.DeserializeObject<EmbedData>(System.Net.WebUtility.HtmlDecode(youtubeEmbedJson));
                map.YoutubeEmbed = embedData;
                map.YoutubeUrl = embedData.url;
            }

            var tagNodes = article.GetDescendantsByClass("blog-item-tag-wrapper");

            if(tagNodes == null) 
                return;

            foreach (var tagNode in tagNodes)
            {
                var tagAnchor = tagNode.ChildNodes.FirstOrDefault();

                if(tagAnchor == null) 
                    continue;

                if (!map.Tags.Contains(tagAnchor.InnerText))
                    map.Tags.Add(tagAnchor.InnerText);
            }
        }
    }
}
