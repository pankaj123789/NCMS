using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Type;
using Credential = F1Solutions.Naati.Common.Dal.Domain.Credential;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    public class QuerySearchHelper
    {
        protected CredentialApplication CredentialApplication => null;
        protected Person Person => null;
        protected IssuedCredentialCredentialRequest IssuedCredentialCredentialRequest => null;
        protected CredentialType CredentialType => null;
        protected TestSessionSkill TestSessionSkill => null;
        protected CredentialType TestSessionCredentialType => null;
        protected NaatiEntity Entity => null;
        protected NaatiEntity RecipientEntity => null;
        protected User RecipientUser => null;
        protected Email Email => null;
        protected EmailSendStatusType EmailSendStatusType => null;
        protected PersonName PersonName => null;
        protected Phone Phone => null;
        protected CredentialRequest CredentialRequest => null;
        protected CredentialRequestStatusType CredentialRequestStatusType => null;
        protected CredentialApplicationType CredentialApplicationType => null;
        protected CredentialApplicationTypeCredentialType CredentialApplicationTypeCredentialType => null;
        protected CredentialApplicationTypeCategory CredentialApplicationTypeCategory => null;
        protected CredentialApplicationStatusType CredentialApplicationStatusType = null;
        protected Credential Credential => null;
        protected CredentialCredentialRequest CredentialCredentialRequest => null;
        protected SkillApplicationType SkillApplicationType => null;
        protected DirectionType DirectionType => null;
        protected EmailMessage EmailMessage => null;
        protected User CreatedUser => null; 
        protected EndorsedQualification EndorsedQualification => null; 
        protected CredentialCategory CredentialCategory => null;
        protected Language Language1 => null;
        protected Language Language2 => null;
        protected Language TestMateriaLanguage => null;
        protected Skill Skill => null;
        protected SkillType SkillType => null;
        protected User OwnedByUser => null;
        protected Institution SponsorInstitution => null;
        protected NaatiEntity SponsorEntity => null;
        protected InstitutionName SponsorInstitutionName => null;
        protected InstitutionName InstitutionName => null;
        protected Office ReceivingOffice => null;
        protected LatestPersonName LatestPersonName => null;
        protected LatestInstitutionName LatestInstitutionName => null;
        protected ContactPerson ContactPerson => null;
        protected Title Title => null;
        protected Address Address => null;
        protected OdAddressVisibilityType OdAddressVisibilityType => null;
        protected Country AddressCountry => null;
        protected Suburb Suburb => null;
        protected PanelMembershipCredentialType PanelMembershipCredentialType => null;
        protected CredentialType MembershipCredentialType => null;
        protected State State => null;
        protected Postcode Postcode => null;
        protected CertificationPeriod CertificationPeriod => null;
        protected TestLocation PreferredTestLocation => null;
        protected Office Office => null;
        protected PanelMembership PanelMembership => null;
        protected Panel Panel => null;
        protected Language PanelLanguage => null;
        protected PanelType PanelType => null;
        protected PanelRole PanelRole => null;
        protected PanelRoleCategory PanelRoleCategory => null;
        protected TestSitting TestSitting => null;
        protected TestSession TestSession => null;
        protected TestSpecification TestSpecification => null;
        protected TestComponent TestComponent => null;
        protected TestComponentBaseType TestComponentBaseType => null;
        protected TestMaterialLastUsed TestMaterialLastUsed => null;
        protected TestComponentType TestComponentType => null;
        protected TestMaterialType TestMaterialType => null;
        protected TestMaterialDomain TestMaterialDomain => null;
        protected TestComponent TestMaterialComponent => null;
        protected TestSittingTestMaterial TestSittingTestMaterial => null;
        protected TestMaterial TestMaterial => null;
        protected TestSession IntendedTestSession => null;
        protected Venue Venue => null;
        protected Office TestOffice => null;
        protected TestLocation TestLocation => null;
        protected Institution Institution => null;
        protected MaterialRequestRoundLatest MaterialRequestRoundLatest => null;
        protected MaterialRequestStatusType MaterialRequestStatusType => null;
        protected MaterialRequestRoundStatusType MaterialRequestRoundStatusType => null;
        protected MaterialRequestRound MaterialRequestRound => null;
        protected MaterialRequestPanelMembership MaterialRequestPanelMembership => null;
        protected MaterialRequestPanelMembershipTask MaterialRequestPanelMembershipTask => null;
        protected Domain.MaterialRequest MaterialRequest => null;
        protected TestMaterial OutputMaterial => null;
        protected TestMaterial SourceMaterial => null;
        protected TestStatus TestStatus => null;
        protected TestStatusType TestStatusType => null;
        protected Job Job => null;
        protected TestResult TestResult => null;
        protected TestComponentResult TestComponentResult => null;
        protected JobExaminer JobExaminer => null;
        protected ExaminerMarking ExaminerMarking => null;
        protected Language Language => null;
        protected LanguageGroup LanguageGroup => null;
        protected LanguageGroup LanguageGroup1 => null;
        protected LanguageGroup LanguageGroup2 => null;
		protected EmailTemplate EmailTemplate => null;
		protected CredentialWorkflowActionEmailTemplate CredentialWorkflowActionEmailTemplate => null;
		protected SystemActionEmailTemplate SystemActionEmailTemplate => null;
		protected SystemActionType SystemActionType => null;
        protected TestSessionRolePlayer TestSessionRolePlayer => null;
        protected RolePlayer RolePlayer => null;

        protected CredentialApplicationFieldData CredentialApplicationFieldData => null;
        protected CredentialApplicationField CredentialApplicationField => null;

        protected virtual Junction GetPhoneFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var phoneRestriction = GetPhoneRestriction(criteria);
            junction.Add(phoneRestriction);
            return junction;
        }

        protected IProjection Concatenate( params IProjection[] projections)
        {
            return Projections.SqlFunction("CONCAT", NHibernateUtil.String, projections);
        }

        protected ICriterion GetPhoneRestriction<S>(ISearchCriteria<S> criteria)
        {
            var numberProjection = Concatenate(Projections.Property(() => Phone.CountryCode),
                Projections.Property(() => Phone.AreaCode),
                Projections.Property(() => Phone.LocalNumber));

            return Restrictions.Like(numberProjection, (criteria.Values.FirstOrDefault() ?? string.Empty).Replace(" ", string.Empty));
        }

        protected IProjection GetSurnameProjection()
        {
            var surnameProjection = Projections.Property(() => PersonName.Surname);
            var emptySurnameCondition = Restrictions.Eq(surnameProjection, PersonName.SurnameNotStated);
            var emptyProjection = Projections.Constant(String.Empty, NHibernateUtil.String);

            return Projections.Conditional(
                emptySurnameCondition,
                emptyProjection,
                surnameProjection);
        }

        protected virtual IProjection GetDirectionProjection()
        {
            var language1Replace = Projections.SqlFunction("REPLACE", NHibernateUtil.String,
               Projections.Property(() => DirectionType.Name),
               Projections.Constant("L1", NHibernateUtil.String),
               Projections.SqlFunction("CONCAT", NHibernateUtil.String, Projections.Constant(" "), Projections.Property(() => Language1.Name), Projections.Constant(" ")));

            return Projections.SqlFunction("REPLACE", NHibernateUtil.String,
                language1Replace,
                Projections.Constant("L2", NHibernateUtil.String),
                Projections.SqlFunction("CONCAT", NHibernateUtil.String, Projections.Constant(" "), Projections.Property(() => Language2.Name), Projections.Constant(" ")));
        }

        protected IProjection GetTestLocationProjection()
        {
            var testLocationId = Projections.Property(() => PreferredTestLocation.Id);
            var stateId = Projections.Property(() => State.Id);

            var locationName = Projections.Conditional(Restrictions.IsNull(testLocationId), Projections.Constant("", NHibernateUtil.String), Projections.Property(() => PreferredTestLocation.Name));

            var stateName = Projections.Conditional(Restrictions.IsNull(stateId), Projections.Constant("", NHibernateUtil.String), Projections.SqlFunction("CONCAT", NHibernateUtil.String, Projections.Constant(", ", NHibernateUtil.String), Projections.Property(() => State.Name)));

            return Projections.SqlFunction("CONCAT", NHibernateUtil.String, locationName, stateName);
        }

        protected virtual IProjection GetPhoneProjection()
        {
            var phoneFunction = Projections.SqlFunction("CONCAT", NHibernateUtil.String,
                Projections.Property(() => Phone.CountryCode),
                Projections.Constant(" "),
                Projections.Property(() => Phone.AreaCode),
                Projections.Constant(" "),
                Projections.Property(() => Phone.LocalNumber));

            return Projections.Conditional(
                Restrictions.And(
                    Restrictions.Eq(Projections.Property(() => Phone.Invalid), false),
                    Restrictions.Eq(Projections.Property(() => Phone.PrimaryContact), true)
                ),
                phoneFunction,
                Projections.Constant(null, NHibernateUtil.String));
        }

        protected IProjection GetNameProjection()
        {
            return Projections.SqlFunction("CONCAT", NHibernateUtil.String,
                Projections.Property(() => PersonName.GivenName),
                Projections.Constant(" "),
                Projections.Property(() => PersonName.Surname));
        }

        protected Junction GetEmailTemplateFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<EmailTemplate>(at => EmailTemplate.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetApplicationTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialApplicationType>(at => CredentialApplicationType.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetApplicationTypeCategoryFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialApplicationTypeCategory>(at => CredentialApplicationTypeCategory.Id.IsIn(typeList));
            return junction;
        }

        protected virtual Junction GetNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();
            junction.Add<Entity>(
                e => Entity.NaatiNumber.IsIn(naatiNumberList) || SponsorEntity.NaatiNumber.IsIn(naatiNumberList));
            return junction;
        }

        protected ICriterion GetActiveApplicationRestriction()
        {
            var statuses = new List<CredentialApplicationStatusTypeName>
            {
                CredentialApplicationStatusTypeName.Draft,
                CredentialApplicationStatusTypeName.Rejected,
                CredentialApplicationStatusTypeName.Completed,
                CredentialApplicationStatusTypeName.Deleted
            };

            return Restrictions.Not(
                Restrictions.In(Projections.Property(() => CredentialApplicationStatusType.Id), statuses));
        }

        protected Junction GetActiveApplicationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var active = criteria.ToList<S, bool>().First();

            var activeApplicationRestriction = GetActiveApplicationRestriction();
            if (active)
            {
                return junction.Add(activeApplicationRestriction);
            }

            return junction.Add(Restrictions.Not(activeApplicationRestriction));
        }

        protected Junction GetAutoCreatedApplicationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var autoCreated = criteria.ToList<S, bool>().First();

            if (!autoCreated)
            {
                return junction.Add(Restrictions.Or(Restrictions.Eq(Projections.Property(() => CredentialApplication.AutoCreated), autoCreated),
                                                    Restrictions.IsNull(Projections.Property(() => CredentialApplication.AutoCreated))));
            }

            return junction.Add(Restrictions.Eq(Projections.Property(() => CredentialApplication.AutoCreated), autoCreated));
        }

        protected Junction GetAutoCreatedCredentialRequestFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var autoCreated = criteria.ToList<S, bool>().First();

            if (!autoCreated)
            {
                return junction.Add(Restrictions.Or(Restrictions.Eq(Projections.Property(() => CredentialRequest.AutoCreated), autoCreated),
                                                Restrictions.IsNull(Projections.Property(() => CredentialRequest.AutoCreated))));
            }

            return junction.Add(Restrictions.Eq(Projections.Property(() => CredentialRequest.AutoCreated), autoCreated));
        }

        protected IProjection GetCredentialStatusProjection()
        {
            var terminatedStatusRestriction = GetTerminatedCredentialRestriction();
            var expiredStatusRestriction = GetExpiredCredentialRestriction();
            var activeStatusRestriction = GetActiveCredentialRestriction();
            var futureStatusRestriction = GeFutureCredentialRestriction();

            var terminatedProjection = GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Terminated);
            var expiredProjection = GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Expired);
            var activeProjection = GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Active);
            var futureProjection = GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Future);
            var unknownProjection = GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Unknown);

            return Projections.Conditional(terminatedStatusRestriction, terminatedProjection,
                Projections.Conditional(expiredStatusRestriction, expiredProjection,
                    Projections.Conditional(activeStatusRestriction, activeProjection,
                    Projections.Conditional(futureStatusRestriction, futureProjection, unknownProjection))));
        }


        protected IProjection GetCredentialStatusTypeIdProjection(CredentialStatusTypeName status)
        {
            return Projections.Constant((int)status, NHibernateUtil.Int32);
        }

        protected IProjection AddDays(IProjection quantity, IProjection dateProjection)
        {
            return Projections.SqlFunction("ADD_DAYS", NHibernateUtil.DateTime, quantity, dateProjection); 
        }

        protected IProjection AddMinutes( IProjection quantity, IProjection dateProjection)
        {
            return Projections.SqlFunction("ADD_MINUTES", NHibernateUtil.DateTime, quantity, dateProjection);
        }

        protected IProjection StringAgg(IProjection projection, string separator)
        {
            return Projections.SqlFunction("STRING_AGG", NHibernateUtil.String, projection, Projections.Constant(separator, NHibernateUtil.AnsiString));
        }

        protected IProjection GetExpiryDateForCredential()
        {
            var certificationProperty = Projections.Property(() => CredentialType.Certification);
            var expiryDateForCertificationCredential = GetExpiryDateForCertificationCredential();
            var expiryDateForNonCertificationCredential = GetExpiryDateForNonCertificationCredential();

            var expiryDateProperty = Projections.Conditional(Restrictions.Eq(certificationProperty, true),
                expiryDateForCertificationCredential,
                expiryDateForNonCertificationCredential);

            return expiryDateProperty;
        }

        private IProjection GetExpiryDateForCertificationCredential()
        {
            var terminationDateProperty = Projections.Property(() => Credential.TerminationDate);
            var nullTerminationDateRestrition = Restrictions.IsNull(terminationDateProperty);
            var endDateProperty = Projections.Property(() => CertificationPeriod.EndDate);

            var earliestDateProjection = Projections.Conditional(
                Restrictions.GeProperty(terminationDateProperty, endDateProperty),
                endDateProperty, terminationDateProperty);

            return Projections.Conditional(nullTerminationDateRestrition, endDateProperty, earliestDateProjection);
        }

        private IProjection GetExpiryDateForNonCertificationCredential()
        {
            var terminationDateProperty = Projections.Property(() => Credential.TerminationDate);
            var expiryDateProperty = Projections.Property(() => Credential.ExpiryDate);

            var maximumDateProjection = Projections.Constant(new DateTime(3000, 1, 1), NHibernateUtil.DateTime);
            var terminationDateProjection = Projections.Conditional(Restrictions.IsNull(terminationDateProperty),
                maximumDateProjection, terminationDateProperty);
            var expiryDateProjection = Projections.Conditional(Restrictions.IsNull(expiryDateProperty),
                maximumDateProjection, expiryDateProperty);

            var earliestDateProjection = Projections.Conditional(
                Restrictions.GeProperty(terminationDateProjection, expiryDateProjection),
                expiryDateProjection, terminationDateProjection);

            return earliestDateProjection;
        }

        private ICriterion GetTerminatedCredentialRestriction()
        {
            var terminationDatePropety = Projections.Property(() => Credential.TerminationDate);

            return Restrictions.And(Restrictions.IsNotNull(terminationDatePropety),
                Restrictions.Le(terminationDatePropety, DateTime.Now));
        }

        private ICriterion GetExpiredCredentialRestriction()
        {
            var expiryDate = GetExpiryDateForCredential();
            return Restrictions.Lt(expiryDate, DateTime.Now.Date);
        }

        private ICriterion GeFutureCredentialRestriction()
        {
            var startDate = Projections.Property(() => Credential.StartDate);
            return Restrictions.Ge(startDate, DateTime.Now);
        }

        private ICriterion GetActiveCredentialRestriction()
        {
            var startDateProperty = Projections.Property(() => Credential.StartDate);
            var nullStartDate = Restrictions.IsNull(startDateProperty);

            var todayDateProjection = Projections.Constant(DateTime.Now.Date, NHibernateUtil.DateTime);

            var startDateProjection =
                Projections.Conditional(nullStartDate, todayDateProjection, startDateProperty);
            var endDateProjection = GetExpiryDateForCredential();

            var startDateLessThanToday = Restrictions.LeProperty(startDateProjection, todayDateProjection);

            var expiryDateGreaterThanToday = Restrictions.GeProperty(endDateProjection, todayDateProjection);

            return Restrictions.And(startDateLessThanToday, expiryDateGreaterThanToday);
        }

        protected IProjection GetBooleanProjectionFor(ICriterion criterion)
        {
            return Projections.Conditional(criterion,
                Projections.Constant(true, NHibernateUtil.Boolean),
                Projections.Constant(false, NHibernateUtil.Boolean));
        }

        protected IProjection GetIntValueProjectionFor(ICriterion criterion)
        {
            return Projections.Conditional(criterion,
                Projections.Constant(1, NHibernateUtil.Int32),
                Projections.Constant(0, NHibernateUtil.Int32));
        }

        protected Junction GetCredentialRequestTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialType>(at => CredentialType.Id.IsIn(typeList));
            return junction;
        }

        protected virtual Junction GetSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Skill>(c => Skill.Id.IsIn(typeList));
            return junction;
        }

        protected virtual Junction GetLanguageFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var languageList = criteria.ToList<S, int>();
            junction.Add<Language>(x => Language1.Id.IsIn(languageList) || Language2.Id.IsIn(languageList));
            return junction;
        }

        protected virtual Junction GetStateFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<State>(c => State.Id.IsIn(typeList));
            return junction;
        }

        protected IProjection GetMd5HashProjectionFor(IProjection projection)
        {
            var md5Projection = Projections.SqlFunction("HASHBYTES", NHibernateUtil.String, Projections.Constant("md5"), projection);
            return md5Projection;
        }

        protected ICriterion InList(IProjection projection,  IEnumerable<int> ids)
        {
            return new StringListExpression(projection, ids);            
        }
        protected IProjection SumProjections(params IProjection[] projections)
        {
            if (projections.Length == 1)
            {
                return projections[0];
            }

            var sumProjection = Projections.SqlFunction(new VarArgsSQLFunction("(", "+", ")"), NHibernateUtil.Int32, projections);

            return sumProjection;
        }

        protected IProjection SubtractProjections(params IProjection[] projections)
        {
            if (projections.Length == 1)
            {
                return projections[0];
            }

            var sumProjection = Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"), NHibernateUtil.Int32, projections);

            return sumProjection;
        }

        protected IProjection MultiplyProjections(params IProjection[] projections)
        {
            if (projections.Length == 1)
            {
                return projections[0];
            }

            var sumProjection = Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"), NHibernateUtil.Int32, projections);

            return sumProjection;
        }

        protected IProjection DivideProjections(params IProjection[] projections)
        {
            if (projections.Length == 1)
            {
                return projections[0];
            }

            var sumProjection = Projections.SqlFunction(new VarArgsSQLFunction("(", "/", ")"), NHibernateUtil.Int32, projections);

            return sumProjection;
        }

        protected ICriterion GetActiveCredentialFilter()
        {
            var status = GetCredentialStatusProjection();
            return Restrictions.EqProperty(status, GetCredentialStatusTypeIdProjection(CredentialStatusTypeName.Active));
        }

        protected IProjection GetDateProjectionFrom(IProjection projection)
        {
            return Projections.SqlFunction(new VarArgsSQLFunction("cast(", "", " as Date)"),
                NHibernateUtil.Date,
                projection);
        }

        protected IProjection TryGetDateProjectionFrom(IProjection projection)
        {
            return Projections.SqlFunction(new VarArgsSQLFunction("TRY_CAST(", "", " as Date)"),
                NHibernateUtil.Date,
                projection);
        }

        protected IProjection ToDateTime(IProjection projection)
        {
            return Projections.SqlFunction(new VarArgsSQLFunction("cast(", "", " as datetime)"),
                NHibernateUtil.DateTime,
                projection);
        }

        protected IProjection GetAge(IProjection dateOfBirth)
        {
            var currentDate = Projections.Constant(DateTime.Now, NHibernateUtil.DateTime);

            var currentChar = ConvertDateTimeToChar(currentDate);
            var currentDateInt = ConvertToInt(currentChar);
            var dateOfBirthChar = ConvertDateTimeToChar(dateOfBirth);
            var substraction = SubtractProjections(currentDateInt, dateOfBirthChar);
            var result = DivideProjections(substraction, Projections.Constant(10000, NHibernateUtil.Int32));
            return result;
        }


        private IProjection Convert(string type, IProjection projection, IType returnType, int? format)
        {
            var formatString = format.HasValue ? "," + format.Value : string.Empty;
            return Projections.SqlFunction(new VarArgsSQLFunction($"CONVERT({type},", "", $"{formatString})"),
                returnType,
                projection);
        }

        private IProjection ConvertDateTimeToChar(IProjection projection)
        {
            var charType = "char(8)";
            var returnType = NHibernateUtil.AnsiString;
            var format = 112;
            return Convert(charType, projection, returnType, format);
        }

        private IProjection ConvertToInt(IProjection projection)
        {
            var charType = "int";
            var returnType = NHibernateUtil.Int32;
            return Convert(charType, projection, returnType, null);
        }

        protected IProjection GetStringOrDefault(IProjection projection, string defaultValue)
        {
            return Projections.Conditional(Restrictions.IsNull(projection),
                Projections.Constant(defaultValue, NHibernateUtil.String), projection);
        }

    }
}
