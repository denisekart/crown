using Crown.Structure;
using NUnit.Framework;

namespace CrownTests
{
    [TestFixture]
    public class MultipleEmptyLinesTests
    {
        [Test]
        public void TestMultipleEmptyLinesInAClass()
        {
            // Arrange
            const string declaration = @"using System;

namespace ConsoleApp1
{
	class MyTestClass
	{


	}
}";
            var project = Utilities.CreateProject(new[] { declaration });

            // Act
            var diagnostics = Utilities.Analyze(project, new MultipleEmptyLinesDiagnostic());


            // Assert
            Assert.IsNotEmpty(diagnostics);
            Assert.AreEqual("CROWN_010", diagnostics[0].Id);
        }
    }
}