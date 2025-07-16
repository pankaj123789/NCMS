using System.Collections.Generic;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public sealed class WiiseInvoiceReference : WiiseReference
    {
        private const int ReferenceTextPartIndex = 1;

        private string _referenceText;
        public string ReferenceText => _referenceText ?? (_referenceText = ReferenceParts.Length > ReferenceTextPartIndex ? ReferenceParts[ReferenceTextPartIndex] : string.Empty);

        public WiiseInvoiceReference(string wiiseReference, IEnumerable<Office> offices)
            : base(wiiseReference, offices)
        {
        }

        /// <summary>
        /// Office constructor
        /// </summary>
        /// <param name="office"></param>
        public WiiseInvoiceReference(Office office)
        {
            Office = office;
            Reference = $"{OfficeAbbreviation}";
        }

        /// <summary>
        /// Reference Text constructor
        /// </summary>
        /// <param name="office"></param>
        /// <param name="referenceText"></param>
        public WiiseInvoiceReference(Office office, string referenceText)
        {
            Office = office;
            _referenceText = referenceText;
            Reference = $"{OfficeAbbreviation}-{referenceText}";
        }
    }
}
