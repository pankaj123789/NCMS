using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public static class ProductType
    {
        public const string SelfInkingStamp = "Self-inking stamp";
        public const string RubberStamp = "Rubber stamp";
        public const string LaminatedCertificate = "Laminated certificate";
        public const string UnlaminatedCertificate = "Unlaminated certificate";
        public const string SingleIdCard = "Accreditation & Recognition ID card";
        public const string AccreditationIdCard = "Accreditation ID card";
        public const string RecognitionIdCard = "Recognition ID card";
        public const string PDRegistration = "Practitioners directory registration";
        public const string PDRenewal = "Practitioners directory renewal";

        public static IEnumerable<string> List
        {
            get
            {
                return new List<string>()
                {
                    SelfInkingStamp,
                    RubberStamp,
                    LaminatedCertificate,
                    UnlaminatedCertificate,
                    SingleIdCard,
                    AccreditationIdCard,
                    RecognitionIdCard,
                    PDRegistration,
                    PDRenewal
                };
            }
        }
    }
}