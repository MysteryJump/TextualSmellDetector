using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace TextualSmellDetector
{
    abstract class LsiMatrix
    {
        private int desirableDimension;
        protected IEnumerable<IComponentLeaf> Documents { get; set; }
        private int documentCount;
        protected IDictionary<Term, int> IntegratedDictionary { get; set; }
        protected double[,] tfIdfMatrix;
        protected Dictionary<int, Term> termMatrixCorrespondenceTable;
        protected Dictionary<int, CodeComponent> documentMatrixCorrespondenceTable;
        protected Dictionary<Term, int> appearedCountPerDocument;

        public LsiMatrix(CodeComponent component)
        {
            Documents = component.Children.OfType<IComponentLeaf>();
            IntegratedDictionary = GetIntegratedDictionary(component);
            var length = 0;
            appearedCountPerDocument = new Dictionary<Term, int>();
            foreach (var pair in IntegratedDictionary)
            {
                appearedCountPerDocument.Add(pair.Key, 0);
                foreach (var componentLeaf in Documents)
                {
                    if (componentLeaf.TermDictionary.ContainsKey(pair.Key))
                    {
                        appearedCountPerDocument[pair.Key]++;
                    }
                    length++;
                }
            }


            documentCount = length;
        }

        protected abstract IDictionary<Term, int> GetIntegratedDictionary(CodeComponent component);

        public void Calculate()
        {
            tfIdfMatrix = new double[documentCount,IntegratedDictionary.Count];
            var i = 0;
            foreach (var document1 in Documents)
            {
                var j = 0;
                foreach (var termPair in IntegratedDictionary)
                {
                    tfIdfMatrix[i, j] = CalculateTfIdf(termPair.Key, document1);
                    j++;
                }

                i++;
            }
        }

        private double CalculateTf([NotNull] Term targetTerm, [NotNull] IComponentLeaf targetComponent)
        {
            return (double)targetComponent.TermDictionary[targetTerm] / IntegratedDictionary[targetTerm];
        }

        private double CalculateIdf(Term targetTerm)
        {
            return Math.Log2((double) documentCount / appearedCountPerDocument[targetTerm]);
        }

        private double CalculateTfIdf(Term targetTerm, IComponentLeaf targetComponent)
        {
            return CalculateIdf(targetTerm) * CalculateTf(targetTerm, targetComponent);
        }
    }
}
