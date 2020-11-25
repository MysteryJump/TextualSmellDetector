using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TextualSmellDetector
{
    abstract class LsiMatrix
    {
        private int desirableDimension;
        protected IList<MethodComponent> Documents { get; set; }
        private int documentCount;
        protected IDictionary<Term, int> IntegratedDictionary { get; set; }
        protected double[,] tfIdfMatrix;
        //protected Dictionary<int, Term> termMatrixCorrespondenceTable;
        //protected Dictionary<int, CodeComponent> documentMatrixCorrespondenceTable;
        protected Dictionary<Term, int> appearedCountPerDocument;

        public LsiMatrix(CodeComponent component)
        {
            Documents = component.Children.OfType<MethodComponent>().ToList();
            IntegratedDictionary = GetIntegratedDictionary(component);
            var length = 0;
            appearedCountPerDocument = new Dictionary<Term, int>();
            var isInit = true;
            foreach (var pair in IntegratedDictionary)
            {
                appearedCountPerDocument.Add(pair.Key, 0);

                foreach (var componentLeaf in Documents)
                {
                    if (componentLeaf.TermDictionary.ContainsKey(pair.Key))
                    {
                        appearedCountPerDocument[pair.Key]++;
                    }

                    if (isInit)
                    {
                        length++;

                    }

                }
                isInit = false;
            }


            documentCount = length;
            desirableDimension = (int)Math.Pow(documentCount * IntegratedDictionary.Count, 0.2);
        }

        protected abstract IDictionary<Term, int> GetIntegratedDictionary(CodeComponent component);

        public double Calculate()
        {
            if (documentCount <= 1 || IntegratedDictionary.Count == 0)
            {
                return -1;
            }
            tfIdfMatrix = new double[documentCount, IntegratedDictionary.Count];
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

            var matrix = DenseMatrix.OfArray(tfIdfMatrix);
            var svd = matrix.Svd();
            var reducedThreshold = svd.S.OrderByDescending(x => x * x).Aggregate((default(double), 0), (current, next) =>
             {
                 if (current.Item2 < desirableDimension)
                 {
                     return (next, current.Item2 + 1);
                 }
                 else
                 {
                     return (current.Item1, current.Item2 + 1);
                 }
             }).Item1;

            var reduced = svd.S.Select(x => Math.Abs(x) < Math.Abs(reducedThreshold) ? 0 : x);
            var reducedMatrix = new double[documentCount, IntegratedDictionary.Count];
            i = 0;
            foreach (var r in reduced)
            {
                reducedMatrix[i, i++] = r;
            }

            var newW = DenseMatrix.OfArray(reducedMatrix);
            var calculatedMat = svd.U * newW * svd.VT;
            var gen = Generator(documentCount).ToList();
            var simSum = default(double);
            //Console.WriteLine(matrix);
            //Console.WriteLine(calculatedMat);
            foreach (var valueTuple in gen)
            {
                var first = calculatedMat.Row(valueTuple.Item1);
                var second = calculatedMat.Row(valueTuple.Item2);
                var den = default(double);
                var num1 = default(double);
                var num2 = default(double);
                for (int j = 0; j < first.Count; j++)
                {
                    den += first[j] * second[j];
                    num1 += first[j] * first[j];
                    num2 += second[j] * second[j];
                }

                if (num1 != 0 && num2 != 0)
                {
                    simSum += den / (Math.Sqrt(num1) * Math.Sqrt(num2));
                }

            }

            var mean = Math.Abs(simSum / gen.Count);
            return 1 - mean;
        }

        private IEnumerable<(int, int)> Generator(int count)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    yield return (i, j);
                }
            }
        }

        private double CalculateTf(Term targetTerm, IComponentLeaf targetComponent)
        {
            if (targetComponent.TermDictionary.ContainsKey(targetTerm))
            {
                return (double)targetComponent.TermDictionary[targetTerm] / IntegratedDictionary[targetTerm];
            }
            else
            {
                return 0;
            }
        }

        private double CalculateIdf(Term targetTerm)
        {
            return Math.Log2((double)documentCount / appearedCountPerDocument[targetTerm]) + 1;
        }

        private double CalculateTfIdf(Term targetTerm, IComponentLeaf targetComponent)
        {
            return CalculateIdf(targetTerm) * CalculateTf(targetTerm, targetComponent);
        }
    }
}
