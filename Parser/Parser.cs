using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Model;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Parser.Exception;

namespace Parser
{
    public class Parser
    {
        private const string Url = "http://www.porjati.net/";
        private const string UrlTemplate = "http://www.porjati.net/page/{0}/";

        private readonly IVideoRepository _repository;
        private readonly HtmlWeb _htmlWeb;
        private readonly ConcurrentBag<string> _errors;

        public IReadOnlyCollection<string> Errors => _errors as IReadOnlyCollection<string>;

        public Parser(IVideoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _errors = new ConcurrentBag<string>();
            _htmlWeb = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("windows-1251"),
                UsingCache = false
            };
        }

        public async Task RunAsync()
        {
            List<Video> result;
            if (_repository.GetAllVideos().Count > 0)
            {
                result = await ContinueAsync().ConfigureAwait(false);
                result.ForEach(v => _repository.Add(v));
            }
            else
            {
                var tasks = ParseAll();
                Debug.WriteLine("All threads started...");
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
                foreach (var task in tasks)
                {
                    task.Result.ForEach(v => _repository.Add(v));
                }
            }

            _repository.SaveChanges();
        }

        private IEnumerable<Task<List<Video>>> ParseAll()
        {
            var result = new List<Task<List<Video>>>();
            var threads = Environment.ProcessorCount;
            var allPages = GetPagesCount();
            if (threads >= allPages)
            {
                threads = 1;
            }
            var partSize = allPages / threads;
            for (int i = 0; i < threads; i++)
            {
                var start =  i == 0 ? 1 : (i * partSize) + 1;
                var last = (i + 1) * partSize;
                if (i == threads - 1)
                {
                    last += allPages % threads;
                }
                var task = Task.Run(async () => await ProcessPagesAsync(start, last).ConfigureAwait(false));
                result.Add(task);
            }

            return result;
        }

        private async Task<List<Video>> ContinueAsync()
        {
            Debug.WriteLine("Continue started");

            var result = new List<Video>();
            var allPages = GetPagesCount();
            var currentVideos = _repository.GetAllVideos();
            var isEnd = false;
            for (int i = 0; i < allPages; i++)
            {
                if (isEnd) break;

                var pageUrl = string.Format(UrlTemplate, i);
                foreach (var url in GetPostUrls(pageUrl))
                {
                    try
                    {
                        if (currentVideos.Any(v => v.PostUrl == url))
                        {
                            Debug.WriteLine($"Parsed done!. Last post [{url}]");
                            isEnd = true;
                            break;
                        }
                        var video = await ProcessPostAsync(url).ConfigureAwait(false);
                        if (video != null)
                        {
                            result.Add(video);
                        }
                        Debug.WriteLine($"Post: {url} - Done! Has video: ({video != null})");
                    }
                    catch (System.Exception e)
                    {
                        Debug.WriteLine($"Post parsing error [{url}]: {e.Message}");
                        _errors.Add($"Post parsing error [{url}]: {e.Message}");
                    }
                }
            }

            return result;
        }

        private async Task<List<Video>> ProcessPagesAsync(int start, int last)
        {
            Debug.WriteLine($"Start: {start} - {last}");

            var buffer = new List<Video>();
            var pageUrl = string.Empty;
            var currentVideos = _repository.GetAllVideos();
            for (var i = start; i <= last; i++)
            {
                try
                {
                    pageUrl = string.Format(UrlTemplate, i);
                    foreach (var url in GetPostUrls(pageUrl))
                    {
                        try
                        {
                            if (currentVideos.Any(v => v.PostUrl == url))
                            {
                                continue;
                            }
                            var video = await ProcessPostAsync(url)
                               .ConfigureAwait(false);
                            if (video != null)
                            {
                                buffer.Add(video);
                            }
                            Debug.WriteLine($"Post: {url} - Done! Has video: ({video != null})");
                        }
                        catch (System.Exception e)
                        {
                            Debug.WriteLine($"Post parsing error [{url}]: {e.Message}");
                            _errors.Add($"Post parsing error [{url}]: {e.Message}");
                        }
                    }
                    Debug.WriteLine($"Page: {pageUrl} - Done!");
                }
                catch (System.Exception e)
                {
                    Debug.WriteLine($"Page [{pageUrl}] parsing error: {e.Message}");
                    _errors.Add($"Page [{pageUrl}] parsing error: {e.Message}");
                }
            }

            Debug.WriteLine($"End: {start} - {last}");

            return buffer;
        }

        private int GetPagesCount()
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

                Debug.WriteLine($"Pages: [{result}]");

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

        private async Task<Video> ProcessPostAsync(string url)
        {
            Video result = null;

            var postDocument = _htmlWeb.Load(url);
            var videoLink = GetVideoLink(url, postDocument);
            if (videoLink != null)
            {
                var title = postDocument.QuerySelector("div.head")?.InnerText.Trim();
                var category = postDocument.QuerySelector("div.post_c a")?.InnerText.Trim();
                var imageTag = postDocument.QuerySelector("div.post_c img")
                    ?.GetAttributeValue("src", string.Empty);
                var footerSpans = postDocument.QuerySelectorAll("div.post_f span.postinfo3 > span");
                var author = footerSpans.Count >= 2
                    ? footerSpans[0].QuerySelector("a")?.InnerText.Trim()
                    : "N/A";
                var publish = footerSpans.Count >= 2
                    ? ProcessDate(footerSpans[1].InnerText.Trim())
                    : DateTime.MinValue;
                var videoUrl = await GetVideoUrlAsync(videoLink).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(videoUrl))
                {
                    result = new Video(videoUrl)
                    {
                        PostUrl = url,
                        ImageUrl = imageTag != null
                            ? ProcessImage(imageTag)
                            : string.Empty,
                        Title = title,
                        Category = category,
                        Author = author,
                        Publish = publish,
                        Parsed = DateTime.Now,
                    };
                }
            }

            return result;
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

            return videoLink?.GetAttributeValue("href", string.Empty);
        }

        private static async Task<string> GetVideoUrlAsync(string videoLink)
        {
            var result = string.Empty;

            using var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            using var http = new HttpClient(httpClientHandler);
            http.DefaultRequestHeaders.Add("Referer", Url);
            using var response = await http.GetAsync(videoLink).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Found) return result;

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var document = new HtmlDocument();
            document.LoadHtml(content);

            return document.QuerySelector("a")?.GetAttributeValue("href", string.Empty);
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
                result = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, hour, minutes, 0);
            }
            else
            {
                DateTime.TryParse(date, out result);
            }

            return result;
        }

        private static string ProcessImage(string url)
        {
            if (!url.StartsWith("http"))
            {
                return $"{Url.TrimEnd('/')}/{url.TrimStart('/')}";
            }
            return url;
        }
    }
}