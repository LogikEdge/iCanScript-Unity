//using UnityEngine;
//using System.Collections;
//
//public class HtmlParserExample {
//
//    protected void ScanLinks(string url)
//     {
//       // Download page
//       WebClient client = new WebClient();
//       string html = client.DownloadString(url);
//
//       // Scan links on this page
//       HtmlTag tag;
//       HtmlParser parse = new HtmlParser(html);
//       while (parse.ParseNext("a", out tag))
//       {
//         // See if this anchor links to us
//         string value;
//         if (tag.Attributes.TryGetValue("href", out value))
//         {
//           // value contains URL referenced by this link
//         }
//       }
//     }
//}
