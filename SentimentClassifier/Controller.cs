using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentimentClassifier.Models;

namespace SentimentClassifier
{
    public class Controller
    {       
        public ApiResponseModel Classify(string text)
        {
            var repositoryPath = ConfigurationManager.AppSettings["EvidenceRepository"];
            var positiveReviews = new Evidence("Positive", repositoryPath + "\\Positive.Evidence.csv");
            var negativeReviews = new Evidence("Negative", repositoryPath + "\\Negative.Evidence.csv");

            var classifier = new Classifier(positiveReviews, negativeReviews);
            var scoresText = classifier.Classify(text, Helpers.ClassifierHelper.ExcludeList);

            if (scoresText != null)
            {
                double scorePos = Convert.ToDouble(scoresText["Positive"].ToString());
                double scoreNeg = Convert.ToDouble(scoresText["Negative"].ToString());
                var responseModel = new ApiResponseModel()
                {
                    Content = text,
                    Negative = Convert.ToInt32(scoreNeg),
                    Positive = Convert.ToInt32(scorePos)
                };

                return responseModel;
            }

            return null;
        }
    }
}
