using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Developpez.MagazineTool.PostToXml
{
    public class PostToXmlConverter
    {
        private Generator _generator;
        private ArticleType _article;

        private PostToXmlConverter(Generator generator, ArticleType article)
        {
            _generator = generator;
            _article = article;
        }

        public static bool CreateXmlFromUrl(Generator generator, ArticleType article)
        {
            PostToXmlConverter converter = new PostToXmlConverter(generator, article);
            bool ok = false;

            if (article.URL.StartsWith("https://www.developpez.net/forums/"))
            {
                // Message dans le forum. Par exemple, une actualité
                string xmlFilePath = generator.GetArticleXmlFile(article);
                DirectoryInfo sourceDirectory = generator.GetArticleSourceDirectory(article);
                DirectoryInfo imagesDirectory = sourceDirectory.CreateSubdirectory("images");

                using (WebClient webClient = new WebClient())
                {
                    Regex regex = new Regex("#post([0-9]+)");
                    Regex bTag = new Regex("<b>.*</b>");
                    webClient.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    string content = webClient.DownloadString(article.URL);
                    Match match = regex.Match(article.URL);
                    long postID = long.Parse(match.Groups[1].Value);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
                    HtmlNode node = doc.DocumentNode.SelectSingleNode(String.Format("//div[@id='post_message_{0}']/blockquote", postID));
                    ReplaceImgNode(generator, article, node);
                    string postContent = node.InnerHtml.Trim();
                    postContent.Replace("<br>", "\n");

                    File.WriteAllText(xmlFilePath, postContent);
                    ok = true;
                }
            }
            
            return ok;
        }

        private static void ReplaceImgNode(Generator generator, ArticleType article, HtmlNode node)
        {
            HtmlNode div = node.SelectSingleNode(".//div[img]");
            int count = 1;
            DirectoryInfo articleDirectory =  generator.GetArticleSourceDirectory(article);
            DirectoryInfo imageDirectory = articleDirectory.CreateSubdirectory("images");

            while (div != null)
            {
                HtmlNode img = div.SelectSingleNode("img");
                string imageURL = img.Attributes["src"].Value;
                string imageExtension = Path.GetExtension(imageURL);
                string imageName = String.Format("{0}{1}", count, imageExtension);
                string imageLocalPath = Path.Combine("images", imageName);
                string imageLocalFullPath = Path.Combine(imageDirectory.FullName, imageName);
                HtmlNode newNode = HtmlNode.CreateNode(String.Format("<image src=\"./{0}\" align=\"center\"/>", imageLocalPath));
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imageURL, imageLocalFullPath);
                }
                div.ParentNode.ReplaceChild(newNode, div);

                div = node.SelectSingleNode(".//div[img]");
                count++;
            }
        }
    }
}
