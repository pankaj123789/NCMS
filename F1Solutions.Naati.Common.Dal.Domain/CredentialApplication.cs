using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplication : EntityBase
    {
        private IList<CredentialApplicationNote> mCredentialApplicationNotes;
        private IList<CredentialRequest> mCredentialRequests;
        private IList<CredentialApplicationFieldData> mCredentialApplicationFieldsData;
        private IList<CredentialWorkflowFee> mCredentialWorkflowFees;

        public const string ApplicationReferencePrefix = "APP";
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual CredentialApplicationStatusType CredentialApplicationStatusType { get; set; }
        public virtual IEnumerable<CredentialApplicationFieldData> CredentialApplicationFieldsData => mCredentialApplicationFieldsData;

        public virtual DateTime EnteredDate { get; set; }

        public virtual Person Person { get; set; }
        public virtual Institution SponsorInstitution { get; set; }
        public virtual ContactPerson SponsorInstitutionContactPerson { get; set; }
        public virtual TestLocation PreferredTestLocation { get; set; }
        public virtual User EnteredUser { get; set; }
        public virtual Office ReceivingOffice { get; set; }

        public virtual DateTime StatusChangeDate { get; set; }

        public virtual User StatusChangeUser { get; set; }

        public virtual User OwnedByUser { get; set; }

        public virtual bool OwnedByApplicant { get; set; }
        public virtual IEnumerable<CredentialApplicationNote> CredentialApplicationNotes => mCredentialApplicationNotes;
        public virtual IEnumerable<CredentialRequest> CredentialRequests => mCredentialRequests; 
        public virtual IList<CredentialWorkflowFee> CredentialWorkflowFees => mCredentialWorkflowFees;

        private string mReference = "";
        public virtual string Reference { get { return mReference; } }
        public virtual bool? AutoCreated { get; set; }

        public CredentialApplication()
        {
            mCredentialApplicationNotes = new List<CredentialApplicationNote>();
            mCredentialRequests = new List<CredentialRequest>();
            mCredentialApplicationFieldsData = new List<CredentialApplicationFieldData>();
            mCredentialWorkflowFees = new List<CredentialWorkflowFee>();
        }

        protected override string AuditName => "Credential Application";
        
        public override IAuditObject RootAuditObject => Person;

        public virtual void RemoveApplicationNote(CredentialApplicationNote applicationNote)
        {
            var result = (from pn in mCredentialApplicationNotes
                          where pn.Id == applicationNote.Id
                select pn).SingleOrDefault();

            if (result != null)
            {
                mCredentialApplicationNotes.Remove(result);
                applicationNote.CredentialApplication = null;
            }
        }
    }
}
