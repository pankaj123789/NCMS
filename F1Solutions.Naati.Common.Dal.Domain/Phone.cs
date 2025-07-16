namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Phone : EntityBase
    {

        public virtual NaatiEntity Entity { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string AreaCode { get; set; }
        public virtual string LocalNumber { get; set; }
        public virtual string Note { get; set; }
        public virtual bool IncludeInPD { get; set; }
        public virtual bool PrimaryContact { get; set; }
        public virtual bool AllowSmsNotification { get; set; }
        public virtual bool Invalid { get; set; }
        public virtual bool ExaminerCorrespondence { get; set; }
        public virtual string Number
        {
            get
            {
                // Old expresion from the database.
                // (rtrim(ltrim(([CountryCode] + ' ' + [AreaCode] + ' ' + case when (len([LocalNumber]) = 8) then (substring([LocalNumber],1,4) + ' ' + substring([LocalNumber],5,4)) else [LocalNumber] end))))

                string localNumber = LocalNumber;

                if(LocalNumber.Length == 8)
                {
                    localNumber = string.Format("{0} {1}", LocalNumber.Substring(0, 4), LocalNumber.Substring(4, 4)).Trim();
                }

                return string.Format("{0} {1} {2}", CountryCode, AreaCode, localNumber).Trim();
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
