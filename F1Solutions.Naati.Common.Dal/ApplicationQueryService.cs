using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Security;
using F1Solutions.Global.Common.Mapping;
using Credential = F1Solutions.Naati.Common.Dal.Domain.Credential;
using InvoiceDto = F1Solutions.Naati.Common.Contracts.Dal.DTO.InvoiceDto;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace F1Solutions.Naati.Common.Dal
{

    public class ApplicationQueryService : IApplicationQueryService
    {
        private readonly Dictionary<LookupType, Func<LookupTypeResponse>> mLookupTypeDictionary;
        private readonly IFinanceService _financeService;
        private readonly IPersonQueryService _personQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ApplicationQueryService(IFinanceService financeService, IPersonQueryService personQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _financeService = financeService;
            _personQueryService = personQueryService;
            _autoMapperHelper = autoMapperHelper;
            mLookupTypeDictionary = new Dictionary<LookupType, Func<LookupTypeResponse>>
            {
                {LookupType.CredentialApplicationType, GetDynamicLookupType<CredentialApplicationType>},
                {
                    LookupType.CredentialApplicationStatusType,
                    GetDynamicLookupTypeOrderBy<CredentialApplicationStatusType>
                },
                {LookupType.CredentialRequestStatusType, GetDynamicLookupTypeOrderBy<CredentialRequestStatusType>},
                {LookupType.CredentialCategory, GetDynamicLookupType<CredentialCategory>},
                {LookupType.CredentialApplicationTypeBackend, GetCredentialApplicaionTypesBackend},
                {LookupType.CredentialType, GetCredentialTypes},
                {LookupType.CredentialTypeTestSession, GetCredentialTypesTestSession},
                {LookupType.ApplicationOwner, GetUserLookup},
                {LookupType.Office, GetOfficeLoockup},
                {LookupType.PublicApplicationForms, () => ToLookupReponse(GetPublicApplicationForms())},
                {LookupType.PrivateApplicationForms, () => ToLookupReponse(GetPrivateApplicationForms())},
                {LookupType.ActiveApplicationForms, () => ToLookupReponse(GetActiveApplicationForms())},
                {LookupType.Languages, GetLanguages},
                {LookupType.TestLocation, GetTestLocation},
                {LookupType.Institution, GetInstition},

                {LookupType.EndorsedQualificationsInstitution, GetEndorsedQualificationsInstition},
                {LookupType.EndorsedQualifications, GetEndorsedQualifications},
                {LookupType.EndorsedQualificationsLocation, GetEndorsedQualificationsLocation},
                {LookupType.EndorsedQualificationIds, GetEndorsedQualificationIds},

                {LookupType.InstitutionById, GetInstitutionById},
                {LookupType.VenueName, GetVenue},
                {LookupType.OfficeAbbreviation, GetOfficeAbbreviationLoockup},
                {LookupType.TestStatusType, GetDynamicLookupType<TestStatusType>},
                {LookupType.ExaminerStatusType, GetDynamicLookupType<ExaminerStatusType>},
                {LookupType.TestResultType, GetTestResultTypes},
                {LookupType.IntendedTestSession, GetIntendedTestSessionLocation},
                {LookupType.PractitionerApplicationForms, () => ToLookupReponse(GetPractitionerApplicationForms())},
                {
                    LookupType.RecertificationApplicationForms,
                    () => ToLookupReponse(GetRecertificationApplicationForms())
                },
                {LookupType.Panel, GetPanelsLookup},
                {LookupType.CredentialStatusType, GetCredentialStatusTypes},
                {LookupType.LanguageGroup, GetLanguageGroup},
                {LookupType.SkillType, GetSkillType},
                {LookupType.DirectionType, GetDirectionType},
                {LookupType.OdAddressVisibilityTypeBackend, GetDynamicLookupType<OdAddressVisibilityType>},
                {LookupType.TestMaterialDomain, GetDynamicLookupType<TestMaterialDomain>},
                {LookupType.SystemActionType, GetDynamicLookupType<SystemActionType>},
                {LookupType.DocumentType, GetDynamicLookupType<DocumentType>},
                {LookupType.RolePlayerStatusType, GetDynamicLookupType<RolePlayerStatusType>},
                {LookupType.RolePlayerRoleType, GetDynamicLookupType<RolePlayerRoleType>},
                {LookupType.PersonType, GetPesonTypeLookup},
                {LookupType.State, GetStateLookup},
                {LookupType.TestMaterialStatus, GetTestmaterialStatuses},
                {LookupType.TaskType, GetTaskTypes},
                {LookupType.TestSpecification, GetTestSpecifications},
                {LookupType.EmailStatus, GetDynamicLookupType<EmailSendStatusType>},
                {LookupType.MaterialRequestRoundStatus, GetDynamicLookupType<MaterialRequestRoundStatusType>},
                {LookupType.MaterialRequestStatus, GetDynamicLookupType<MaterialRequestStatusType>},
                {LookupType.MaterialRequestRoundMembershipType, GetDynamicLookupType<MaterialRequestPanelMembershipType>},
                {LookupType.TestMaterialType, GetDynamicLookupType<TestMaterialType>},
                {LookupType.TestMaterialLinkType, GetDynamicLookupType<TestMaterialLinkType>},
                {LookupType.NaatiUser, GetUsers },
                {LookupType.MaterialRequestTaskType, GetDynamicLookupType<MaterialRequestTaskType>},
                {LookupType.MaterialRequestSpecification, ()=> GetLookup<ProductSpecification>(x=> !x.Inactive && x.ProductCategory.Id == (int)ProductCategoryTypeName.MaterialCreationFee, x=> x.Id, y=> y.Description + $" - ${y.CostPerUnit:##.00}")},
                {LookupType.MaterialSpecificationCost, ()=> GetLookup<ProductSpecification>(x=> x.ProductCategory.Id == (int)ProductCategoryTypeName.MaterialCreationFee, x=> x.Id, y=> $"{y.CostPerUnit:##.00}" )},
                {LookupType.MaterialRequestSpecificationCode, ()=> GetLookup<ProductSpecification>(x=> x.ProductCategory.Id == (int)ProductCategoryTypeName.MaterialCreationFee, x=> x.Id, y=> y.Code + " ( " + y.GLCode.Code + ")")},
                {LookupType.RefundMethodType, GetDynamicLookupType<RefundMethodType>}
            };
        }


        private LookupTypeResponse GetLookup<T>(Func<T, bool> filter, Func<T, int> idSelector, Func<T, string> displayNameSelctor) where T : EntityBase
        {

            var results = NHibernateSession.Current.Query<T>().Where(filter).ToList()
                .Select(x => new LookupTypeDetailedDto
                {
                    Id = idSelector(x),
                    DisplayName = displayNameSelctor(x)
                });

            return new LookupTypeResponse { Results = results };
        }

        private LookupTypeResponse GetUsers()
        {
            var results = NHibernateSession.Current.Query<User>().Where(x => !x.SystemUser)
                .Select(x => new LookupTypeDetailedDto
                {
                    Id = x.Id,
                    DisplayName = x.FullName
                });

            return new LookupTypeResponse { Results = results };
        }

        private LookupTypeResponse GetTestmaterialStatuses()
        {
            var values = Enum.GetValues(typeof(TestMaterialStatusTypeName))
                .OfType<TestMaterialStatusTypeName>().Where(x => x != TestMaterialStatusTypeName.UsedByApplicants)
                .Select(x =>

                    new LookupTypeDto()
                    {
                        Id = (int)x,
                        DisplayName = x.ToString().ToDisplayName()
                    }
                );

            return new LookupTypeResponse
            {
                Results = values
            };
        }
        private LookupTypeResponse GetPesonTypeLookup()
        {
            var values = Enum.GetValues(typeof(PersonType))
                .OfType<PersonType>()
                .Select(x =>

                    new LookupTypeDto()
                    {
                        Id = (int)x,
                        DisplayName = x.ToString().ToDisplayName()
                    }
                );

            return new LookupTypeResponse
            {
                Results = values
            };
        }

        private LookupTypeResponse GetStateLookup()
        {
            var excludedStates = new List<int>() { 10, 109, 117 };
            var states = NHibernateSession.Current.Query<State>()
                .Where(x => !excludedStates.Contains(x.Id))
                .Select(x => new LookupTypeDto
                {
                    Id = x.Id,
                    DisplayName = x.Id == 9 ? x.Name : x.Abbreviation
                })
                .ToList();

            return new LookupTypeResponse
            {
                Results = states
            };
        }

        private LookupTypeResponse ToLookupReponse(IEnumerable<LookupTypeDto> dtos)
        {
            return new LookupTypeResponse() { Results = dtos };
        }

        private LookupTypeResponse GetPanelsLookup()
        {
            var results = NHibernateSession.Current.Query<Panel>()
                .Select(x => new LookupTypeDetailedDto
                {
                    Id = x.Id,
                    DisplayName = x.Name
                });

            return new LookupTypeResponse { Results = results };

        }

        private LookupTypeResponse GetIntendedTestSessionLocation()
        {
            var testSession =
                NHibernateSession.Current.Query<TestSession>()
                    .Where(x => x.TestDateTime.Date >= DateTime.Now.Date && !x.Completed)
                    .OrderBy(x => x.Id)
                    .Select(x => new LookupTypeDto
                    {
                        Id = x.Id,
                        DisplayName = x.TestDateTime.ToString("dd/MM/yyyy") + " - TS" + x.Id + " - " + x.Name + " - " +
                                      x.Venue.TestLocation.Name
                    });
            return new LookupTypeResponse { Results = testSession };
        }

        private LookupTypeResponse GetTestResultTypes()
        {
            var venue = from v in NHibernateSession.Current.Query<ResultType>()
                        select new LookupTypeDto
                        {
                            Id = v.Id,
                            DisplayName = v.Result
                        };

            return new LookupTypeResponse { Results = venue.OrderBy(x => x.DisplayName) };
        }

        public LookupTypeResponse GetCredentialTypeSkills(GetCredentialTypeSkillsRequest request)
        {
            var ids = request.CredentialTypeIds.Union(new[] { 0 }).ToList();
            var queryable = from credentialType in NHibernateSession.Current.Query<CredentialType>()
                            join skill in NHibernateSession.Current.Query<Skill>()
                                on credentialType.SkillType.Id equals skill.SkillType.Id
                            where ids.Contains(credentialType.Id)
                            select new LookupTypeDto
                            {
                                Id = skill.Id,
                                DisplayName = skill.DirectionType
                                    .DisplayName.Replace("[Language 1]", skill.Language1.Name)
                                    .Replace("[Language 2]", skill.Language2.Name)
                            };

            //Todo: Improve this query
            return new LookupTypeResponse
            {
                Results = queryable.ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(y => y.DisplayName)
            };
        }

        public List<SkillEmailTokenObject> GetCredentialSkills(GetCredentialSkillsRequest request)
        {
            var ids = request.CredentialIds.Union(new[] { 0 }).ToList();
            var queryable = from credential in NHibernateSession.Current.Query<Credential>()
                            join credentialCredentialRequest in NHibernateSession.Current.Query<CredentialCredentialRequest>() on credential.Id equals credentialCredentialRequest.Credential.Id
                            join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on credentialCredentialRequest.CredentialRequest.Id equals credentialRequest.Id
                            join credentialType in NHibernateSession.Current.Query<CredentialType>() on credentialRequest.CredentialType.Id equals credentialType.Id
                            join skill in NHibernateSession.Current.Query<Skill>() on credentialRequest.Skill.Id equals skill.Id
                            where ids.Contains(credential.Id)
                            select new SkillEmailTokenObject
                            {
                                DisplayName = skill.DirectionType
                                    .DisplayName.Replace("[Language 1]", skill.Language1.Name)
                                    .Replace("[Language 2]", skill.Language2.Name),
                                CredentialId = credential.Id,
                                Externalname = credentialType.ExternalName
                            };

            //Todo: Improve this query
            //return new LookupTypeResponse
            //{
            return queryable.GroupBy(x => new { x.CredentialId, x.DisplayName, x.Externalname }).Select(x => x.First()).OrderBy(y => y.DisplayName).ToList();
            //};
        }

        public LookupTypeResponse GetCredentialTypeSkillsTestSession(GetCredentialTypeSkillsRequest request)
        {
            var ids = request.CredentialTypeIds.Union(new[] { 0 }).ToList();
            var queryable = from testSession in NHibernateSession.Current.Query<TestSession>()
                            join test in NHibernateSession.Current.Query<TestSessionSkill>() on testSession.Id equals test
                                .TestSession.Id
                            join skill in NHibernateSession.Current.Query<Skill>()
                            on test.Skill.Id equals skill.Id
                            where ids.Contains(testSession.CredentialType.Id)
                            select new LookupTypeDto
                            {
                                Id = skill.Id,
                                DisplayName = skill.DirectionType
                                    .DisplayName.Replace("[Language 1]", skill.Language1.Name)
                                    .Replace("[Language 2]", skill.Language2.Name)
                            };

            return new LookupTypeResponse
            {
                Results = queryable.ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(y => y.DisplayName)
            };
        }

        private LookupTypeResponse GetLanguages()
        {
            var languages = NHibernateSession.Current.Query<Language>()
                .ToList()
                .Select(x =>
                {
                    var groupName = x.LanguageGroup != null ? " (" + x.LanguageGroup.Name + ")" : string.Empty;
                    return new LookupTypeDto { Id = x.Id, DisplayName = $"{x.Name}{groupName}" };
                })
                .ToList();

            return new LookupTypeResponse { Results = languages };
        }

        private LookupTypeResponse GetTestLocation()
        {
            var testLocation = from test in NHibernateSession.Current.Query<TestLocation>()
                               join office in NHibernateSession.Current.Query<Office>() on test.OfficeId equals office.Id
                               join institution in NHibernateSession.Current
                                   .Query<Institution>() on office.Institution.Id equals institution.Id
                               select new LookupTypeDto
                               {
                                   Id = test.Id,
                                   DisplayName = test.Name + ", " + institution.LatestInstitutionName.InstitutionName.Abbreviation
                               };
            return new LookupTypeResponse { Results = testLocation.OrderBy(x => x.DisplayName) };
        }

        private LookupTypeResponse GetInstition()
        {
            var unOrderInstitutions = NHibernateSession.Current.Query<LatestInstitutionName>()
                .Select(x => new
                {
                    Id = x.Institution.Entity.NaatiNumber,
                    Name = x.Institution.InstitutionName
                })
                .ToList()
                .OrderBy(x => x.Name);

            var institutions = unOrderInstitutions.Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = $"{x.Id} - {x.Name}"
            });

            return new LookupTypeResponse { Results = institutions };
        }

        private LookupTypeResponse GetEndorsedQualificationsInstition()
        {
            var query = from latestInstitutionName in NHibernateSession.Current.Query<LatestInstitutionName>()
                        join endorsedQualification in NHibernateSession.Current.Query<EndorsedQualification>() on latestInstitutionName.Institution.Id equals endorsedQualification.Institution.Id
                        select new { Id = latestInstitutionName.Institution.Entity.NaatiNumber, Name = latestInstitutionName.Institution.InstitutionName };

            var unOrderInstitutions = query.ToList().Distinct().OrderBy(x => x.Name);

            var institutions = unOrderInstitutions.Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = $"{x.Id} - {x.Name}"
            });

            return new LookupTypeResponse { Results = institutions };
        }

        private LookupTypeResponse GetEndorsedQualifications()
        {
            char tab = '\u0009';
            var rnd = new Random();

            var query = NHibernateSession.Current.Query<EndorsedQualification>()
                .Select(x => new
                {
                    Name = x.Qualification.Trim().Replace(tab.ToString(), ""),
                    DisplayName = x.Qualification.Trim().Replace(tab.ToString(), "")
                })
                .Distinct()
                .ToList()
                .OrderBy(x => x.Name);

            var qualifications = query.Select(x => new LookupTypeDto
            {
                Id = rnd.Next(),
                Name = x.Name,
                DisplayName = x.Name
            });

            return new LookupTypeResponse { Results = qualifications };
        }

        private LookupTypeResponse GetEndorsedQualificationsLocation()
        {
            char tab = '\u0009';
            var rnd = new Random();

            var query = NHibernateSession.Current.Query<EndorsedQualification>()
                .Select(x => new
                {
                    Name = x.Location.Trim().Replace(tab.ToString(), ""),
                    DisplayName = x.Location.Trim().Replace(tab.ToString(), "")
                })
                .Distinct()
                .ToList()
                .OrderBy(x => x.Name);

            var locations = query.Select(x => new LookupTypeDto
            {
                Id = rnd.Next(),
                Name = x.Name,
                DisplayName = x.Name
            });

            return new LookupTypeResponse { Results = locations };
        }

        private LookupTypeResponse GetEndorsedQualificationIds()
        {
            var query = NHibernateSession.Current.Query<EndorsedQualification>()
                .Select(x => new
                {
                    Id = x.Id,
                    DisplayName = x.Id.ToString()
                })
                .ToList()
                .OrderBy(x => x.Id);

            var result = query.Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            });

            return new LookupTypeResponse { Results = result };
        }

        private LookupTypeResponse GetInstitutionById()
        {
            var unOrderInstitutions = NHibernateSession.Current.Query<LatestInstitutionName>()
                .Select(x => new
                {
                    Id = x.Institution.Id,
                    Name = x.Institution.InstitutionName,
                    NaatiNumber = x.Institution.Entity.NaatiNumber
                })
                .ToList()
                .OrderBy(x => x.Name);

            var institutions = unOrderInstitutions.Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = $"{x.NaatiNumber} - {x.Name}"
            });

            return new LookupTypeResponse { Results = institutions };
        }

        private LookupTypeResponse GetVenue()
        {
            var venue = from v in NHibernateSession.Current.Query<Venue>()
                        select new LookupTypeDto
                        {
                            Id = v.Id,
                            DisplayName = v.Name
                        };

            return new LookupTypeResponse { Results = venue.OrderBy(x => x.DisplayName) };
        }

        private LookupTypeResponse GetTaskTypes()
        {
            var taskTypes = from v in NHibernateSession.Current.Query<TestComponentType>()
                            select new
                            {
                                Id = v.Id,
                                DisplayName = v.Name,
                                CredentialType = v.TestSpecification.CredentialType.InternalName,
                                v.TestSpecification.Active
                            };

            return new LookupTypeResponse
            {
                Results = taskTypes.ToList().Select(t =>
                {
                    var unavailableText = !t.Active ? " - Unavailable" : String.Empty;
                    var dispplayName = $"{t.DisplayName} ({t.CredentialType}{unavailableText})";

                    return new LookupTypeDto()
                    {
                        Id = t.Id,
                        DisplayName = dispplayName
                    };
                }).OrderBy(x => x.DisplayName)
            };
        }
        private LookupTypeResponse GetTestSpecifications()
        {
            var testSpecifications = from v in NHibernateSession.Current.Query<TestSpecification>()
                                     select new
                                     {
                                         Id = v.Id,
                                         DisplayName = v.Description,
                                         v.Active
                                     };
            var result = testSpecifications.ToList().Select(x => new LookupTypeDto { Id = x.Id, DisplayName = x.Active ? x.DisplayName : $"{x.DisplayName} (Inactive)" });
            return new LookupTypeResponse { Results = result.OrderBy(x => x.DisplayName) };
        }

        private IEnumerable<FormLookupTypeDto> GetPublicApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => f.CredentialApplicationFormUserType.Id ==
                            (int)CredentialApplicationFormUserTypeName.AnonymousUser && !f.Inactive)
                .Select(r => new FormLookupTypeDto()
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }

        private IEnumerable<FormLookupTypeDto> GetActiveApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => !f.Inactive)
                .Select(r => new FormLookupTypeDto()
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }

        private IEnumerable<FormLookupTypeDto> GetPrivateApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => f.CredentialApplicationFormUserType.Id ==
                            (int)CredentialApplicationFormUserTypeName.LoggedInUser && !f.Inactive)
                .Select(r => new FormLookupTypeDto
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }

        private IEnumerable<FormLookupTypeDto> GetPractitionerApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => f.CredentialApplicationFormUserType.Id ==
                            (int)CredentialApplicationFormUserTypeName.PractitionerUser && !f.Inactive)
                .Select(r => new FormLookupTypeDto
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }

        private IEnumerable<FormLookupTypeDto> GetNonPractitionerApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => f.CredentialApplicationFormUserType.Id ==
                            (int)CredentialApplicationFormUserTypeName.NonPractitionerUser && !f.Inactive)
                .Select(r => new FormLookupTypeDto
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }

        private IEnumerable<FormLookupTypeDto> GetRecertificationApplicationForms()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationForm>()
                .Where(f => f.CredentialApplicationFormUserType.Id ==
                            (int)CredentialApplicationFormUserTypeName.RecertificationUser && !f.Inactive)
                .Select(r => new FormLookupTypeDto
                {
                    Id = r.Id,
                    DisplayName = r.Name,
                    Url = r.Url
                });
            return result.ToList();
        }


        private LookupTypeResponse GetOfficeLoockup()
        {
            var result = NHibernateSession.Current.Query<Office>()
                .Select(o => new LookupTypeDto
                {
                    Id = o.Id,
                    DisplayName = o.Institution.InstitutionName
                })
                .OrderBy(l => l.DisplayName);

            return new LookupTypeResponse { Results = result };
        }

        private LookupTypeResponse GetOfficeAbbreviationLoockup()
        {
            var result = NHibernateSession.Current.Query<Office>()
                .Select(o => new LookupTypeDto
                {
                    Id = o.Id,
                    DisplayName = o.Institution.InstitutionAbberviation
                })
                .OrderBy(l => l.DisplayName);

            return new LookupTypeResponse { Results = result };
        }

        public bool CheckDuplicatedApplication(int naatiNumber, int credentialApplicationTypeId)
        {
            var person = NHibernateSession.Current.Query<Person>()
                .SingleOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            var query =
                NHibernateSession.Current.QueryOver<CredentialApplication>()
                    .Where(
                        x =>
                            x.Person.Id == person.Id && x.CredentialApplicationType.Id == credentialApplicationTypeId &&
                            x.CredentialApplicationStatusType.Id !=
                            (int)CredentialApplicationStatusTypeName.Rejected &&
                            x.CredentialApplicationStatusType.Id !=
                            (int)CredentialApplicationStatusTypeName.Completed &&
                            x.CredentialApplicationStatusType.Id != (int)CredentialApplicationStatusTypeName.Deleted);

            return query.RowCount() > 0;
        }

        public GetCredentialApplicationFormResponse GetCredentialApplicationForm(int applicationFormId)
        {
            var appliationForm = NHibernateSession.Current.Get<CredentialApplicationForm>(applicationFormId);
            var applicationFormDto = _autoMapperHelper.Mapper.Map<CredentialApplicationFormDto>(appliationForm);
            return new GetCredentialApplicationFormResponse { Result = applicationFormDto };
        }

        public string GetInvoiceNumberByApplicationId(int applicationId)
        {
            var credentialWorkflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .FirstOrDefault(x => x.CredentialApplication.Id == applicationId);
            return credentialWorkflowFee != null ? credentialWorkflowFee.InvoiceNumber : string.Empty;
        }

        public GetCredentialApplicationFormSectionsResponse GetCredentialApplicationFormSections(int applicationFormId)
        {
            var sections = NHibernateSession.Current.Query<CredentialApplicationFormSection>()
                .Where(s => s.CredentialApplicationForm.Id == applicationFormId);

            var sectionsDtos =
                sections.ToList().Select(_autoMapperHelper.Mapper.Map<CredentialApplicationFormSectionDto>);

            return new GetCredentialApplicationFormSectionsResponse { Results = sectionsDtos };
        }

        public CredentialApplicationFormQuestionDto GetCredentialApplicationFormQuestion(int questionId)
        {
            var question = NHibernateSession.Current.Get<CredentialApplicationFormQuestion>(questionId);

            var result = _autoMapperHelper.Mapper.Map<CredentialApplicationFormQuestionDto>(question);
            return result;
        }

        public ApplicationSearchResultResponse SearchApplication(GetApplicationSearchRequest request)
        {
            var queryHelper = new ApplicationQueryHelper();
            return new ApplicationSearchResultResponse { Results = queryHelper.SearchApplications(request) };
        }

        public ApplicationsWithCredentialRequestsResponse GetApplicationsWithCredentialRequests(
            GetApplicationSearchRequest request)
        {
            var queryHelper = new ApplicationQueryHelper();
            var applications = queryHelper.SearchApplications(request);
            var credentialRequests = new List<System.Tuple<int, CredentialRequestDto>>();
            applications.ForEach(x =>
            {
                credentialRequests = credentialRequests
                    .Concat(GetCredentialRequests(x.Id, Enumerable.Empty<CredentialRequestStatusTypeName>())
                        .Results.Select(b => new System.Tuple<int, CredentialRequestDto>(x.Id, b)))
                    .ToList();
            });

            return new ApplicationsWithCredentialRequestsResponse
            {
                ApplicationResults = applications,
                CredentialRequestResults =
                    credentialRequests.Select(
                        x => new System.Tuple<ApplicationSearchDto, CredentialRequestDto>(
                            applications.First(b => b.Id == x.Item1), x.Item2))
            };
        }

        public ApplicationSearchResultResponse GetApplicationsForCredential(int credentialId)
        {
            var credentialApplications = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .Where(x => x.Credential.Id == credentialId)
                .Select(x =>
                    new ApplicationSearchDto()
                    {
                        Id = x.CredentialRequest.CredentialApplication.Id,
                        ApplicationReference = x.CredentialRequest.CredentialApplication.Reference,
                        ApplicationType = x.CredentialRequest.CredentialApplication.CredentialApplicationType
                            .DisplayName,
                        EnteredDate = x.CredentialRequest.CredentialApplication.EnteredDate
                    });

            return new ApplicationSearchResultResponse { Results = credentialApplications };
        }

        public CredentialRequestsResponse GetCredentialRequests(int credentialApplicationId, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id == credentialApplicationId);

            var excludedTypesIds = excludedStatuses.Select(t => Convert.ToInt32(t)).ToList();
            if (excludedTypesIds.Any())
            {
                credentialRequests =
                    credentialRequests.Where(x => !excludedTypesIds.Contains(x.CredentialRequestStatusType.Id));
            }

            var credentialRequestFields = credentialRequests.FirstOrDefault()
                                              ?
                                              .CredentialApplication.CredentialApplicationType
                                              .CredentialApplicationFields.Where(f => f.PerCredentialRequest)
                                              .ToList() ??
                                          new List<CredentialApplicationField>();

            var credentialRequestIds = credentialRequests.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList();
            var queyrHelper = new ApplicationQueryHelper();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(credentialRequestIds);
            var credentialRequestAssociations = NHibernateSession.Current.Query<CredentialRequestCredentialRequest>()
                .Where(x => credentialRequestIds.Contains(x.AssociatedCredentialRequest.Id))
                .ToLookup(x => x.AssociatedCredentialRequest.Id);

            var credentialRequestDtos = new List<CredentialRequestDto>();
            foreach (var credentialRequest in credentialRequests)
            {
                var credentialRequestDto = CredentialRequestToDto(credentialRequest, credentialRequestFields,
                    credentialStatuses, credentialRequestAssociations);
                credentialRequestDtos.Add(credentialRequestDto);
            }

            return new CredentialRequestsResponse { Results = credentialRequestDtos };
        }

        public CredentialRequestBasicResponse GetBasicCredentialRequestsByNaatiNumber(int naatiNumber, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>().Where(x => x.CredentialApplication.Person.Entity.NaatiNumber == naatiNumber);

            var excludedTypesIds = excludedStatuses.Select(t => Convert.ToInt32(t)).ToList();
            if (excludedTypesIds.Any())
            {
                credentialRequests = credentialRequests.Where(x => !excludedTypesIds.Contains(x.CredentialRequestStatusType.Id));
            }

            var queyrHelper = new ApplicationQueryHelper();
            var credentialRequestIds = credentialRequests.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(credentialRequestIds);
            var credentialTestDtos = new List<CredentialRequestBasicDto>();
            foreach (var credentialRequest in credentialRequests)
            {
                var credentialTestDto = CredentialTestMapToDto(credentialRequest, credentialStatuses);
                credentialTestDtos.Add(credentialTestDto);
            }

            return new CredentialRequestBasicResponse { Results = credentialTestDtos };
        }

        public CredentialRequestBasicResponse GetBasicCredentialRequestsByApplicationId(int credentialApplicationId, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>().Where(x => x.CredentialApplication.Id == credentialApplicationId);

            var excludedTypesIds = excludedStatuses.Select(t => Convert.ToInt32(t)).ToList();
            if (excludedTypesIds.Any())
            {
                credentialRequests = credentialRequests.Where(x => !excludedTypesIds.Contains(x.CredentialRequestStatusType.Id));
            }

            var queyrHelper = new ApplicationQueryHelper();
            var credentialRequestIds = credentialRequests.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(credentialRequestIds);
            var credentialTestDtos = new List<CredentialRequestBasicDto>();
            foreach (var credentialRequest in credentialRequests)
            {
                var credentialTestDto = CredentialTestMapToDto(credentialRequest, credentialStatuses);
                credentialTestDtos.Add(credentialTestDto);
            }

            return new CredentialRequestBasicResponse { Results = credentialTestDtos };
        }

        public bool HasTestFee(int credentialApplicationTypeId, int credentialTypeId)
        {
            var feeProduct = NHibernateSession.Current.Query<CredentialFeeProduct>()
                .FirstOrDefault(x => x.CredentialApplicationType.Id == credentialApplicationTypeId &&
                                     x.FeeType.Id == (int)FeeTypeName.Test && x.CredentialType == null
                );

            if (feeProduct == null)
            {
                var maxFee = NHibernateSession.Current.Query<CredentialFeeProduct>()
                    .Where(x => x.CredentialApplicationType.Id == credentialApplicationTypeId &&
                                x.CredentialType.Id == credentialTypeId && x.FeeType.Id == (int)FeeTypeName.Test)
                    .OrderByDescending(y => y.ProductSpecification.CostPerUnit)
                    .ToList()
                    .FirstOrDefault();

                var hasTestFee = maxFee != null;

                return hasTestFee;
            }

            return true;
        }

        public bool HasTest(int credentialApplicationTypeId, int credentialTypeId)
        {
            var credentialApplicationTypeCredentialType = NHibernateSession.Current
                .Query<CredentialApplicationTypeCredentialType>()
                .Where(x => x.CredentialApplicationType.Id == credentialApplicationTypeId)
                .FirstOrDefault(x => x.CredentialType.Id == credentialTypeId);

            var hasTest = credentialApplicationTypeCredentialType != null &&
                          credentialApplicationTypeCredentialType.HasTest;

            return hasTest;
        }

        public AvailableTestSessionsResponse GetAllAvailableTestSessionsAndRejectableTestSession(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId);
            var credentialType = credentialRequest.CredentialType;

            // get all available test sessions for the credential request
            var helper = new TestSessionAvailabiltyQueryHelper();
            var availableTestSessions = helper.GetAllAvailableTestSessions(credentialRequest.Id);

            // get the allocated test session for the credential request if one exists
            var allocatedTestSession = NHibernateSession.Current.Query<TestSitting>()
                .FirstOrDefault(x => x.CredentialRequest.Id == credentialRequestId && !x.Rejected && x.Supplementary == credentialRequest.Supplementary)?.TestSession;

            // if an allocated test session does not exist then just return the available test sessions
            if (allocatedTestSession == null)
            {
                return new AvailableTestSessionsResponse()
                {
                    AvailableTestSessions = availableTestSessions
                };
            }

            // if allocated test session exists, check if it is rejectable or not
            AvailableTestSessionDto allocatedTestSessionDto = null;

            var minTestSessionRejectableDateTime = DateTime.Now.AddHours(credentialRequest.CredentialType.TestSessionBookingRejectHours);
            // if not rejectable then only return the available test sessions (this case should never happen)
            if (allocatedTestSession.TestDateTime <= minTestSessionRejectableDateTime)
            {
                return new AvailableTestSessionsResponse()
                {
                    AvailableTestSessions = availableTestSessions
                };
            }

            allocatedTestSessionDto = new AvailableTestSessionDto
            {
                TestSessionId = allocatedTestSession.Id,
                TestDateTime = allocatedTestSession.TestDateTime,
                Name = allocatedTestSession.Name,
                TestSessionDuration = allocatedTestSession.Duration ?? 0,
                TestLocation = $"{allocatedTestSession.Venue.TestLocation.Office.State.Abbreviation} - {allocatedTestSession.Venue?.TestLocation?.Name}",
                VenueName = allocatedTestSession.Venue.Name,
                VenueAddress = allocatedTestSession.Venue.Address,
                BookingRejectHours = allocatedTestSession.CredentialType.TestSessionBookingRejectHours,
                Selected = true
            };

            // if the allocated test date is before the booking closed date then only return the allocated test session
            // this is for when the credential type reject hours is less than the booking closed weeks in the credential type table
            var bookingClosedDate = DateTime.Now.AddDays(credentialType.TestSessionBookingClosedWeeks * 7);
            if (allocatedTestSession.TestDateTime <= bookingClosedDate)
            {
                return new AvailableTestSessionsResponse()
                {
                    AllocatedTestSession = allocatedTestSessionDto,
                    //putting this back in. Booking closed only means the date that it's too late to book for.
                    AvailableTestSessions = availableTestSessions
                };
            }

            return new AvailableTestSessionsResponse()
            {
                AvailableTestSessions = availableTestSessions,
                AllocatedTestSession = allocatedTestSessionDto
            };
        }

        public CredentialTypeUpgradePathResponse GetValidCredentialTypeUpgradePaths(
            GetUpgradeCredentialPathRequest request)
        {
            var list = request.CredentialTypesIdsFrom.Union(new[] { 0 }).ToList();
            var results = NHibernateSession.Current.Query<CredentialTypeUpgradePath>()
                .Where(x => list.Contains(x.CredentialTypeFrom.Id));
            var dtos = results.Select(x => new CredentialTypeUpgradePathDto
            {
                Id = x.Id,
                CredentialTypeFromId = x.CredentialTypeFrom.Id,
                CredentialTypeToId = x.CredentialTypeTo.Id,
                MatchDirection = x.MatchDirection
            });

            return new CredentialTypeUpgradePathResponse
            {
                Results = dtos
            };
        }

        public CredentialLookupTypeResponse GetCredentialApplicationFormCredentialTypes(int credentailApplicationFormId)
        {

            var result = NHibernateSession.Current.Query<CredentialApplicationFormCredentialType>()
                .Where(x => x.CredentialApplicationForm.Id == credentailApplicationFormId)
                .Select(y => new CredentialLookupTypeDto()
                {
                    Id = y.CredentialType.Id,
                    DisplayName = y.CredentialType.ExternalName,
                    CategoryId = y.CredentialType.CredentialCategory.Id
                })
                .ToList();

            return new CredentialLookupTypeResponse
            {
                Results = result
            };
        }

        public CredentialTypeResponse GetCredentialTypeByTestSittingId(int testSittingId)
        {
            var testSitting = NHibernateSession.Current.Get<TestSitting>(testSittingId);
            var credentialRequest = testSitting.CredentialRequest;
            var response = new CredentialTypeResponse
            {
                CredentialTypeId = credentialRequest.CredentialType.Id,
                SkillId = credentialRequest.Skill.Id
            };

            return response;
        }

        public CredentialTypeResponse GetCredentialTypeById(int credentialTypeId)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(credentialTypeId);
            var response = new CredentialTypeResponse
            {
                CredentialTypeId = credentialType.Id,
                Certification = credentialType.Certification,
                InternalName = credentialType.InternalName,
                DisplayName = credentialType.DisplayName
            };

            return response;
        }


        public int GetSkillFromLanguagesAndCredentialApplicationType(int? direction, string l1, string l2, int applicationTypeId, int? credentialTypeId = null)
        {
            if (l1 == "Ethics")
            {
                var query1 =  from skill in NHibernateSession.Current.Query<Skill>()
                         join language1 in NHibernateSession.Current.Query<Language>() on skill.Language1 equals language1
                         join language2 in NHibernateSession.Current.Query<Language>() on skill.Language2 equals language2
                         join skillType in NHibernateSession.Current.Query<SkillType>() on skill.SkillType.Id equals skillType.Id
                         join skillApplicationType in NHibernateSession.Current.Query<SkillApplicationType>() on skill equals skillApplicationType.Skill
                         where //(skillApplicationType.CredentialApplicationType.Id == applicationTypeId) &&
                         (skillType.Name == "Ethics") &&
                         (language1.Name == l1) &&
                         (language2.Name == l2)
                             select skill.Id;
                return query1.FirstOrDefault();
            }
            var skillTypeId = (from credentialType in NHibernateSession.Current.Query<CredentialType>()
                               where credentialType.Id == credentialTypeId
                               select credentialType.SkillType.Id).FirstOrDefault();

            //if(!skillTypeId.HasValue)//non credential
            //{
            //    var query2 = from skill in NHibernateSession.Current.Query<Skill>()
            //                join language1 in NHibernateSession.Current.Query<Language>() on skill.Language1 equals language1
            //                join language2 in NHibernateSession.Current.Query<Language>() on skill.Language2 equals language2
            //                join skillType in NHibernateSession.Current.Query<SkillType>() on skill.SkillType.Id equals skillType.Id
            //                join skillApplicationType in NHibernateSession.Current.Query<SkillApplicationType>() on skill equals skillApplicationType.Skill
            //                where (skillApplicationType.CredentialApplicationType.Id == applicationTypeId) &&
            //                (skillType.Id == credentialTypeId) &&
            //                (language1.Name == l1) &&
            //                (language2.Name == l2)
            //                select skill.Id;
            //    return query2.FirstOrDefault();
            //}

            //credential
            var query3 = from skill in NHibernateSession.Current.Query<Skill>()
                        join language1 in NHibernateSession.Current.Query<Language>() on skill.Language1 equals language1
                        join language2 in NHibernateSession.Current.Query<Language>() on skill.Language2 equals language2
                        join skillType in NHibernateSession.Current.Query<SkillType>() on skill.SkillType.Id equals skillType.Id
                        join skillApplicationType in NHibernateSession.Current.Query<SkillApplicationType>() on skill equals skillApplicationType.Skill
                        where (skillApplicationType.CredentialApplicationType.Id == applicationTypeId) &&
                        (skillType.Id == skillTypeId) &&
                            (language1.Name == l1) &&
                            (language2.Name == l2) &&
                        (skill.DirectionType.Id == direction)
                        select skill.Id;
            return query3.FirstOrDefault();

        }

        public GetCredentialRequestsCountResponse GetCredentialRequestsCount(int credentialApplicationId, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id == credentialApplicationId);

            var excludedTypesIds = excludedStatuses.Select(t => Convert.ToInt32(t)).ToList();
            if (excludedTypesIds.Any())
            {
                credentialRequests = credentialRequests.Where(x => !excludedTypesIds.Contains(x.CredentialRequestStatusType.Id));
            }

            var credentialRequestCounnt = credentialRequests.Count();

            return new GetCredentialRequestsCountResponse { CredentialRequestsCount = credentialRequestCounnt };
        }


        public bool GetCheckCredentialRequestBelongsToCurrentUser(int credentialApplicationId, IEnumerable<CredentialRequestStatusTypeName> excludedStatuses, int checkCredentialRequestId)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id == credentialApplicationId);

            var excludedTypesIds = excludedStatuses.Select(t => Convert.ToInt32(t)).ToList();
            if (excludedTypesIds.Any())
            {
                credentialRequests = credentialRequests.Where(x => !excludedTypesIds.Contains(x.CredentialRequestStatusType.Id));
            }

            var credentialRequest = credentialRequests.Where(x => x.Id == checkCredentialRequestId).ToList();

            if (credentialRequest.Any())
            {
                return true;
            }

            return false;
        }

        public GetCredentialsCountResponse GetCredentialsCount(int credentialApplicationId)
        {
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>().Where(x => x.CredentialApplication.Id == credentialApplicationId).ToList();
            var credentialRequestCounnt = credentialRequests.Where(x => x.Credentials.Count > 0).ToList().Count;
            return new GetCredentialsCountResponse { CredentialsCount = credentialRequestCounnt };
        }


        public int GetNaatiNumberByApplicationId(int applicationId)
        {
            var credentialApplicaiton = NHibernateSession.Current.Get<CredentialApplication>(applicationId);
            var naatiNumber = credentialApplicaiton.Person.Entity.NaatiNumber;
            return naatiNumber;
        }

        public CredentialRequestsResponse GetOtherCredentialRequests(int credentialApplicationId)
        {
            var application = NHibernateSession.Current.Load<CredentialApplication>(credentialApplicationId);
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id != credentialApplicationId)
                .Where(x => x.CredentialApplication.CredentialApplicationStatusType.Id != Convert.ToInt32(CredentialApplicationStatusTypeName.Deleted))
                .Where(x => x.CredentialApplication.Person.Id == application.Person.Id)
                .Where(x => x.CredentialRequestStatusType.Id != Convert.ToInt32(CredentialRequestStatusTypeName.Deleted))
                .OrderByDescending(x => x.StatusChangeDate)
                .Take(5).ToList();

            var credentialRequestIds = credentialRequests.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList();
            var queyrHelper = new ApplicationQueryHelper();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(credentialRequestIds);
            var credentialRequestAssociations = NHibernateSession.Current.Query<CredentialRequestCredentialRequest>()
                .Where(x => credentialRequestIds.Contains(x.AssociatedCredentialRequest.Id)).ToLookup(x => x.AssociatedCredentialRequest.Id);
            var credentialRequestDtos = new List<CredentialRequestDto>();
            foreach (var credentialRequest in credentialRequests)
            {
                var credentialRequestFields = credentialRequest.CredentialApplication.CredentialApplicationType
                                                  .CredentialApplicationFields
                                                  .Where(f => f.PerCredentialRequest).ToList() ??
                                              new List<CredentialApplicationField>();

                var credentialRequestDto = CredentialRequestToDto(credentialRequest, credentialRequestFields, credentialStatuses, credentialRequestAssociations);
                credentialRequestDtos.Add(credentialRequestDto);
            }

            return new CredentialRequestsResponse { Results = credentialRequestDtos };
        }

        public CredentialRequestResponse GetCredentialRequest(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Load<CredentialRequest>(credentialRequestId);

            var credentialRequestFields = credentialRequest?.CredentialApplication.CredentialApplicationType
                                              .CredentialApplicationFields
                                              .Where(f => f.PerCredentialRequest).ToList() ??
                                          new List<CredentialApplicationField>();
            var credentialRequestAssociations = NHibernateSession.Current.Query<CredentialRequestCredentialRequest>()
                .Where(x => x.AssociatedCredentialRequest.Id == credentialRequestId).ToLookup(x => x.AssociatedCredentialRequest.Id);
            var queyrHelper = new ApplicationQueryHelper();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(new List<int> { credentialRequest?.Id ?? 0 });
            var credentialRequestDto = CredentialRequestToDto(credentialRequest, credentialRequestFields, credentialStatuses, credentialRequestAssociations);

            return new CredentialRequestResponse { CredentialRequest = credentialRequestDto };
        }

        public CredentialWorkFlowFeesCredentialRequestResponse GetPaidWorkflowFeesForCredentialRequest(
            int credentialRequestId)
        {
            var workflowFeeData = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .Where(x => x.CredentialRequest.Id == credentialRequestId && x.PaymentActionProcessedDate != null)
                .Select(
                    x =>
                        new CalculateRefundInputData
                        {
                            CredentialWorkFlowFeeId = x.Id,
                            PaymentActionProcessedDate = x.PaymentActionProcessedDate,
                            CredentialTypeId = x.CredentialRequest.CredentialType.Id,
                            PaymentReference = x.PaymentReference,
                            OrderNumber = x.OrderNumber,
                            TransactionId = x.TransactionId,
                            InvoiceNumber = x.InvoiceNumber,
                            CredentialRequestStatusTypeId = x.CredentialRequest.CredentialRequestStatusType.Id,
                            CredentialRequestId = x.CredentialRequest.Id,
                            Policy = x.CredentialApplicationRefundPolicy == null ? null : new CredentialApplicationRefundPolicyData()
                            {
                                Id = x.CredentialApplicationRefundPolicy.Id,
                                Description = x.CredentialApplicationRefundPolicy.Description,
                                Name = x.CredentialApplicationRefundPolicy.Name,
                                RefundPolicyParameters = x.CredentialApplicationRefundPolicy.RefundPolicyParameters.Select(parameter => new Contracts.Dal.RefundPolicyParameterData
                                {
                                    Name = parameter.Name,
                                    Value = parameter.Value
                                }).ToList()
                            }
                        }).ToList()
                .OrderByDescending(y => y.PaymentActionProcessedDate).FirstOrDefault();

            var result = new CredentialWorkFlowFeesCredentialRequestResponse();

            if (workflowFeeData == null)
            {
                return result;
            }

            result.Data = workflowFeeData;

            return result;
        }

        public GenericResponse<DateTime?> GetPaypalPaymentProcessedDate(
            int credentialRequestId)
        {
            var credentialWorkflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .Where(x => x.CredentialRequest != null && x.CredentialRequest.Id == credentialRequestId)
                .Select(x => x)
                .ToList()
                .FirstOrDefault();

            if (credentialWorkflowFee.IsNull())
            {
                return new GenericResponse<DateTime?>(null)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not find credential workflow fee for credential request {credentialRequestId}." }
                };
            }

            var externalAccountingOperation = NHibernateSession.Current.Query<ExternalAccountingOperation>()
                .Where(x => x.ProcessedDateTime != null && x.CompletionInput.Contains(credentialWorkflowFee.Id.ToString()) && x.Reference.Contains("PAYPAL"))
                .Select(x => x)
                .ToList()
                .FirstOrDefault();

            if (externalAccountingOperation.IsNull())
            {
                return new GenericResponse<DateTime?>(null)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not find external accounting operation for credential workflow fee {credentialWorkflowFee.Id}." }
                };
            }

            var payPalPaymentDate = externalAccountingOperation.ProcessedDateTime;

            if (payPalPaymentDate == null)
            {
                return new GenericResponse<DateTime?>(null)
                {
                    Success = false,
                    Errors = new List<string>() { $"Processed date time was null for external accounting operation {externalAccountingOperation.Id}." }
                };
            }

            return payPalPaymentDate;
        }

        public CredentialRequestResponse GetCredentialRequestForUser(int naatiNumber, int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>()
                .First(cr => cr.Id == credentialRequestId &&
                             cr.CredentialApplication.Person.Entity.NaatiNumber == naatiNumber);

            var credentialRequestFields = credentialRequest?.CredentialApplication.CredentialApplicationType
                                              .CredentialApplicationFields
                                              .Where(f => f.PerCredentialRequest).ToList() ??
                                          new List<CredentialApplicationField>();
            var credentialRequestAssociations = NHibernateSession.Current.Query<CredentialRequestCredentialRequest>()
                .Where(x => x.AssociatedCredentialRequest.Id == credentialRequestId).ToLookup(x => x.AssociatedCredentialRequest.Id);
            var queyrHelper = new ApplicationQueryHelper();
            var credentialStatuses = queyrHelper.GetCredentialStatuses(new List<int> { credentialRequest?.Id ?? 0 });
            var credentialRequestDto = CredentialRequestToDto(credentialRequest, credentialRequestFields, credentialStatuses, credentialRequestAssociations);

            return new CredentialRequestResponse { CredentialRequest = credentialRequestDto };
        }

        public void UpdateCredential(CredentialDto credentialDto)
        {
            var credential = NHibernateSession.Current.Load<Credential>(credentialDto.Id);
            credential.ShowInOnlineDirectory = credentialDto.ShowInOnlineDirectory;
            NHibernateSession.Current.Save(credential);
            NHibernateSession.Current.Flush();
        }

        public bool IsValidCredentialByNaatiNumber(int credentialId, int naatiNumber)
        {
            var credential = NHibernateSession.Current.Get<Credential>(credentialId);
            if (credential == null) return false;

            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == naatiNumber);
            if (person != null && credential.CertificationPeriod.Person.Id == person.Id)
            {
                return true;
            }

            return false;
        }

        public CredentialsResponse GetCredential(GetCredentialRequest request)
        {
            CredentialsResponse response = new CredentialsResponse();

            var credentialCredentialRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .FirstOrDefault(x => x.CredentialRequest.Id == request.CredentialRequestId);

            if (credentialCredentialRequest == null)
            {
                return response;
            }

            var credential = credentialCredentialRequest.Credential;
            var credentialType = credentialCredentialRequest.CredentialRequest.CredentialType;
            var certificationPeriod = credential.CertificationPeriod;
            var queryHelper = new ApplicationQueryHelper();
            var status = queryHelper.GetCredentialStatuses(new List<int> { request.CredentialRequestId }).Values.First();

            response.Credential = new CredentialDto
            {
                Id = credential.Id,
                StartDate = credential.StartDate,
                ExpiryDate = credential.ExpiryDate,
                TerminationDate = credential.TerminationDate,
                ShowInOnlineDirectory = credential.ShowInOnlineDirectory,
                CredentialTypeInternalName = credentialType.InternalName,
                Status = status.ToString(),
                StatusId = (int)status,
                StoredFileIds = credential.CredentialAttachments.Select(x => x.StoredFile.Id).ToList()
            };

            if (certificationPeriod != null)
            {
                response.Credential.CertificationPeriod = new CertificationPeriodDto
                {
                    Id = certificationPeriod.Id,
                    StartDate = certificationPeriod.StartDate,
                    EndDate = certificationPeriod.EndDate,
                    OriginalEndDate = certificationPeriod.OriginalEndDate,
                    CredentialApplicationId = certificationPeriod.CredentialApplication.Id
                };
            }

            return response;
        }

        public int? GetFeesQuestionId()
        {
            var feesQuestion = NHibernateSession.Current.
                Query<CredentialApplicationFormQuestion>(
                ).FirstOrDefault(x => x.CredentialApplicationFormSection.Name == "Fees" &&
                                      x.CredentialApplicationFormSection.CredentialApplicationForm.CredentialApplicationType.Name == "CCL V2" &&
                                      x.CredentialApplicationFormQuestionType.CredentialApplicationFormAnswerType.Name == "Fees" &&
                                      x.CredentialApplicationField.CredentialApplicationType.Name == "CCL V2" &&
                                      x.CredentialApplicationField.Name.Contains("Payment Option"));


            return feesQuestion?.Id;
        }

        public GetInvoicesResponse GetActionInvoices(GetActionInvoicesRequest request)
        {

            var query = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .Where(x => x.OnPaymentCreatedSystemActionType.Id == request.ActionId &&
                            x.InvoiceNumber != null);

            if (request.ApplicationId.HasValue)
            {
                query = query.Where(x => x.CredentialApplication.Id == request.ApplicationId);
            }

            if (request.CredentialRequestId.HasValue)
            {
                query = query.Where(x => x.CredentialRequest.Id == request.CredentialRequestId);
            }

            var invoiceNumbers = query.Select(x => x.InvoiceNumber).ToArray();
            if (!invoiceNumbers.Any())
            {
                return new GetInvoicesResponse { Invoices = new InvoiceDto[] { } };
            }

            return _financeService.GetInvoices(new GetInvoicesRequest
            {
                InvoiceNumber = invoiceNumbers,
                ExcludeCreditNotes = true
            });
        }

        public WorkFlowFeesResponse GetInvoicesAndPaymentsToProcess()
        {
            var workflowFees = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .Where(x => (x.OnInvoiceCreatedSystemActionType != null && x.InvoiceId != null && x.InvoiceActionProcessedDate == null) ||
                (x.OnPaymentCreatedSystemActionType != null && x.PaymentActionProcessedDate == null && x.PaymentReference != null)).Select(MapCredentialWorkflowFee).ToList();

            return new WorkFlowFeesResponse
            {
                CredentialWorkflowFees = workflowFees
            };
        }

        public RefundDtoResponse GetCreditNotesAndPaymentsToProcess()
        {
            var refunds = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                .Where(x => (x.OnCreditNoteCreatedSystemActionType != null && x.CreditNoteId != null && x.CreditNoteProcessedDate == null) ||
                            (x.OnPaymentCreatedSystemActionType != null && x.CreditNotePaymentProcessedDate == null && x.PaymentReference != null)).Select(MapRefundRequest).ToList();

            return new RefundDtoResponse
            {
                Results = refunds
            };
        }

        public RefundDtoResponse GetCredentialRequestRefunds(int credentialRequestId)
        {
            var refunds = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                .Where(x => x.CredentialWorkflowFee.CredentialRequest.Id == credentialRequestId && !x.IsRejected)
                .Select(MapRefundRequest).ToList();

            return new RefundDtoResponse() { Results = refunds };
        }

        public RefundDtoResponse GetNotFlushedProcessedRefunds(NotFlushedRefundRequest request)
        {
            var refunds = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                .Where(x => (x.IsRejected && x.BankDetails != null && x.CreatedDate <= request.MaxProcessedDate) ||
                (!x.IsRejected && x.BankDetails != null &&
                 x.CredentialWorkflowFee.CredentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.Withdrawn &&
                 x.CredentialWorkflowFee.CredentialRequest.StatusChangeDate <= request.MaxProcessedDate)
                 ).Take(request.Take)
                 .Select(MapRefundRequest).ToList();

            return new RefundDtoResponse() { Results = refunds };
        }


        private RefundDto MapRefundRequest(CredentialApplicationRefund refund)
        {
            return new RefundDto
            {
                CreatedDate = refund.CreatedDate,
                CredentialWorkflowFeeId = refund.CredentialWorkflowFee.Id,
                CredentialApplicationId = refund.CredentialWorkflowFee.CredentialApplication.Id,
                CredentialRequestId = refund.CredentialWorkflowFee.CredentialRequest.Id,
                CreditNoteId = refund.CreditNoteId,
                CreditNoteNumber = refund.CreditNoteNumber,
                CreditNotePaymentProcessedDate = refund.CreditNotePaymentProcessedDate,
                CreditNoteProcessedDate = refund.CreditNoteProcessedDate,
                Id = refund.Id,
                InitialPaidAmount = refund.InitialPaidAmount,
                OnCreditNoteCreatedSystemActionTypeId = refund.OnCreditNoteCreatedSystemActionType?.Id,
                OnPaymentCreatedSystemActionTypeId = refund.OnPaymentCreatedSystemActionType?.Id,
                PaymentReference = refund.PaymentReference,
                UserId = refund.User.Id,
                RefundAmount = refund.RefundAmount,
                RefundMethodTypeId = refund.RefundMethodType.Id,
                RefundPercentage = refund.RefundPercentage,
                RefundedDate = refund.RefundedDate,
                RefundTransactionId = refund.RefundTransactionId,
                OrderNumber = refund.CredentialWorkflowFee.OrderNumber,
                Comments = refund.Comments,
                BankDetails = UnProtectBankDetails(refund.BankDetails),
                ProductCategoryId = refund.CredentialWorkflowFee.ProductSpecification.ProductCategory.Id
            };
        }

        private CredentialWorkflowFeeDto MapCredentialWorkflowFee(CredentialWorkflowFee workflowFee)
        {
            return new CredentialWorkflowFeeDto
            {
                Id = workflowFee.Id,
                CredentialApplicationId = workflowFee.CredentialApplication.Id,
                CredentialRequestId = workflowFee.CredentialRequest?.Id,
                OnInvoiceActionType = workflowFee.OnInvoiceCreatedSystemActionType != null
                    ? (SystemActionTypeName?)Enum.Parse(typeof(SystemActionTypeName),
                        workflowFee.OnInvoiceCreatedSystemActionType.Name)
                    : null,
                InvoiceActionProcessedDate = workflowFee.InvoiceActionProcessedDate,
                OnPaymentActionType = workflowFee.OnPaymentCreatedSystemActionType != null
                    ? (SystemActionTypeName?)Enum.Parse(typeof(SystemActionTypeName),
                        workflowFee.OnPaymentCreatedSystemActionType.Name)
                    : null,
                InvoiceNumber = workflowFee.InvoiceNumber,
                PaymentReference = workflowFee.PaymentReference,
                PaymentActionProcessedDate = workflowFee.PaymentActionProcessedDate,
                InvoiceId = workflowFee.InvoiceId,
                OrderNumber = workflowFee.OrderNumber,
                TransactionId = workflowFee.TransactionId,
                CredentialApplicationRefundPolicy = new CredentialApplicationRefundPolicyData
                {
                    Id = workflowFee.CredentialApplicationRefundPolicy.Id,
                    Name = workflowFee.CredentialApplicationRefundPolicy.Name,
                    Description = workflowFee.CredentialApplicationRefundPolicy.Description,
                    RefundPolicyParameters = workflowFee.CredentialApplicationRefundPolicy.RefundPolicyParameters.Select(x => new Contracts.Dal.RefundPolicyParameterData
                    {
                        Name = x.Name,
                        Value = x.Value
                    }).ToList()
                }
            };
        }

        public RefundDtoResponse GetOutstandingRefunds(GetOutstandingRefundRequest request)
        {
            var creditNoteFilterList = request.CreditNotes.ToList();
            var notPaidRefunds = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                .Where(x => x.CreditNotePaymentProcessedDate == null && !x.IsRejected);

            if (creditNoteFilterList.Any())
            {
                notPaidRefunds = notPaidRefunds.Where(x => creditNoteFilterList.Contains(x.CreditNoteNumber));
            }

            var results = notPaidRefunds.Select(MapRefundRequest).ToList();

            return new RefundDtoResponse
            {
                Results = results
            };
        }
        public GetOutstandingInvoicesResponse GetOutstandingInvoices(GetOutstandingInvoicesRequest request)
        {
            var workflowFees = NHibernateSession.Current.Query<CredentialWorkflowFee>()
                .Where(x => x.PaymentActionProcessedDate == null);

            if (!string.IsNullOrEmpty(request.InvoiceNumber))
            {
                workflowFees = workflowFees.Where(x => x.InvoiceNumber == request.InvoiceNumber);
            }

            var dtos = new Dictionary<string, CredentialWorkflowFeeDto>();
            foreach (var workflowFee in workflowFees)
            {
                dtos.Add(workflowFee.InvoiceNumber, MapCredentialWorkflowFee(workflowFee));
            }

            if (!dtos.Any())
            {
                return new GetOutstandingInvoicesResponse { CredentialWorkflowFees = dtos.Values };
            }


            int skip = 0;
            const int take = 30;
            do
            {
                var batch = dtos.Keys
                    .Skip(skip)
                    .Take(take)
                    .ToArray();

                if (batch.Any())
                {
                    var invoicesResponse = _financeService.GetInvoices(
                        new GetInvoicesRequest
                        {
                            IncludeFullPaymentInfo = false,
                            IncludeVoidedStatus = true,
                            InvoiceNumber = batch
                        });

                    if (invoicesResponse.Error)
                    {
                        throw new Exception(invoicesResponse.ErrorMessage);
                    }

                    foreach (var invoice in invoicesResponse.Invoices)
                    {
                        dtos[invoice.InvoiceNumber].Invoice = invoice;
                    }
                }

                skip += take;
            } while (skip < dtos.Count);

            return new GetOutstandingInvoicesResponse
            {
                CredentialWorkflowFees = dtos.Values
            };
        }

        public void UpdateProcessedWorkflowFees(UpdateProcessedWorkflowFeeRequest request)
        {
            if (!request.Fees?.Any() ?? false)
            {
                return;
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                UpdateProcessedWorkflowFees(request.Fees);
                transaction.Commit();
            }
        }

        private void UpdateProcessedWorkflowFees(IEnumerable<ProcessFeeDto> fees)
        {
            var ids = fees.Select(x => x.CredentialWorkflowFeeId).ToList();
            var worflowFees = NHibernateSession.Current.Query<CredentialWorkflowFee>().Where(v => ids.Contains(v.Id));

            var processFeeDtoDictionary = fees.ToDictionary(x => x.CredentialWorkflowFeeId, y => y);
            foreach (var workflowFee in worflowFees)
            {
                var processFeeDto = processFeeDtoDictionary[workflowFee.Id];

                switch (processFeeDto.Type)
                {
                    case ProcessTypeName.Invoice:
                        workflowFee.InvoiceActionProcessedDate = DateTime.Now;
                        break;
                    case ProcessTypeName.Payment:
                        workflowFee.PaymentActionProcessedDate = DateTime.Now;
                        workflowFee.PaymentReference = string.IsNullOrWhiteSpace(processFeeDto.PaymentReference) ? workflowFee.PaymentReference : processFeeDto.PaymentReference;
                        workflowFee.TransactionId = string.IsNullOrWhiteSpace(processFeeDto.TransactionId) ? workflowFee.TransactionId : processFeeDto.TransactionId;
                        workflowFee.OrderNumber = string.IsNullOrWhiteSpace(processFeeDto.OrderNumber) ? workflowFee.OrderNumber : processFeeDto.OrderNumber;
                        break;
                    default:
                        throw new NotSupportedException($"Process Type {processFeeDto.Type} is not spported");
                }
                NHibernateSession.Current.Update(workflowFee);
            }
        }

        public void RemoveWorkFlowFees(RemoveWorkflowFeeRequest request)
        {
            if (!request.CredentialWorkflowFeeIds?.Any() ?? false)
            {
                return;
            }
            var ids = request.CredentialWorkflowFeeIds.ToList();
            var worflowFees = NHibernateSession.Current.Query<CredentialWorkflowFee>().Where(v => ids.Contains(v.Id));

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var workflowFee in worflowFees)
                {
                    NHibernateSession.Current.Delete(workflowFee);
                }
                transaction.Commit();
            }
        }

        public void DeleteWorkflowFees(DeleteVoidedInvoicesRequest request)
        {
            var ids = request.CredentialWorkflowFees.Select(x => x.Id).ToList();
            var credentialWorkflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>().Where(v => ids.Contains(v.Id));

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var workflowFee in credentialWorkflowFee)
                {
                    NHibernateSession.Current.Delete(workflowFee);
                }
                transaction.Commit();
            }
        }

        private CredentialRequestBasicDto CredentialTestMapToDto(CredentialRequest credentialRequest, IDictionary<int, CredentialStatusTypeName> credentialStatuses)
        {
            var testSitting = credentialRequest.TestSittings.FirstOrDefault(x => !x.Rejected && x.Supplementary == credentialRequest.Supplementary);


            var credentialRequestDto = new CredentialRequestBasicDto
            {
                Id = credentialRequest.Id,
                StatusTypeId = credentialRequest.CredentialRequestStatusType.Id,
                ApplicationTypeDisplayName = credentialRequest.CredentialApplication.CredentialApplicationType.DisplayName,
                CredentialTypeExternalName = CredentialQueryService.CredentialTypeToDto(credentialRequest.CredentialType).ExternalName,
                SkillDisplayName = credentialRequest.Skill.DisplayName,
                Status = credentialRequest.CredentialRequestStatusType.DisplayName,
                HasAvailableTestSessions = HasAvailableTestSessions(credentialRequest.Id),
                CredentialApplicationId = credentialRequest.CredentialApplication.Id,
                Supplementary = credentialRequest.Supplementary,
                TestSessionId = testSitting?.TestSession.Id,
                TestDate = testSitting?.TestSession.TestDateTime,
                VenueName = testSitting?.TestSession.Venue.TestLocation.Name,
            };

            return credentialRequestDto;
        }


        private CredentialRequestDto CredentialRequestToDto(CredentialRequest credentialRequest,
            List<CredentialApplicationField> credentialRequestFields,
            IDictionary<int, CredentialStatusTypeName> credentialStatuses, ILookup<int, CredentialRequestCredentialRequest> associations)
        {

            var credentialFieldDataByFieldTypeId =
                            credentialRequest.CredentialRequestFieldsData.ToDictionary(x => x.CredentialApplicationField.Id,
                                y => y);

            var credentialFieldsData = credentialRequestFields.Select(fieldType =>
            {
                CredentialRequestFieldData fieldData;
                credentialFieldDataByFieldTypeId.TryGetValue(fieldType.Id, out fieldData);

                return new CredentialRequestFieldDataDto
                {
                    Id = fieldData?.Id ?? 0,
                    FieldTypeId = fieldType.Id,
                    Name = fieldType.Name,
                    Section = fieldType.Section,
                    DataTypeId = fieldType.DataType.Id,
                    DefaultValue = fieldType.DefaultValue,
                    PerCredentialRequest = fieldType.PerCredentialRequest,
                    Description = fieldType.Description,
                    Value = fieldData != null ? fieldData.Value : fieldType.DefaultValue,
                };
            });

            var locationId = credentialRequest.CredentialApplication?.PreferredTestLocation?.Id ?? 0;
            var associateTestSessionForCredentialRequest = credentialRequest.TestSittings.Select(x => GetAssociatedTestSession(x, locationId)).ToList();

            var credentialRequestDto = new CredentialRequestDto
            {
                Id = credentialRequest.Id,
                CredentialApplicationId = credentialRequest.CredentialApplication.Id,
                ApplicationTypeDisplayName = credentialRequest.CredentialApplication.CredentialApplicationType.DisplayName,
                Category = credentialRequest.CredentialType.CredentialCategory.DisplayName,
                CategoryId = credentialRequest.CredentialType.CredentialCategory.Id,
                Certification = credentialRequest.CredentialType.Certification,
                CredentialName = credentialRequest.CredentialType.InternalName,
                ExternalCredentialName = credentialRequest.CredentialType.ExternalName,
                Direction = credentialRequest.Skill.DisplayName,
                DirectionTypeId = credentialRequest.Skill.DirectionType.Id,
                Status = credentialRequest.CredentialRequestStatusType.DisplayName,
                AutoCreated = credentialRequest.AutoCreated,
                StatusTypeId = credentialRequest.CredentialRequestStatusType.Id,
                SkillId = credentialRequest.Skill.Id,
                StatusChangeDate = credentialRequest.StatusChangeDate,
                ModifiedBy = credentialRequest.StatusChangeUser.FullName,
                StatusChangeUserId = credentialRequest.StatusChangeUser.Id,
                CredentialTypeId = credentialRequest.CredentialType.Id,
                CredentialRequestPathTypeId = credentialRequest.CredentialRequestPathType.Id,
                CredentialType = CredentialQueryService.CredentialTypeToDto(credentialRequest.CredentialType),
                Skill = _autoMapperHelper.Mapper.Map<SkillDto>(credentialRequest.Skill),
                Credentials = credentialRequest.Credentials.Select(x =>
                    {
                        var credential = _autoMapperHelper.Mapper.Map<CredentialDto>(x);
                        credential.CredentialTypeInternalName = credentialRequest.CredentialType.InternalName;
                        credential.SkillDisplayName = credentialRequest.Skill.DisplayName;
                        credential.Status = credentialStatuses[credential.Id].ToString();
                        credential.StatusId = (int)credentialStatuses[credential.Id];
                        credential.StoredFileIds = x.CredentialAttachments.Select(y => y.StoredFile.Id).ToList();
                        return credential;
                    }
                ).ToList(),
                Supplementary = credentialRequest.Supplementary,
                Fields = credentialFieldsData,
                SkillLanguage1Id = credentialRequest.Skill.Language1.Id,
                CredentialWorkflowFees = credentialRequest.CredentialWorkflowFees.Select(_autoMapperHelper.Mapper.Map<CredentialWorkflowFeeDto>).ToList(),
                RefundRequests = GetRefundRequestsByCredentialRequestId(credentialRequest.Id),
                TestSessions = associateTestSessionForCredentialRequest,
                ConcededFromCredentialRequestId = associations[credentialRequest.Id].
                FirstOrDefault(x => x.AssociationType.Id == (int)CredentialRequestAssociationTypeName.ConcededPass)?.OriginalCredentialRequest.Id,
                TestSittings = credentialRequest.TestSittings.Select(x => MapTestSitting(x)).ToList()
            };

            return credentialRequestDto;
        }

        private TestSittingHistoryItemDto MapTestSitting(TestSitting testSitting)
        {
            return new TestSittingHistoryItemDto
            {
                TestSittingId = testSitting.Id,
                Rejected = testSitting.Rejected,
                CredentialRequestId = testSitting.CredentialRequest.Id,
                CredentialRequestStatusTypeId = testSitting.CredentialRequest.CredentialRequestStatusType.Id,
                TestSessionId = testSitting.TestSession.Id
            };
        }

        private IEnumerable<RefundDto> GetRefundRequestsByCredentialRequestId(int credentialRequestId)
        {
            return NHibernateSession.Current.Query<CredentialApplicationRefund>().Where(refund =>
                refund.CredentialWorkflowFee.CredentialRequest.Id == credentialRequestId && !refund.IsRejected).Select(refund => new RefundDto()
                {
                    Id = refund.Id,
                    RefundPercentage = refund.RefundPercentage,
                    CredentialWorkflowFeeId = refund.CredentialWorkflowFee.Id,
                    CreditNotePaymentProcessedDate = refund.CreditNotePaymentProcessedDate,
                    RefundMethodTypeId = refund.RefundMethodType.Id,
                    RefundAmount = refund.RefundAmount,
                    CreditNoteProcessedDate = refund.CreditNoteProcessedDate,
                    UserId = refund.User.Id,
                    RefundedDate = refund.RefundedDate,
                    InitialPaidAmount = refund.InitialPaidAmount,
                    RefundTransactionId = refund.RefundTransactionId,
                    DisallowProcessing = refund.DisallowProcessing,
                    CreditNoteId = refund.CreditNoteId,
                    CreditNoteNumber = refund.CreditNoteNumber,
                    OnPaymentCreatedSystemActionTypeId = refund.OnPaymentCreatedSystemActionType.Id,
                    OnCreditNoteCreatedSystemActionTypeId = refund.OnCreditNoteCreatedSystemActionType.Id,
                    IsRejected = refund.IsRejected,
                    InitialPaidTax = refund.InitialPaidTax,
                    PaymentReference = refund.PaymentReference,
                    Comments = refund.Comments,
                    BankDetails = UnProtectBankDetails(refund.BankDetails)
                }).ToList();
        }

        private CredentialRequestTestSessionDto GetAssociatedTestSession(TestSitting testSitting, int preferredTestLocationId)
        {
            var credentialRequestTestSessionDto = _autoMapperHelper.Mapper.Map<CredentialRequestTestSessionDto>(testSitting);
            credentialRequestTestSessionDto.ShowNonPreferredTestLocationInfo =
                preferredTestLocationId != 0 &&
                testSitting.TestSession.Venue.TestLocation.Id != preferredTestLocationId;
            credentialRequestTestSessionDto.TestLocation = testSitting.TestSession.Venue.TestLocation.Name;
            credentialRequestTestSessionDto.TestLocationState = testSitting.TestSession.Venue.TestLocation.Office.State.Abbreviation;

            credentialRequestTestSessionDto.Materials =
                testSitting.TestSittingTestMaterials.Select(GetTestSittingMaterialDto).ToList();
            credentialRequestTestSessionDto.HasDefaultSpecification = testSitting.HasDefaultSpecification();

            credentialRequestTestSessionDto.TestDate = testSitting.TestSession.TestDateTime;

            return credentialRequestTestSessionDto;
        }

        private TestSittingMaterialDto GetTestSittingMaterialDto(TestSittingTestMaterial testSittingMaterial)
        {
            return new TestSittingMaterialDto
            {
                TestSittingTestMaterialId = testSittingMaterial.Id,
                TestMaterialId = testSittingMaterial.TestMaterial.Id,
                TestTaskId = testSittingMaterial.TestComponent.Id
            };
        }

        public DowngradedCredentialRequestDto GetDowngradedCredentialRequest(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId);

            var downgradePath = NHibernateSession.Current.Query<CredentialTypeDowngradePath>()
                .Single(x => x.CredentialTypeFrom.Id == credentialRequest.CredentialType.Id);

            var credentialType = downgradePath.CredentialTypeTo;

            var requestSkill = credentialRequest.Skill;
            var skill = NHibernateSession.Current
                .Query<Skill>()
                .Single(x => x.SkillType.Id == credentialType.SkillType.Id &&
                ((x.DirectionType.Id == requestSkill.DirectionType.Id && x.Language1.Id == requestSkill.Language1.Id && x.Language2.Id == requestSkill.Language2.Id) ||
                (x.DirectionType.Id == 1 && requestSkill.DirectionType.Id == 2 && x.Language1.Id == requestSkill.Language2.Id && x.Language2.Id == requestSkill.Language1.Id) ||
                (x.DirectionType.Id == 2 && requestSkill.DirectionType.Id == 1 && x.Language1.Id == requestSkill.Language2.Id && x.Language2.Id == requestSkill.Language1.Id)));


            return new DowngradedCredentialRequestDto
            {
                CredentailTypeId = credentialType.Id,
                SkillId = skill.Id,
                CategorId = credentialType.CredentialCategory.Id,
                NaatiNumber = credentialRequest.CredentialApplication.Person.Entity.NaatiNumber,
                Skill = skill.DisplayName,
                CredentialTypeInternalName = credentialType.InternalName,
                CredentialTypeExternalName = credentialType.ExternalName,
                Certification = credentialType.Certification

            };

        }

        public bool HasAvailableTestSessions(int credentialRequestId)
        {
            var helper = new TestSessionAvailabiltyQueryHelper();

            var result = helper.HasAllAvailableTestSessions(credentialRequestId);

            if (!result)
            {
                result = NHibernateSession.Current.Query<TestSitting>()
                    .Any(x => x.CredentialRequest.Id == credentialRequestId && !x.Rejected && x.CredentialRequest.Supplementary == x.Supplementary);
            }

            return result;
        }





        public LookupTypeResponse GetLookupType(string lookupType)
        {
            var type = (LookupType)Enum.Parse(typeof(LookupType), lookupType);

            return mLookupTypeDictionary[type]();
        }

        private LookupTypeResponse GetDynamicLookupType<T>() where T : class, IDynamicLookupType
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<T>()
                .Select(Projections.Property(nameof(IDynamicLookupType.Id)).WithAlias(() => lookupTypeDto.Id),
                    Projections.Property(nameof(IDynamicLookupType.DisplayName))
                        .WithAlias(() => lookupTypeDto.DisplayName))
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());

            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(nameof(IDynamicLookupType.DisplayName)));
            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>() };
        }

        private LookupTypeResponse GetDynamicLookupTypeOrderBy<T>() where T : class, IDynamicLookupType
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<T>()
                .Select(Projections.Property(nameof(IDynamicLookupTypeOrderBy.Id)).WithAlias(() => lookupTypeDto.Id), Projections.Property(nameof(IDynamicLookupTypeOrderBy.DisplayOrder)).WithAlias(() => lookupTypeDto.DisplayOrder),
                    Projections.Property(nameof(IDynamicLookupTypeOrderBy.DisplayName)).WithAlias(() => lookupTypeDto.DisplayName))
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());

            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>().OrderBy(x => x.DisplayOrder) };
        }

        private LookupTypeResponse GetCredentialTypes()
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<CredentialType>()
                .Select(Projections.Property(nameof(CredentialType.Id)).WithAlias(() => lookupTypeDto.Id), Projections.Property(nameof(CredentialType.DisplayOrder)).WithAlias(() => lookupTypeDto.DisplayOrder),
                    Projections.Property(nameof(CredentialType.InternalName)).WithAlias(() => lookupTypeDto.DisplayName))
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());

            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>().OrderBy(x => x.DisplayOrder) };
        }

        private LookupTypeResponse GetCredentialStatusTypes()
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<CredentialStatusType>()
                .Select(Projections.Property(nameof(CredentialStatusType.Id)).WithAlias(() => lookupTypeDto.Id),
                    Projections.Property(nameof(CredentialStatusType.DisplayName))
                        .WithAlias(() => lookupTypeDto.DisplayName))
                .Where(x => x.Id != (int)CredentialStatusTypeName.Unknown)
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());

            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>() };
        }

        private LookupTypeResponse GetLanguageGroup()
        {
            var queryable = NHibernateSession.Current.Query<LanguageGroup>().Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.Name
            }).ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Id);

            return new LookupTypeResponse { Results = queryable };
        }

        private LookupTypeResponse GetSkillType()
        {
            var queryable = NHibernateSession.Current.Query<SkillType>().Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            }).ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Id);

            return new LookupTypeResponse { Results = queryable };
        }

        private LookupTypeResponse GetDirectionType()
        {
            var queryable = NHibernateSession.Current.Query<DirectionType>().Select(x => new LookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            }).ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Id);

            return new LookupTypeResponse { Results = queryable };
        }

        private LookupTypeResponse GetCredentialTypesTestSession()
        {
            var queryable = NHibernateSession.Current.Query<TestSession>().Select(x => new LookupTypeDto
            {
                Id = x.CredentialType.Id,
                DisplayName = x.CredentialType.InternalName
            }).ToList().GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Id);

            return new LookupTypeResponse { Results = queryable };
        }

        private LookupTypeResponse GetCredentialApplicaionTypesBackend()
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<CredentialApplicationType>()
                .Select(Projections.Property(nameof(CredentialApplicationType.Id)).WithAlias(() => lookupTypeDto.Id),
                    Projections.Property(nameof(CredentialApplicationType.DisplayName)).WithAlias(() => lookupTypeDto.DisplayName))
                .Where(x => x.BackOffice)
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());
            queryOver.UnderlyingCriteria.AddOrder(Order.Asc(nameof(CredentialApplicationType.DisplayName)));

            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>() };
        }

        public LookupTypeResponse GetCredentialApplicaionTypes(GetCredentialApplicaionTypesRequest request)
        {
            LookupTypeDto lookupTypeDto = null;

            CredentialApplicationType credentialApplicationType = null;
            CredentialApplicationTypeCredentialType credentialApplicationTypeCredentialType = null;
            CredentialType credentialType = null;
            SkillType skillType = null;

            var queryOver = NHibernateSession.Current.QueryOver(() => credentialApplicationType)
                .Inner.JoinAlias(c => c.CredentialApplicationTypeCredentialTypes, () => credentialApplicationTypeCredentialType)
                .Inner.JoinAlias(c => credentialApplicationTypeCredentialType.CredentialType, () => credentialType)
                .Inner.JoinAlias(c => credentialType.SkillType, () => skillType)
                .Select(Projections.Property(nameof(CredentialApplicationType.Id)).WithAlias(() => lookupTypeDto.Id),
                    Projections.Property(nameof(CredentialApplicationType.DisplayName)).WithAlias(() => lookupTypeDto.DisplayName));

            var restrictions = Restrictions.Conjunction();

            if (request.SkillTypeIds != null && request.SkillTypeIds.Any())
            {
                restrictions.Add(Restrictions.In(Projections.Property(() => skillType.Id), request.SkillTypeIds.ToArray()));
            }

            queryOver = queryOver.Where(restrictions).TransformUsing(Transformers.AliasToBean<LookupTypeDto>());
            return new LookupTypeResponse { Results = queryOver.List<LookupTypeDto>() };
        }

        private LookupTypeResponse GetUserLookup()
        {
            LookupTypeDto lookupTypeDto = null;

            var queryOver = NHibernateSession.Current.QueryOver<User>()
                .Where(x => x.Active)
                .Select(Projections.Property(nameof(User.Id)).WithAlias(() => lookupTypeDto.Id),
                    Projections.Property(nameof(User.FullName)).WithAlias(() => lookupTypeDto.DisplayName))
                .TransformUsing(Transformers.AliasToBean<LookupTypeDto>());

            var result = queryOver.List<LookupTypeDto>();
            result.Insert(0, new LookupTypeDto { Id = 0, DisplayName = "Applicant" });
            return new LookupTypeResponse { Results = result.OrderBy(x => x.DisplayName) };
        }


        public GetApplicationDetailsResponse GetApplicationDetails(GetApplicationDetailsRequest request)
        {
            if (request == null || (request.ApplicationId < 1 && request.CredentialRequestId < 1 && request.TestSittingId < 1))
            {
                throw new Exception("Application ID or TestSitting Id not provided.");
            }
            CredentialApplication application;
            if (request.TestSittingId > 0)
            {
                application = NHibernateSession.Current.Get<TestSitting>(request.TestSittingId)
                    .CredentialRequest.CredentialApplication;
            }
            else if (request.CredentialRequestId > 0)
            {
                application = NHibernateSession.Current.Get<CredentialRequest>(request.CredentialRequestId).CredentialApplication;
            }
            else
            {
                application = NHibernateSession.Current.Get<CredentialApplication>(request.ApplicationId);
            }

            var result = new GetApplicationDetailsResponse
            {
                ApplicationInfo = CredentialApplicationToDto(application),
                ApplicantDetails = PersonQueryService.PersonToBasicDetails(application.Person),
                ApplicationType = _autoMapperHelper.Mapper.Map<CredentialApplicationType, CredentialApplicationTypeDto>(application.CredentialApplicationType),
                ApplicationStatus = _autoMapperHelper.Mapper.Map<LookupTypeDetailedDto>(application.CredentialApplicationStatusType),
                CredentialRequests = GetCredentialRequests(application.Id, request.ExcludedRequestStauses ?? Enumerable.Empty<CredentialRequestStatusTypeName>()).Results,
                Fields = GetApplicationFieldsData(application.Id).Result,
                CredentialWorkflowFees = application.CredentialWorkflowFees.Select(_autoMapperHelper.Mapper.Map<CredentialWorkflowFeeDto>)
            };

            // my mapping for this didn't work, so this is done manually
            result.ApplicationType.Category = (CredentialApplicationTypeCategoryName)application.CredentialApplicationType.CredentialApplicationTypeCategory.Id;

            return result;
        }

        public GetCredentialApplicationTypeResponse GetCredentialApplicationType(int credentialApplicationTypeId)
        {
            var credentialApplicationType = NHibernateSession.Current.Get<CredentialApplicationType>(credentialApplicationTypeId);

            var applicationTypeDto = _autoMapperHelper.Mapper.Map<CredentialApplicationTypeDto>(credentialApplicationType);
            applicationTypeDto.Category = (CredentialApplicationTypeCategoryName)credentialApplicationType.CredentialApplicationTypeCategory.Id;

            return new GetCredentialApplicationTypeResponse { Result = applicationTypeDto };
        }

        public UpsertApplicationResponse UpsertApplication(UpsertCredentialApplicationRequest request)
        {
            var credentialApplication = GetCredentialApplicationToUpsert(request);

            // create fields for new application
            var fields = request.Fields;

            if (credentialApplication.Id == 0)
            {
                var fieldsList = credentialApplication.CredentialApplicationType.CredentialApplicationFields
                    .Select(x => new ApplicationFieldData { FieldTypeId = x.Id }).ToList();

                fieldsList = GetParentMandatoryFields(request.Fields, fieldsList);

                fields = fieldsList;
            }

            var applicationDataToUpdate = GetApplicationDataToUpsert(fields, credentialApplication);
            var activitiesToUpsertAndDelete =
                GetPdActivitiesToUpsertAndDelete(request.PdActivities ?? Enumerable.Empty<PdActivityData>(),
                    credentialApplication);

            var credentialRequestInfoToUpdate = GetCredentialRequestsInfoToUpsertAndDelete(request.CredentialRequests, credentialApplication);
            var credentialInfoToUpsert = GetCredentialsInfoUpsert(request.CredentialRequests);
            var notesInfoToUpsert = GetNotesInfoToUpsert(request.Notes, credentialApplication);
            var personNotesInfoToUpsert = GetPeersonNotesInfoToUpsert(request.PersonNotes, credentialApplication);
            var standardCompomentsToUpsert = GetStandardTestComponentResultsToUpdate(request.StandardTestComponents);
            var rubricCompomentsToUpsert = GetRubricTestComponentResultsToUpdate(request.RubricTestComponents);
            var recertificationsToInsert = GetRecertificationsToInsert(request, credentialApplication);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.SaveOrUpdate(credentialApplication);

                Util.ForEach(applicationDataToUpdate, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(activitiesToUpsertAndDelete.ItemsToDelete, NHibernateSession.Current.Delete);
                Util.ForEach(activitiesToUpsertAndDelete.ItemsToUpsert, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(credentialRequestInfoToUpdate.ItemsToDelete, NHibernateSession.Current.Delete);
                Util.ForEach(credentialRequestInfoToUpdate.ItemsToUpsert, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(notesInfoToUpsert.Item1, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(notesInfoToUpsert.Item2, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(personNotesInfoToUpsert.Item1, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(personNotesInfoToUpsert.Item2, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(credentialInfoToUpsert.Item1, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(credentialInfoToUpsert.Item2, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(standardCompomentsToUpsert, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(rubricCompomentsToUpsert, NHibernateSession.Current.SaveOrUpdate);
                Util.ForEach(recertificationsToInsert, NHibernateSession.Current.SaveOrUpdate);
                if (request.ProcessedFees?.Any() == true)
                {
                    UpdateProcessedWorkflowFees(request.ProcessedFees);
                }
                if(request.Attachments?.Any() == true)
                {
                    foreach (var attachment in request.Attachments)
                    {
                        attachment.CredentialApplicationId = credentialApplication.Id;
                        attachment.StoragePath = attachment.StoragePath.Replace("<applicationid>", credentialApplication.Id.ToString());

                        CreateOrReplaceApplicationAttachmentResponse response = null;

                        try
                        {
                            response = CreateOrReplaceAttachment(attachment);
                            LoggingHelper.LogInfo($"APP{attachment.CredentialApplicationId}, attachment {attachment.Title} has been automatically created.");
                        }
                        catch (WebServiceException e)
                        {
                            throw new UserFriendlySamException(e.Message);
                        }
                    } 
                }

                transaction.Commit();
            }

            return new UpsertApplicationResponse
            {
                CredentialApplicationId = credentialApplication.Id,
                ApplicationFieldDataIds = applicationDataToUpdate.Select(x => x.Id),
                CredentialRequestIds = credentialRequestInfoToUpdate.ItemsToUpsert.OfType<CredentialRequest>().Select(x => x.Id),
                NoteIds = notesInfoToUpsert.Item1.Select(x => x.Id)
            };
        }

        private List<ApplicationFieldData> GetParentMandatoryFields(IEnumerable<ApplicationFieldData> fields, List<ApplicationFieldData> fieldsList)
        {
            if (fields.IsNull() || fieldsList.IsNull())
            {
                return fieldsList;
            }

            foreach (var field in fields)
            {
                foreach (var item in fieldsList.Where(x => x.FieldTypeId == field.FieldTypeId))
                {
                    if (item.IsNotNull())
                    {
                        item.Value = field.Value;
                    }
                }
            }

            return fieldsList;
        }

        /// <summary>
        /// For any new CredentialRequests being created, which have a CredentialId already assigned, return a new CredentialCredentialRequest to be created.
        /// This is used when adding a CredentialRequest to a Recertification application.
        /// </summary>
        private IList<CredentialCredentialRequest> GetCredentialCredentialRequestsToInsert(IEnumerable<CredentialRequestData> credentialRequestUpsertData, IEnumerable<CredentialRequest> credentialRequestsToUpsert)
        {
            var results = new List<CredentialCredentialRequest>();
            foreach (var cr in credentialRequestUpsertData.Where(x => x.Id == 0 && x.CredentialId > 0))
            {
                var newCredentialRequest = credentialRequestsToUpsert.SingleOrDefault(x =>
                        x.Id == 0 &&
                        x.Skill.Id == cr.SkillId)
                    .NotNull($"Missing new credential request for Skill {cr.SkillId}");

                results.Add(new CredentialCredentialRequest
                {
                    CredentialRequest = newCredentialRequest,
                    Credential = NHibernateSession.Current.Get<Credential>(cr.CredentialId)
                });
            }
            return results;
        }

        private IList<Recertification> GetRecertificationsToInsert(UpsertCredentialApplicationRequest request, CredentialApplication credentialApplication)
        {
            var recertifications = new List<Recertification>();

            if (request.Recertification != null &&
                request.Recertification.Id == 0 &&
                request.Recertification.CertificationPeriodId > 0)
            {
                recertifications.Add(new Recertification
                {
                    CertificationPeriod = NHibernateSession.Current.Get<CertificationPeriod>(request.Recertification.CertificationPeriodId),
                    CredentialApplication = credentialApplication
                });
            }

            return recertifications;
        }

        private System.Tuple<IList<Credential>, IList<CredentialCredentialRequest>> GetCredentialsInfoUpsert(IEnumerable<CredentialRequestData> credentialRequests)
        {
            var result = new System.Tuple<IList<Credential>, IList<CredentialCredentialRequest>>(
                new List<Credential>(), new List<CredentialCredentialRequest>());

            foreach (var credentialRequest in credentialRequests)
            {
                if (credentialRequest.Credentials != null)
                {
                    foreach (var credentialDto in credentialRequest.Credentials)
                    {
                        if (credentialDto.Id == 0)
                        {
                            var credential = _autoMapperHelper.Mapper.Map<Credential>(credentialDto);
                            result.Item1.Add(credential);
                            result.Item2.Add(new CredentialCredentialRequest
                            {
                                Credential = credential,
                                CredentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequest.Id)
                            });
                        }
                        else
                        {
                            result.Item1.Add(NHibernateSession.Current.Get<Credential>(credentialDto.Id));
                        }
                    }
                }
            }
            return result;
        }

        public IList<CredentialApplicationFieldData> GetApplicationDataToUpsert(
            IEnumerable<ApplicationFieldData> fields, CredentialApplication application)
        {
            var existingFields = application.CredentialApplicationFieldsData?
                .ToDictionary(i => i.Id, v => v) ?? new Dictionary<int, CredentialApplicationFieldData>();

            var fieldsToUpsert = new List<CredentialApplicationFieldData>();

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    CredentialApplicationFieldData applicationFieldData;
                    if (!existingFields.TryGetValue(field.FieldDataId, out applicationFieldData))
                    {
                        applicationFieldData = new CredentialApplicationFieldData
                        {
                            CredentialApplication = application,
                            CredentialApplicationField = NHibernateSession.Current.Load<CredentialApplicationField>(field.FieldTypeId)
                        };
                    }

                    applicationFieldData.Value = field.Value;
                    applicationFieldData.CredentialApplicationFieldOptionOption = NHibernateSession.Current.Get<CredentialApplicationFieldOptionOption>(field.FieldOptionId);
                    fieldsToUpsert.Add(applicationFieldData);
                }
            }

            return fieldsToUpsert;
        }

        private CredentialApplication GetCredentialApplicationToUpsert(UpsertCredentialApplicationRequest request)
        {
            var credentialApplication = NHibernateSession.Current.Get<CredentialApplication>(request.ApplicationId);

            if (credentialApplication == null)
            {
                if (request.ApplicationId != 0)
                {
                    throw new Exception($"Application with  id {request.ApplicationId} was not found");
                }

                var entity = NHibernateSession.Current.Query<NaatiEntity>()
                    .SingleOrDefault(x => x.NaatiNumber == request.NaatiNumber);

                credentialApplication = new CredentialApplication
                {
                    CredentialApplicationType = NHibernateSession.Current.Load<CredentialApplicationType>(request.ApplicationTypeId),
                    Person = NHibernateSession.Current.Query<Person>().SingleOrDefault(x => x.Entity.Id == entity.Id),
                    EnteredUser = NHibernateSession.Current.Get<User>(request.EnteredUserId),
                    EnteredDate = request.EnteredDate
                };
            }

            if (request.SponsorInstitutionNaatiNumber > 0)
            {
                var entity = NHibernateSession.Current.Query<NaatiEntity>()
                    .SingleOrDefault(x => x.NaatiNumber == request.SponsorInstitutionNaatiNumber);

                var institution = NHibernateSession.Current.Query<Institution>()
                    .SingleOrDefault(x => x.Entity.Id == entity.Id);

                if (institution != null)
                    credentialApplication.SponsorInstitution = NHibernateSession.Current.Get<Institution>(institution.Id);
            }
            else
            {
                credentialApplication.SponsorInstitution = null;
            }

            credentialApplication.CredentialApplicationStatusType = NHibernateSession.Current.Get<CredentialApplicationStatusType>(request.ApplicationStatusTypeId);
            credentialApplication.StatusChangeDate = request.StatusChangeDate;
            credentialApplication.StatusChangeUser = NHibernateSession.Current.Get<User>(request.StatusChangeUserId);
            credentialApplication.ReceivingOffice = NHibernateSession.Current.Get<Office>(request.ReceivingOfficeId);
            credentialApplication.OwnedByUser = NHibernateSession.Current.Get<User>(request.OwnedByUserId);
            credentialApplication.OwnedByApplicant = request.OwnedByApplicant;
            credentialApplication.AutoCreated = request.AutoCreated;
            credentialApplication.PreferredTestLocation = NHibernateSession.Current.Get<TestLocation>(request.PreferredTestLocationId);
            credentialApplication.SponsorInstitutionContactPerson = NHibernateSession.Current.Get<ContactPerson>(request.SponsorInstitutionContactPersonId);

            if (credentialApplication.EnteredUser == null)
            {
                credentialApplication.EnteredUser = NHibernateSession.Current.Get<User>(request.EnteredUserId);
            }

            return credentialApplication;
        }

        public (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetCredentialRequestsInfoToUpsertAndDelete(IEnumerable<CredentialRequestData> credentialRequestsData, CredentialApplication application)
        {
            var existingCredentialRequests = application.CredentialRequests?.ToDictionary(i => i.Id, v => v) ??
                                             new Dictionary<int, CredentialRequest>();

            var credentialFieldDataByCredentialRequest = new Dictionary<CredentialRequest, IEnumerable<CredentialFieldData>>();

            var credentialTestSessionByCredentialRequest = new Dictionary<CredentialRequest, IEnumerable<CredentialRequestTestSessionDto>>();

            var workPracticeToUpsert = new List<object>();
            var workPracticeToDelete = new List<object>();
            var refundRequestsToUpsert = new List<object>();
            var refundRequestsToDelete = new List<object>();

            var briefToUpsert = new List<object>();
            var briefToDelete = new List<object>();
            var credentialRequestAssociations = new List<object>();
            foreach (var credentialRequestData in credentialRequestsData)
            {
                CredentialRequest credentialRequest;
                if (!existingCredentialRequests.TryGetValue(credentialRequestData.Id, out credentialRequest))
                {
                    credentialRequest = new CredentialRequest
                    {
                        CredentialApplication = application,
                        CredentialType = NHibernateSession.Current.Load<CredentialType>(credentialRequestData.CredentialTypeId),
                        Skill = NHibernateSession.Current.Load<Skill>(credentialRequestData.SkillId),
                        AutoCreated = credentialRequestData.AutoCreated
                    };
                    if (credentialRequestData.ConcededFromCredentialRequestId.HasValue)
                    {
                        credentialRequestAssociations.Add(new CredentialRequestCredentialRequest
                        {
                            OriginalCredentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestData.ConcededFromCredentialRequestId),
                            AssociatedCredentialRequest = credentialRequest,
                            AssociationType = NHibernateSession.Current.Load<CredentialRequestAssociationType>((int)CredentialRequestAssociationTypeName.ConcededPass)
                        });
                    }
                }

                credentialRequest.StatusChangeUser =
                    NHibernateSession.Current.Load<User>(credentialRequestData.StatusChangeUserId);

                credentialRequest.StatusChangeDate = credentialRequestData.StatusChangeDate;

                credentialRequest.CredentialRequestStatusType =
                    NHibernateSession.Current.Load<CredentialRequestStatusType>(credentialRequestData.StatusTypeId);

                credentialRequest.CredentialRequestPathType =
                    NHibernateSession.Current.Load<CredentialRequestPathType>(credentialRequestData.CredentialRequestPathTypeId);

                credentialFieldDataByCredentialRequest.Add(credentialRequest, credentialRequestData.Fields);
                credentialTestSessionByCredentialRequest.Add(credentialRequest, credentialRequestData.TestSessions);
                credentialRequest.Supplementary = credentialRequestData.Supplementary;

                var workPracticeToUpsertAndDelete = GetWorkPracticesToUpdateOrDelete(credentialRequestData.WorkPractices ?? Enumerable.Empty<WorkPracticeData>(), credentialRequest);
                var refundRequestToUpdate = GetRefundRequestsToUpdateOrDelete(credentialRequestData.RefundRequests ?? Enumerable.Empty<RefundDto>(), credentialRequest);
                workPracticeToUpsert.AddRange(workPracticeToUpsertAndDelete.ItemsToUpsert);
                workPracticeToDelete.AddRange(workPracticeToUpsertAndDelete.ItemsToDelete);
                refundRequestsToUpsert.AddRange(refundRequestToUpdate.ItemsToUpsert);
                refundRequestsToDelete.AddRange(refundRequestToUpdate.ItemsToDelete);
                var briefToUpsertAndDelete = GetBriefsToUpdateOrDelete(credentialRequestData.Briefs ?? Enumerable.Empty<CandidateBriefDto>());
                briefToUpsert.AddRange(briefToUpsertAndDelete.ItemsToUpsert);
                briefToDelete.AddRange(briefToUpsertAndDelete.ItemsToDelete);
            }

            var credentialFieldsDataToUpsert = GetCredentialFieldsDataToUpsert(credentialFieldDataByCredentialRequest);
            var credentialTestSessionDetails = GetCredentialTestSessionsDetailsToUpsertAndDelete(credentialTestSessionByCredentialRequest);


            var itemsToUpsert = new List<object>();
            itemsToUpsert.AddRange(credentialFieldDataByCredentialRequest.Keys);
            itemsToUpsert.AddRange(credentialFieldsDataToUpsert);
            itemsToUpsert.AddRange(credentialTestSessionDetails.ItemsToUpsert);
            itemsToUpsert.AddRange(workPracticeToUpsert);
            itemsToUpsert.AddRange(briefToUpsert);
            itemsToUpsert.AddRange(credentialRequestAssociations);
            itemsToUpsert.AddRange(refundRequestsToUpsert);

            var itemsToDelete = new List<object>();
            itemsToDelete.AddRange(credentialTestSessionDetails.ItemsToDelete);
            itemsToDelete.AddRange(workPracticeToDelete);
            itemsToDelete.AddRange(briefToDelete);
            itemsToDelete.AddRange(refundRequestsToDelete);

            return (itemsToUpsert, itemsToDelete);

        }

        private (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetRefundRequestsToUpdateOrDelete(
            IEnumerable<RefundDto> refundRequests, CredentialRequest credentialRequest)
        {
            var refundRequestsList = refundRequests.ToList();
            var refundRequestsIds = new List<int> { 0 };

            foreach (var refundRequest in refundRequestsList)
            {
                refundRequestsIds.Add(refundRequest.Id);
            }
            var existingRefundRequests = GetItems<CredentialApplicationRefund>(refundRequestsIds).ToLookup(x => x.Id);

            var objectsDeleted = new List<object>();
            var objectsToUpsert = new List<object>();

            foreach (var refundRequest in refundRequestsList)
            {
                var existingRefundRequest = existingRefundRequests[refundRequest.Id].FirstOrDefault();

                switch (refundRequest.ObjectStatusId)
                {
                    case (int)ObjectStatusTypeName.None: continue;
                    case (int)ObjectStatusTypeName.Deleted:
                        objectsDeleted.Add(existingRefundRequest);
                        break;
                    case (int)ObjectStatusTypeName.Created:
                        existingRefundRequest = new CredentialApplicationRefund();
                        existingRefundRequest.CredentialWorkflowFee = NHibernateSession.Current.Load<CredentialWorkflowFee>(refundRequest.CredentialWorkflowFeeId);
                        existingRefundRequest.CreditNoteNumber = refundRequest.CreditNoteNumber;
                        existingRefundRequest.CreditNoteId = refundRequest.CreditNoteId;
                        existingRefundRequest.PaymentReference = refundRequest.PaymentReference;
                        existingRefundRequest.InitialPaidAmount = refundRequest.InitialPaidAmount;
                        existingRefundRequest.RefundAmount = refundRequest.RefundAmount;
                        existingRefundRequest.RefundPercentage = refundRequest.RefundPercentage;
                        existingRefundRequest.RefundMethodType = NHibernateSession.Current.Load<RefundMethodType>(refundRequest.RefundMethodTypeId);
                        existingRefundRequest.User = NHibernateSession.Current.Load<User>(refundRequest.UserId);
                        existingRefundRequest.RefundedDate = refundRequest.RefundedDate;
                        existingRefundRequest.CreditNoteProcessedDate = refundRequest.CreditNoteProcessedDate;
                        existingRefundRequest.CreditNotePaymentProcessedDate = refundRequest.CreditNotePaymentProcessedDate;
                        existingRefundRequest.CreatedDate = DateTime.Now;
                        existingRefundRequest.RefundTransactionId = refundRequest.RefundTransactionId;
                        existingRefundRequest.DisallowProcessing = refundRequest.DisallowProcessing.GetValueOrDefault();
                        existingRefundRequest.IsRejected = refundRequest.IsRejected.Value;
                        existingRefundRequest.InitialPaidTax = refundRequest.InitialPaidTax;
                        existingRefundRequest.Comments = refundRequest.Comments;
                        existingRefundRequest.BankDetails = ProtectBankDetails(refundRequest.BankDetails);
                        objectsToUpsert.Add(existingRefundRequest);
                        break;
                    case (int)ObjectStatusTypeName.Updated:
                        existingRefundRequest.CreditNoteNumber = refundRequest.CreditNoteNumber ?? existingRefundRequest.CreditNoteNumber;
                        existingRefundRequest.CreditNoteId = refundRequest.CreditNoteId ?? existingRefundRequest.CreditNoteId;
                        existingRefundRequest.PaymentReference = refundRequest.PaymentReference ?? existingRefundRequest.PaymentReference;
                        existingRefundRequest.InitialPaidAmount = refundRequest.InitialPaidAmount ?? existingRefundRequest.InitialPaidAmount;
                        existingRefundRequest.RefundAmount = refundRequest.RefundAmount ?? existingRefundRequest.InitialPaidAmount;
                        existingRefundRequest.RefundPercentage = refundRequest.RefundPercentage ?? existingRefundRequest.RefundPercentage;
                        existingRefundRequest.RefundMethodType = refundRequest.RefundMethodTypeId > 0 ? NHibernateSession.Current.Load<RefundMethodType>(refundRequest.RefundMethodTypeId) : existingRefundRequest.RefundMethodType;
                        existingRefundRequest.RefundedDate = refundRequest.RefundedDate ?? existingRefundRequest.RefundedDate;
                        existingRefundRequest.CreditNoteProcessedDate = refundRequest.CreditNoteProcessedDate ?? existingRefundRequest.CreditNoteProcessedDate;
                        existingRefundRequest.CreditNotePaymentProcessedDate = refundRequest.CreditNotePaymentProcessedDate ?? existingRefundRequest.CreditNotePaymentProcessedDate;
                        existingRefundRequest.RefundTransactionId = refundRequest.RefundTransactionId ?? existingRefundRequest.RefundTransactionId;
                        existingRefundRequest.DisallowProcessing = refundRequest.DisallowProcessing ?? existingRefundRequest.DisallowProcessing;
                        existingRefundRequest.IsRejected = refundRequest.IsRejected ?? existingRefundRequest.IsRejected;
                        existingRefundRequest.InitialPaidTax = refundRequest.InitialPaidTax ?? existingRefundRequest.InitialPaidTax;
                        existingRefundRequest.Comments = refundRequest.Comments ?? existingRefundRequest.Comments;
                        existingRefundRequest.BankDetails = ProtectBankDetails(refundRequest.BankDetails ?? existingRefundRequest.BankDetails);
                        objectsToUpsert.Add(existingRefundRequest);
                        break;
                    default:
                        throw new NotSupportedException($"ObjectStatusTypeName {refundRequest.ObjectStatusId}");
                }
            }

            return (objectsToUpsert, objectsDeleted);
        }

        private (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetWorkPracticesToUpdateOrDelete(
            IEnumerable<WorkPracticeData> workPractices, CredentialRequest credentialRequest)
        {

            var workPracticeLlist = workPractices.ToList();
            var workPracticeIds = new List<int> { 0 };
            var workPracticeCredentialRequestIds = new List<int> { 0 };
            foreach (var workPractice in workPracticeLlist)
            {
                workPracticeIds.Add(workPractice.WorkPracticeId);
                workPracticeCredentialRequestIds.Add(workPractice.WorkPracticeCredentialRequestId);
            }

            var existingWorkPracticeCredentialRequests = GetItems<WorkPracticeCredentialRequest>(workPracticeCredentialRequestIds).ToLookup(x => x.Id);
            var existingWorkPractices = GetItems<WorkPractice>(workPracticeIds).ToLookup(x => x.Id);

            var objectsDeleted = new List<object>();
            var objectsToupsert = new List<object>();
            foreach (var workPracticeData in workPracticeLlist)
            {
                var workPracticeCredentialRequest =
                    existingWorkPracticeCredentialRequests[workPracticeData.WorkPracticeCredentialRequestId].FirstOrDefault();


                switch (workPracticeData.ObjectStatusId)
                {
                    case (int)ObjectStatusTypeName.None: continue;
                    case (int)ObjectStatusTypeName.Deleted:
                        objectsDeleted.Add(workPracticeCredentialRequest);
                        break;
                    case (int)ObjectStatusTypeName.Created:
                    case (int)ObjectStatusTypeName.Updated:
                        workPracticeCredentialRequest = workPracticeCredentialRequest ?? new WorkPracticeCredentialRequest();
                        workPracticeCredentialRequest.CredentialRequest = credentialRequest;
                        workPracticeCredentialRequest.WorkPractice = existingWorkPractices[workPracticeData.WorkPracticeId].First();
                        objectsToupsert.Add(workPracticeCredentialRequest);
                        break;
                    default:
                        throw new NotSupportedException($"ObjectStatusTypeName {workPracticeData.ObjectStatusId}");
                }
            }

            return (objectsToupsert, objectsDeleted);
        }

        private (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetBriefsToUpdateOrDelete(
            IEnumerable<CandidateBriefDto> briefs)
        {

            var briefList = briefs.ToList();
            var candidateBriefIds = new List<int> { 0 }.Concat(briefList.Where(x => x.CandidateBriefId > 0).Select(y => y.CandidateBriefId)).ToList();
            var existingBriefs = GetItems<CandidateBrief>(candidateBriefIds).ToLookup(x => x.Id);

            var objectsDeleted = new List<object>();
            var objectsToupsert = new List<object>();
            foreach (var brief in briefList)
            {
                var candidateBrief = existingBriefs[brief.CandidateBriefId].FirstOrDefault() ?? new CandidateBrief();

                candidateBrief.TestMaterialAttachment = NHibernateSession.Current.Load<TestMaterialAttachment>(brief.TestMaterialAttachmentId);
                candidateBrief.TestSitting = NHibernateSession.Current.Load<TestSitting>(brief.TestSittingId);
                candidateBrief.EmailedDate = brief.EmailedDate;
                objectsToupsert.Add(candidateBrief);
            }

            return (objectsToupsert, objectsDeleted);
        }

        private IList<T> GetItems<T>(IList<int> itemIds) where T : EntityBase
        {
            //Prevents db exception when many ids are requested.
            var result = new List<T>();
            var parametersLimit = 2000;
            var processedItems = 0;
            var subItemsIds = itemIds.Skip(processedItems).Take(parametersLimit).ToList();

            while (subItemsIds.Count > 0)
            {
                processedItems += subItemsIds.Count;
                var data = NHibernateSession.Current.Query<T>().Where(x => subItemsIds.Contains(x.Id)).ToList();
                result.AddRange(data);
                subItemsIds = itemIds.Skip(processedItems).Take(parametersLimit).ToList();
            }

            return result;
        }

        private (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetPdActivitiesToUpsertAndDelete(IEnumerable<PdActivityData> activities, CredentialApplication credentialApplication)
        {
            var objectsDeleted = new List<object>();
            var objectsToupsert = new List<object>();
            var activityList = activities.ToList();
            var activityIds = new List<int> { 0 };
            var pdApplicationIds = new List<int> { 0 };
            foreach (var activity in activityList)
            {
                activityIds.Add(activity.ActivityId);
                pdApplicationIds.Add(activity.PdApplicationId);
            }

            var existingPdAppliations = GetItems<ProfessionalDevelopmentCredentialApplication>(pdApplicationIds).ToLookup(x => x.Id);
            var existingActivities = GetItems<ProfessionalDevelopmentActivity>(activityIds).ToLookup(x => x.Id);

            foreach (var activity in activityList)
            {
                var developmentActivity = existingPdAppliations[activity.PdApplicationId].FirstOrDefault();
                switch (activity.ObjectStatusId)
                {
                    case (int)ObjectStatusTypeName.None: continue;
                    case (int)ObjectStatusTypeName.Deleted:
                        objectsDeleted.Add(developmentActivity);
                        break;
                    case (int)ObjectStatusTypeName.Created:
                    case (int)ObjectStatusTypeName.Updated:
                        developmentActivity = developmentActivity ?? new ProfessionalDevelopmentCredentialApplication();
                        developmentActivity.CredentialApplication = credentialApplication;
                        developmentActivity.ProfessionalDevelopmentActivity = existingActivities[activity.ActivityId].First();
                        objectsToupsert.Add(developmentActivity);
                        break;
                    default:
                        throw new NotSupportedException($"ObjectStatusTypeName {activity.ObjectStatusId}");
                }
            }

            return (objectsToupsert, objectsDeleted);
        }


        private List<CredentialRequestFieldData> GetCredentialFieldsDataToUpsert(
            IReadOnlyDictionary<CredentialRequest, IEnumerable<CredentialFieldData>> credentialsInfo)
        {
            var credentialRequestIds = credentialsInfo.Keys.Select(x => x.Id).ToList();

            var existingCredentialRequestFields = new Dictionary<int, CredentialRequestFieldData>();
            if (credentialRequestIds.Any())
            {
                existingCredentialRequestFields = NHibernateSession.Current.Query<CredentialRequestFieldData>()
                    .Where(f => credentialRequestIds.Contains(f.CredentialRequest.Id))
                    .ToDictionary(x => x.Id, y => y);
            }

            var credentialFieldsToUpsert = new List<CredentialRequestFieldData>();
            foreach (var credentialInfo in credentialsInfo)
            {
                if (credentialInfo.Value != null)
                {
                    foreach (var credentialFieldData in credentialInfo.Value)
                    {
                        CredentialRequestFieldData credentialRequestFieldData;
                        if (!existingCredentialRequestFields.TryGetValue(credentialFieldData.Id, out credentialRequestFieldData))
                        {
                            credentialRequestFieldData = new CredentialRequestFieldData
                            {
                                CredentialRequest = credentialInfo.Key,
                                CredentialApplicationField = NHibernateSession.Current.Load<CredentialApplicationField>(credentialFieldData.FieldTypeId)
                            };
                        }

                        credentialRequestFieldData.Value = credentialFieldData.Value;

                        credentialFieldsToUpsert.Add(credentialRequestFieldData);
                    }
                }
            }

            return credentialFieldsToUpsert;
        }

        private (IList<object> ItemsToUpsert, IList<object> ItemsToDelete) GetCredentialTestSessionsDetailsToUpsertAndDelete(
            IReadOnlyDictionary<CredentialRequest, IEnumerable<CredentialRequestTestSessionDto>> credentialTestSessionInfo)
        {
            var credentialRequestIds = credentialTestSessionInfo.Keys.Select(x => x.Id).ToList();

            var existingCredentialRequestFields = new Dictionary<int, TestSitting>();
            if (credentialRequestIds.Any())
            {
                existingCredentialRequestFields = NHibernateSession.Current.Query<TestSitting>()
                    .Where(f => credentialRequestIds.Contains(f.CredentialRequest.Id))
                    .ToDictionary(x => x.Id, y => y);
            }

            var detailsToUpsert = new List<object>();
            var detailsToDelete = new List<object>();

            foreach (var credentialTestSession in credentialTestSessionInfo)
            {
                if (credentialTestSession.Value != null)
                {
                    foreach (var credentialFieldData in credentialTestSession.Value)
                    {
                        TestSitting testSitting;
                        if (!existingCredentialRequestFields.TryGetValue(credentialFieldData.CredentialTestSessionId, out testSitting))
                        {
                            testSitting = new TestSitting
                            {
                                CredentialRequest = credentialTestSession.Key,
                                TestSession = NHibernateSession.Current.Load<TestSession>(credentialFieldData.TestSessionId)
                            };
                        }

                        testSitting.Rejected = credentialFieldData.Rejected;
                        testSitting.Sat = credentialFieldData.Sat;
                        testSitting.Supplementary = credentialFieldData.Supplementary;
                        testSitting.TestSpecification = NHibernateSession.Current.Load<TestSpecification>(credentialFieldData.TestSpecificationId);
                        testSitting.AllocatedDate = credentialFieldData.AllocatedDate;
                        testSitting.RejectedDate = credentialFieldData.RejectedDate ?? testSitting.RejectedDate;

                        detailsToUpsert.Add(testSitting);

                        foreach (var testSittingMaterial in credentialFieldData.Materials)
                        {
                            var material = NHibernateSession.Current.Get<TestSittingTestMaterial>(testSittingMaterial.TestSittingTestMaterialId);

                            switch (testSittingMaterial.ObjectStatusId)
                            {
                                case (int)ObjectStatusTypeName.None: continue;
                                case (int)ObjectStatusTypeName.Deleted:
                                    detailsToDelete.Add(material);
                                    break;
                                case (int)ObjectStatusTypeName.Created:
                                case (int)ObjectStatusTypeName.Updated:
                                    material = new TestSittingTestMaterial
                                    {
                                        TestSitting = testSitting,
                                        TestMaterial = NHibernateSession.Current.Get<TestMaterial>(testSittingMaterial.TestMaterialId),
                                        TestComponent = NHibernateSession.Current.Get<TestComponent>(testSittingMaterial.TestTaskId)
                                    };
                                    detailsToUpsert.Add(material);
                                    break;
                                default:
                                    throw new NotSupportedException($"ObjectStatusTypeName {testSittingMaterial.ObjectStatusId}");
                            }

                        }


                    }


                }
            }

            return (detailsToUpsert, detailsToDelete);
        }

        private System.Tuple<List<Note>, List<CredentialApplicationNote>> GetNotesInfoToUpsert(
            IEnumerable<ApplicationNoteData> notesData, CredentialApplication application)
        {
            var existingNotesByNoteId = application.CredentialApplicationNotes?
                                            .Select(x => x.Note)
                                            .ToDictionary(n => n.Id, v => v) ?? new Dictionary<int, Note>();

            var notesToUpsert = new List<Note>();
            var applicationNotesToUpsert = new List<CredentialApplicationNote>();
            foreach (var noteData in notesData)
            {
                Note note;
                if (!existingNotesByNoteId.TryGetValue(noteData.NoteId, out note))
                {
                    note = new Note
                    {
                        CreatedDate = noteData.CreatedDate,
                        Highlight = noteData.Highlight,
                        ReadOnly = noteData.ReadOnly,
                    };

                    applicationNotesToUpsert.Add(new CredentialApplicationNote
                    {
                        CredentialApplication = application,
                        Note = note
                    });
                }

                note.Description = noteData.Description;
                note.User = NHibernateSession.Current.Get<User>(noteData.UserId);
                notesToUpsert.Add(note);
            }

            return new System.Tuple<List<Note>, List<CredentialApplicationNote>>(notesToUpsert, applicationNotesToUpsert);
        }

        private IList<TestComponentResult> GetStandardTestComponentResultsToUpdate(IEnumerable<StandardTestComponentContract> components)
        {
            var results = new List<TestComponentResult>();
            foreach (var component in components ?? Enumerable.Empty<StandardTestComponentContract>())
            {
                var result = NHibernateSession.Current.Get<TestComponentResult>(component.TestComponentResultId);
                if (result != null)
                { // Is standard Marking
                    result.MarkingResultType = NHibernateSession.Current.Get<MarkingResultType>(component.MarkingResultTypeId);
                    results.Add(result);
                }

            }
            return results;
        }

        private IList<RubricTestComponentResult> GetRubricTestComponentResultsToUpdate(IEnumerable<RubricTestComponentContract> components)
        {
            var results = new List<RubricTestComponentResult>();
            foreach (var component in components ?? Enumerable.Empty<RubricTestComponentContract>())
            {
                var result = NHibernateSession.Current.Get<RubricTestComponentResult>(component.RubricTestComponentResultId);
                if (result != null)
                { // Is standard Rubric
                    result.MarkingResultType = NHibernateSession.Current.Get<MarkingResultType>(component.MarkingResultTypeId);
                    results.Add(result);
                }

            }
            return results;
        }

        private System.Tuple<List<Note>, List<NaatiEntityNote>> GetPeersonNotesInfoToUpsert(
            IEnumerable<PersonNoteData> personNotesData, CredentialApplication application)
        {
            var existingNotesByNoteId = NHibernateSession.Current.Query<NaatiEntityNote>()
                                        .Where(x => x.Entity.Id == application.Person.Entity.Id)
                                        .Select(x => x.Note)
                                        .ToDictionary(n => n.Id, v => v);

            var notesToUpsert = new List<Note>();
            var applicationNotesToUpsert = new List<NaatiEntityNote>();
            foreach (var noteData in personNotesData ?? Enumerable.Empty<PersonNoteData>())
            {
                Note note;
                if (!existingNotesByNoteId.TryGetValue(noteData.NoteId, out note))
                {
                    note = new Note
                    {
                        CreatedDate = noteData.CreatedDate,
                        Highlight = noteData.Highlight,
                        ReadOnly = noteData.ReadOnly,
                    };

                    applicationNotesToUpsert.Add(new NaatiEntityNote
                    {
                        Entity = application.Person.Entity,
                        Note = note
                    });
                }

                note.Description = noteData.Description;
                note.User = NHibernateSession.Current.Get<User>(noteData.UserId);
                notesToUpsert.Add(note);
            }

            return new System.Tuple<List<Note>, List<NaatiEntityNote>>(notesToUpsert, applicationNotesToUpsert);
        }

        public GetApplicationFieldsDataResponse GetApplicationFieldsData(int applicationId)
        {
            var application = NHibernateSession.Current.Get<CredentialApplication>(applicationId);

            var applicationFieldDataByFieldTypeId = application.CredentialApplicationFieldsData
                .ToDictionary(x => x.CredentialApplicationField.Id, y => y);

            var applicationFieldsData = application
                .CredentialApplicationType
                .CredentialApplicationFields.Where(f => !f.PerCredentialRequest)
                .Select(fieldType =>
                {
                    CredentialApplicationFieldData fieldData;
                    applicationFieldDataByFieldTypeId.TryGetValue(fieldType.Id, out fieldData);

                    return new CredentialApplicationFieldDataDto
                    {
                        FieldDataId = fieldData?.Id ?? 0,
                        FieldTypeId = fieldType.Id,
                        Name = fieldType.Name,
                        Section = fieldType.Section,
                        DataTypeId = fieldType.DataType.Id,
                        DefaultValue = fieldType.DefaultValue,
                        PerCredentialRequest = fieldType.PerCredentialRequest,
                        Description = fieldType.Description,
                        Disabled = fieldType.Disabled,
                        DisplayOrder = fieldType.DisplayOrder,
                        Value = fieldData != null ? fieldData.Value : fieldType.DefaultValue,
                        FieldOptionId = fieldData?.CredentialApplicationFieldOptionOption?.CredentialApplicationFieldOption?.Id ?? 0,
                        Options = LoadCredentialApplicationFieldOptions(fieldType).ToList()
                    };
                });

            return new GetApplicationFieldsDataResponse { Result = applicationFieldsData.ToList() };
        }

        private IEnumerable<CredentialApplicationFieldOptionDto> LoadCredentialApplicationFieldOptions(CredentialApplicationField fieldType)
        {
            switch (fieldType.DataType.Id)
            {
                case (int)DataTypeName.EndorseInstitutionLookup:
                    var lookupEndorseInstitution = GetEndorsedQualificationsInstition().Results.Select(x => new CredentialApplicationFieldOptionDto
                    {
                        DisplayName = x.DisplayName,
                        FieldOptionId = x.Id
                    });
                    return lookupEndorseInstitution;

                case (int)DataTypeName.EndorsedLocationLookup:
                    var lookupEndorsedLocation = GetEndorsedQualificationsLocation().Results.Select(x => new CredentialApplicationFieldOptionDto
                    {
                        DisplayName = x.DisplayName,
                        FieldOptionId = x.Id
                    });
                    return lookupEndorsedLocation;

                case (int)DataTypeName.EndorsedQualificationLookup:
                    var lookupEndorsedQualifications = GetEndorsedQualifications().Results.Select(x => new CredentialApplicationFieldOptionDto
                    {
                        DisplayName = x.DisplayName,
                        FieldOptionId = x.Id
                    });
                    return lookupEndorsedQualifications;
            }

            var options = fieldType.Options.Select(x => new CredentialApplicationFieldOptionDto
            {
                DisplayName = x.CredentialApplicationFieldOption.DisplayName,
                FieldOptionId = x.CredentialApplicationFieldOption.Id
            });

            return options;
        }

        public GetApplicationResponse GetApplication(int applicationId)
        {
            var application = NHibernateSession.Current.Get<CredentialApplication>(applicationId);
            if(application == null)
            {
                return null;
            }
            return new GetApplicationResponse { Result = CredentialApplicationToDto(application) };
        }

        public GetApplicationResponse GetApplicationByCredentialRequestId(int credentialRequestId)
        {
            var application = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId).CredentialApplication;
            return new GetApplicationResponse { Result = CredentialApplicationToDto(application) };
        }

        private bool GetShowNonPreferredTestLocationFlagByApplication(IList<CredentialRequest> credentialRequests, CredentialApplication application)
        {
            var applicationPreferredTestLocationId = application.PreferredTestLocation?.Id ?? 0;
            foreach (var credentialRequest in credentialRequests)
            {
                var testsittings = credentialRequest.TestSittings.Where(x => !x.Rejected).ToList();
                var showNonPreferredTestLocationInfo = testsittings.Any(s => applicationPreferredTestLocationId != 0 && !s.TestSession.Venue.TestLocation.Id.ToString().Equals(applicationPreferredTestLocationId.ToString()));
                if (showNonPreferredTestLocationInfo)
                {
                    return true;
                }
            }

            return false;
        }

        private bool GetErrorNonPreferredTestLocationFlagByApplication(IList<CredentialRequest> credentialRequests, CredentialApplication application)
        {
            var applicationPreferredTestLocationId = application.PreferredTestLocation?.Id ?? 0;
            var errorNonPreferredTestLocationInfo =
                credentialRequests.Any(x => x.TestSittings.Any(y => !y.Rejected && y.TestSession.Venue.TestLocation.Id.IsNotNull())) &&
                applicationPreferredTestLocationId == 0;

            return errorNonPreferredTestLocationInfo;
        }

        private CredentialApplicationDto CredentialApplicationToDto(CredentialApplication application)
        {
            var person = application.Person;
            var isAustraliaAddress = person.PrimaryAddress?.Country?.Id == 13; // Australia -? Move this to common config;
            var credentialRequests = application.CredentialRequests.ToList();
            var showNonPreferredTestLocationInfo = GetShowNonPreferredTestLocationFlagByApplication(credentialRequests, application);
            var errorNonPreferredTestLocationInfo = GetErrorNonPreferredTestLocationFlagByApplication(credentialRequests, application);
            var recertificationApplication = NHibernateSession.Current.Query<Recertification>()
                .FirstOrDefault(x => x.CredentialApplication.Id == application.Id);
            return new CredentialApplicationDto
            {
                ApplicationId = application.Id,
                ApplicationReference = application.Reference,
                ApplicationTypeId = application.CredentialApplicationType.Id,
                ApplicationStatusTypeId = application.CredentialApplicationStatusType.Id,
                AutoCreated = application.AutoCreated,
                EnteredUserName = application.EnteredUser.FullName,
                EnteredUserId = application.EnteredUser.Id,
                ReceivingOfficeId = application.ReceivingOffice.Id,
                ReceivingInstitutionId = application.ReceivingOffice.Institution.Id,
                ReceivingInstitutionName = application.ReceivingOffice.Institution.InstitutionName,
                ReceivingInstitutionAbbreviation = application.ReceivingOffice.Institution.InstitutionAbberviation,
                StatusChangeUserId = application.StatusChangeUser.Id,
                OwnedByUserId = application.OwnedByUser?.Id,
                SponsorInstitutionId = application.SponsorInstitution?.Id,
                SponsorEntityId = application.SponsorInstitution?.Entity.Id,
                EnteredDate = application.EnteredDate,
                NaatiNumber = person.Entity.NaatiNumber,
                PractitionerNumber = person.PractitionerNumber,
                ApplicationTypeName = application.CredentialApplicationType.DisplayName,
                ApplicationOwner = application.OwnedByUser?.FullName,
                ApplicationStatus = application.CredentialApplicationStatusType.DisplayName,
                StatusChangeDate = application.StatusChangeDate,
                StatusModifiedBy = application.StatusChangeUser.FullName,
                ApplicantGivenName = person.GivenName,
                ApplicantFamilyName = person.Surname,
                ApplicantPrimaryEmail = person.PrimaryEmailAddress,
                OwnedByApplicant = application.OwnedByApplicant,
                PreferredTestLocationId = application.PreferredTestLocation?.Id ?? 0,
                ShowNonPreferredTestLocationInfo = showNonPreferredTestLocationInfo,
                ErrorNonPreferredTestLocationInfo = errorNonPreferredTestLocationInfo,
                SponsorInstitutionNaatiNumber = application.SponsorInstitution?.Entity.NaatiNumber ?? 0,
                SponsorInstitutionName = application.SponsorInstitution?.CurrentName.Name,
                SponsorInstitutionContactPersonId = application.SponsorInstitutionContactPerson?.Id ?? 0,
                SponsorInstitutionContactPersonName = application.SponsorInstitutionContactPerson?.Name ?? string.Empty,
                TrustedInstitutionPayer = application.SponsorInstitution?.TrustedPayer,
                SponsorEmail = application.SponsorInstitution?.Entity.PrimaryEmail?.EmailAddress,
                CertificationPeriodId = recertificationApplication?.CertificationPeriod?.Id ?? 0,
                ApplicationTypeCategory = (CredentialApplicationTypeCategoryName)application.CredentialApplicationType.CredentialApplicationTypeCategory.Id,
                HasAddressInAustralia = isAustraliaAddress
            };
        }

        public CredentialLookupTypeResponse GetCredentialTypesForApplication(int applicationId, int? categoryId)
        {
            var types = NHibernateSession.Current.Get<CredentialApplication>(applicationId)
                .CredentialApplicationType
                .CredentialApplicationTypeCredentialTypes
                .Select(x => x.CredentialType)
                .Where(x => categoryId == null || x.CredentialCategory.Id == categoryId);

            return new CredentialLookupTypeResponse
            {
                //Results = types.OrderBy(x => x.UpgradePath).ThenBy(y => y.ExternalName).Select(x => new CredentialLookupTypeDto
                //{
                //    DisplayName = x.ExternalName,
                //    Id = x.Id,
                //    UpgradePath = x.UpgradePath,
                //    CategoryId = x.CredentialCategory.Id
                //})

                Results = types.OrderBy(x => x.DisplayName).ThenBy(y => y.ExternalName).Select(x => new CredentialLookupTypeDto
                {
                    DisplayName = x.ExternalName,
                    Id = x.Id,
                    DisplayOrder = x.DisplayOrder,
                    CategoryId = x.CredentialCategory.Id
                }).ToList()

            };
        }

        public LookupTypeResponse GetCredentialTypesForApplicationType(int applicationTypeId)
        {
            var types = NHibernateSession.Current.Get<CredentialApplicationType>(applicationTypeId)?
                .CredentialApplicationTypeCredentialTypes?
                .Select(x => x.CredentialType) ?? Enumerable.Empty<CredentialType>();

            return new LookupTypeResponse
            {
                //Results = types.OrderBy(x => x.UpgradePath).ThenBy(y => y.InternalName).Select(x => new LookupTypeDto
                //{
                //    DisplayName = x.InternalName,
                //    Id = x.Id
                //})
                Results = types.OrderBy(x => x.DisplayOrder).ThenBy(y => y.InternalName).Select(x => new LookupTypeDto
                {
                    DisplayName = x.InternalName,
                    Id = x.Id
                })
            };
        }



        public GetSkillsForCredentialTypeResponse GetSkillsForCredentialType(GetSkillsForCredentialTypeRequest request)
        {
            List<int> applicationTypeIds = new List<int>();
            if (request.CredentialApplicationTypeIds != null)
            {
                applicationTypeIds = request.CredentialApplicationTypeIds.ToList();
            }

            if (request.ApplicationId != null)
            {
                var application = NHibernateSession.Current.Load<CredentialApplication>(request.ApplicationId);
                applicationTypeIds.Add(application.CredentialApplicationType.Id);
            }

            var credentialsTypes = NHibernateSession.Current.Query<CredentialType>().Where(c => request.CredentialTypeIds.Contains(c.Id));
            var skillApplicationTypes = NHibernateSession.Current.Query<SkillApplicationType>()
                .Where(ca => applicationTypeIds.Contains(ca.CredentialApplicationType.Id));
            var skills = new Dictionary<int, IEnumerable<Skill>>();
            var includedSkillIds = skillApplicationTypes.Select(x => x.Skill.Id).ToList();

            foreach (var ct in credentialsTypes)
            {
                skills.Add(ct.Id, NHibernateSession.Current.Query<Skill>()
                    .Where(x => x.SkillType.Id == ct.SkillType.Id &&
                                (includedSkillIds.Contains(x.Id) || !applicationTypeIds.Any())));
            }

            var results = new List<SkillLookupDto>();
            foreach (var key in skills)
            {
                results.AddRange(key.Value.Select(v => new SkillLookupDto
                {
                    Id = v.Id,
                    DisplayName = v.DisplayName,
                    CredentialTypeId = key.Key,
                    Language1Id = v.Language1.Id,
                    Language2Id = v.Language2.Id,
                    DirectionTypeId = v.DirectionType.Id,
                }));
            }

            return new GetSkillsForCredentialTypeResponse
            {
                Results = results
            };
        }

        public GetTestTaskResponse GetTestTask(GetTestTaskRequest request)
        {
            var credentialsType = NHibernateSession.Current.Query<CredentialType>().Where(c => request.CredentialTypeIds.Contains(c.Id)).ToList();
            var components = credentialsType.SelectMany(ct => ct.TestSpecifications).SelectMany(ts => ts.TestComponentTypes).Distinct().Select(tc => new TestTaskLookupTypeDto
            {
                Id = tc.Id,
                DisplayName = tc.Name,
                TestComponentBaseTypeId = tc.TestComponentBaseType.Id,
                TestSpecificationId = tc.TestSpecification.Id,
                Active = tc.TestSpecification.Active,
                CandidateBriefRequired = tc.CandidateBriefRequired,
                DefaultMaterialRequestHours = tc.DefaultMaterialRequestHours
            });
            return new GetTestTaskResponse { Results = components };
        }

        public GetSkillsDetailsResponse GetSkillsDetailsForCredentialType(GetSkillsDetailsRequest request)
        {
            var credentialTypes = NHibernateSession.Current.Query<CredentialType>().Where(ct => request.CredentialTypeIds.Contains(ct.Id));

            var skillTypes = credentialTypes.Select(ct => ct.SkillType.Id).Distinct().ToList();
            var skills = NHibernateSession.Current.Query<Skill>()
                .Where(x => skillTypes.Contains(x.SkillType.Id));


            if (request.Language1Id.HasValue)
            {
                skills = skills.Where(x => x.Language1.Id == request.Language1Id.Value);
            }

            //skills may not have been defined for credential type. The UI works but this code was missing a filter
            //to support the System.Skills.Languages feature
            //this code filters out any Skills not defined.
            var credentialType = credentialTypes.First().CredentialApplicationTypeCredentialTypes.FirstOrDefault();
            if (credentialType != null)
            { 
                CredentialApplicationTypeCredentialType credentialApplicationType = null;

                //this next bit is to do with inactive forms. Currently this is just CCL.
                //We only want CredentialApplicationTypeCredentialTypes that are listed in Credenial Application Form as active. There are two forms in CCl that are inactive
                //These next lines of code could be more concise but this way it helps to understand beter.

                var credentialApplicationTypeCredentialTypes = credentialTypes.First().CredentialApplicationTypeCredentialTypes.ToList();

                foreach(var type in credentialApplicationTypeCredentialTypes)
                {
                    //chec for inactive form
                    var credentialApplicationFormsForApplicationType = NHibernateSession.Current.Query<CredentialApplicationForm>().Where(x => x.Inactive == false && x.CredentialApplicationType.Id == type.CredentialApplicationType.Id).FirstOrDefault();
                    if(credentialApplicationFormsForApplicationType != null)
                    {
                        //only include if form is active
                        credentialApplicationType = type;
                    }
                }    
                

                if (credentialApplicationType != null)
                {
                    var credentialApplicationTypeId = credentialApplicationType.CredentialApplicationType.Id;
                    //Only take skills where the credential has it specified in tblSkillApplicationType
                    var filteredSkills = new List<Skill>();
                    foreach (var skill in skills)
                    {

                        var result = skill.SkillApplicationTypes.FirstOrDefault(x => x.CredentialApplicationType.Id == credentialApplicationTypeId);
                        if (result != null)
                        {
                            filteredSkills.Add(skill);
                        }
                    }
                    skills = filteredSkills.AsQueryable();
                }
            }

            skills = skills.Distinct();
            var data = skills.ToList().OrderBy(x => x.DisplayName);
            return new GetSkillsDetailsResponse
            {
                Results = data.Select(x =>
                {
                    var groupName = x.Language1.LanguageGroup != null ? " (" + x.Language1.LanguageGroup.Name + ")" : string.Empty;
                    return new SkillDto { Id = x.Id, Language1Id = x.Language1.Id, Language2Id = x.Language2.Id, Language2Name = $"{x.Language2.Name}{groupName}", Language1Name = $"{x.Language1.Name}{groupName}", DisplayName = x.DisplayName };
                }).ToList()
            };
        }

        public GetUserResponse GetUser(GetUserRequest request)
        {
            var user = NHibernateSession.Current.Query<User>().FirstOrDefault(x => x.UserName == request.UserName);
            return new GetUserResponse
            {
                UserId = user?.Id,
                OfficeId = user?.Office?.Id
            };
        }

        public CredentialLookupTypeResponse GetCredentialTypesForApplicationForm(int applicatonFormId)
        {
            var types = NHibernateSession.Current.Get<CredentialApplicationForm>(applicatonFormId)?
                .CredentialApplicationType
                .CredentialApplicationTypeCredentialTypes
                .Select(x => x.CredentialType);

            return new CredentialLookupTypeResponse
            {
                Results = types?.Select(x => new CredentialLookupTypeDto
                {
                    DisplayName = x.InternalName,
                    Id = x.Id,
                    DisplayOrder = x.DisplayOrder,
                    CategoryId = x.CredentialCategory.Id,
                })
            };
        }

        public GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId)
        {
            var applicationCredentialTypes = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id == applicationId && x.CredentialRequestStatusType.Id != (int)CredentialRequestStatusTypeName.Deleted)
                .Select(y => y.CredentialType.Id).Distinct().ToList();

            var query = (from a in NHibernateSession.Current.Query<EndorsedQualification>()
                         where applicationCredentialTypes.Contains(a.CredentialType.Id)
                            && a.Active == true
                         select a
                         ).ToList();

            return new GetEndorsedQualificationForApplicationFormResponse
            {
                Results = query?.Select(x => new EndorsedQualificationDto
                {
                    EndorsedQualificationId = x.Id,
                    InstitutionId = x.Institution.Id,
                    CredentialTypeId = x.CredentialType.Id,
                    CredentialTypeExternalName = x.CredentialType.ExternalName,
                    Qualification = x.Qualification,
                    Institution = x.Institution.InstitutionName,
                    Location = x.Location,
                    Notes = x.Notes,
                    EndorsementPeriodFrom = x.EndorsementPeriodFrom,
                    EndorsementPeriodTo = x.EndorsementPeriodTo,
                    Active = x.Active,
                })
            };
        }

        public LookupTypeResponse GetCredentialCategoriesForApplicationType(int applicationId)
        {
            var categories = NHibernateSession.Current.Get<CredentialApplication>(applicationId)?
                                 .CredentialApplicationType
                                 .CredentialApplicationTypeCredentialTypes
                                 .Select(x => x.CredentialType.CredentialCategory)
                                 .GroupBy(x => x.Id)
                                 .Select(x => x.First()) ?? Enumerable.Empty<CredentialCategory>();

            return new LookupTypeResponse
            {
                Results = categories?.Select(x => new LookupTypeDto
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id
                }).ToList()
            };
        }


        private void AddCredentialTypesToList(List<CredentialLookupTypeDto> list,
            IEnumerable<CredentialLookupTypeDto> credentialTypeDtos)
        {
            foreach (var credentialType in credentialTypeDtos)
            {
                if (list.All(x => x.Id != credentialType.Id))
                {
                    list.Add(credentialType);
                }
            }
        }

        public CredentialLookupTypeResponse GetAvailableCredentials(int applicationId, int naatiNumber)
        {
            var credentials = GetCurrentCertificationCredentials(naatiNumber).ToList();
            var higherCredentialTypes = GetHigherCredentialTypes(credentials, applicationId);
            var lowerCredentialTypes = GetLowerCredentialTypes(credentials, applicationId);
            var crossCredentialTypes = GetCrossCredentialTypes(credentials, applicationId);

            var currentTypes = credentials.Select(x =>
            {
                var credentialRequest = x.CredentialCredentialRequests.First().CredentialRequest;
                return new CredentialLookupTypeDto
                {
                    Id = credentialRequest.CredentialType.Id,
                    DisplayName = credentialRequest.CredentialType.ExternalName,
                    DisplayOrder = credentialRequest.CredentialType.DisplayOrder,
                    CategoryId = credentialRequest.CredentialType.CredentialCategory.Id,
                    CredentialRequestPathTypeId = (int)CredentialRequestPathTypeName.NewSkill
                };
            }).ToList();

            var upgradeTypes = higherCredentialTypes.Select(x => new CredentialLookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.ExternalName,
                DisplayOrder = x.DisplayOrder,
                CategoryId = x.CredentialCategory.Id,
                CredentialRequestPathTypeId = (int)CredentialRequestPathTypeName.Upgrade
            }).ToList();

            var lowerTypes = lowerCredentialTypes.Select(x => new CredentialLookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.ExternalName,
                DisplayOrder = x.DisplayOrder,
                CategoryId = x.CredentialCategory.Id,
                CredentialRequestPathTypeId = (int)CredentialRequestPathTypeName.NewSkill
            }).ToList();

            var crossTypes = crossCredentialTypes.Select(x => new CredentialLookupTypeDto
            {
                Id = x.Id,
                DisplayName = x.ExternalName,
                DisplayOrder = x.DisplayOrder,
                CategoryId = x.CredentialCategory.Id,
                CredentialRequestPathTypeId = (int)CredentialRequestPathTypeName.New
            }).ToList();


            var allTypes = new List<CredentialLookupTypeDto>();
            AddCredentialTypesToList(allTypes, crossTypes);
            AddCredentialTypesToList(allTypes, lowerTypes);
            AddCredentialTypesToList(allTypes, currentTypes);
            AddCredentialTypesToList(allTypes, upgradeTypes);

            var finalQuery = from u in allTypes
                             group u by new { Id = u.CategoryId, CategoryName = NHibernateSession.Current.Get<CredentialCategory>(u.CategoryId).DisplayName } into category
                             select new CredentialLookupTypeDto
                             {
                                 Id = category.Key.Id,
                                 DisplayName = category.Key.CategoryName,
                                 Children = (from c in category orderby c.DisplayOrder select c)
                             };

            var result = finalQuery.ToList();
            return new CredentialLookupTypeResponse
            {
                Results = result
            };
        }

        public GetSkillsForCredentialTypeResponse GetAdditionalSkills(GetCredentialTypeSkillsRequest request)
        {
            var application = NHibernateSession.Current.Load<CredentialApplication>(request.ApplicationId);
            var skills = application.CredentialApplicationType.Skills.Select(s => s.Skill.Id).ToList();
            skills.Add(0);

            var ids = request.CredentialTypeIds.Union(new[] { 0 }).ToList();

            var resultSkills = new Dictionary<int, IEnumerable<Skill>>();
            var credentialsType = NHibernateSession.Current.Query<CredentialType>().Where(c => ids.Contains(c.Id));

            var currentCredentialIds = GetCurrentCertificationCredentials(request.NAATINumber)
                .Select(x => x.Id)
                .ToList();

            var skillQuery = NHibernateSession.Current.Query<SkillApplicationType>().Where(c => ids.Contains(c.CredentialApplicationType.Id));
            foreach (var ct in credentialsType)
            {
                IQueryable<Skill> query;

                if (request.CredentialRequestPathTypeId == (int)CredentialRequestPathTypeName.Upgrade)
                {
                    query = from upgradePath in NHibernateSession.Current.Query<CredentialTypeUpgradePath>()
                            from credential in NHibernateSession.Current.Query<Credential>()
                            from credentialCredentialRequest in credential.CredentialCredentialRequests
                            from skill in NHibernateSession.Current.Query<Skill>()
                            where upgradePath.CredentialTypeTo.Id == ct.Id
                                  && skill.SkillType.Id == ct.SkillType.Id
                                  && skills.Contains(skill.Id)
                                  && currentCredentialIds.Contains(credential.Id)
                                  && upgradePath.CredentialTypeTo.Id == ct.Id
                                  && upgradePath.CredentialTypeFrom.Id == credentialCredentialRequest.CredentialRequest.CredentialType.Id
                                  && skill.Language1.Id == credentialCredentialRequest.CredentialRequest.Skill.Language1.Id
                                  && skill.Language2.Id == credentialCredentialRequest.CredentialRequest.Skill.Language2.Id
                                  && ((skill.DirectionType.Id == credentialCredentialRequest.CredentialRequest.Skill.DirectionType.Id && upgradePath.MatchDirection) || !upgradePath.MatchDirection)
                            select skill;
                }
                else
                {
                    query = from skill in NHibernateSession.Current.Query<Skill>()
                            where skill.SkillType.Id == ct.SkillType.Id
                                  && skills.Contains(skill.Id)
                                  && (from credential in NHibernateSession.Current.Query<Credential>()
                                      from credentialCredentialRequest in credential.CredentialCredentialRequests
                                      where credentialCredentialRequest.CredentialRequest.CredentialType.Id == ct.Id
                                            && currentCredentialIds.Contains(credential.Id)
                                            && credentialCredentialRequest.CredentialRequest.Skill.Id == skill.Id
                                      select 1).Count() == 0
                            select skill;

                }


                resultSkills.Add(ct.Id, query);
            }

            var results = new List<SkillLookupDto>();
            foreach (var key in resultSkills)
            {
                results.AddRange(key.Value.Select(v => new SkillLookupDto
                {
                    Id = v.Id,
                    DisplayName = v.DisplayName,
                    CredentialTypeId = key.Key,
                    Language1Id = v.Language1.Id,
                    Language2Id = v.Language2.Id
                }));
            }

            return new GetSkillsForCredentialTypeResponse
            {
                Results = results
            };
        }

        private IEnumerable<CredentialType> GetHigherCredentialTypes(IEnumerable<Credential> credentials, int applicationId)
        {
            var applicationTypes = NHibernateSession.Current.Get<CredentialApplication>(applicationId)
                .CredentialApplicationType
                .CredentialApplicationTypeCredentialTypes;

            var credentialTypes = credentials.SelectMany(c => c.CredentialCredentialRequests)
                .Select(c => c.CredentialRequest.CredentialType.Id)
                .ToList();

            if (!credentialTypes.Any())
            {
                return Enumerable.Empty<CredentialType>();
            }

            var nextPaths = NHibernateSession.Current.Query<CredentialTypeUpgradePath>()
                .Where(c => credentialTypes.Contains(c.CredentialTypeFrom.Id))
                .Select(c => c.CredentialTypeTo.Id)
                .ToList();

            return applicationTypes
                .Where(c => nextPaths.Contains(c.CredentialType.Id))
                .Select(c => c.CredentialType)
                .ToList();
        }

        private IEnumerable<CredentialType> GetLowerCredentialTypes(IEnumerable<Credential> credentials, int applicationId)
        {
            var applicationTypes = NHibernateSession.Current.Get<CredentialApplication>(applicationId)
                .CredentialApplicationType
                .CredentialApplicationTypeCredentialTypes;

            var credentialTypes = credentials.SelectMany(c => c.CredentialCredentialRequests)
                .Select(c => c.CredentialRequest.CredentialType.Id)
                .ToList();

            if (!credentialTypes.Any())
            {
                return Enumerable.Empty<CredentialType>();
            }

            var previousPaths = new List<int>();
            FindLowerCredentialTypes(previousPaths, credentialTypes);

            return applicationTypes
                .Where(c => previousPaths.Contains(c.CredentialType.Id))
                .Select(c => c.CredentialType)
                .ToList();
        }

        private void FindLowerCredentialTypes(List<int> foundCredentialTypes, List<int> credentialTypeIds)
        {
            var previousPaths = NHibernateSession.Current.Query<CredentialTypeUpgradePath>()
                .Where(c => credentialTypeIds.Contains(c.CredentialTypeTo.Id))
                .Select(c => c.CredentialTypeFrom.Id)
                .ToList();

            var newPathsFound = previousPaths.Where(x => !foundCredentialTypes.Contains(x)).ToList();

            if (newPathsFound.Any())
            {
                foundCredentialTypes.AddRange(newPathsFound);
                FindLowerCredentialTypes(foundCredentialTypes, newPathsFound);
            }
        }

        private IEnumerable<CredentialType> GetCrossCredentialTypes(IEnumerable<Credential> credentials, int applicationId)
        {
            var applicationTypes = NHibernateSession.Current.Get<CredentialApplication>(applicationId)
                .CredentialApplicationType
                .CredentialApplicationTypeCredentialTypes;

            var credentialTypes = credentials.SelectMany(c => c.CredentialCredentialRequests)
                .Select(c => c.CredentialRequest.CredentialType.Id)
                .ToList();

            if (!credentialTypes.Any())
            {
                return Enumerable.Empty<CredentialType>();
            }

            var crossCredentialType = NHibernateSession.Current.Query<CredentialTypeCrossSkill>()
                .Where(c => credentialTypes.Contains(c.CredentialTypeFrom.Id))
                .Select(c => c.CredentialTypeTo.Id)
                .ToList();

            return applicationTypes
                .Where(c => crossCredentialType.Contains(c.CredentialType.Id))
                .Select(c => c.CredentialType)
                .ToList();
        }

        private IEnumerable<Credential> GetCurrentCertificationCredentials(int naatiNumber)
        {
            var queryHelper = new ApplicationQueryHelper();

            var credentials = NHibernateSession.Current.Query<Credential>()
                .Where(c => c.CertificationPeriod.Person.Entity.NaatiNumber == naatiNumber &&
                            c.CertificationPeriod.Id > 0).ToList();

            var activeCredentials = queryHelper
                .GetCredentialStatusesByCredentialIds(credentials.Select(x => x.Id).ToList())
                .Where(y => y.Value == CredentialStatusTypeName.Active).ToList();


            return credentials.Where(x => activeCredentials.Any(y => y.Key == x.Id)).ToList();
        }

        public LookupTypeResponse GetVenue(IEnumerable<int> testLocationId)
        {
            var ids = testLocationId.Union(new[] { 0 }).ToList();
            var venue = from v in NHibernateSession.Current.Query<Venue>()
                        where ids.Contains(v.TestLocation.Id)
                        select new LookupTypeDto
                        {
                            Id = v.Id,
                            DisplayName = v.Name,
                            Name = v.TestLocation.Name
                        };

            return new LookupTypeResponse { Results = venue.OrderBy(x => x.DisplayName) };
        }

        public LookupTypeResponse GetVenuesShowingInactive(IEnumerable<int> testLocationId)
        {
            var inactiveTag = " (Inactive)";
            var ids = testLocationId.Union(new[] { 0 }).ToList();
            var venue = (from v in NHibernateSession.Current.Query<Venue>()
                        where ids.Contains(v.TestLocation.Id)
                        select new LookupTypeDto
                        {
                            Id = v.Id,
                            DisplayName = $"{v.Name} {(v.Inactive? inactiveTag : "")}",
                            Name = v.TestLocation.Name
                        }).ToList<LookupTypeDto>();


            var orderedCurrentVenues = venue.Where(x => !x.DisplayName.Contains(inactiveTag)).Select(x => x).OrderBy(x => x.DisplayName).ToList();
            var orderedInactiveVenues = venue.Where(x => x.DisplayName.Contains(inactiveTag)).Select(x => x).OrderBy(x => x.DisplayName).ToList();
            orderedCurrentVenues.AddRange(orderedInactiveVenues);
            return new LookupTypeResponse { Results = orderedCurrentVenues };
        }

        public GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForApplicationType(int applicationId)
        {
            var names = NHibernateSession.Current.Get<CredentialApplication>(applicationId)?
                .CredentialApplicationType
                .CredentialApplicationTypeDocumentTypes
                .Select(x => x.DocumentType.Name);

            return new GetDocumentTypesForApplicationTypeResponse
            {

                Results = names
            };

        }

        public GetApplicationAttachmentsResponse GetAttachments(GetApplicationAttachmentsRequest request)
        {
            var query = NHibernateSession.Current.Query<CredentialApplicationAttachment>();

            var attachments = query.Where(n => n.CredentialApplication.Id == request.ApplicationId).ToList().Select(n => new CredentialApplicationAttachmentDto
            {
                CredentialApplicationAttachmentId = n.Id,
                CredentialApplicationId = n.CredentialApplication.Id,
                StoredFileId = n.StoredFile.Id,
                FileName = n.StoredFile.FileName,
                Description = n.Description,
                DocumentType = n.StoredFile.DocumentType.DisplayName,
                UploadedByName = n.StoredFile.UploadedByUser.FullName,
                UploadedByUserId = n.StoredFile.UploadedByUser.Id,
                UploadedDateTime = n.StoredFile.UploadedDateTime,
                FileSize = n.StoredFile.FileSize,
                Type = (StoredFileType)n.StoredFile.DocumentType.Id,
                SoftDeleteDate = n.StoredFile.StoredFileStatusType.Name != "Current" ? n.StoredFile.StoredFileStatusChangeDate : null
            });

            return new GetApplicationAttachmentsResponse
            {
                Attachments = attachments.ToArray()
            };
        }

        public CreateOrReplaceApplicationAttachmentResponse CreateOrReplaceAttachment(CreateOrReplaceApplicationAttachmentRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);

            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = request.StoredFileId != 0 ? (int?)request.StoredFileId : null,
                UpdateFileName = request.StoredFileId != 0 ? request.FileName : null,
                Type = request.Type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
                TokenToRemoveFromFilename = request.TokenToRemoveFromFilename
            });

            var storeFile = NHibernateSession.Current.Load<StoredFile>(response.StoredFileId);
            var application = NHibernateSession.Current.Load<CredentialApplication>(request.CredentialApplicationId);
            var applicationAttachment = NHibernateSession.Current.Query<CredentialApplicationAttachment>().FirstOrDefault(n => n.StoredFile.Id == response.StoredFileId);

            if (applicationAttachment == null || request.PrerequisiteApplicationDocument == true)
            {
                applicationAttachment = new CredentialApplicationAttachment
                {
                    StoredFile = storeFile,
                    CredentialApplication = application,
                };
            }

            if (request.Title.IsNull() && request.FileName.IsNotNull())
            {
                request.Title = System.IO.Path.GetFileNameWithoutExtension(request.FileName);
            }
           
            applicationAttachment.Description = request.Title;

            NHibernateSession.Current.Save(applicationAttachment);
            NHibernateSession.Current.Flush();

            return new CreateOrReplaceApplicationAttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public DeleteApplicationAttachmentResponse DeleteAttachment(DeleteApplicationAttachmentRequest request)
        {
            var attachment = NHibernateSession.Current.Query<CredentialApplicationAttachment>().SingleOrDefault(n => n.StoredFile.Id == request.StoredFileId);
            NHibernateSession.Current.Delete(attachment);
            NHibernateSession.Current.Flush();

            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            fileService.DeleteFile(new DeleteFileRequest
            {
                StoredFileId = request.StoredFileId
            });

            return new DeleteApplicationAttachmentResponse();
        }

        public CreateCredentialCertificateResponse CreateCredentialDocuments(CreateCredentialDocumentsRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);

            var storedFileIds = new List<int>();

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                //File uploaded dates are used in comparisons and files uploaded at the same time must have the same uploaded date.
                var batchDateTime = DateTime.Now;

                foreach (var document in request.Documents)
                {
                    var createFileRequest = new CreateOrUpdateFileRequest
                    {
                        UpdateStoredFileId = null,
                        UpdateFileName = null,
                        Type = document.StoredFileType,
                        StoragePath = $"{document.StoredFileType}/{request.ApplicationId}/{Path.GetFileName(document.FilePath)}",
                        UploadedByUserId = request.UserId,
                        UploadedDateTime = batchDateTime,
                        FilePath = document.FilePath,
                        //FileTitle = document.Title
                    };

                    var response = fileService.CreateOrUpdateFile(createFileRequest);

                    var credentialAttachment = new CredentialAttachment
                    {
                        Credential = NHibernateSession.Current.Get<Credential>(request.CredentialId),
                        StoredFile = NHibernateSession.Current.Get<StoredFile>(response.StoredFileId),
                        DocumentNumber = document.DocumentNumber
                    };
                    NHibernateSession.Current.Save(credentialAttachment);

                    storedFileIds.Add(response.StoredFileId);
                }

                transaction.Commit();
            }

            return new CreateCredentialCertificateResponse { StoredFileIds = storedFileIds };
        }

        private int GetTotalCredentialDocumentFor(int credentialId)
        {
            return NHibernateSession.Current.Query<CredentialAttachment>()
                .Count(x => x.Credential.Id == credentialId);
        }

        private StoredFileType GetOutputDocumentTypeFor(StoredFileType documentType)
        {
            switch (documentType)
            {
                case StoredFileType.CredentialLetterTemplate:
                    return StoredFileType.CredentialLetter;
                case StoredFileType.CertificateTemplate:
                    return StoredFileType.Certificate;
                default:
                    throw new Exception($"Output Document Type not specifified for DocumenType {documentType} ");
            }
        }

        private string GetFile(int storageFileId, string temporalPath)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.GetFile(new GetFileRequest
            {
                StoredFileId = storageFileId,
                TempFileStorePath = temporalPath
            });

            return response.FilePaths.First();
        }

        public GetCredentialTypeTemplateResponse GetCredentialTypeTemplates(GetCredentialTypeTemplateRequest request)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(request.CredentialTypeId);

            var credentialTypeTemplates = credentialType.CredentialTypeTemplates.ToList();

            if (!credentialTypeTemplates.Any())
            {
                throw new Exception($"No credential template found for credentialType {credentialType.InternalName}");
            }

            var dtos = credentialTypeTemplates.Select(template =>
            {
                var templateDocumentType =
                    (StoredFileType)Enum.Parse(typeof(StoredFileType), template.StoredFile.DocumentType.Name);

                var outputDocumentType = GetOutputDocumentTypeFor(templateDocumentType);
                return new CredentialTypeTemplateDto
                {
                    TemplateDocumentType = templateDocumentType,
                    OutputDocumentType = outputDocumentType,
                    DocumentNameTemplate = template.DocumentNameTemplate,
                    FilePath = GetFile(template.StoredFile.Id, request.TempFilePath)
                };
            }).ToList();

            SetDocumentNumbers(dtos, request.CredentialId);
            return new GetCredentialTypeTemplateResponse { Results = dtos };
        }

        private void SetDocumentNumbers(IEnumerable<CredentialTypeTemplateDto> templateDtos, int credentialId)
        {
            var totalDocuments = GetTotalCredentialDocumentFor(credentialId);

            foreach (var template in templateDtos)
            {
                totalDocuments++;
                template.NextDocumentIdentifier = totalDocuments;
            }
        }

        public GetEmailTemplateResponse GetEmailTemplate(GetEmailTemplateRequest request)
        {
            var templates = new List<EmailTemplateDetailDto>();
            var response = new GetEmailTemplateResponse { Data = templates };



            var query = NHibernateSession.Current
                .Query<CredentialWorkflowActionEmailTemplate>()
                .Where(x => x.CredentialApplicationType.Id == (int)request.ApplicationType
                            && x.SystemActionEmailTemplate.SystemActionType.Id == (int)request.Action && x.SystemActionEmailTemplate.Active);

            if (request.ActionEvents != null)
            {
                var eventsIds = request.ActionEvents?.Select(x => (int)x).ToList();
                query = query.Where(x => eventsIds.Contains(x.SystemActionEmailTemplate.SystemActionEventType.Id));
            }

            var mappings = query.ToList();

            if (!mappings.Any())
            {
                response.Error = true;
                response.ErrorMessage = $"Could not find an email template for the Application Type/Action combination of {request.ApplicationType}/{request.Action}.";
            }
            else
            {
                foreach (var template in mappings)
                {

                    var emailTemplate = _autoMapperHelper.Mapper.Map<EmailTemplateDetailDto>(template.SystemActionEmailTemplate.EmailTemplate);
                    emailTemplate.EmailTemplateDetails = template.SystemActionEmailTemplate.EmailTemplateDetails.Select(x => (EmailTemplateDetailTypeName)x.Id);
                    emailTemplate.SystemActionEventType = (SystemActionEventTypeName)template.SystemActionEmailTemplate.SystemActionEventType.Id;
                    templates.Add(emailTemplate);
                }

            }
            return response;
        }

        public CreateOrUpdateCredentialResponse CreateOrUpdateCredential(CreateOrUpdateCredentialRequest request)
        {
            var response = new CreateOrUpdateCredentialResponse();
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(request.CredentialRequestId);

                CertificationPeriod certificationPeriod = null;
                Credential credential = null;
                CredentialCredentialRequest credentialCredentialRequest = null;

                if (request.CertificationPeriod != null)
                {
                    if (request.CertificationPeriod.Id > 0)
                    {
                        certificationPeriod = NHibernateSession.Current.Load<CertificationPeriod>(request.CertificationPeriod.Id);
                    }
                    else
                    {
                        certificationPeriod = new CertificationPeriod
                        {
                            StartDate = request.CertificationPeriod.StartDate,
                            EndDate = request.CertificationPeriod.EndDate,
                            OriginalEndDate = request.CertificationPeriod.OriginalEndDate
                        };

                        if (certificationPeriod.StartDate.Year < 1900 || certificationPeriod.EndDate.Year < 1900)
                        {
                            throw new Exception("Certification period is out of range.");
                        }
                        certificationPeriod.Person = credentialRequest.CredentialApplication.Person;
                        certificationPeriod.CredentialApplication = NHibernateSession.Current.Load<CredentialApplication>(request.CertificationPeriod.CredentialApplicationId);
                        NHibernateSession.Current.Save(certificationPeriod);
                    }
                }

                request.CertificationPeriod = null;

                if (request.CredentialId != 0)
                {
                    credential = NHibernateSession.Current.Get<Credential>(request.CredentialId);
                    credentialCredentialRequest = new CredentialCredentialRequest
                    {
                        Credential = credential,
                        CredentialRequest = credentialRequest
                    };
                }
                else
                {
                    credentialCredentialRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                                                      .FirstOrDefault(v => v.CredentialRequest.Id == credentialRequest.Id) ??
                                                  new CredentialCredentialRequest { CredentialRequest = credentialRequest, Credential = new Credential() };
                    credential = credentialCredentialRequest.Credential;
                }

                credential.CertificationPeriod = certificationPeriod;
                credential.StartDate = request.StartDate;
                credential.ExpiryDate = request.ExpiryDate;
                credential.ShowInOnlineDirectory = request.ShowInOnlineDirectory;
                credential.TerminationDate = request.TerminationDate;

                NHibernateSession.Current.SaveOrUpdate(credential);
                NHibernateSession.Current.SaveOrUpdate(credentialCredentialRequest);
                transaction.Commit();

                response.Data = _autoMapperHelper.Mapper.Map<CredentialDto>(credential);
                response.Data.SkillDisplayName = credentialCredentialRequest.CredentialRequest.Skill.DisplayName;
                response.Data.CredentialTypeInternalName = credentialCredentialRequest.CredentialRequest.CredentialType.InternalName;
            }
            return response;
        }

        public void RollbackIssueCredential(RollbackIssueCredentialRequest request)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(request.CredentialRequestId);
            if (credentialRequest != null)
            {
                credentialRequest.CredentialRequestStatusType = NHibernateSession.Current.Get<CredentialRequestStatusType>(request.CredentialRequestOriginalStatusId);
                credentialRequest.StatusChangeUser = NHibernateSession.Current.Get<User>(request.CredentialRequestOriginalStatusUserId);
                credentialRequest.StatusChangeDate = request.CredentialRequestOriginalStatusDate;
                NHibernateSession.Current.Save(credentialRequest);
            }

            var application = NHibernateSession.Current.Get<CredentialApplication>(request.ApplicationId);
            application.CredentialApplicationStatusType = NHibernateSession.Current.Get<CredentialApplicationStatusType>(request.ApplicationOriginalStatusId);
            application.StatusChangeUser = NHibernateSession.Current.Get<User>(request.ApplicationOriginalStatusUserId);
            application.StatusChangeDate = request.ApplicationOriginalStatusDate;
            NHibernateSession.Current.Save(application);

            var credentialCredentialRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .FirstOrDefault(x => x.CredentialRequest.Id == request.CredentialRequestId);

            var foundCredential = credentialCredentialRequest?.Credential;

            if (request.Credential != null && foundCredential != null)
            {
                foundCredential.ExpiryDate = request.Credential.ExpiryDate;
                foundCredential.CertificationPeriod =
                    NHibernateSession.Current.Get<CertificationPeriod>(request.Credential.CertificationPeriod?.Id ?? 0);
                foundCredential.StartDate = request.Credential.StartDate;
                NHibernateSession.Current.Update(foundCredential);
            }
            else if (request.Credential == null && foundCredential != null)
            {
                NHibernateSession.Current.Delete(credentialCredentialRequest);
                NHibernateSession.Current.Delete(foundCredential);
            }

            NHibernateSession.Current.Flush();

            if (request.StoredFileIds != null)
            {
                var storedIds = request.StoredFileIds.Concat(new[] { 0 }).ToList();

                var credentialAttachments = NHibernateSession.Current.Query<CredentialAttachment>()
                    .Where(x => storedIds.Contains(x.StoredFile.Id));

                credentialAttachments.ForEach(NHibernateSession.Current.Delete);

                var emailAttachments = NHibernateSession.Current.Query<EmailMessageAttachment>()
                    .Where(x => storedIds.Contains(x.StoredFile.Id));

                emailAttachments.ForEach(NHibernateSession.Current.Delete);

                var fileService = new FileSystemFileStorageService(_autoMapperHelper);
                foreach (var storedFileId in request.StoredFileIds)
                {
                    fileService.DeleteFile(new DeleteFileRequest
                    {
                        StoredFileId = storedFileId
                    });
                }
            }
        }

        public GetCredentialAttachmentsByIdResponse GetCredentialAttachmentsById(GetCredentialAttachmentsByIdRequest request)
        {

            var result = from credentialAttachment in NHibernateSession.Current.Query<CredentialAttachment>()
                         join credential in NHibernateSession.Current.Query<Credential>() on credentialAttachment.Credential.Id equals credential.Id
                         join storeFile in NHibernateSession.Current.Query<StoredFile>() on credentialAttachment.StoredFile.Id equals storeFile.Id
                         join credentialCredentialRequest in NHibernateSession.Current.Query<CredentialCredentialRequest>() on credential.Id equals credentialCredentialRequest.Credential.Id
                         join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on credentialCredentialRequest.CredentialRequest.Id equals credentialRequest.Id
                         where (credentialRequest.CredentialApplication.Person.Entity.NaatiNumber == request.NaatiNumber && credential.Id == request.CredentialId)
                         select new CredentialAttachmentDto
                         {
                             CredentialId = credential.Id,
                             CredentialAttachmentId = credentialAttachment.Id,
                             StoredFileId = storeFile.Id,
                             FileName = storeFile.FileName,
                             DocumentType = storeFile.DocumentType.DisplayName,
                             UploadedByName = storeFile.UploadedByUser.FullName,
                             UploadedDateTime = storeFile.UploadedDateTime,
                             FileSize = storeFile.FileSize,
                             FilePath = storeFile.ExternalFileId
                         };

            return new GetCredentialAttachmentsByIdResponse
            {
                Attachments = result.ToArray()
            };
        }

        public GetCredentialAttachmentFileResponse GetCredentialAttachmentFileByCredentialAttachmentId(GetCredentialAttachmentFileRequest request)
        {
            var credentialAttachment = NHibernateSession.Current
                .Query<CredentialAttachment>().FirstOrDefault(x => x.StoredFile.Id == request.StoredFileId);

            var applicationId = credentialAttachment?.Credential.CredentialCredentialRequests.FirstOrDefault()
                ?.CredentialRequest?.CredentialApplication?.Id;
            int? personHash = null;
            if (applicationId != null)
            {
                personHash = _personQueryService.GetPersonToken(new GetTokenRequest()
                {
                    Type = TokenRequestType.ApplicationId,
                    Value = applicationId.GetValueOrDefault()
                }).Token;
            }


            var material = NHibernateSession.Current.Query<StoredFile>().Single(x => x.Id == request.StoredFileId);

            var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
            var getFileRequest = new GetFileRequest
            {
                StoredFileId = request.StoredFileId,
                TempFileStorePath = request.TempFileStorePath
            };

            var storedFile = mFileStorageService.GetFile(getFileRequest);


            return new GetCredentialAttachmentFileResponse
            {
                FileName = material.FileName,
                FilePaths = storedFile.FilePaths,
                PersonHash = personHash,
                ExternalField = material.ExternalFileId
            };
        }

        public CredentialRequestSummarySearchResultResponse SearchCredentialRequestSummary(GetCredentialRequestSummarySearchRequest request)
        {
            var queryHelper = new ApplicationQueryHelper();
            var credentialRequestSummaryResponse = new CredentialRequestSummarySearchResultResponse { Results = queryHelper.SearchCredentialRequestSummary(request) };
            return credentialRequestSummaryResponse;
        }

        public GetApplicationTypeFeesResponse GetApplicationTypeFees(int credentialApplicationTypeId, FeeTypeName feeType)
        {
            credentialApplicationTypeId.Requires(x => x > 0, "CredentialApplicationTypeId not provided.");

            var fees = NHibernateSession.Current
                .Query<CredentialFeeProduct>()
                .Where(x => x.CredentialApplicationType.Id == credentialApplicationTypeId &&
                            x.FeeType.Id == (int)feeType);

            var respose = new GetApplicationTypeFeesResponse
            {
                FeeProducts = fees.Select(x =>
                new CredentialFeeProductDto
                {
                    Id = x.Id,
                    CredentialApplicationTypeId = credentialApplicationTypeId,
                    CredentialTypeId = x.CredentialType == null ? null : (int?)x.CredentialType.Id,
                    FeeType = (FeeTypeName)x.FeeType.Id,
                    ProductSpecification = MapProductSpecificationDetailsDto(x.ProductSpecification)
                }).ToList()
            };

            return respose;
        }

        public bool HasAnyFee(int credentialApplicationTypeId)
        {
            credentialApplicationTypeId.Requires(x => x > 0, "CredentialApplicationTypeId not provided.");

            var fees = NHibernateSession.Current
                .Query<CredentialFeeProduct>()
                .Where(x => x.CredentialApplicationType.Id == credentialApplicationTypeId);

            return fees.Any();
        }

        private ProductSpecificationDetailsDto MapProductSpecificationDetailsDto(ProductSpecification productSpecification)
        {
            return new ProductSpecificationDetailsDto
            {
                Id = productSpecification.Id,
                Name = productSpecification.Name,
                Code = productSpecification.Code,
                Inactive = productSpecification.Inactive,
                Description = productSpecification.Description,
                CostPerUnit = productSpecification.CostPerUnit,

                GlCode = productSpecification.GLCode.Code,
                GstApplies = productSpecification.GSTApplies
            };
        }

        public CredentialRequestApplicantsResponse GetCredentialRequestApplicants(CredentialRequestApplicantsRequest request)
        {
            var skillsCount = request.SkillIds?.Count() ?? 0;
            var skills = skillsCount > 0 ? request.SkillIds : new[] { 0 };

            var query = from credentialRequest in NHibernateSession.Current.Query<CredentialRequest>()

                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id
                        where (request.CredentialApplicationTypeId == 0 || credentialApplication.CredentialApplicationType.Id == request.CredentialApplicationTypeId)
                        && (request.CredentialTypeId == 0 || credentialRequest.CredentialType.Id == request.CredentialTypeId)
                        && (request.CredentialRequestStatusTypeId == 0 || credentialRequest.CredentialRequestStatusType.Id == request.CredentialRequestStatusTypeId)
                        && credentialApplication.PreferredTestLocation != null
                        && credentialApplication.PreferredTestLocation.Id == request.TestLocationId
                        && (skillsCount == 0 || skills.Contains(credentialRequest.Skill.Id))
                        select new CredentialRequestApplicantDto
                        {
                            CredentialRequestId = credentialRequest.Id,
                            ApplicationId = credentialRequest.CredentialApplication.Id,
                            CustomerNo = person.Entity.NaatiNumber,
                            PersonId = person.Id,
                            Name = $"{person.GivenName} {person.Surname}",
                            ApplicationReference = credentialApplication.Reference,
                            Status = credentialRequest.CredentialRequestStatusType.DisplayName,
                            StatusId = credentialRequest.CredentialRequestStatusType.Id,
                            AutoCreated = credentialApplication.AutoCreated,
                            ApplicationSubmittedDate = credentialApplication.EnteredDate,
                            Language1 = credentialRequest.Skill.Language1.Name,
                            Language2 = credentialRequest.Skill.Language2.Name,
                            DirectionDisplayName = credentialRequest.Skill.DirectionType.DisplayName,
                            SkillId = credentialRequest.Skill.Id
                        };

            return new CredentialRequestApplicantsResponse { Results = query.ToList() };
        }



        public CredentialRequestTestResponse GetAllCredentialTests(GetAllCredentialTestsRequest request)
        {
            var filters = new List<TestSessionSearchCriteria>
            {
                new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.TestDateFromString,
                    Values = new List<string>() {request.Start.ToFilterString()}
                },
                new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.TestDateToString,
                    Values = new List<string>() {request.End.ToFilterString()}
                },
                new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.IncludeCompletedSessionsBoolean,
                    Values = new List<string>() { true.ToString()}
                },
            };


            if (request.PreferredTestLocation.Any())
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.TestLocationIntList,
                    Values = request.PreferredTestLocation.Select(x => x.ToString()).ToList()
                });
            }

            if (request.TestVenue.Any())
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.TestVenueIntList,
                    Values = request.TestVenue.Select(x => x.ToString()).ToList()
                });
            }

            if (request.Credential.Any())
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.CredentialIntList,
                    Values = request.Credential.Select(x => x.ToString()).ToList()
                });
            }

            if (request.CredentialSkill.Any())
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.CredentialSkillIntList,
                    Values = request.CredentialSkill.Select(x => x.ToString()).ToList()
                });
            }


            if (request.IsActive)
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.IsActiveBoolean,
                    Values = new List<string>() { true.ToString() }
                });
            }

            if (!request.IsActive)
            {
                filters.Add(new TestSessionSearchCriteria()
                {
                    Filter = TestSessionFilterType.IsActiveBoolean,
                    Values = new List<string>() { false.ToString() }
                });
            }

            var searchRequest = new GetTestSessionSearchRequest
            {
                Filters = filters,
            };

            var sessionQueryHelper = new TestSessionQueryHelper();
            var result = sessionQueryHelper.SearchTestSessions(searchRequest);

            var data = result.Select(x => new CredentialRequestTestRequestDto
            {
                Id = x.TestSessionId,
                TestSessionName = x.SessionName,
                TestDate = x.TestDate.GetValueOrDefault(),
                Duration = x.Duration,
                Attendees = x.PendingToAccept + x.Accepted,
                Accepted = x.Accepted,
                Allocated = x.PendingToAccept,
                Completed = x.Completed,
                CredentialTypeDisplayName = x.CredentialTypeInternalName,
                RejectedCount = x.Rejected,
                SkillDisplayName = x.CredentialTypeInternalName,
                Capacity = x.Capacity,
                VenueName = x.Venue,
                TestSessionActive = x.IsActive

            }).ToList();

            return new CredentialRequestTestResponse
            {
                Results = data
            };

            //TODO:REMOVE THIS CODE
            //var preferredTestLocation = request.PreferredTestLocation.Union(new[] { 0 }).ToList();
            //var testVenue = request.TestVenue.Union(new[] { 0 }).ToList();
            //var credentialRequestType = request.Credential.Union(new[] { 0 }).ToList();
            //var skillList = request.CredentialSkill.Union(new[] { 0 }).ToList();
            //var skillQuery = NHibernateSession.Current.Query<TestSessionSkill>();

            //var sessions = (from testSession in NHibernateSession.Current.Query<TestSession>()
            //                where request.Start <= testSession.TestDateTime
            //                    && testSession.TestDateTime <= request.End
            //                    && (!request.PreferredTestLocation.Any() || preferredTestLocation.Contains(testSession.Venue.TestLocation.Id))
            //                    && (!request.TestVenue.Any() || testVenue.Contains(testSession.Venue.Id))
            //                    && (!request.Credential.Any() || credentialRequestType.Contains(testSession.CredentialType.Id))
            //                    && (!request.CredentialSkill.Any() || skillQuery.Count(s => skillList.Contains(s.Id)) > 0)
            //                select testSession).ToList();

            // var credentialRequestTestResult = sessions.Select(testSession => new CredentialRequestTestRequestDto
            //  {
            // Id = testSession.Id,
            // TestSessionName = testSession.Name,
            // TestDate = testSession.TestDateTime,
            // Duration = testSession.Duration,
            // CredentialTypeDisplayName = testSession.CredentialType?.ExternalName,
            // VenueName = testSession.Venue.Name,
            // VenueCapacity = testSession.Venue.Capacity,
            // SkillTypeDisplayName = testSession.CredentialType?.SkillType.DisplayName,
            // Attendees = testSession.TestSittings.Count(cr => !cr.Rejected),
            //  Allocated = testSession.TestSittings.Count(cr => !cr.Rejected && cr.CredentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestSessionAllocated),
            //  Accepted = testSession.TestSittings.Count(ts => ts.Accepted),
            // Completed = testSession.Completed,
            // RejectedCount = testSession.TestSittings.Count(cr => cr.Rejected),
            //  });
            //    return new CredentialRequestTestResponse
            //    {
            //        Results = credentialRequestTestResult.ToArray();
            //    };
        }

        public RecertificationRequestStatusResponse GetRecertificationRequestStatus(int credentialId)
        {
            var response = new RecertificationRequestStatusResponse();

            var credential = NHibernateSession.Current.Get<Credential>(credentialId);
            var certificationPeriod = credential.CertificationPeriod;
            response.CredentialTerminationDate = credential.TerminationDate;
            response.SubmitedRecertificationRequest = GetSubmittedRecertificationRequestForCredential(credentialId);

            var credentialRequests = credential.CredentialCredentialRequests;
            response.AllowRecertification = credentialRequests.All(HasRecertification);

            if (certificationPeriod != null)
            {

                response.CertificationPeriod = new CertificationPeriodDto
                {
                    Id = certificationPeriod.Id,
                    EndDate = certificationPeriod.EndDate,
                    OriginalEndDate = certificationPeriod.OriginalEndDate,
                    StartDate = certificationPeriod.StartDate,
                    CertificationPeriodStatus = BusinessLogicHelper.GetCertificationPeriodStatus(certificationPeriod.StartDate, certificationPeriod.EndDate),
                    CredentialApplicationId = certificationPeriod.CredentialApplication.Id
                };

                var logBookQueryService = new LogbookQueryService(_autoMapperHelper);
                response.RecertificationInfo = logBookQueryService.GetSubmittedRecertificationApplicationForPeriod(certificationPeriod.Id);
            }
            return response;
        }

        private bool HasRecertification(CredentialCredentialRequest credentialCredentialRequest)
        {
            return credentialCredentialRequest.CredentialRequest.Skill.SkillApplicationTypes.Any(sk => sk.CredentialApplicationType.Id == (int)CredentialApplicationTypeName.Recertification);
        }

        public CertificationPeriodDetailsResponse GetCertificationPeriodDetails(int certificationPeriodId)
        {
            var period = NHibernateSession.Current.Get<CertificationPeriod>(certificationPeriodId);
            period.NotNull($"Certification period with ID {certificationPeriodId} not found.");

            return new CertificationPeriodDetailsResponse
            {
                CertificationPeriod = _autoMapperHelper.Mapper.Map<CertificationPeriodDto>(period),
                Recertifications = period.Recertifications.Select(
                        x => new RecertificationDto
                        {
                            ApplicationId = x.CredentialApplication.Id,
                            CertificationPeriodId = certificationPeriodId,
                            Id = x.Id,
                            CredentialApplicationStatus = x.CredentialApplication.CredentialApplicationStatusType.Name.ParseEnum<CredentialApplicationStatusTypeName>(),
                            SubmittedDate = x.CredentialApplication.EnteredDate
                        })
                    .ToList()
            };
        }

        public MoveCredentialResponse MoveCredential(MoveCredentialRequest request)
        {
            request.CertificationPeriodId.NotZero($"Certification period with ID {request.CertificationPeriodId} not found.");
            request.CredentialId.NotZero($"Credential with ID {request.CredentialId} not found.");
            request.Notes.NotNullOrWhiteSpace($"Notes can not be empty.");

            var credential = NHibernateSession.Current.Get<Credential>(request.CredentialId);
            credential.NotNull($"Credential with ID {request.CredentialId} not found.");

            var certificationPeriod = NHibernateSession.Current.Get<CertificationPeriod>(request.CertificationPeriodId);
            certificationPeriod.NotNull($"Certification period with ID {request.CertificationPeriodId} not found.");

            var originalCertificationPeriod = credential.CertificationPeriod;
            credential.CertificationPeriod = certificationPeriod;
            NHibernateSession.Current.Save(credential);


            var credentialRequest = credential.CredentialCredentialRequests.Select(cr => cr.CredentialRequest).FirstOrDefault();
            credentialRequest.NotNull($"Credential Request not found.");

            var note = new Note
            {
                CreatedDate = DateTime.Now,
                ReadOnly = true,
                Highlight = true,
                User = NHibernateSession.Current.Get<User>(request.UserId),
                Description = $@"Certification {credentialRequest.CredentialType.InternalName} {credentialRequest.Skill.DisplayName} has been moved to another Certification Period.\n
Moved from Certification Period {originalCertificationPeriod.Id} (with expiry {originalCertificationPeriod.EndDate:dd/MM/yyyy}) to {certificationPeriod.Id} (with expiry {certificationPeriod.EndDate:dd/MM/yyyy}).\n
Reason: {request.Notes}."
            };
            NHibernateSession.Current.Save(note);

            var naatiEntityNote = new NaatiEntityNote
            {
                Entity = credential.CertificationPeriod.Person.Entity,
                Note = note
            };
            NHibernateSession.Current.Save(naatiEntityNote);

            NHibernateSession.Current.Flush();

            return new MoveCredentialResponse();
        }

        public LookupTypeResponse GetLocations(int applicationFormId)
        {
            var credentialApplicationForm = NHibernateSession.Current.Get<CredentialApplicationForm>(applicationFormId);

            var locations = from test in credentialApplicationForm.CredentialApplicationType.Locations
                            join office in NHibernateSession.Current.Query<Office>() on test.TestLocation.OfficeId equals office.Id
                            join institution in NHibernateSession.Current.Query<Institution>() on office.Institution.Id equals institution.Id
                            where credentialApplicationForm.Id == applicationFormId
                            select new LookupTypeDto
                            {
                                Id = test.TestLocation.Id,
                                DisplayName = test.TestLocation.Name + ", " + institution.LatestInstitutionName.InstitutionName.Abbreviation
                            };

            if (!locations.Any())
            {
                return GetTestLocation();
            }

            return new LookupTypeResponse { Results = locations.OrderBy(x => x.DisplayName) };
        }

        public CredentialRequestInfoDto GetCredentialRequestForCredential(int credentialId)
        {
            var result = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .Where(x => x.Credential.Id == credentialId)
                .OrderByDescending(y => y.Id)
                .First();

            return new CredentialRequestInfoDto
            {
                CredentialRequestId = result.CredentialRequest.Id,
                CredentialRequestStatusType =
                    (CredentialRequestStatusTypeName)result.CredentialRequest.CredentialRequestStatusType.Id
            };
        }

        public CredentialRequestInfoDto GetSubmittedRecertificationRequestForCredential(int credentialId)
        {
            CredentialRequestInfoDto dto = null;
            Credential credential = null;
            CertificationPeriod certificationPeriod = null;
            Recertification recertification = null;
            CredentialApplication credentialApplication = null;
            CredentialApplicationStatusType credentialApplicationStatusType = null;
            CredentialRequest credentialRequest = null;
            Skill skill = null;
            CredentialType credentialType = null;
            CredentialCredentialRequest credentialCredentialRequest = null;
            CredentialRequestStatusType credentialRequestStatusType = null;
            Skill credentialSkill = null;
            CredentialType credentialCredentialType = null;
            CredentialRequest credentialRequest2 = null;
            var invalidapplicaitonStatuses = BusinessLogicHelper.GetInvalidRecertificationApplicationStatuses();
            var invalidCredentialRequestStatuses = BusinessLogicHelper.GetInvalidRecertificationCredentialRequestStatuses();

            var result = NHibernateSession.Current.QueryOver(() => credential)
                .JoinQueryOver(c => c.CertificationPeriod, () => certificationPeriod)
                .JoinQueryOver(p => p.Recertifications, () => recertification)
                .JoinQueryOver(r => r.CredentialApplication, () => credentialApplication)
                .JoinQueryOver(r => r.CredentialApplicationStatusType, () => credentialApplicationStatusType, JoinType.InnerJoin, Restrictions.Not(Restrictions.In(Projections.Property(() => credentialApplicationStatusType.Id), invalidapplicaitonStatuses)))
                .Inner.JoinAlias(r => credentialApplication.CredentialRequests, () => credentialRequest)
                .Inner.JoinAlias(r => credentialRequest.CredentialRequestStatusType, () => credentialRequestStatusType)
                .Inner.JoinAlias(r => credentialRequest.Skill, () => skill)
                .Inner.JoinAlias(r => credentialRequest.CredentialType, () => credentialType)
                .Inner.JoinAlias(r => credential.CredentialCredentialRequests, () => credentialCredentialRequest)
                .Inner.JoinAlias(r => credentialCredentialRequest.CredentialRequest, () => credentialRequest2)
                .Inner.JoinAlias(r => credentialRequest2.CredentialType, () => credentialCredentialType)
                .Inner.JoinAlias(r => credentialRequest2.Skill, () => credentialSkill)
                .Where(Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => credential.Id), credentialId))
                .Add(Restrictions.EqProperty(Projections.Property(() => credentialSkill.Id), Projections.Property(() => skill.Id)))
                .Add(Restrictions.EqProperty(Projections.Property(() => credentialCredentialType.Id), Projections.Property(() => credentialType.Id)))
                .Add(Restrictions.Not(Restrictions.In(Projections.Property(() => credentialRequestStatusType.Id), invalidCredentialRequestStatuses))))
                .Select(Projections.ProjectionList()
                .Add(Projections.Property(() => credentialRequest.Id))
                .Add(Projections.Property(() => credentialRequestStatusType.Id))
                .Add(Projections.Property(() => credentialRequest.Id).WithAlias(() => dto.CredentialRequestId))
                .Add(Projections.Property(() => credentialRequestStatusType.Id).WithAlias(() => dto.CredentialRequestStatusType)))
                .OrderBy(Projections.Property(() => credentialRequest.Id)).Desc
                .TransformUsing(Transformers.AliasToBean<CredentialRequestInfoDto>())
                .List<CredentialRequestInfoDto>();

            return result.FirstOrDefault();
        }

        public FormLookupTypeResponse GetCredentialApplicationForms(GetFormRequest request)
        {
            var forms = new List<FormLookupTypeDto>();

            if (request.Public)
            {
                forms.AddRange(GetPublicApplicationForms());
            }

            if (request.Private)
            {
                forms.AddRange(GetPrivateApplicationForms());
            }

            if (request.Practitioner)
            {
                forms.AddRange(GetPractitionerApplicationForms());
            }

            if (request.Recertification)
            {
                forms.AddRange(GetRecertificationApplicationForms());
            }

            if (request.NonPractitioner)
            {
                forms.AddRange(GetNonPractitionerApplicationForms());
            }

            return new FormLookupTypeResponse { Results = forms.GroupBy(x => x.Id).Select(y => y.First()) };
        }

        public LookupTypeResponse GetEndorsementQualificationLookup(GetEndorsementQualificationLookupRequest request)
        {
            char tab = '\u0009';
            var rnd = new Random();
            var endorsementQualifications = NHibernateSession.Current.Query<EndorsedQualification>()
                                           .Where(x => request.InstitutionNaatiNumbers.Contains(x.Institution.Entity.NaatiNumber) && request.Locations.Contains(x.Location))
                                           .Select(x => new
                                           {
                                               Name = x.Qualification.Trim().Replace(tab.ToString(), ""),
                                               DisplayName = x.Qualification.Trim().Replace(tab.ToString(), "")
                                           })
                                           .Distinct()
                                           .ToList()
                                           .OrderBy(x => x.Name);


            var qualifications = endorsementQualifications.Select(x => new LookupTypeDto
            {
                Id = rnd.Next(),
                Name = x.Name,
                DisplayName = x.Name
            });

            return new LookupTypeResponse() { Results = qualifications };
        }

        public LookupTypeResponse GetEndorsementLocationLookup(GetEndorsementLocationLookupRequest request)
        {
            char tab = '\u0009';
            var rnd = new Random();
            var endorsementLocations = NHibernateSession.Current.Query<EndorsedQualification>()
                              .Where(x => request.InstitutionNaatiNumbers.Contains(x.Institution.Entity.NaatiNumber))
                              .Select(x => new
                              {
                                  Name = x.Location.Trim().Replace(tab.ToString(), ""),
                                  DisplayName = x.Location.Trim().Replace(tab.ToString(), "")
                              })
                             .Distinct()
                             .ToList()
                             .OrderBy(x => x.Name);

            var locations = endorsementLocations.Select(x => new LookupTypeDto
            {
                Id = rnd.Next(),
                DisplayName = x.DisplayName
            });

            return new LookupTypeResponse { Results = locations };

        }

        public IEnumerable<int> GetCertificationPeriodCredentialFromApplicationId(int applicationId)
        {
            var result = NHibernateSession.Current.Query<Credential>()
                .Where(x => x.CertificationPeriod.Id > 0
                         && x.CertificationPeriod.CredentialApplication.Id == applicationId).Select(x => x.Id).ToList();

            return result;
        }

        public IEnumerable<int> GetApplicationIdsWithRecertificationReminders(GetApplicationIdsWithRecertificationRemindersRequest request)
        {
            var dateList = request.ExpiryDates.ToList();

            var result = NHibernateSession.Current.Query<Credential>()
                .Where(x => x.CertificationPeriod.Id > 0 && dateList.Contains(x.CertificationPeriod.EndDate) && x.TerminationDate == null && x.CertificationPeriod.Person.Deceased == false)
                .Select(x => x.CertificationPeriod.CredentialApplication.Id).Distinct().ToList();

            return result;
        }

        public IEnumerable<TestSessionReminderObject> GetTestSessionReminders(GetTestSessionRemindersRequest request)
        {
            var dateList = request.TestSessionDates.Select(x => x.Date).ToList();

            var result = from testSession in NHibernateSession.Current.Query<TestSession>()
                         join testSitting in NHibernateSession.Current.Query<TestSitting>() on testSession.Id equals testSitting.TestSession.Id
                         join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on testSitting.CredentialRequest.Id equals credentialRequest.Id
                         where dateList.Contains(testSession.TestDateTime.Date) &&
                         credentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestSessionAccepted &&
                         !testSitting.Sat &&
                         !testSitting.Rejected
                         select new TestSessionReminderObject
                         {
                             ApplicationId = credentialRequest.CredentialApplication.Id,
                             CredentialRequestId = credentialRequest.Id
                         };

            return result;
        }

        public CertificationPeriodDetailsResponse GetCertificationPeriodDetailsByApplicationId(int applicationId)
        {
            var period = NHibernateSession.Current
                .Query<CertificationPeriod>().First(x => x.CredentialApplication.Id == applicationId);


            return new CertificationPeriodDetailsResponse
            {
                CertificationPeriod = new CertificationPeriodDto
                {
                    Id = period.Id,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    OriginalEndDate = period.OriginalEndDate
                }
            };
        }

        public IEnumerable<int> GetExpiringCertificationPeriodApplications(GetExpiringCertificationPeriodRequest request)
        {
            var certificationPeriodDates = request.ExpiryDates.Select(x => x.Date).ToList();
            var minDate = request.ExpiryDates.Min(x => x.Date);

            var result = NHibernateSession.Current.Query<Credential>()
                .Where(
                    x => (x.TerminationDate == null || x.TerminationDate > minDate)
                         && x.CertificationPeriod != null &&
                         certificationPeriodDates.Contains(x.CertificationPeriod.EndDate))
                .Select(x => x.CertificationPeriod.CredentialApplication.Id).Distinct().ToList();
            return result;
        }

        public bool HasSubmittedApplications(int naatiNumber)
        {
            var nonSubmittedStatuses = BusinessLogicHelper.GetNonSubmittedApplicationStatuses();

            var result = NHibernateSession.Current.Query<CredentialApplication>()
                .Count(
                    x => x.Person.Entity.NaatiNumber == naatiNumber &&
                         !nonSubmittedStatuses.Contains(x.CredentialApplicationStatusType.Id)) > 0;
            return result;
        }

        public bool CanCreateNewApplication(int naatiNumber, int applicationTypeId)
        {
            var nonActiveApplicationStatus = BusinessLogicHelper.GetNonActiveApplicationStatuses();
            var result = NHibernateSession.Current.Query<CredentialApplication>()
                .Count(
                    x => x.Person.Entity.NaatiNumber == naatiNumber &&
                         x.CredentialApplicationType.Id == applicationTypeId
                         && x.CredentialApplicationType.AllowMultiple == false
                         && !nonActiveApplicationStatus.Contains(x.CredentialApplicationStatusType.Id)) == 0;
            return result;
        }

        public IList<BasicApplicationSearchDto> GetDraftApplications(int naatiNumber, int applicationTypeId)
        {
            var result = NHibernateSession.Current.Query<CredentialApplication>()
                             .Where(
                                 x => x.Person.Entity.NaatiNumber == naatiNumber &&
                                      x.CredentialApplicationType.Id == applicationTypeId
                                      && x.CredentialApplicationStatusType.Id == (int)CredentialApplicationStatusTypeName.Draft)
                             .Select(y => new BasicApplicationSearchDto
                             {
                                 Id = y.Id,
                                 ApplicationStatus = y.CredentialApplicationStatusType.DisplayName,
                                 EnteredDate = y.EnteredDate,
                                 StatusChangeDate = y.StatusChangeDate
                             }).ToList();
            return result;
        }

        public IEnumerable<CredentialRequestWithPendingRefundRequest> GetCredentialRequestsWithPendingRefundRequests()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                            .Where(credentialApplicationRefund => credentialApplicationRefund.CredentialWorkflowFee.CredentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.RefundRequestApproved)
                            .Select(credentialApplicationRefund => new CredentialRequestWithPendingRefundRequest
                            {
                                CredentialApplicationId = credentialApplicationRefund.CredentialWorkflowFee.CredentialApplication.Id,
                                CredentialRequestId = credentialApplicationRefund.CredentialWorkflowFee.CredentialRequest.Id,
                                InvoiceNumber = credentialApplicationRefund.CredentialWorkflowFee.InvoiceNumber,
                                InitialPaidAmount = credentialApplicationRefund.InitialPaidAmount.Value
                            })
                            .ToList();

            return result;
        }

        public void UpdateCredentialApplicationRefundRequest(UpdateCredentialApplicationRefundRequest refundDto)
        {
            var credentialApplicationRefundRequest = NHibernateSession.Current.Load<CredentialApplicationRefund>(refundDto.Id);
            if (credentialApplicationRefundRequest == null)
            {
                throw new Exception("Refund request not found for update");
            }

            credentialApplicationRefundRequest.DisallowProcessing = refundDto.DisallowProcessing ?? credentialApplicationRefundRequest.DisallowProcessing;
            credentialApplicationRefundRequest.InitialPaidAmount = refundDto.InitialPaidAmount ?? credentialApplicationRefundRequest.InitialPaidAmount;
            credentialApplicationRefundRequest.RefundAmount = refundDto.RefundAmount ?? credentialApplicationRefundRequest.RefundAmount;
            credentialApplicationRefundRequest.InitialPaidTax = refundDto.InitialPaidTax ?? credentialApplicationRefundRequest.InitialPaidTax;

            NHibernateSession.Current.Save(credentialApplicationRefundRequest);
            NHibernateSession.Current.Flush();
        }

        public IEnumerable<ApprovalPendingRefundRequestDto> GetApprovalPendingRefundRequests()
        {
            var result = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                            .Where(credentialApplicationRefund => credentialApplicationRefund.CredentialWorkflowFee.CredentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.RefundRequested
                            && !credentialApplicationRefund.IsRejected)
                            .Select(credentialApplicationRefund => new ApprovalPendingRefundRequestDto
                            {
                                Id = credentialApplicationRefund.Id,
                                CredentialWorkflowFeeId = credentialApplicationRefund.CredentialWorkflowFee.Id,
                                InitialPaidAmount = credentialApplicationRefund.InitialPaidAmount,
                                RefundMethodTypeId = credentialApplicationRefund.RefundMethodType.Id,
                                RefundAmount = credentialApplicationRefund.RefundAmount,
                                RefundPercentage = credentialApplicationRefund.RefundPercentage,
                                PaymentReference = credentialApplicationRefund.PaymentReference,
                                InvoiceNumber = credentialApplicationRefund.CredentialWorkflowFee.InvoiceNumber,
                                CredentialApplicationId = credentialApplicationRefund.CredentialWorkflowFee.CredentialApplication.Id,
                                NAATINumber = credentialApplicationRefund.CredentialWorkflowFee.CredentialApplication.Person.Entity.NaatiNumber,
                                Policy = credentialApplicationRefund.CredentialWorkflowFee.CredentialApplicationRefundPolicy.Description,
                                Comments = credentialApplicationRefund.Comments,
                                BankDetails = UnProtectBankDetails(credentialApplicationRefund.BankDetails),
                            });

            return result;
        }

        public CredentialWorkflowFeeDto GetCredentialWrokflowFeeById(int credentialWorkflowFeeId)
        {
            var credentialWorkflow = NHibernateSession.Current.Get<CredentialWorkflowFee>(credentialWorkflowFeeId);

            return new CredentialWorkflowFeeDto
            {
                CredentialApplicationId = credentialWorkflow.CredentialApplication.Id,
                CredentialRequestId = credentialWorkflow.CredentialRequest.Id
            };
        }

        public IEnumerable<PaymentMethodTypeModel> GetPaymentMethodTypes()
        {
            var paymentMethodTypes = NHibernateSession.Current.Query<PaymentMethodType>();
            return paymentMethodTypes.Select(paymentMethod => new PaymentMethodTypeModel
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.Name, 
                DisplayName = paymentMethod.DisplayName,
                ExternalExternalReferenceId = paymentMethod.ExternalReferenceId
            });
        }

        public CredentialApplicationRefundPolicyData GetCredentialApplicationRefundPolicy(int credentialTypeId, int applicationTypeId, int productSpecificationId)
        {
            var credentialFeeProduct = NHibernateSession.Current.Query<CredentialFeeProduct>()
                                .Where(feeProduct => feeProduct.CredentialType.Id == credentialTypeId &&
                                        feeProduct.CredentialApplicationType.Id == applicationTypeId &&
                                        feeProduct.ProductSpecification.Id == productSpecificationId)
                                .Single();

            return new CredentialApplicationRefundPolicyData
            {
                Id = credentialFeeProduct.CredentialApplicationRefundPolicy.Id,
                Description = credentialFeeProduct.CredentialApplicationRefundPolicy.Description,
                Name = credentialFeeProduct.CredentialApplicationRefundPolicy.Name
            };
        }

        public void FlushRefundBankDetails(IList<int> refundIds)
        {
            var refunds = NHibernateSession.Current.Query<CredentialApplicationRefund>()
                .Where(x => refundIds.Contains(x.Id)).ToList();

            foreach (var refund in refunds)
            {
                refund.BankDetails = null;
                NHibernateSession.Current.SaveOrUpdate(refund);
            }
        }

        public GenericResponse<List<NewCredentialsThatCanProgressToEligibleForTesting>> GetNewCredentialsThatCanprogressToEligibleForTesting()
        {
            //var verifiedCredentials = new List<NewCredentialsThatCanProgressToEligibleForTesting>();
            //get new Credentials
            var newCredentials = NHibernateSession.Current.TransformSqlQueryDataRowResult<NewCredentialsThatCanProgressToEligibleForTesting>("exec GetNewCredentialsThatCanProgressToEligibleForTesting");
            //foreach(NewCredentialsThatCanProgressToEligibleForTesting credential in newCredentials)
            //{
            //    var canProgress = NHibernateSession.Current.CreateSQLQuery($"exec CheckNewCredentialSiblingsCanProgressToEligibleForTesting @CredentialApplicationId={credential.CredentialApplicationId},@CredentialRequestId={credential.CredentialRequestId}").UniqueResult<int>();
            //    if(canProgress == 1)
            //    {
            //        verifiedCredentials.Add(credential);
            //    }
            //}

            //return verifiedCredentials;
            return newCredentials.ToList();
        }

        public GenericResponse<bool> UpdateOnHoldCredentialToOnHoldToBeIssued(OnHoldCredential onHoldCredential)
        {
            // get credential request from id
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(onHoldCredential.CredentialRequestId);

            if (credentialRequest.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Credential Request does not exist for id {onHoldCredential.CredentialRequestId}." }
                };
            }

            // update the status to OnHoldToBeIssued => Id: 41
            credentialRequest.CredentialRequestStatusType = NHibernateSession.Current.Get<CredentialRequestStatusType>(41);
            credentialRequest.StatusChangeDate = DateTime.Now;
            credentialRequest.StatusChangeUser = NHibernateSession.Current.Get<User>(onHoldCredential.CurrentUserId);

            if (credentialRequest.CredentialRequestStatusType.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Credential Request Status Type does not exist for id 41." }
                };
            }

            if (credentialRequest.CredentialRequestStatusType.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not get current user with id of {onHoldCredential.CurrentUserId}." }
                };
            }

            NHibernateSession.Current.SaveOrUpdate(credentialRequest);
            NHibernateSession.Current.Flush();

            return new GenericResponse<bool>(true);
        }

        public GenericResponse<List<CredentialRequestDto>> GetCredentialRequestsOnHoldToBeIssued()
        {
            var credentialRequestList = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialRequestStatusType.Id == 41)
                .ToList();

            var credentialRequestDtoList = new List<CredentialRequestDto>();

            var credentialRequestFields = credentialRequestList.FirstOrDefault()
                                             ?
                                             .CredentialApplication.CredentialApplicationType
                                             .CredentialApplicationFields.Where(f => f.PerCredentialRequest)
                                             .ToList() ??
                                         new List<CredentialApplicationField>();

            var queyrHelper = new ApplicationQueryHelper();

            var credentialRequestIds = credentialRequestList.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList();

            var credentialRequestAssociations = NHibernateSession.Current.Query<CredentialRequestCredentialRequest>()
                .Where(x => credentialRequestIds.Contains(x.AssociatedCredentialRequest.Id)).ToLookup(x => x.AssociatedCredentialRequest.Id);


            foreach (var credentialRequest in credentialRequestList)
            {
                
                var credentialStatuses = queyrHelper.GetCredentialStatuses(new List<int> { credentialRequest?.Id ?? 0 });

                var credentialRequestDto = CredentialRequestToDto(credentialRequest, credentialRequestFields, credentialStatuses, credentialRequestAssociations);

                credentialRequestDtoList.Add(
                    credentialRequestDto
                );
            }

            return credentialRequestDtoList;
        }

        public GenericResponse<bool> GetIsPractitionerFromNaatiNumber(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().Where(x => x.Entity.NaatiNumber == naatiNumber).FirstOrDefault();

            if (person.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not get person with naati number {naatiNumber}." }
                };
            }

            var isPractitioner = person.IsPractitioner;

            return isPractitioner;
        }

        public GenericResponse<bool> CreateNotesForOnHoldToBeIssued(ApplicationNoteDto applicationNoteDto)
        {
            var noteToCreateDto = applicationNoteDto.Note;

            var noteToCreate = new Note()
            {
                User = NHibernateSession.Current.Get<User>(noteToCreateDto.UserId),
                Description = noteToCreateDto.Description,
                CreatedDate = noteToCreateDto.CreatedDate,
                ModifiedDate = noteToCreateDto.ModifiedDate,
                Highlight = noteToCreateDto.Highlight,
                ReadOnly = noteToCreateDto.ReadOnly
            };

            NHibernateSession.Current.SaveOrUpdate(noteToCreate);
            NHibernateSession.Current.Flush();

            var applicationNoteToCreate = new CredentialApplicationNote()
            {
                CredentialApplication = NHibernateSession.Current.Get<CredentialApplication>(applicationNoteDto.CredentialApplicationId),
                Note = noteToCreate
            };

            NHibernateSession.Current.SaveOrUpdate(applicationNoteToCreate);
            NHibernateSession.Current.Flush();

            return true;
        }

        public GenericResponse<bool> DoesCredentialBelongToUser(int storedFileId, int currentUserNaatiNumber)
        {
            var credentialAttachment = NHibernateSession.Current.Query<CredentialAttachment>().FirstOrDefault(x => x.StoredFile.Id == storedFileId);
            //no such credential
            if (credentialAttachment == null)
            {
                LoggingHelper.LogError($"StoredFileId {storedFileId} does not belong to {currentUserNaatiNumber}");
                return new GenericResponse<bool>()
                {
                    Success = false
                };
            }
            var naatiNumber = credentialAttachment.Credential.CredentialCredentialRequests.First().CredentialRequest.CredentialApplication.Person.NaatiNumberDisplay;
            var success = naatiNumber == currentUserNaatiNumber.ToString();
            //user does not have that credential
            if (!success)
            {
                LoggingHelper.LogError($"StoredFileId {storedFileId} does not belong to {currentUserNaatiNumber}");
                return new GenericResponse<bool>()
                {
                    Success = false
                };
            }
            return success;
        }

        private string ProtectBankDetails(string details)
        {
            if (string.IsNullOrWhiteSpace(details))
            {
                return null;
            }
            var plainTextBytes = Encoding.Unicode.GetBytes(details);
            var encrypted = MachineKey.Protect(plainTextBytes, "refund");
            return Convert.ToBase64String(encrypted);
        }

        private string UnProtectBankDetails(string encryptedDetails)
        {
            if (string.IsNullOrWhiteSpace(encryptedDetails))
            {
                return null;
            }

            var plainTextBytes = Convert.FromBase64String(encryptedDetails);
            var decrypted = MachineKey.Unprotect(plainTextBytes, "refund");
            if (decrypted != null)
            {
                var value = Encoding.UTF8.GetString(decrypted).Replace("\0", "");
                return value;
            }

            return null;
        }
    }
}
