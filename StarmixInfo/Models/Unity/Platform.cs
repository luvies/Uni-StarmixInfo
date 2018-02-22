using System;
using System.ComponentModel;

namespace StarmixInfo.Models.Unity
{
    public enum Platform
    {
        [Description("standalonewindows64")]
        Windows64,
        [Description("standalonewindows")]
        Windows32,
        [Description("standaloneosxintel64")]
        OSX,
        [Description("standalonelinux64")]
        Linux64,
        [Description("standalonelinux")]
        Linux32,
        [Description("standalonelinuxuniversal")]
        LinuxUniversal,
        [Description("android")]
        Android
    }
}
