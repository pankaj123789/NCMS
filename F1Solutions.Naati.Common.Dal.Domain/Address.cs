using System;
using System.Text;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Address : EntityBase
    {

        public virtual NaatiEntity Entity { get; set; }
        public virtual Country Country { get; set; }
        public virtual Postcode Postcode { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? SubscriptionExpiryDate { get; set; }
        public virtual DateTime? SubscriptionRenewSentDate { get; set; }
        public virtual OdAddressVisibilityType OdAddressVisibilityType { get; set; }
        public virtual bool PrimaryContact { get; set; }
        public virtual bool Invalid { get; set; }
        public virtual bool ValidateInExternalTool { get; set; }
        public virtual string Note { get; set; }
        public virtual string StreetDetails { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual bool ExaminerCorrespondence { get; set; }


        public virtual string SuburbOrCountry
        {
            get
            {
                var str = new StringBuilder();
                if (Country != null)
                {
                    if (Country.Name == "Australia")
                    {
                        if (Postcode != null)
                        {
                            if (Postcode.Suburb != null)
                            {
                                if (!string.IsNullOrEmpty(Postcode.Suburb.Name))
                                    str.Append(Postcode.Suburb.Name + " ");
                                if (Postcode.Suburb.State != null && !string.IsNullOrEmpty(Postcode.Suburb.State.Abbreviation))
                                    str.Append(Postcode.Suburb.State.Abbreviation + " ");
                            }
                            if (!string.IsNullOrEmpty(Postcode.PostCode))
                                str.Append(Postcode.PostCode);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Country.Name))
                            str.Append(Country.Name);
                    }
                }
                return str.ToString().Trim();
            }
        }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return Entity.RootAuditObject;
            }
        }
    }
}
