using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Constants;

namespace AutoDependencies.Core.Extensions;
public static class StringExtensions
{
    public static string ToGeneratedFileName(this string name)
    {
        if (name.EndsWith(CoreConstants.GeneratedDocumentExtension))
        {
            return name;
        }

        if (name.EndsWith(".cs"))
        {
            name = name.Replace(".cs", string.Empty);
        }

        return $"{name}{CoreConstants.GeneratedDocumentExtension}";
    }
}
