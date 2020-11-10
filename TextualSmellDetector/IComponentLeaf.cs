using System;
using System.Collections.Generic;
using System.Text;

namespace TextualSmellDetector
{
    interface IComponentLeaf
    {
        public IEnumerable<Term> Terms { get; set; }
    }
}
