using System.Collections.Generic;

namespace TextualSmellDetector
{
    public interface ICodeComponent
    {
        public IEnumerable<ICodeComponent> Children { get; set; }

    }
}