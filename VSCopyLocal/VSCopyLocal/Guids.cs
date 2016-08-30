// Guids.cs
// MUST match guids.h
using System;

namespace CodeValue.VSCopyLocal
{
    static class GuidList
    {
        public const string GuidVsPackageTemplatePkgString = "5af1aede-b682-4f97-840f-5f602bd82bb4";

        public const string GuidVSCopyLocalCmdSetString = "44fc6acc-72b4-43b9-b720-dc7d4f956b50";
        public const string GuidVSCopyLocalCmdReferencesSetString = "44fc6acc-72b4-43b9-b720-dc7d4f956b51";
        public const string GuidVSCopyLocalCmdSolutionSetString = "44fc6acc-72b4-43b9-b720-dc7d4f956b52";

        public static readonly Guid GuidVSCopyLocalCmdSet = new Guid(GuidVSCopyLocalCmdSetString);
        public static readonly Guid GuidVSCopyLocalCmdReferencesSet = new Guid(GuidVSCopyLocalCmdReferencesSetString);
        public static readonly Guid GuidVSCopyLocalCmdSolutionSet = new Guid(GuidVSCopyLocalCmdSolutionSetString);
    };
}
