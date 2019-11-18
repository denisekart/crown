using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Crown.Utilities
{
    public static class RoslynExtensions
    {
        public static Solution GetSolution(this SyntaxNodeAnalysisContext context)
        {
            try
            {
                var workspace = context.Options.GetPrivatePropertyValue<object>("Workspace");
                return workspace.GetPrivatePropertyValue<Solution>("CurrentSolution");
            }
            catch
            {
                return null;
            }
        }

        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var pi = obj.GetType().GetRuntimeProperty(propName);

            if (pi == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propName), $"Property {propName} was not found in Type {obj.GetType().FullName}");
            }

            return (T)pi.GetValue(obj, null);
        }
    }
}