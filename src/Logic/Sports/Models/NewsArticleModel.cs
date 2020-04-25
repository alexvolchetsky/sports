﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sports.Models
{
    public class NewsArticleModel
    {
        public string Title { get; set; }
        public Uri Url { get; set; }
        public int CommentsCount { get; set; }
        public bool IsHotContent { get; set; }
    }
}
