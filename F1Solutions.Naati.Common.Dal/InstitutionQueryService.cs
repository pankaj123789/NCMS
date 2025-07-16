using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal
{
   public class InstitutionQueryService : IInstitutionQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public InstitutionQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public AddNameResponse AddName(InstitutionDto model)
        {
            var institution = NHibernateSession.Current.Query<Institution>()
                 .SingleOrDefault(x => x.Id == model.InstitutionId);

            if (institution == null)
            {
                throw new WebServiceException($"Institution not found (NAATI Number {model.NaatiNumber})");
            }

            var institutionName = new InstitutionName
            {
                Name = model.Name,
                TradingName = "",
                Abbreviation = model.AbbreviatedName,
                EffectiveDate = DateTime.Now
            };

            institution.AddName(institutionName);

            NHibernateSession.Current.Evict(institution.Entity.PrimaryAddresses);
            NHibernateSession.Current.Save(institution);
            NHibernateSession.Current.Flush();

            return new AddNameResponse();
        }

        public void UpdateInstitution(InstitutionDto model)
        {
            var institution = NHibernateSession.Current.Query<Institution>()
                .SingleOrDefault(x => x.Id == model.InstitutionId);

            if (institution == null)
            {
                throw new WebServiceException($"Institution not found (NAATI Number {model.InstitutionId})");
            }

            institution.TrustedPayer = model.TrustedPayer ?? false;

            NHibernateSession.Current.Evict(institution.Entity.PrimaryAddresses);
            NHibernateSession.Current.SaveOrUpdate(institution);
            NHibernateSession.Current.Flush();
        }


        public InstitutionInsertResponse InsertInstitution(InstitutionDto model)
        {
            var naatiEntity = new NaatiEntity
            {
                WebsiteUrl = string.Empty,
                WebsiteInPD = false,
                Abn = string.Empty,
                Note = string.Empty,
                UseEmail = false,
                GstApplies = false,
                NaatiNumber = model.NaatiNumber > 0 ? model.NaatiNumber : GetNextNaatiNumber(),
                EntityTypeId = 3

            };


            var newInstitution = new Institution
            {
                Entity = naatiEntity,
                IsManageCoursesAndQualification = true,
                IsGoThroughApprovalProcess = true,
                IsUniversity = false,
                IsVetRto = false,
                RtoNumber = string.Empty,
                IsOfferCourseToStudentVisa = false,
                CricosProviderCode = string.Empty,
                TrustedPayer = false
            };

            var institutionName = new InstitutionName
            {
                Name = model.Name,
                EffectiveDate = DateTime.Now,
                Institution = newInstitution,
                TradingName = "",
                Abbreviation = ""
            };

            institutionName.ChangeInsitution(newInstitution);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.Save(naatiEntity);
                NHibernateSession.Current.Save(newInstitution);
                NHibernateSession.Current.Save(institutionName);
                transaction.Commit();
            }
            return new InstitutionInsertResponse { IsSuccessful = true, NaatiNumber = naatiEntity.NaatiNumber };
        }

        private static int GetNextNaatiNumber()
        {
            return KeyAllocation.GetSingleKey("InstitutionNaatiNumber");
        }

        public InstitutionInsertResponse CheckDuplicatedInstitution(InstitutionDto model)
        {
            //check duplicate NaatiNo
            var entity = NHibernateSession.Current.Query<NaatiEntity>()
                .FirstOrDefault(x => x.NaatiNumber == model.NaatiNumber);
            Institution institution = null;
            if (entity != null)
            {
                institution = NHibernateSession.Current.Query<Institution>()
                    .FirstOrDefault(x => x.Entity.Id == entity.Id);

                var person = NHibernateSession.Current.Query<Person>()
                    .FirstOrDefault(x => x.Entity.Id == entity.Id);

                if (institution != null)
                {
                    return new InstitutionInsertResponse
                    {
                        NaatiNumber = entity.NaatiNumber,
                        Name = institution.InstitutionName,
                        InstitutionId = institution.Id,
                        IsSuccessful = false
                    };
                }

                if (person != null)
                {
                    return new InstitutionInsertResponse
                    {
                        NaatiNumber = entity.NaatiNumber,
                        Name = person.FullName,
                        IsSuccessful = false
                    };
                }
            }

            //check duplicate name
            institution = NHibernateSession.Current.Query<Institution>()
                .FirstOrDefault(
                    x => x.InstitutionName.ToLower() == model.Name.ToLower());

            if (institution != null)
            {
                return new InstitutionInsertResponse
                {
                    NaatiNumber = institution.Entity.NaatiNumber,
                    Name = institution.InstitutionName,
                    InstitutionId = institution.Id,
                    IsSuccessful = false,
                    IsWarned = true
                };
            }
            return new InstitutionInsertResponse { IsSuccessful = true };
        }

        public InstituteSearchResponse SearchInstitute(GetInstituteSearchRequest request)
        {
            var instituteQueryHelper = new InstituteQueryHelper();
            return new InstituteSearchResponse { Results = instituteQueryHelper.SearchInstitute(request) };
        }


        public InstitutionDto GetInstitution(int naatiNumber)
        {
            var entity = NHibernateSession.Current.Query<NaatiEntity>()
                .SingleOrDefault(x => x.NaatiNumber == naatiNumber);

            var institution = NHibernateSession.Current.Query<Institution>()
                .SingleOrDefault(x => x.Entity.Id == entity.Id);

            if (entity == null)
            {
                throw new WebServiceException($"Entity not found (NAATI Number {naatiNumber})");
            }

            return new InstitutionDto
            {
                NaatiNumber = entity.NaatiNumber,
                InstitutionId = institution?.Id,
                EntityId = entity.Id,
                EntityTypeId = entity.EntityTypeId,
                NaatiNumberDisplay = entity.NaatiNumberDisplay,
                Name = institution?.InstitutionName,
                TrustedPayer = institution?.TrustedPayer,
                NameHistory = institution?.InstitutionNames.Select(x => new InstitutionNameDto
                {
                    Name = x.Name,
                    Abbreviation = x.Abbreviation,
                    EffectiveDate = x.EffectiveDate,
                    InstitutionNameId = x.Id
                }).ToList(),
                ContactDetails = new GetContactDetailsResponse
                {
                    Addresses = GetPersonAddresses(entity.Id),
                    Phones = GetPersonPhones(entity.Id),
                    Emails = GetPersonEmails(entity.Id),
                    Websites = GetPersonWebsites(entity),
                    ShowWebsite = true
                }

            };

        }
        private IList<AddressDetailsDto> GetPersonAddresses(int entityId)
        {
            var addresses = NHibernateSession.Current.TransformSqlQueryDataRowResult<AddressDetailsDto>("exec AddressSelect " + entityId);
            return addresses;
        }

        private IList<PhoneDetailsDto> GetPersonPhones(int entityId)
        {
            return NHibernateSession.Current.TransformSqlQueryDataRowResult<PhoneDetailsDto>("exec PhoneSelect " + entityId);
        }

        private IList<EmailDetailsDto> GetPersonEmails(int entityId)
        {
            return NHibernateSession.Current.TransformSqlQueryDataRowResult<EmailDetailsDto>("exec EmailSelect " + entityId);
        }
        private IList<WebsiteDetailsDto> GetPersonWebsites(NaatiEntity entity)
        {
            var list = new List<WebsiteDetailsDto>();

            if (!string.IsNullOrWhiteSpace(entity.WebsiteUrl))
            {
                list.Add(new WebsiteDetailsDto
                {
                    EntityId = entity.Id,
                    IncludeInPd = entity.WebsiteInPD,
                    Url = entity.WebsiteUrl
                });
            }

            return list;
        }

        

        public ServiceResponse<IEnumerable<EndorsedQualificationSearchResultDto>> SearchEndorsedQualification(GetEndorsedQualificationSearchRequest request)
        {
            var queryHelper = new EndorsedQualificationQueryHelper();

            var result = queryHelper.SearchQualification(request);
            return new ServiceResponse<IEnumerable<EndorsedQualificationSearchResultDto>>() { Data = result };

        }

        public CreateOrUpdateResponse CreateOrUpdateQualification(EndorsedQualificationDto model)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(model.CredentialTypeId);
            var institution = NHibernateSession.Current.Get<Institution>(model.InstitutionId);
            
            if (credentialType == null)
            {
                throw new WebServiceException($"Credential Type was not found (ID {model.CredentialTypeId})");
            }

            if (institution == null)
            {
                throw new WebServiceException($"institution was not found (ID {model.InstitutionId})");
            }

            EndorsedQualification endorsedQualification;
            if (model.EndorsedQualificationId > 0)
            {
                endorsedQualification = NHibernateSession.Current.Get<EndorsedQualification>(model.EndorsedQualificationId);
            }
            else
            {
                endorsedQualification = new EndorsedQualification();
            }

            endorsedQualification.CredentialType = credentialType;
            endorsedQualification.Institution = institution;

            endorsedQualification.Active = model.Active;
            endorsedQualification.Location = model.Location;
            endorsedQualification.Qualification = model.Qualification;
            endorsedQualification.EndorsementPeriodFrom = model.EndorsementPeriodFrom;
            endorsedQualification.EndorsementPeriodTo = model.EndorsementPeriodTo;
            endorsedQualification.Notes = model.Notes;
            
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.SaveOrUpdate(endorsedQualification);
                transaction.Commit();
            }

            return new CreateOrUpdateResponse { Id = endorsedQualification.Id };
        }

        public ServiceResponse<EndorsedQualificationDto> GetEndorsedQualification(int endorsedQualificationId)
        {
            var result = NHibernateSession.Current.Get<EndorsedQualification>(endorsedQualificationId);

            var data = _autoMapperHelper.Mapper.Map<EndorsedQualificationDto>(result);
            data.Qualification = result.Qualification;
            data.Active = result.Active;
            data.EndorsedQualificationId = result.Id;
            data.CredentialTypeId = result.CredentialType.Id;
            data.CredentialTypeExternalName = result.CredentialType.ExternalName;
            data.EndorsementPeriodFrom = result.EndorsementPeriodFrom;
            data.EndorsementPeriodTo = result.EndorsementPeriodTo;
            data.InstitutionId = result.Institution.Id;
            data.Notes = result.Notes;
            return new ServiceResponse<EndorsedQualificationDto>() { Data = data };
        }
    }
}
