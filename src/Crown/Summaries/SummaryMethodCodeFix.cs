using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crown.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Crown.Summaries
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SummaryMethodCodeFix)), Shared]
    public class SummaryMethodCodeFix : CSharpCodeFixBase
    {
        public override string DiagnosticId => "CROWN_006";
        protected override async Task<List<CodeAction>> CreateCodeActions(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            return new List<CodeAction>
            {
                CodeAction.Create(
                    title: Description.Title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: DiagnosticId)
            };
        }
        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, MethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

            VariableDeclaratorSyntax field = declarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
            string declaration = field.Identifier.ValueText;
            string formattedDeclaration = Parsers.Parse(Parsers.SplitByCapitalLetter(declaration), Parsers.ToLower);
            string comment = Parsers.Format(Description.MessageFormat, declaration, formattedDeclaration);

            DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => SummaryUtils.CreateOnlySummaryDocumentationCommentTrivia(comment), cancellationToken);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
            MethodDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

            SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}