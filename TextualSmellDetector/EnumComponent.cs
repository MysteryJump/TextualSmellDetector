using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class EnumComponent : CodeComponent, IComponentLeaf
    {
        public string Name { get; set; }

        public EnumComponent(string name, IEnumerable<string> terms)
        {
            Name = name;
            Identifiers = terms;
        }

        public IEnumerable<string> Identifiers { get; set; }
        public IEnumerable<Term> Terms { get; set; }
        public IDictionary<Term, int> TermDictionary { get; set; }
    }
}
