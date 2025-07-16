using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyNaati.Contracts.Portal.PayPal
{
    public class OrderSubmission
    {
        public class PayPalProcessStatus
        {
            public const string Created = "created";
            public const string Approved = "approved";
            public const string Failed = "failed";
        }
    }
}
