using System;
using System.Collections.Generic;

namespace TextualSmellDetector
{
    public class ClassComponent : CodeComponent
    {
        public string ClassName { get; set; }

        public ClassComponent(string name, IEnumerable<CodeComponent> children)
        {
            ClassName = name;
            Children = children;
        }
        public Dictionary<Term, int> GetMethodTermDictionary()
        {
            var dictionary = new Dictionary<Term, int>();
            foreach (var codeComponent in Children)
            {
                if (codeComponent is IComponentLeaf leaf)
                {
                    leaf.NormalizeTerms();
                    foreach (var pair in leaf.TermDictionary)
                    {
                        if (dictionary.ContainsKey(pair.Key))
                        {
                            dictionary[pair.Key] += pair.Value;
                        }
                        else
                        {
                            dictionary.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }

            return dictionary;
        }
    }
}