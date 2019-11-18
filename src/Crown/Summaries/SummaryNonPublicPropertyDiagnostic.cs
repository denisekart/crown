using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Summaries
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SummaryNonPublicPropertyDiagnostic : SummaryDiagnosticBase<PropertyDeclarationSyntax>
    {
        public override string DiagnosticId => "CROWN_009";
        public override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return base.ShouldCreateDiagnostic(context) && !(context.Node as PropertyDeclarationSyntax).Modifiers.Any(x => ((SyntaxToken)x).IsKind(SyntaxKind.PublicKeyword)); ;
        }

        public override Location GetLocation(PropertyDeclarationSyntax node, SyntaxNodeAnalysisContext context)
        {
            return node.Identifier.GetLocation();
        }
    }
}