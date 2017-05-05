using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace SentimentClassifier.Helpers
{
    public class ClassifierHelper
    {
        #region privates

        private static readonly LoggerHelper _logger = new LoggerHelper();

        #endregion

        #region public

        public static readonly HashSet<string> ExcludeList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static readonly Dictionary<char, char> TokenCharMappings;

        #endregion

        static ClassifierHelper()
        {
            //Tokens to exclude
            var excludedTokens = ConfigurationManager.AppSettings["ExcludedTokensList"];

            if (!string.IsNullOrWhiteSpace(excludedTokens))
            {
                //excludedTokens = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, excludedTokens);
                if (!File.Exists(excludedTokens))
                {
                    _logger.WriteLog(null, "Unable to find Exluded Files List at " + excludedTokens);
                    throw new FileNotFoundException("Unable to find Exluded Files List at " + excludedTokens);
                }
                using (var file = new StreamReader(excludedTokens))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line.Trim())) continue;

                        if (line.StartsWith("--")) continue;

                        if (!ExcludeList.Contains(line.Trim()))
                            ExcludeList.Add(line);
                    }
                }
            }

            #region Token character mappings

            var alphabets = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            var numerics = "0123456789".ToCharArray();

            TokenCharMappings = alphabets.ToDictionary(alpha => alpha);
            foreach (var number in numerics)
            {
                TokenCharMappings.Add(number, ' ');
            }

            #endregion Token character mappings
        }

        public static void BackupFile(string fullFileName)
        {
            try
            {
                if (!File.Exists(fullFileName))
                {
                    return;
                }

                var ext = Path.GetExtension(fullFileName);
                var newFileName = fullFileName + DateTime.Now.ToString(".yyyy.MM.dd.HH.mm.ss.fff") + ext;
                File.Copy(fullFileName, newFileName);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex, "");                
                throw new Exception("BackupFile - ", ex);
            }
        }
    }
}
