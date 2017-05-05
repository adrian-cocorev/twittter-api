using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Newtonsoft.Json;

namespace TwitterApi.Controllers
{
    public class TweetDetails
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public long TweetID { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }

    public class TweetsController : ApiController
    {       
        public IHttpActionResult GetFilterTweets(string searchTerm)
        {
            var tweets = GetTweets(searchTerm);
            if (tweets == null)
            {
                return NotFound();
            }
            return Ok(tweets);
        }
        
        private IEnumerable<TweetDetails> GetTweets(string searchTerm)
        {
            Auth.SetUserCredentials("YZYc1Jyp31tKZ5zgJNWIVhLeT", "x8QRxQxtUa6JmatvMJhViVYyihm3EYpkfO89LE1jd1wAL4BGVp", "852507449630261248-hEPZwkIzKALj2sEzmumHozsl5GpcQPv", "7SOZWdC7qy7Y8zPIOj1HZ7r6CTUzB2dPUGaV89VejZiVE");

            var searchParameter = Search.CreateTweetSearchParameter(searchTerm);
            searchParameter.Lang = LanguageFilter.English;
            searchParameter.MaximumNumberOfResults = 1000;
            searchParameter.Since = new DateTime(2017, 1, 1);
            var tweets = Search.SearchTweets(searchParameter);
            return GetTweetDetails(tweets);
        }

        [NonAction]
        private IEnumerable<TweetDetails> GetTweetDetails(IEnumerable<ITweet> rawTweets)
        {
            return rawTweets.Select(tweet => new TweetDetails()
            {
                UserID = tweet.CreatedBy.Id,
                UserName = tweet.CreatedBy.Name,
                TweetID = tweet.Id,
                Text = tweet.Text,
                Created = tweet.CreatedAt
            });
        }
        
        [NonAction]
        private static List<ITweet> GetUserTimelineTweets(string userName, int maxNumberOfTweets)
        {
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            RateLimit.QueryAwaitingForRateLimit += (sender, args) =>
            {
                Console.WriteLine($"Query : {args.Query} is awaiting for rate limits!");
            };

            Auth.SetUserCredentials("YZYc1Jyp31tKZ5zgJNWIVhLeT", "x8QRxQxtUa6JmatvMJhViVYyihm3EYpkfO89LE1jd1wAL4BGVp", "852507449630261248-hEPZwkIzKALj2sEzmumHozsl5GpcQPv", "7SOZWdC7qy7Y8zPIOj1HZ7r6CTUzB2dPUGaV89VejZiVE");
            var user = Tweetinvi.User.GetUserFromScreenName(userName);

            var lastTweets = Timeline.GetUserTimeline(user.Id, 200).ToArray();

            var allTweets = new List<ITweet>(lastTweets);
            var beforeLast = allTweets;

            while (lastTweets.Length > 0 && allTweets.Count <= 3200)
            {
                var idOfOldestTweet = lastTweets.Select(x => x.Id).Min();
                Console.WriteLine($"Oldest Tweet Id = {idOfOldestTweet}");

                var numberOfTweetsToRetrieve = allTweets.Count > 3000 ? 3200 - allTweets.Count : 200;
                var timelineRequestParameters = new UserTimelineParameters
                {
                    // MaxId ensures that we only get tweets that have been posted 
                    // BEFORE the oldest tweet we received
                    MaxId = idOfOldestTweet - 1,
                    MaximumNumberOfTweetsToRetrieve = numberOfTweetsToRetrieve
                };

                lastTweets = Timeline.GetUserTimeline(user.Id, timelineRequestParameters).ToArray();
                allTweets.AddRange(lastTweets);
            }

            return allTweets;
        }
    }
}
