using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public enum DeploymentNodeTypes
    {
        [Description("Physical")]
        Physical = 0,
        [Description("VM")]
        VM = 1,
        [Description("VPS")]
        VPS = 2,
    }
}
