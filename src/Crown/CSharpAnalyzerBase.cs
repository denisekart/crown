using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Crown.Configuration;
using Crown.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown
{
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class CSharpAnalyzerBase : DiagnosticAnalyzer
    {
        private CrownDiagnostic _diagnosticDescription;
        public abstract string DiagnosticId { get; }
        public abstract SyntaxKind SyntaxKind { get; }
        public CrownDiagnostic Description => _diagnosticDescription ?? (_diagnosticDescription =
                                                  ConfigurationReader.Instance.Configuration.Diagnostics
                                                      .SingleOrDefault(x => x.Id.Equals(DiagnosticId)));
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind);
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
        }


        protected void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if(Description == null)
                return;

            if(!ConfigurationReader.Instance.Configuration.CurrentProfile().IsDiagnosticEnabled(DiagnosticId))
                return;
            
            if (ShouldCreateDiagnostic(context))
            {
                var diagnostic = CreateDiagnostic(context);
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        protected abstract bool ShouldCreateDiagnostic(SyntaxNodeAnalysisContext context);
        protected abstract Diagnostic CreateDiagnostic(SyntaxNodeAnalysisContext context);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Description==null?ImmutableArray<DiagnosticDescriptor>.Empty : ImmutableArray.Create(new DiagnosticDescriptor(Parsers.NullIfEmpty(Description.CodeFix.DiagnosticIdReference)??DiagnosticId, Description.Title, Description.MessageFormat, Description.Category, 
            Description.Severity==CrownSeverity.Error
                ?DiagnosticSeverity.Error
                :Description.Severity==CrownSeverity.Warning
                    ?DiagnosticSeverity.Warning
                    : Description.Severity == CrownSeverity.Info 
                        ? DiagnosticSeverity.Info 
                        : DiagnosticSeverity.Hidden, true));
    }
}