using System;
using System.Collections.Generic;
using System.Text;

namespace TextualSmellDetector
{
    class FieldComponent : CodeComponent, IComponentLeaf
    {
        public string Name { get; set; }

        public FieldComponent(string name)
        {
            Name = name;
            Identifiers = new List<string>() { name };
        }

        public IEnumerable<string> Identifiers { get; set; }
        public IEnumerable<Term> Terms { get; set; }
        public IDictionary<Term, int> TermDictionary { get; set; }
    }
}
