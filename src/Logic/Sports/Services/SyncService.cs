﻿using Microsoft.Extensions.Logging;
using Sports.Data.Context;
using Sports.Data.Entities;
using Sports.Services.Interfaces;
using Sports.SportsRu.Api.Helpers;
using Sports.SportsRu.Api.Models;
using Sports.SportsRu.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sports.Services
{
    public class SyncService : ISyncService
    {
        private readonly SportsContext _sportsContext;
        private readonly ISportsRuApiService _sportsRuApiService;
        private readonly INewsService _newsService;
        private readonly ILogger<SyncService> _logger;

        public SyncService(SportsContext sportsContext, ISportsRuApiService sportsRuApiService
            , INewsService newsService, ILogger<SyncService> logger)
        {
            _sportsContext = sportsContext;
            _sportsRuApiService = sportsRuApiService;
            _newsService = newsService;
            _logger = logger;
        }

        public async Task SyncNewsAsync()
        {
            var newsResponse = await _sportsRuApiService.GetNewsAsync(NewsType.HomePage, NewsPriority.Main, NewsContentOrigin.Mixed, 100).ConfigureAwait(false);
            var hotContent = await _sportsRuApiService.GetHotContentAsync().ConfigureAwait(false);
            IEnumerable<int> hotNewsIds = null;
            if(hotContent.IsSuccess)
            {
                hotNewsIds = hotContent.Content.News;
            }
            if(newsResponse.IsSuccess)
            {
                foreach (var newsArticle in newsResponse.Content)
                {
                    if (newsArticle.BodyIsEmpty ||
                        newsArticle.ContentOption?.Name == "special" ||
                        !SportsRuHelper.IsInternalUrl(newsArticle.DesktopUrl)) //usually this is not a news article but some promotion
                    {
                        continue;
                    }
                    string idString = newsArticle.Id.ToString(CultureInfo.InvariantCulture);
                    var existingArticle = _sportsContext.NewsArticles.FirstOrDefault(x => x.ExternalId == idString);
                    if (existingArticle == null)
                    {
                        var newArticle = new NewsArticle()
                        {
                            ExternalId = idString
                        };
                        Map(newsArticle, newArticle, hotNewsIds);
                        _sportsContext.NewsArticles.Add(newArticle);
                    }
                    else
                    {
                        Map(newsArticle, existingArticle, hotNewsIds);
                        _sportsContext.NewsArticles.Update(existingArticle);
                    }
                }
                _sportsContext.SaveChanges();
            }
        }

        public async Task SyncPopularNewsCommentsAsync(DateTimeOffset fromDate, int newsCount)
        {
            var popularNews = _newsService.GetPopularNews(fromDate, newsCount);
            foreach (var newsArticle in popularNews)
            {
                if(int.TryParse(newsArticle.ExternalId, out int id))
                {
                    var commentsIdsResponse = await _sportsRuApiService.GetCommentsIdsAsync(id, MessageClass.News, Sort.Top10).ConfigureAwait(false);
                    if (commentsIdsResponse.IsSuccess)
                    {
                        var commentsByIdsResponse = await _sportsRuApiService.GetCommentsByIds(commentsIdsResponse.Content).ConfigureAwait(false);
                        if(commentsByIdsResponse.IsSuccess)
                        {
                            foreach (var comment in commentsByIdsResponse.Content.Data.Comments)
                            {
                                var existingComment = _sportsContext.NewsArticlesComments.FirstOrDefault(x => x.ExternalId == comment.Id.ToString(CultureInfo.InvariantCulture));
                                if(existingComment == null)
                                {
                                    var newComment = new NewsArticleComment()
                                    {
                                        NewsArticleId = newsArticle.Id,
                                        ExternalId = comment.Id.ToString(CultureInfo.InvariantCulture),
                                    };
                                    Map(comment, newComment);
                                    _sportsContext.NewsArticlesComments.Add(newComment);
                                }
                                else
                                {
                                    Map(comment, existingComment);
                                    _sportsContext.NewsArticlesComments.Update(existingComment);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Can't get comments by ids.\nRequest data: {JsonSerializer.Serialize(commentsIdsResponse.Content)}.\nResponse data: {commentsByIdsResponse.ErrorMessage}");
                        }
                    }
                }
            }
            _sportsContext.SaveChanges();
        }

        public void DeleteOldData(DateTimeOffset oldestDateToKeep)
        {
            var date = oldestDateToKeep.UtcDateTime;
            var oldArticles = _sportsContext.NewsArticles
                .Where(x => x.PublishedDate < date).ToArray();
            _sportsContext.NewsArticles.RemoveRange(oldArticles);

            _sportsContext.SaveChanges();
        }

        private void Map(CommentInfo from, NewsArticleComment to)
        {
            to.Rating = from.Rating.Plus + from.Rating.Minus;
            to.Text = from.Text.Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase);
        }

        private void Map(NewsArticleInfo from, NewsArticle to, IEnumerable<int> hotNewsIds)
        {
            to.Title = from.Title;
            to.Url = from.DesktopUrl;
            to.IsHotContent = hotNewsIds != null && hotNewsIds.Contains(from.Id);
            to.CommentsCount = from.CommentsCount;
            to.PublishedDate = DateTimeOffset
                        .FromUnixTimeSeconds(from.Published.Timestamp)
                        .UtcDateTime;
        }
    }
}
