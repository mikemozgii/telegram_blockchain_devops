using System.ComponentModel;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public enum DeploymentModuleTypes
    {
        [Description("Docker Image")]
        Docker = 1,
        [Description("Native")]
        Native = 2
    }
}
