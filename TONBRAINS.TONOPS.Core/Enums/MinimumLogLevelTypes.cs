using System.ComponentModel;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    /// <summary>
    /// Types log level
    /// </summary>
    public enum MinimumLogLevelTypes
    {
        [Description("Debug")]
        Debug = 0,
        [Description("Verbose")]
        Verbose = 1,
        [Description("Information")]
        Information = 2,
        [Description("Warrning")]
        Warrning = 3,
        [Description("Error")]
        Error = 4,
        [Description("Fatal")]
        Fatal = 5
    }
}
