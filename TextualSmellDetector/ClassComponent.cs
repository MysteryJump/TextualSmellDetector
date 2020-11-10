using System.Collections.Generic;

namespace TextualSmellDetector
{
    public class ClassComponent : ICodeComponent
    {
        public string ClassName { get; set; }
        public IEnumerable<ICodeComponent> Children { get; set; }

        public ClassComponent(string name, IEnumerable<ICodeComponent> children)
        {
            ClassName = name;
            Children = children;
        }
    }
}