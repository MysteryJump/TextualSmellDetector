using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class MethodComponent : ICodeComponent, IComponentLeaf
    {
        public string Name { get; set; }
        public IEnumerable<ICodeComponent> Children { get; set; } = new List<ICodeComponent>();
        public IEnumerable<Term> Terms { get; set; }

        public MethodComponent(string name, IEnumerable<string> tokens)
        {
            Name = name;
            Terms = tokens.Select(x => new Term() { Text = x });
        }
    }
}
