using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace Crown.Summaries
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SummaryPublicMethodDiagnostic : SummaryDiagnosticBase<MethodDeclarationSyntax>
    {
        public override string DiagnosticId => "CROWN_006";
        public override SyntaxKind SyntaxKind => SyntaxKind.MethodDeclaration;
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return base.ShouldCreateDiagnostic(context) && (context.Node as MethodDeclarationSyntax).Modifiers.Any(x => CSharpExtensions.IsKind((SyntaxToken) x, SyntaxKind.PublicKeyword)); ;
        }

        public override Location GetLocation(MethodDeclarationSyntax node, SyntaxNodeAnalysisContext context)
        {
            return node.Identifier.GetLocation();
        }
    }
}