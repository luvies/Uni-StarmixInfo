using System.ComponentModel;

namespace StarmixInfo.Models.Unity
{
    public enum BuildStatus
    {
        [Description("queued")]
        Queued,
        [Description("started")]
        Started,
        [Description("restarted")]
        Restarted,
        [Description("success")]
        Success,
        [Description("failure")]
        Failure,
        [Description("cancelled")]
        Cancelled,
        [Description("upload")]
        Upload,
        [Description("unknown")]
        Unknown
    }
}
