using System;
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
        
        private readonly IVideoRepository _repository;
        private readonly HtmlWeb _htmlWeb;
        private readonly uint _threads;

        public Parser(uint threads, IVideoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _threads = threads;
            _htmlWeb = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("windows-1251")
            };
        }

        public Task Parse()
        {
            var allPages = GetPages();
            
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
                var pageNumber = lastPage.InnerText;

                var result = 0;
                if (int.TryParse(pageNumber, out var page))
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
    }
}