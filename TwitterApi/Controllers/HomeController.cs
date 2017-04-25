using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterApi.Models;

namespace TwitterApi.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Index(string searchTerm, int tweetsNumber)
        {
            var tweets = GetTweets(searchTerm, tweetsNumber);
            return View(tweets);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var tweets = GetTweets("", 0);
            return View(tweets);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private List<TweetInfo> GetTweets(string searchTerm, int tweetsNumber)
        {
            if(String.IsNullOrEmpty(searchTerm))
            {
                return new List<TweetInfo>();
            }

            Auth.SetUserCredentials("YZYc1Jyp31tKZ5zgJNWIVhLeT", "x8QRxQxtUa6JmatvMJhViVYyihm3EYpkfO89LE1jd1wAL4BGVp", "852507449630261248-hEPZwkIzKALj2sEzmumHozsl5GpcQPv", "7SOZWdC7qy7Y8zPIOj1HZ7r6CTUzB2dPUGaV89VejZiVE");

            var searchParameter = Tweetinvi.Search.CreateTweetSearchParameter(searchTerm);
            searchParameter.Lang = LanguageFilter.English;
            searchParameter.MaximumNumberOfResults = tweetsNumber;
            searchParameter.Since = new DateTime(2017, 1, 1);
            var tweets = Tweetinvi.Search.SearchTweets(searchParameter);
            return GetTweetDetails(tweets);
        }

        private List<TweetInfo> GetTweetDetails(IEnumerable<ITweet> rawTweets)
        {
            return rawTweets.Select(tweet => new TweetInfo()
            {
                UserID = tweet.CreatedBy.Id,
                UserName = tweet.CreatedBy.Name,
                TweetID = tweet.Id,
                Text = tweet.Text,
                Created = tweet.CreatedAt
            }).ToList();
        }
    }
}