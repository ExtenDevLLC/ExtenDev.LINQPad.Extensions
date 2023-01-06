using System;
using System.Collections.Generic;
using System.Text;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class ExtensionSettings
    {
        public static bool CopyableStringDumping { get; set; } = false;

        // TODO: currently this assumes it's in the path, but ideally there should be ways to infer this from logic similar to vswhere
        // TODO: Should settings be centralized in a single static class like this or separated out
        // TODO: Provide customization, persistence of settings, etc.
        public static string TeamFoundationClientExePath { get; set; } = "TF.exe";

    }
}
