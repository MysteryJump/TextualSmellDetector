using System.Collections.Generic;

namespace TextualSmellDetector
{
    public class Term : IEqualityComparer<Term>
    {
        public string Text { get; set; }
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(Term x, Term y) => y != null && x != null && x.Text == y.Text;

        public int GetHashCode(Term obj) => obj.Text.GetHashCode();
    }
}