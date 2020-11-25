using System;
using System.Collections.Generic;
using System.Linq;

namespace TextualSmellDetector
{
    public class ClassComponent : CodeComponent
    {
        public string ClassName { get; }
        public string Namespace { get; }

        public int Loc { get; }

        private Dictionary<Term, int> termDictionary;

        public int UniqueTokenCount => termDictionary.Count;

        public int TokenCount => termDictionary.Values.Sum(x => x);

        public ClassComponent(string name, IEnumerable<CodeComponent> children, string package = "", int loc = 0)
        {
            ClassName = name;
            Children = children;
            Namespace = package;
            Loc = loc;
        }
        public Dictionary<Term, int> GetMethodTermDictionary()
        {
            var dictionary = new Dictionary<Term, int>(new Term());
            foreach (var codeComponent in Children)
            {
                if (codeComponent is IComponentLeaf leaf && codeComponent is MethodComponent)
                {
                    leaf.NormalizeTerms();
                    foreach (var (key, value) in leaf.TermDictionary)
                    {
                        if (dictionary.ContainsKey(key))
                        {
                            dictionary[key] += value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }
                }
            }

            termDictionary = dictionary;
            return dictionary;
        }
    }
}