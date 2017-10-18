using System;
using System.ComponentModel;

namespace StarmixInfo.Models.Unity
{
    public enum Platform
    {
        [Description("standaloneosxintel64")]
        OSX,
        [Description("standalonewindows")]
        Windows32,
        [Description("standalonewindows64")]
        Windows64,
        [Description("ios")]
        iOS,
        [Description("android")]
        Android,
        [Description("standalonelinux")]
        Linux,
        [Description("standalonelinux64")]
        Linux32,
        [Description("standalonelinuxuniversal")]
        Linux64
    }
}
