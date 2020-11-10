using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class NamespaceComponent : ICodeComponent
    {
        public IEnumerable<ICodeComponent> Children { get; set; }
        public string Name { get; set; }
        public string NamespaceFullName { get; set; }
        public NamespaceComponent Parent { get; set; }

        public NamespaceComponent(string fullName, IEnumerable<ICodeComponent> components)
        {
            var split = fullName.Split('.');
            Name = split.Last();
            NamespaceFullName = fullName;
        }
    }

    
}
