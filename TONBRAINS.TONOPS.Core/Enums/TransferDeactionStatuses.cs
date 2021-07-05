using System.ComponentModel;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    /// <summary>
    /// Types node
    /// </summary>
    public enum TransferDeactionStatuses
    {
        Paused = 0,
        WaitingForAuthToken = 1,
        Executing = 2,
        Completed = 3,
    }
}
