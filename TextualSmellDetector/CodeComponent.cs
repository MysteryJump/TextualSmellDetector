using System;
using System.Collections.Generic;
using System.Linq;
using Porter2Stemmer;

namespace TextualSmellDetector
{
    public class CodeComponent
    {
        public IEnumerable<CodeComponent> Children { get; set; }

        
        //protected IEnumerable<Term> NormalizeTerm(string term)
        //{
        //    var split = SplitWord(term);
        //    var stemmer = new EnglishPorter2Stemmer();
        //    return split.Select(x => new Term() { Text = stemmer.Stem(x).Value });
            
        //}

        //protected IEnumerable<string> SplitWord(string term)
        //{
        //    var ls = new List<string>();
        //    var isUpper = char.IsUpper(term[0]);
        //    var beforeInd = 0;
        //    for (int i = 0; i < term.Length; i++)
        //    {
        //        var currentIsUpper = char.IsUpper(term[i]);
        //        var currentIsNumOrSym = !char.IsLetter(term[i]);

        //        if (!isUpper && currentIsUpper || currentIsNumOrSym)
        //        {
        //            ls.Add(term.Substring(beforeInd, (i - 1) - beforeInd + 1));
        //            beforeInd = currentIsNumOrSym ? i + 1 : i;
        //        }
        //        isUpper = currentIsUpper;

        //    }
        //    ls.Add(term.Substring(beforeInd, term.Length - beforeInd));

        //    return ls.Where(x => !x.Any(char.IsSymbol) && 
        //                         !x.Any(char.IsNumber) && 
        //                         !string.IsNullOrWhiteSpace(x));
        //}

    }
}