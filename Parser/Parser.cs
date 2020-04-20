using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Parser.Exception;

namespace Parser
{
    public class Parser
    {
        private const string Url = @"http://www.porjati.net/";
        private const string UrlTemplate = @"http://www.porjati.net/page/{0}/";
        
        private readonly IVideoRepository _repository;
        private readonly HtmlWeb _htmlWeb;
        private readonly uint _threads;
        private readonly ConcurrentBag<string> _errors;

        public IReadOnlyCollection<string> Errors => _errors as IReadOnlyCollection<string>;
        
        public Parser(uint threads, IVideoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _errors = new ConcurrentBag<string>();
            _threads = threads;
            _htmlWeb = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("windows-1251"),
                UsingCache = false
            };
        }

        public Task Parse()
        {
            //var allPages = GetPages(); // TODO: uncomment

            for (var i = 2; i <= 2; i++)
            {
                var pageUrl = string.Format(UrlTemplate, i);
                var postUrls = GetPostUrls(pageUrl);
                foreach (var url in postUrls)
                {
                    ProcessPost(url);
                    break; // TODO: remove
                }
            }
            
            return  Task.CompletedTask;
        }

        private int GetPages()
        {
            try
            {
                var mainPage = _htmlWeb.Load(Url);
                var paginator = mainPage.QuerySelector("#dle-content > div[align=\"center\"]");
                var pageLinks = paginator.QuerySelectorAll("a");
                var lastPage = pageLinks[^2];
                var lastPageNumber = lastPage.InnerText;

                var result = 0;
                if (int.TryParse(lastPageNumber, out var page))
                {
                    result = page;
                }
                else
                {
                    throw new ParseException("Pages parsing error: Can't parse number");
                }

                return result;
            }
            catch (ParseException)
            {
                throw;
            }
            catch (System.Exception e)
            {
                throw new ParseException($"Page parsing error: {e.Message}");
            }
        }

        private IEnumerable<string> GetPostUrls(string pageUrl)
        {
            var result = new List<string>();
            try
            {
                var pageDocument = _htmlWeb.Load(pageUrl);
                var posts = pageDocument.QuerySelectorAll("div.post");
                foreach (var post in posts)
                {
                    try
                    {
                        var postLink = post.QuerySelector("div.post_f a");
                        var postUrl = postLink.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(postUrl))
                        {
                            result.Add(postUrl);
                        }
                    }
                    catch (System.Exception e)
                    {
                       _errors.Add(e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new ParseException($"Video urls parsing error: {e.Message}");
            }
            
            return result;
        }

        private async Task ProcessPost(string url)
        {
            var postDocument = _htmlWeb.Load(url);
            var title = postDocument.QuerySelector("div.head")?.InnerText.Trim();
            var category = postDocument.QuerySelector("div.post_c a")?.InnerText.Trim();
            var imageTag = postDocument.QuerySelector("div.post_c img")
                ?.GetAttributeValue("src", string.Empty);
            var footerSpans = postDocument.QuerySelectorAll("div.post_f span.postinfo3 > span");
            var author = footerSpans.Count >= 2 
                ? footerSpans[0].QuerySelector("a")?.InnerText
                : "N/A";
            var publish = footerSpans.Count >= 2
                ? ProcessDate(footerSpans[1].InnerText)
                : DateTime.MinValue;
            var videoLink = GetVideoLink(url, postDocument);
            var videoUrl = await GetVideoUrl(videoLink);
        }

        private static string GetVideoLink(string url, HtmlDocument html)
        {
            var result = string.Empty;
            
            var lastPart = url.Split('/').LastOrDefault();

            if (string.IsNullOrEmpty(lastPart)) return result;
            
            var idPart = lastPart.Split('-').FirstOrDefault();
            
            if (!int.TryParse(idPart, out var id)) return result;
            
            var selector = $"news-id-{id}";
            var videoLink = html.QuerySelector($"div#{selector} a[target=\"_blank\"]");
            result = videoLink?.GetAttributeValue("href", string.Empty);

            return result;
        }
        
        private static async Task<string> GetVideoUrl(string videoLink)
        {
            var result = string.Empty;

            using var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            using var http = new HttpClient(httpClientHandler);
            http.DefaultRequestHeaders.Add("Referer", Url);
            using var response = await http.GetAsync(videoLink);
            
            if (response.StatusCode != HttpStatusCode.Found) return result;
            
            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(content);
            result = document.QuerySelector("a")?.GetAttributeValue("href", string.Empty);

            return result;
        }

        private static DateTime ProcessDate(string date)
        {
            var result = DateTime.MinValue;
           
            if (date.Contains("Сегодня") || date.Contains("Вчера"))
            {
                var baseDate = date.Contains("Вчера")
                    ? DateTime.Now.AddDays(-1)
                    : DateTime.Now;
                        
                var time = date.Split(',').LastOrDefault()?.Trim();
                if (time == null) return result;
                int.TryParse(time.Split(':').First(), out var hour);
                int.TryParse(time.Split(':').Last(), out var minutes);
                result = new DateTime(baseDate.Year,baseDate.Month,baseDate.Day, hour, minutes, 0);
            }
            else
            {
                DateTime.TryParse(date, out result);
            }

            return result;
        }
    }
}