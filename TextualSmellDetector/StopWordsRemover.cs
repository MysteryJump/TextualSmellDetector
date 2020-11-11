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
        static StopWordsRemover()
        {
            words = JsonSerializer.Deserialize<string[]>(File.OpenRead("StopWords.json"));
        }

        public static bool IsStopWord(string word) => words.Contains(word);
    }
}
