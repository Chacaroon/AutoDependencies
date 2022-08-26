using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly ISecondService _secondService;

    public void DoSmth()
    {
    }
}
