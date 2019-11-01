using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Summaries
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SummaryPublicConstructorDiagnostic : SummaryDiagnosticBase<ConstructorDeclarationSyntax>
    {
        public override string DiagnosticId => "CROWN_003";
        public override SyntaxKind SyntaxKind => SyntaxKind.ConstructorDeclaration;
        protected override bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            return base.ShouldCreateDiagnostic(context) && (context.Node as ConstructorDeclarationSyntax).Modifiers.Any(SyntaxKind.PublicKeyword);
        }

        public override Location GetLocation(ConstructorDeclarationSyntax node, SyntaxNodeAnalysisContext context)
        {
            return node.Identifier.GetLocation();
        }
    }
}
