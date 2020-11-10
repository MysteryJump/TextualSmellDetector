using System;
using System.Collections.Generic;
using System.Text;

namespace TextualSmellDetector
{
    class FieldComponent : ICodeComponent
    {
        public string Name { get; set; }
        public IEnumerable<ICodeComponent> Children { get; set; } = new List<ICodeComponent>();

        public FieldComponent(string name)
        {
            Name = name;
        }
    }
}
