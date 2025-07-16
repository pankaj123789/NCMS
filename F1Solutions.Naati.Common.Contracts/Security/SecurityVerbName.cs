using System;
using System.ComponentModel;

namespace F1Solutions.Naati.Common.Contracts.Security
{
    [Flags]
    public enum SecurityVerbName : long
    {
        Search = 0x1,
        [Description("View")]
        Read = 0x2,
        Update = 0x4,
        Create = 0x8,
        Delete = 0x10,
        Manage = 0x20,
        Reactivate = 0x40,
        Upload = 0x80,
        Download = 0x100,
        Override = 0x200,
        Revert = 0x400,
        [Description("Validate/Check")]
        Validate = 0x800,
        Send = 0x1000,
        [Description("Allocate/Assign")]
        Assign = 0x2000,
        Issue = 0x4000,
        Finalise = 0x8000,
        Reject = 0x10000,
        Close = 0x20000,
        Invalidate = 0x40000,
        Cancel = 0x80000,
        Invite = 0x100000,
        Uninvite = 0x200000,
        Withdraw = 0x400000,
        Approve = 0x800000,
        Notify = 0x1000000,
        Notes = 0x2000000,
        Extend = 0x4000000, 
        PreviewEmail = 0x8000000,
        MarkAsPaid = 0x10000000,
        Assess = 0x20000000,
        Configure = 0x40000000,
        RequestRefund = 0x80000000,
        ApproveRefund = 0x100000000,
        RejectRefund = 0x200000000,
        ProcessRefund = 0x400000000,
        AssignPastSession = 0x1000000000,
    }
}