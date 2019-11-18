using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace Crown.Summaries
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SummaryNonPublicFieldDiagnostic : SummaryDiagnosticBase<ClassDeclarationSyntax>
    {
        public override string DiagnosticId => "CROWN_005";
        public override SyntaxKind SyntaxKind => SyntaxKind.FieldDeclaration;
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return base.ShouldCreateDiagnostic(context) &&
                   !(context.Node as FieldDeclarationSyntax).Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword));
        }
        public override Location GetLocation(ClassDeclarationSyntax node, SyntaxNodeAnalysisContext context)
        {
            return node.Identifier.GetLocation();
        }
    }
}