using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    class NamespaceComponent : CodeComponent
    {
        public string Name { get; set; }
        public string NamespaceFullName { get; set; }
        public NamespaceComponent Parent { get; set; }

        public NamespaceComponent(string fullName, IEnumerable<CodeComponent> components)
        {
            var split = fullName.Split('.');
            Name = split.Last();
            NamespaceFullName = fullName;
        }
    }

    
}
