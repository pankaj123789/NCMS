using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain.Enums;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class NaatiEntity : EntityBase
    {
        // THE ORDER OF THESE MATTER!!! 
        // This ordering is used when passing parameters to the entity insert stored procedure!
        // The current SP Parameter order is:
        //
        // @NAATINumberType int,
        // @ABN varchar (50), 
        // @UseEmail bit, 
        // @GSTApplies bit,
        // @WebsiteInPD bit,
        // @WebsiteURL varchar (200), 	
        // @Note varchar (8000), 
        // @EntityId int

        //public virtual int NaatiNumberType { get; set; } // used only when inserting a new entity record
        public virtual int EntityTypeId { get; set; }
        public virtual string Abn { get; set; }
        public virtual bool UseEmail { get; set; }
        public virtual bool GstApplies { get; set; }
        public virtual bool WebsiteInPD { get; set; }
        public virtual string WebsiteUrl { get; set; }
        public virtual string Note { get; set; }
        public virtual int NaatiNumber { get; set; }
        public virtual string AccountNumber { get; set; }

        public virtual IList<Email> Emails
        {
            get { return mEmails; }
        }

        public virtual Email PrimaryEmail
        {
            get { return mEmails.FirstOrDefault(e => e.IsPreferredEmail && !e.Invalid); }
        }

        private IList<Email> mEmails;

        public virtual IEnumerable<Address> PrimaryAddresses
        {
            get { return mPrimaryAddresses; }
        }

        public virtual Address PrimaryAddress
        {
            get
            {
                return PrimaryAddresses.FirstOrDefault(x => x.PrimaryContact);
            }
        }

        private IList<Address> mPrimaryAddresses;

        public virtual IEnumerable<Phone> Phones
        {
            get { return mPhones; }
        }
        public virtual IEnumerable<Address> Addresses
        {
            get { return mAddresses; }
        }
        

        public virtual string FirstPhoneNumber
        {
            get
            {
                var phone = Phones.FirstOrDefault(x => !x.Invalid);
                return phone == null ? string.Empty : phone.Number;
            }
        }

        private IList<Phone> mPhones;
        private IList<Address> mAddresses;

        public virtual IEnumerable<Person> People
        {
            get { return mPeople; }
        }

        private IList<Person> mPeople;

        private IList<NaatiEntityNote> mNaatiEntityNotes;

        public virtual IEnumerable<NaatiEntityNote> NaatiEntityNotes
        {
            get
            {
                return mNaatiEntityNotes;
            }
        }

        public virtual void ChangeNaatiNumber(int newNumber)
        {
            NaatiNumber = newNumber;
        }

        public override IAuditObject RootAuditObject
        {
            get { return this; }
        }

        protected override string AuditName
        {
            get { return "Entity"; }
        }

        public virtual string NaatiNumberDisplay
        {
            get
            {
                return string.Format("{0}{1}"
                    , EntityTypeId == (int)NaatiNumberTypeEnum.Other ? "NC" : string.Empty
                            , NaatiNumber);
            }
        }

        public virtual void AddNaatiEntityNote(NaatiEntityNote note)
        {
            note.Entity = this;
            mNaatiEntityNotes.Add(note);
        }

        public virtual void RemoveNaatiEntityNote(NaatiEntityNote note)
        {
            var result = (from pn in mNaatiEntityNotes
                          where pn.Id == note.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mNaatiEntityNotes.Remove(result);
                note.Entity = null;
            }
        }

        public NaatiEntity()
        {
            mPrimaryAddresses = new List<Address>();
            mPeople = new List<Person>();
            mEmails = new List<Email>();
            mPhones = new List<Phone>();
            mNaatiEntityNotes = new List<NaatiEntityNote>();
            mAddresses = new List<Address>();
        }
    }
}
