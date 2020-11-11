using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TextualSmellDetector.Test
{
    public class CodeComponentTest : IComponentLeaf
    {
        [Fact]
        public void SplitTermTest()
        {
            var helloWorld = "HelloWorld";
            var split = (this as IComponentLeaf).SplitWord(helloWorld).ToList();
            Assert.Equal("Hello", split[0]);
            Assert.Equal("World", split[1]);
        }

        [Fact]
        public void SplitTermTest2()
        {
            var helloWorld2 = "Hello_worlDD";
            var split = (this as IComponentLeaf).SplitWord(helloWorld2).ToList();
            Assert.Equal("Hello", split[0]);
            Assert.Equal("worl", split[1]);
            Assert.Equal("DD", split[2]);
        }

        [Fact]
        public void SplitTermTest3()
        {
            var helloWorld3 = "Hello____world";
            var split = (this as IComponentLeaf).SplitWord(helloWorld3).ToList();
            Assert.Equal("Hello", split[0]);
            Assert.Equal("world", split[1]);
        }

        [Fact]
        public void SplitTermTest4()
        {
            var helloWorld4 = "helloWorld__Test_454646Test________";
            var split = (this as IComponentLeaf).SplitWord(helloWorld4).ToList();
            Assert.Equal("hello", split[0]);
            Assert.Equal("World", split[1]);
            Assert.Equal("Test", split[2]);
            Assert.Equal("Test", split[3]);
            Assert.Equal(4, split.Count);
        }

        public IEnumerable<string> Identifiers { get; set; }
        public IEnumerable<Term> Terms { get; set; }
        public IDictionary<Term, int> TermDictionary { get; set; }
    }
}
