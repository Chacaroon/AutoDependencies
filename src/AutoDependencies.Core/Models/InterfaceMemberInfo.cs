using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Models;
public class InterfaceMemberInfo
{
    public string Name { get; set; }

    public TypeSyntax ReturnType { get; set; }

    public ParameterListSyntax ParameterList { get; set; }
}
