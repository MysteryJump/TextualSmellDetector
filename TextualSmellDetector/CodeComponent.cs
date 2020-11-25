using System;
using System.Collections.Generic;
using System.Linq;
using Porter2Stemmer;

namespace TextualSmellDetector
{
    public class CodeComponent
    {
        public IEnumerable<CodeComponent> Children { get; set; }

    }
}