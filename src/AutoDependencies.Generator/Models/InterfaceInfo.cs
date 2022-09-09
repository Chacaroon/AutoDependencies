using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDependencies.Generator.Models;

public record InterfaceInfo(string NamespaceName, InterfaceMemberInfo[] InterfaceMembers);
