using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class EnumComponent : ICodeComponent, IComponentLeaf
    {
        public string Name { get; set; }

        public IEnumerable<ICodeComponent> Children { get; set; } = new List<ICodeComponent>();

        public EnumComponent(string name, IEnumerable<string> terms)
        {
            Name = name;
            Terms = terms.Select(x => new Term() { Text = x });
        }

        public IEnumerable<Term> Terms { get; set; }
    }
}
