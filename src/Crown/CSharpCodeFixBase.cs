using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Crown.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Crown
{
    //[ExportCodeFixProvider(LanguageNames.CSharp)]
    public abstract class CSharpCodeFixBase : CodeFixProvider
    {
        private CrownCodeFix _diagnosticDescription;
        public abstract string DiagnosticId { get; }
        public CrownCodeFix Description => _diagnosticDescription ?? (_diagnosticDescription =
                                               ConfigurationReader.Instance.Configuration.Diagnostics
                                                   .SingleOrDefault(x => x.Id.Equals(DiagnosticId))
                                                   ?.CodeFix);
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if(Description == null)
                return;

            if (!ConfigurationReader.Instance.Configuration.CurrentProfile().IsDiagnosticEnabled(DiagnosticId))
                return;

            List<CodeAction> codeActions =await CreateCodeActions(context);

            Diagnostic diagnostic = context.Diagnostics.First();
            if (codeActions?.Any() ?? false)
            {
                foreach (var codeAction in codeActions)
                {
                    context.RegisterCodeFix(codeAction, diagnostic);
                }
            }
        }

        protected abstract Task<List<CodeAction>> CreateCodeActions(CodeFixContext context);

        public sealed override ImmutableArray<string> FixableDiagnosticIds => Description== null? ImmutableArray<string>.Empty : ImmutableArray.Create(DiagnosticId);
    }
}