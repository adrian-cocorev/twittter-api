using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitterApi.Controllers;

namespace TwitterApi.Models
{
    public class SentimentTweetModel
    {
        public TweetDetails TweetDetails { get; set; }
        public int TweetPositive { get; set; }
        public int TweetNegative { get; set; }

    }
}