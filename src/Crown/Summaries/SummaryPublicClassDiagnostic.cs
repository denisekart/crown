using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Summaries
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SummaryPublicClassDiagnostic : SummaryDiagnosticBase<ClassDeclarationSyntax>
    {
        public override string DiagnosticId => "CROWN_001";
        public override SyntaxKind SyntaxKind => SyntaxKind.ClassDeclaration;
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return base.ShouldCreateDiagnostic(context) &&
                   (context.Node as ClassDeclarationSyntax).Modifiers.Any(x => x.ValueText == "public");
        }

        public override Location GetLocation(ClassDeclarationSyntax node, SyntaxNodeAnalysisContext context)
        {
            return node.Identifier.GetLocation();
        }
    }
}