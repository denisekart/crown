using NUnit.Framework;

namespace CrownTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var ci = Crown.Configuration.ConfigurationReader.Instance;
            var cfg = ci.Configuration;
        }
    }
}