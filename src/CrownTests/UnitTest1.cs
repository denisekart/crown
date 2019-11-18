using System.Linq;
using Crown.Summaries;
using NUnit.Framework;

namespace CrownTests
{
    [TestFixture]
    public class ClassTests
    {
        [Test]
        public void TestNonPublicClassDiagnostic()
        {
            // Arrange
            const string declaration = @"using System;

namespace ConsoleApp1
{
	class MyTestClass
	{
	}
}";
            var project= Utilities.CreateProject(new[] {declaration});

            // Act
            var diagnostics = Utilities.Analyze(project, new SummaryPublicClassDiagnostic());

            // Assert
            Assert.IsNotEmpty(diagnostics);
            Assert.AreEqual(1, diagnostics.Count(x => x.Id == "CROWN_001"));
        }

        [Test]
        public void TestNonPublicClassCodeFix()
        {

        }
    }
}