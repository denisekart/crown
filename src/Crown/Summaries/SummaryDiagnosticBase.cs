using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Summaries
{
    public abstract class SummaryDiagnosticBase<TSyntaxKind> : CSharpAnalyzerBase where TSyntaxKind: CSharpSyntaxNode
    {
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            TSyntaxKind node = context.Node as TSyntaxKind;

            DocumentationCommentTriviaSyntax commentTriviaSyntax = node
                .GetLeadingTrivia()
                .Select(o => o.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (commentTriviaSyntax != null)
            {
                bool hasSummary = commentTriviaSyntax
                    .ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .Any(o => o.StartTag.Name.ToString().Equals("summary"));

                if (hasSummary)
                {
                    return false;
                }
            }

            return true;
        }

        protected override Diagnostic CreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            TSyntaxKind node = context.Node as TSyntaxKind;
            return Diagnostic.Create(SupportedDiagnostics.First(), GetLocation(node,context));
        }

        public abstract Location GetLocation(TSyntaxKind node, SyntaxNodeAnalysisContext context);
    }
}