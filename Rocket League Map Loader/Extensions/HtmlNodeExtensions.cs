using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace RL_Map_Loader.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static List<HtmlNode> GetElementsByClassName(this HtmlDocument document, string className)
            => document?.DocumentNode?.GetDescendantsByClass(className);

        public static HtmlNode GetFirstElementByClassName(this HtmlDocument document, string className)
            => document?.DocumentNode?.GetDescendantsByClass(className)?.FirstOrDefault();

        public static List<HtmlNode> GetDescendantsByClass(this HtmlNode node, string className)
            => node?.Descendants()?.Where(x => x.HasClass(className)).ToList();

        public static HtmlNode GetFirstDescendantByClass(this HtmlNode node, string className)
            => node?.Descendants()?.FirstOrDefault(x => x.HasClass(className));

        public static HtmlNode GetLastDescendantByClass(this HtmlNode node, string className)
            => node?.Descendants()?.LastOrDefault(x => x.HasClass(className));

        public static List<HtmlNode> GetDescendantsByTagName(this HtmlNode node, string tagName)
            => node?.Descendants(tagName)?.ToList();

        public static HtmlNode GetFirstDescendantByTagName(this HtmlNode node, string tagName)
            => node?.Descendants(tagName)?.FirstOrDefault();

        public static HtmlNode GetFirstDescendantByTagNameWithInnerText(this HtmlNode node, string tagName, string innerText)
            => node?.Descendants(tagName)?.FirstOrDefault(x => x.InnerText.Contains(innerText));

        public static HtmlNode GetFirstDescendantOfTypeWithAttribute(this HtmlNode node, string tagName, string attributeName, string attributeValue)
            => node?.Descendants(tagName)?.FirstOrDefault(x => x.GetAttributeValue(attributeName, null) == attributeValue);

        public static HtmlNode GetSecondChild(this HtmlNode node) =>
            node?.ChildNodes != null && node.ChildNodes.Any() && node.ChildNodes.Count >= 2 ? node.ChildNodes[1] : null;
    }
}
