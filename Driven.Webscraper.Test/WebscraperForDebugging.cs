using Domain.Model;
using Domain.Ports;

namespace Driven.Webscraper.Test
{
    public class WebscraperForDebugging : IWebscraperPort
    {
        public async Task<List<Project>> Scrape(ProjectSource source, Project? lastScrapedProject)
        {
            return new List<Project>
            {
                new Project(source, "C# Senior Dev", "www.example.org", "10", ExampleDescription(), null),
                new Project(source, "C# Dev", "www.example.org", "11", null, null),
                new Project(source, ".NET Senior Dev", "www.example.org", "12", ExampleDescription(), null),
                new Project(source, "Typecript Senior Dev", "www.example.org", "13", null, null),
                new Project(source, "DDD", "www.example.org", "14", null, null),
                new Project(source, "Vue.js", "www.example.org", "15", ExampleDescription(), null),
                new Project(source, "domain driven design", "www.example.org", "16", null, null),
            };
        }

        public Task<List<Project>> ScrapeOnlyNew(ProjectSource source, Project lastScrapedProject)
        {
            throw new NotImplementedException();
        }

        public bool ScrapeOnlyNewSupported(ProjectSource source)
        {
            throw new NotImplementedException();
        }

        private string ExampleDescription()
        {
            return "lorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsumlorem ipsum";
        }
    }
}
