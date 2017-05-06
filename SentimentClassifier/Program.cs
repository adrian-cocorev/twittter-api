using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            //These evidences are used as training data for the Dragon Classigfier
            var repositoryPath = ConfigurationManager.AppSettings["EvidenceRepository"];
            var positiveReviews = new Evidence("Positive", repositoryPath + "\\Positive.Evidence.csv");
            var negativeReviews = new Evidence("Negative", repositoryPath + "\\Negative.Evidence.csv");

            var testDataNeg = "Adrian is dumb as fuck."; //GetWebpageContents(url);
            var testDataPos = "you are beautiful.";
            var classifier = new Classifier(positiveReviews, negativeReviews);
            var scoresNeg = classifier.Classify(testDataNeg, Helpers.ClassifierHelper.ExcludeList);
            var scoresPos = classifier.Classify(testDataPos, Helpers.ClassifierHelper.ExcludeList);

            double scorePos = Convert.ToDouble(scoresNeg["Positive"].ToString());
            double scoreNeg = Convert.ToDouble(scoresPos["Negative"].ToString());

            Console.WriteLine("Sentiment for negative phrase: " + testDataNeg);
            Console.WriteLine("Positive score: " + scoresNeg["Positive"]);
            Console.WriteLine("Negative score: " + scoresNeg["Negative"]);

            Console.WriteLine("=========================================");

            Console.WriteLine("Sentiment for positive phrase: " + testDataPos);
            Console.WriteLine("Positive score :" + scoresPos["Positive"]);
            Console.WriteLine("Negative score :" + scoresPos["Negative"]);



            Console.ReadKey();
        }
    }
}
