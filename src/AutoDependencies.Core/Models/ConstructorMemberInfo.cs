using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Models;
public class ConstructorMemberInfo
{
    public string Name { get; set; }

    public TypeSyntax Type { get; set; }
}
