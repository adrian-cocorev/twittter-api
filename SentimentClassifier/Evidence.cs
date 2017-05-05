using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;
using NLPToolkit;
using SentimentClassifier.Interfaces;

namespace SentimentClassifier
{
    public class Evidence : IEvidence
    {
        #region privates

        private readonly LoggerHelper _logger = new LoggerHelper();
        private readonly Dictionary<string, double> _evidences;
        private readonly string _evidenceRepository;
        private readonly string _evidenceFileName;
        private readonly string _evidenceFilePath;
        private const char EVIDENCE_SEPARATOR = ',';
        private double _totalWords;
        private readonly string _className;
        private readonly bool _saveEvidence;

        #endregion

        public string Name()
        {
            return _className;
        }

        public double TotalWords()
        {
            return _totalWords;
        }

        public bool AddEvidenceData(string trainingData, HashSet<string> wordsToIgnore)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(trainingData)) return false;

                var tokens = Tokenizer.TokenizeNow(trainingData).ToList();

                foreach (
                    var token in
                        tokens.Where(token => !string.IsNullOrWhiteSpace(token) && !wordsToIgnore.Contains(token)))
                {
                    if (!_evidences.ContainsKey(token))
                        _evidences[token] = 0;

                    _evidences[token]++;
                    _totalWords++;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex, "");               
                throw new Exception("Failed to load evidence data " + trainingData, ex);
            }
        }

        public bool AddEvidenceData(IEnumerable<string> trainingData, HashSet<string> wordsToIgnore)
        {
            foreach (var data in trainingData)
            {
                AddEvidenceData(data, wordsToIgnore);
            }

            return true;
        }

        public IDictionary<string, double> GetEvidence()
        {
            return _evidences;
        }

        public bool PersistEvidence(bool backupExisting = true)
        {
            try
            {
                if (_evidences == null || _evidences.Count <= 0)
                    return false;

                if (backupExisting)
                    Helpers.ClassifierHelper.BackupFile(_evidenceFilePath);

                using (var file = new StreamWriter(_evidenceFilePath))
                {
                    foreach (
                        var line in
                            _evidences.Select(
                                evidence =>
                                evidence.Key.Trim() + "," + evidence.Value.ToString(CultureInfo.InvariantCulture).Trim())
                        )
                    {
                        file.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex, "");                
                throw new Exception("Error while storing evidence data to " + _evidenceFilePath, ex);
            }

            return true;
        }

        public void LoadEvidenceFromCache(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;

                _logger.WriteLog(null, "Loading evidence cache from " + filePath);

                using (var file = new StreamReader(filePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var keyValue = line.Split(EVIDENCE_SEPARATOR);
                        if (!_evidences.ContainsKey(keyValue[0]))
                        {
                            var value = int.Parse(keyValue[1]);
                            _evidences.Add(keyValue[0], value);
                            _totalWords += value;
                        }
                        else
                        {
                            _logger.WriteLog(new Exception($"Duplicate entries of {keyValue[0]} found while loading evidences from {filePath}"), "");
                            throw new Exception($"Duplicate entries of {keyValue[0]} found while loading evidences from {filePath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex, "");
                throw new Exception("Error while loading evidence from cache", ex);
            }
        }

        public Evidence(string className, string evidenceFilePath, bool loadEvidence = true, bool saveEvidence = false)
        {
            if (string.IsNullOrWhiteSpace(className)) throw new Exception("Class name was not defined");

            _saveEvidence = saveEvidence;
            _className = className;
            _evidences = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            _evidenceFileName = evidenceFilePath;

            _logger.WriteLog(null, "[Classifier.Evidence.Ctor] Creating evidence instance for " + className);            

            if (loadEvidence) LoadEvidenceFromCache(evidenceFilePath);
        }

        ~Evidence()
        {
            if (_saveEvidence) PersistEvidence();
        }
    }
}
