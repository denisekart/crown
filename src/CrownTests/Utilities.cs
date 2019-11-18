using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace CrownTests
{
    public static class Utilities
    {
        public static Diagnostic[] Analyze(Project project, params DiagnosticAnalyzer[] analyzers)
        {
            var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzers));
            var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

            return diagnostics.ToArray();
        }

        public static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
        {
            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

            var projectId = ProjectId.CreateNewId(debugName: "TestProject");
            MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
            MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
            MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, "TestProject", "TestProject", language)
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference);

            int count = 0;
            foreach (var source in sources)
            {
                var newFileName = "Document" + count + ".cs";
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }
            return solution.GetProject(projectId);
        }
    }
}