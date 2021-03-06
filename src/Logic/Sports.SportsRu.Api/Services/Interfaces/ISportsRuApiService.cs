﻿using Sports.Common.Models;
using Sports.SportsRu.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sports.SportsRu.Api.Services.Interfaces
{
    public interface ISportsRuApiService
    {
        Task<ServiceResponse<NewsResponseCollection>> GetNewsAsync(
            NewsType newsType, NewsPriority newsPriority, 
            NewsContentOrigin newsContentOrigin, int count,
            Name name = Name.Undefined);
        Task<ServiceResponse<CommentIdsResponseCollection>> GetCommentsIdsAsync(int messageId, MessageClass messageClass, Sort sort, int commentsCount = 10);
        Task<ServiceResponse<CommentByIdsResponse>> GetCommentsByIds(IEnumerable<int> ids);
        Task<ServiceResponse<HotContentResponse>> GetHotContentAsync();
    }
}
