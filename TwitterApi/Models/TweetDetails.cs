using System;
using System.Security.Policy;

namespace TwitterApi.Models
{
    public class TweetDetails
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public long TweetID { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
    }
}