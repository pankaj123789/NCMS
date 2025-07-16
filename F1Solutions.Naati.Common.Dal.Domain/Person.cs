using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Person : EntityBase
    {
        public virtual NaatiEntity Entity { get; set; }
        public virtual string Gender { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual Country BirthCountry { get; set; }
        [Obsolete("Use the Sponsor Institution associated with an individual application instead.")]
        public virtual Institution SponsorInstitution { get; set; }
        public virtual bool Deceased { get; set; }
        public virtual bool ReleaseDetails { get; set; }
        public virtual bool DoNotInviteToDirectory { get; set; }
        public virtual DateTime EnteredDate { get; set; }
        public virtual string ExpertiseFreeText { get; set; }
        public virtual string NameOnAccreditationProduct { get; set; }
        public virtual bool DoNotSendCorrespondence { get; set; }
        public virtual bool ScanRequired { get; set; }
        public virtual bool AllowAutoRecertification { get; set; }
        public virtual bool? IsEportalActive { get; set; }
        public virtual DateTime? PersonalDetailsLastUpdatedOnEportal { get; set; }
        public virtual DateTime? WebAccountCreateDate { get; set; }
        public virtual bool AllowVerifyOnline { get; set; }
        public virtual bool ShowPhotoOnline { get; set; }
        public virtual bool InterculturalCompetency { get; set; }
        public virtual bool KnowledgeTest { get; set; }
        public virtual bool EthicalCompetency { get; set; }
        public virtual bool? RevalidationScheme { get; set; }
        public virtual string ExaminerSecurityCode { get; set; }
        public virtual string ExaminerTrackingCategory { get; set; }
        public virtual string PractitionerNumber { get; set; }
        public virtual string MfaCode { get; set; }
        public virtual DateTime? MfaExpireStartDate { get; set; }
        public virtual DateTime? EmailCodeExpireStartDate { get; set; }
        public virtual bool AccessDisabledByNcms { get; set; }
        public virtual int LastEmailCode { get; set; }



        public virtual bool IsExaminer => PanelMemberships.Any(
            x => x.PanelRole.PanelRoleCategory.Id == 1 && // todo replace by enum panelroleCategory
                 x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);
        public virtual bool IsFormerPractitioner => CertificationPeriods.Any() && CertificationPeriods.All(p => p.EndDate <= DateTime.Now || p.Credentials.All(c => c.TerminationDate.HasValue && c.TerminationDate <= DateTime.Now));
        public virtual bool IsPractitioner => CertificationPeriods.Any(p => p.EndDate > DateTime.Now && p.Credentials.Any(c => c.StartDate <= DateTime.Now && (!c.TerminationDate.HasValue || c.TerminationDate > DateTime.Now)));

        public virtual bool IsApplicant => CredentialApplications.Any(a => new List<int> { 1, 4, 6, 7 }.Contains(a.CredentialApplicationStatusType.Id));
        public virtual bool IsFuturePractitioner => !IsPractitioner && CertificationPeriods.Any(p => p.EndDate > DateTime.Now && p.Credentials.Any(c => c.StartDate > DateTime.Now && (!c.TerminationDate.HasValue || c.TerminationDate > DateTime.Now)));


        public virtual IList<ExaminerUnavailable> ExaminerUnavailable => mExaminerUnavailable;
        public virtual bool IsRolePlayer => PanelMemberships.Any(
            x => x.PanelRole.PanelRoleCategory.Id == 2 && // todo replace by enum panelroleCategory
                 x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);
        public virtual LatestPersonName LatestPersonName => mLatestPersonName;

        private IList<PersonName> mPersonNames;
        private IList<CredentialApplication> mCredentialApplications;
        private IList<PersonImage> mPersonImages;
        private LatestPersonName mLatestPersonName;
        private IList<PanelMembership> mPanelMemberships;
        private IList<ExaminerUnavailable> mExaminerUnavailable;




        private IEnumerable<CertificationPeriod> mCertificationPeriods;

        public Person()
        {
            mPersonNames = new List<PersonName>();
            mCredentialApplications = new List<CredentialApplication>();
            mPersonImages = new List<PersonImage>();
            mPanelMemberships = new List<PanelMembership>();
            mLatestPersonName = new LatestPersonName();
            mExaminerUnavailable = new List<ExaminerUnavailable>();
            mCertificationPeriods = new List<CertificationPeriod>();
        }

        public virtual IEnumerable<PanelMembership> PanelMemberships
        {
            get
            {
                return mPanelMemberships;
            }
        }

        public virtual IEnumerable<CertificationPeriod> CertificationPeriods
        {
            get { return mCertificationPeriods; }
        }

        protected override string AuditName => nameof(Person);

        public virtual IEnumerable<PersonName> PersonNames
        {
            get { return mPersonNames; }
        }

        public virtual IEnumerable<CredentialApplication> CredentialApplications
        {
            get { return mCredentialApplications; }
        }

        /// <summary>
        /// This gets one of the primary addresses assigned for the person.  
        /// Some people have multiple primary addresses defined, though this seems to be a bug.
        /// If you're interested in getting all primary addresses for a person, take a look at the Entity's PrimaryAddresses.
        /// </summary>
        public virtual Address PrimaryAddress
        {
            get
            {
                return Entity.PrimaryAddress;
            }
        }

        public virtual List<Address> Addresses
        {
            get { return Entity.PrimaryAddresses.ToList(); }
        }

        public virtual IEnumerable<PersonImage> PersonImages
        {
            get { return mPersonImages; }
        }

        public virtual void AddPersonName(PersonName name)
        {
            name.Person = this;
            mPersonNames.Add(name);
        }

        public virtual void AddPersonImage(PersonImage personImage)
        {
            personImage.Person = this;
            mPersonImages.Add(personImage);
        }

        // Formula fields
        public virtual string GivenName { get; set; }
        public virtual string AlternativeGivenName { get; set; }
        public virtual string OtherNames { get; set; }
        public virtual string Surname { get; set; }
        public virtual string AlternativeSurname { get; set; }
        public virtual string Title { get; set; }
        public virtual int? TitleId { get; set; }
        //these are not populated so should be used only for query filtering and not values
        public virtual int AccreditationCountNonPopulated { get; set; }
        public virtual int RecognitionCountNonPopulated { get; set; }
        //SQL version of suburborcountry on address
        public virtual string SuburbOrCountry { get; set; }

        /// <summary>
        /// Gets either the NameOnAccreditationProduct field if it is filled in,
        /// otherwise returns a formatted version of the person's name.
        /// </summary>
        /// <returns>The person's name for accreditation products</returns>
        public virtual string GetNameForAccreditationProduct()
        {
            if (!string.IsNullOrEmpty(NameOnAccreditationProduct))
            {
                return NameOnAccreditationProduct;
            }

            return FullName;
        }

        public virtual string FirstPhoneNumber
        {
            get
            {
                return Entity.FirstPhoneNumber;
            }
        }

        public virtual string PrimaryEmailAddress
        {
            get
            {
                return Entity.PrimaryEmail == null ? string.Empty : Entity.PrimaryEmail.EmailAddress;
            }
        }

        public virtual string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return string.Format("{0} {1}", GivenName, Surname);
                }

                return string.Format("{0} {1} {2}", Title, GivenName, Surname);
            }
        }

        public virtual string SurnameFirstFullName
        {
            get
            {
                var trimChars = new char[] { ' ', ';' };
                return string.Format("{0}; {1} {2}", Surname, GivenName, OtherNames).Trim(trimChars);
            }
        }

        public virtual string SurnameGivenOtherAlternative
        {
            get
            {
                var trimChars = new char[] { ' ', ';' };

                string alternativeName = string.Format("{0}; {1}", AlternativeSurname, AlternativeGivenName).Trim(trimChars);
                alternativeName = (!alternativeName.IsNullOrEmpty() ? string.Format("({0})", alternativeName) : string.Empty);

                string name = string.Format("{0}; {1} {2}", Surname, GivenName, OtherNames).Trim(trimChars);

                return string.Format("{0} {1}", name, alternativeName).Trim(trimChars);
            }
        }

        public virtual int GetNaatiNumber()
        {
            return Entity.NaatiNumber;
        }

        public virtual string NaatiNumberDisplay
        {
            get { return Entity.NaatiNumberDisplay; }
        }

        public virtual bool HasPhoto { get; set; }
        public virtual DateTime? PhotoDate { get; set; }

        public virtual byte[] Photo
        {
            get
            {
                var personImage = PersonImages.FirstOrDefault();

                if (personImage == null)
                {
                    return null;
                }

                return personImage.Photo;
            }
        }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return this.Entity.RootAuditObject;
            }
        }

        public virtual string PrimaryContactNumber
        {
            get
            {
                var phone = Entity.Phones.FirstOrDefault(x => x.PrimaryContact);
                return phone == null ? string.Empty : phone.Number;
            }
        }

        public virtual string SecondaryEmailAddress
        {
            get { return Entity.Emails.FirstOrDefault(x => !x.IsPreferredEmail)?.EmailAddress; }
        }

        public virtual string SecondaryAddress
        {
            get
            {
                return
                    Entity.PrimaryAddresses.Where(x => !x.PrimaryContact)
                        .Select(x => x.StreetDetails + ", " + x.SuburbOrCountry)
                        .FirstOrDefault();
            }
        }

        public virtual string SecondaryContactNumber
        {
            get { return Entity.Phones.ToList().FirstOrDefault(x => x.Number != FirstPhoneNumber && !x.Invalid)?.Number; }
        }

        public virtual string InOdEmails
        {
            get { return string.Join(",", Entity.Emails.Where(x => x.IncludeInPD).Select(x => x.EmailAddress)); }
        }

        public virtual string InOdPhones
        {
            get { return string.Join(",", Entity.Phones.Where(x => x.IncludeInPD).Select(x => x.Number)); }
        }

        public virtual string InOdAddresses
        {
            get { return string.Join(";", Entity.PrimaryAddresses.Where(x => x.OdAddressVisibilityType.Id != 1).Select(x => x.StreetDetails + ", " + x.SuburbOrCountry)); }
        }
    }
}
