using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Porter2Stemmer;

namespace TextualSmellDetector
{
    public interface IComponentLeaf
    {

        public IEnumerable<string> Identifiers { get; set; }
        public IEnumerable<Term> Terms { get; set; }
        public IDictionary<Term, int> TermDictionary { get; set; }
        public void NormalizeTerms()
        {
            var ls = new List<Term>();
            foreach (var identifier in Identifiers)
            {
                var split = SplitWord(identifier);
                var stemmer = new EnglishPorter2Stemmer();
                ls.AddRange(split.Select(x => new Term() { Text = stemmer.Stem(x).Value.ToLower() }));
            }

            Terms = ls.Where(x => !StopWordsRemover.IsStopWord(x.Text));
            TermDictionary = ls.Aggregate(new Dictionary<Term, int>(new Term()), (current, next) =>
            {
                if (current.ContainsKey(next))
                {
                    current[next]++;
                }
                else
                {
                    current.Add(next, 1);
                }
                return current;
            });
        }

        public IEnumerable<string> SplitWord(string term)
        {
            var ls = new List<string>();
            var isUpper = char.IsUpper(term[0]);
            var beforeInd = 0;
            for (int i = 0; i < term.Length; i++)
            {
                var currentIsUpper = char.IsUpper(term[i]);
                var currentIsNumOrSym = !char.IsLetter(term[i]);

                if (!isUpper && currentIsUpper || currentIsNumOrSym)
                {
                    ls.Add(term.Substring(beforeInd, (i - 1) - beforeInd + 1));
                    beforeInd = currentIsNumOrSym ? i + 1 : i;
                }
                isUpper = currentIsUpper;

            }
            ls.Add(term.Substring(beforeInd, term.Length - beforeInd));

            return ls.Where(x => !x.Any(char.IsSymbol) &&
                                 !x.Any(char.IsNumber) &&
                                 !string.IsNullOrWhiteSpace(x));
        }

    }
}
