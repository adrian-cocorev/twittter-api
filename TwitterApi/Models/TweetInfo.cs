using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterApi.Models
{
    public class TweetInfo
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public long TweetID { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}