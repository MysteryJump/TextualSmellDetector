using System;
using System.Collections.Generic;
using System.Text;

namespace TextualSmellDetector
{
    class MethodMethodLsiMatrix : LsiMatrix
    {
        public string Name { get; set; }
        public MethodMethodLsiMatrix(ClassComponent component) : base(component)
        {
            Name = component.ClassName;
            
        }


        protected override IDictionary<Term, int> GetIntegratedDictionary(CodeComponent component)
        {
            if (component is ClassComponent cmp)
            {
                return cmp.GetMethodTermDictionary();
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
