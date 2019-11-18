using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Structure
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MultipleEmptyLinesDiagnostic: CSharpAnalyzerBase
    {
        public override string DiagnosticId => "CROWN_010";
        public override SyntaxKind SyntaxKind => SyntaxKind.None;

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);
            context.RegisterSemanticModelAction(AnalyzeModel);
        }

        private void AnalyzeModel(SemanticModelAnalysisContext obj)
        {
            var text = obj.SemanticModel.SyntaxTree.GetText();
            for (int i = 1; i < text.Lines.Count; i++)
            {
                if (text.Lines[i].End == 0 || text.Lines[i].Span.IsEmpty)
                {
                    if (text.Lines[i-1].End == 0 || text.Lines[i-1].Span.IsEmpty)
                    {
                        var diagnostic = Diagnostic.Create(SupportedDiagnostics.First(),
                            Location.Create(obj.SemanticModel.SyntaxTree, text.Lines[i].Span));
                        obj.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return false;
        }

        protected override Diagnostic CreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}
