using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Crown.Utilities
{
    public static class SummaryUtils
    {
        public const string Summary = "summary";

        public static DocumentationCommentTriviaSyntax CreateOnlySummaryDocumentationCommentTrivia(string content)
        {
            SyntaxList<XmlNodeSyntax> list = SyntaxFactory.List(CreateSummaryPartNodes(content));
            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }

        public static XmlNodeSyntax[] CreateSummaryPartNodes(string content)
        {
            XmlTextSyntax xmlText0 = CreateLineStartTextSyntax();

            XmlElementSyntax xmlElement = CreateSummaryElementSyntax(content);

            XmlTextSyntax xmlText1 = CreateLineEndTextSyntax();

            return new XmlNodeSyntax[] { xmlText0, xmlElement, xmlText1 };

        }

        public static XmlNodeSyntax[] CreateParameterPartNodes(string parameterName, string parameterContent)
        {
            XmlTextSyntax lineStartText = CreateLineStartTextSyntax();

            XmlElementSyntax parameterText = CreateParameterElementSyntax(parameterName, parameterContent);

            XmlTextSyntax lineEndText = CreateLineEndTextSyntax();

            return new XmlNodeSyntax[] { lineStartText, parameterText, lineEndText };
        }

        public static XmlNodeSyntax[] CreateReturnPartNodes(string content)
        {
            XmlTextSyntax lineStartText = CreateLineStartTextSyntax();

            XmlElementSyntax returnElement = CreateReturnElementSyntax(content);

            XmlTextSyntax lineEndText = CreateLineEndTextSyntax();

            return new XmlNodeSyntax[] { lineStartText, returnElement, lineEndText };
        }

        private static XmlElementSyntax CreateSummaryElementSyntax(string content)
        {
            XmlNameSyntax xmlName = SyntaxFactory.XmlName(SyntaxFactory.Identifier(Summary));
            XmlElementStartTagSyntax summaryStartTag = SyntaxFactory.XmlElementStartTag(xmlName);
            XmlElementEndTagSyntax summaryEndTag = SyntaxFactory.XmlElementEndTag(xmlName);

            return SyntaxFactory.XmlElement(
                summaryStartTag,
                SyntaxFactory.SingletonList<XmlNodeSyntax>(CreateSummaryTextSyntax(content)),
                summaryEndTag);
        }

        private static XmlElementSyntax CreateParameterElementSyntax(string parameterName, string parameterContent)
        {
            XmlNameSyntax paramName = SyntaxFactory.XmlName("param");
            XmlNameAttributeSyntax paramAttribute = SyntaxFactory.XmlNameAttribute(parameterName);
            XmlElementStartTagSyntax startTag = SyntaxFactory.XmlElementStartTag(paramName, SyntaxFactory.SingletonList<XmlAttributeSyntax>(paramAttribute));
            XmlTextSyntax content = SyntaxFactory.XmlText(parameterContent);
            XmlElementEndTagSyntax endTag = SyntaxFactory.XmlElementEndTag(paramName);
            return SyntaxFactory.XmlElement(startTag, SyntaxFactory.SingletonList<SyntaxNode>(content), endTag);
        }

        private static XmlElementSyntax CreateReturnElementSyntax(string content)
        {
            XmlNameSyntax xmlName = SyntaxFactory.XmlName("returns");

            XmlElementStartTagSyntax startTag = SyntaxFactory.XmlElementStartTag(xmlName);

            XmlTextSyntax contentText = SyntaxFactory.XmlText(content);

            XmlElementEndTagSyntax endTag = SyntaxFactory.XmlElementEndTag(xmlName);
            return SyntaxFactory.XmlElement(startTag, SyntaxFactory.SingletonList<XmlNodeSyntax>(contentText), endTag);
        }

        private static XmlTextSyntax CreateSummaryTextSyntax(string content)
        {
            content = " " + content;
            SyntaxToken newLine0Token = CreateNewLineToken(); 
            SyntaxTriviaList leadingTrivia = CreateCommentExterior();
            SyntaxToken text1Token = SyntaxFactory.XmlTextLiteral(leadingTrivia, content, content, SyntaxFactory.TriviaList());
            SyntaxToken newLine2Token = CreateNewLineToken();

            SyntaxTriviaList leadingTrivia2 = CreateCommentExterior();
            SyntaxToken text2Token = SyntaxFactory.XmlTextLiteral(leadingTrivia2, " ", " ", SyntaxFactory.TriviaList());

            return SyntaxFactory.XmlText(newLine0Token, text1Token, newLine2Token, text2Token);
        }

        private static XmlTextSyntax CreateLineStartTextSyntax()
        {
            SyntaxTriviaList xmlText0Leading = CreateCommentExterior();
            SyntaxToken xmlText0LiteralToken = SyntaxFactory.XmlTextLiteral(xmlText0Leading, " ", " ", SyntaxFactory.TriviaList());
            XmlTextSyntax xmlText0 = SyntaxFactory.XmlText(xmlText0LiteralToken);
            return xmlText0;
        }

        private static XmlTextSyntax CreateLineEndTextSyntax()
        {
            SyntaxToken xmlTextNewLineToken = CreateNewLineToken();
            XmlTextSyntax xmlText = SyntaxFactory.XmlText(xmlTextNewLineToken);
            return xmlText;
        }
        
        private static SyntaxToken CreateNewLineToken()
        {
            return SyntaxFactory.XmlTextNewLine(Environment.NewLine, false);
        }

        private static SyntaxTriviaList CreateCommentExterior()
        {
            return SyntaxFactory.TriviaList(SyntaxFactory.DocumentationCommentExterior("///"));
        }
    }
}
