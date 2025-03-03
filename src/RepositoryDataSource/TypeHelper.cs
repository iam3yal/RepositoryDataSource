namespace Aspx.WebControls;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Compilation;
    
internal static class TypeHelper
{
    private const string AspxTempFolder = "Temporary ASP.NET Files";

    private static readonly string[] SystemFolders = [
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
        Environment.GetFolderPath(Environment.SpecialFolder.System),
        Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
    ];

    internal static Type GetType(string typeName)
    {
        var type = BuildManager.GetType(typeName, false, true);

        if (type == null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyType = assembly.GetType(typeName);

                var continuing = SystemFolders.Any(
                    folder => assembly.IsDynamic || (assembly.Location.StartsWith(folder, true, null)
                                                     && !assembly.Location.Contains(AspxTempFolder)));

                if (continuing)
                {
                    continue;
                }

                if (type != null && assemblyType != null)
                {
                    throw new InvalidOperationException(string.Format(Strings.TypeAmbiguity, typeName, type.Assembly.Location, assembly.Location));
                }

                if (assemblyType != null)
                {
                    type = assemblyType;
                }
            }
        }

        return type;
    }
}