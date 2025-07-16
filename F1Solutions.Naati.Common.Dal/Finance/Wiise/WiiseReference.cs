using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public abstract class WiiseReference
    {
        protected const string Unknown = "Unknown";
        protected const string Online = "Online";
        protected const string Overseas = "Overseas";
        protected Dictionary<string, string> OfficeNames = new Dictionary<string, string>
        {
            { "ONL", Online },
            { "OS", Overseas }
        };

        protected const int StatePartIndex = 0;

        protected readonly IEnumerable<Office> Offices;

        public string Reference { get; protected set; }

        private string[] _referenceParts;
        public string[] ReferenceParts
        {
            get { return _referenceParts ?? (_referenceParts = Reference.Split('-').Select(x => x.Trim()).ToArray()); }
        }

        private string StatePart => ReferenceParts.Length > StatePartIndex ? ReferenceParts[StatePartIndex] : null;

        private Office _office;
        protected Office Office
        {
            get { return _office ?? (_office = Offices.FirstOrDefault(x => x.Institution.InstitutionAbberviation.Equals(StatePart, StringComparison.InvariantCultureIgnoreCase))); }
            set { _office = value; }
        }

        public int? OfficeId => Office?.Id;

        public string OfficeName => OfficeNames.ContainsKey(OfficeAbbreviation) ? OfficeNames[OfficeAbbreviation] : OfficeAbbreviation;

        private string _officeAbbreviation;
        protected string OfficeAbbreviation => _officeAbbreviation ?? (_officeAbbreviation = Office?.Institution?.InstitutionAbberviation ?? Unknown);

        protected WiiseReference()
        {
            Offices = Enumerable.Empty<Office>();
        }

        protected WiiseReference(IEnumerable<Office> offices)
        {
            Offices = offices;
        }

        protected WiiseReference(string wiiseReference, IEnumerable<Office> offices)
        {
            Reference = wiiseReference ?? string.Empty;
            Offices = offices;
        }
    }
}
