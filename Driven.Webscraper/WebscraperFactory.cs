﻿using Domain.Model;
using Domain.Services.Webscraper;

namespace Driven.Webscraper;

public interface IWebscraper
{
    Task<List<Project>> Scrape();
}

public class WebscraperFactory : IWebscraperPort
{
    public async Task<List<Project>> Scrape(ProjectSource source)
    {
        var webscraper = CreateWebscraper(source);
        return await webscraper.Scrape();
    }

    private IWebscraper CreateWebscraper(ProjectSource source)
    {
        return source switch
        {
            ProjectSource.Hays => new HaysWebscraper(),
            _ => throw new NotImplementedException()
        };
    }
}
