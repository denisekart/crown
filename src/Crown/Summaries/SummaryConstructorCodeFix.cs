using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crown.Configuration;
using Crown.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Crown.Summaries
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SummaryConstructorCodeFix)), Shared]
    public class SummaryConstructorCodeFix : CSharpCodeFixBase
    {
        public override string DiagnosticId => "CROWN_003";
        protected override async Task<List<CodeAction>> CreateCodeActions(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            ConstructorDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

            return new List<CodeAction>()
            {
                CodeAction.Create(
                    title: Description.Title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: DiagnosticId)
            };
        }

        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, ConstructorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();
            DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => CreateDocumentationCommentTriviaSyntax(declarationSyntax), cancellationToken);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
            ConstructorDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

            SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private DocumentationCommentTriviaSyntax CreateDocumentationCommentTriviaSyntax(ConstructorDeclarationSyntax declarationSyntax)
        {
            SyntaxList<XmlNodeSyntax> list = SyntaxFactory.List<XmlNodeSyntax>();
            string className = declarationSyntax.Identifier.ValueText;

            string comment = Parsers.Format(Description.MessageFormat, className,
                Parsers.Parse(Parsers.SplitByCapitalLetter(className), Parsers.ToLower));

            list = list.AddRange(SummaryUtils.CreateSummaryPartNodes(comment));
            if (declarationSyntax.ParameterList.Parameters.Any())
            {
                foreach (ParameterSyntax parameter in declarationSyntax.ParameterList.Parameters)
                {
                    string parameterComment = Parsers.Format(ConfigurationReader.Instance.Configuration.GetMessageFormat("parameter"), parameter.Identifier.ValueText, Parsers.Parse(Parsers.SplitByCapitalLetter(parameter.Identifier.ValueText), Parsers.ToLower));
                    list = list.AddRange(SummaryUtils.CreateParameterPartNodes(parameter.Identifier.ValueText, parameterComment));
                }
            }
            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }
    }
}