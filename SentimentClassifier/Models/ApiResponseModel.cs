using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentClassifier.Models
{
    public class ApiResponseModel
    {
        public string Content { get; set; }
        public int Positive { get; set; }
        public int Negative { get; set; }
    }
}
