using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class MethodComponent : CodeComponent, IComponentLeaf
    {
        public string Name { get; set; }
        public IEnumerable<string> Identifiers { get; set; }
        public IEnumerable<Term> Terms { get; set; }
        public IDictionary<Term, int> TermDictionary { get; set; }

        public MethodComponent(string name, IEnumerable<string> tokens)
        {
            Name = name;
            Identifiers = tokens;
        }
    }
}
