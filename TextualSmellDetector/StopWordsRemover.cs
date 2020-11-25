using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utf8Json;

namespace TextualSmellDetector
{
    static class StopWordsRemover
    {
        private static string[] words;
        private static string[] commons;
        static StopWordsRemover()
        {
            words = JsonSerializer.Deserialize<string[]>(File.OpenRead("StopWords.json"));
            commons = JsonSerializer.Deserialize<string[]>(File.OpenRead("CommonKeywords.json"));
        }

        public static bool IsStopWord(string word) => words.Contains(word);
    }
}
