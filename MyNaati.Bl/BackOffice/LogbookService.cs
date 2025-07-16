using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Bl.BackOffice
{
    public class LogbookService : ILogbookService
    {
        private readonly ILogbookQueryService mLogbookQueryService;
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly IFileStorageService mFileStorageService;

        private readonly IActivityPointsCalculatorService mActivityPointsService;
        private readonly ICredentialPointsCalculatorService mCredentialPointsService;
        private readonly IApplicationBusinessLogicService mBusinessLogicService;


        public LogbookService(ILogbookQueryService logbookQueryService,
            IApplicationQueryService applicationQueryService,
            IFileStorageService fileStorageService,
         IActivityPointsCalculatorService activityPointsService, ICredentialPointsCalculatorService credentialPointsService, IApplicationBusinessLogicService businessLogicService)
        {
            mLogbookQueryService = logbookQueryService;
            mApplicationQueryService = applicationQueryService;
            mFileStorageService = fileStorageService;

            mActivityPointsService = activityPointsService;
            mCredentialPointsService = credentialPointsService;
            mBusinessLogicService = businessLogicService;
        }

        public GetCredentialResponse GetCredentials(int naatiNumber)
        {
            if (naatiNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(naatiNumber));
            }
            var credentials = mLogbookQueryService.GetCredentials(naatiNumber);
            return credentials;
        }


        public IEnumerable<CertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber)
        {
            return mActivityPointsService.GetCertificationPeriodDetails(naatiNumber);
        }

        public IEnumerable<CredentialCertificationPeriodDetailsDto> GetCredentialCertificationPeriodDetails(int naatiNumber, int credentialId)
        {
            return mCredentialPointsService.GetCertificationPeriodDetails(naatiNumber, credentialId);
        }

        public IEnumerable<CertificationPeriodRequests> GetCertificationPeriodRequests(int naatiNumber)
        {
            var certificationPeriods = GetCertificationPeriodDetails(naatiNumber);
            var applicationIds = certificationPeriods.Where(x => x.SubmittedRecertificationApplicationId.HasValue)
                .Select(y => y.SubmittedRecertificationApplicationId.GetValueOrDefault());

            var results = mLogbookQueryService.GetSubmittedRecertificationCredentialRequests(
                    new GetCerfiationPeriodRequest() { ApplicationIds = applicationIds.ToArray() })
                .GroupBy(y => y.CertificationPeriodId)
                .Select(z => new CertificationPeriodRequests
                {
                    CertificationPeriodId = z.Key,
                    Requests = z.Select(w => new CertificationPeriodRequest { ExternalName = w.ExternalName, Skill = w.Skill })

                });

            return results;
        }

        public WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId, int credentialId)
        {
            return mCredentialPointsService.GetCertificationPeriodCredential(naatiNumber, certificationPeriodId, credentialId);
        }

        public RecertificationStatusResponse GetCredentialRecertificationStatus(int credentialId)
        {
            var status = mBusinessLogicService.CalculateCredentialRecertificationStatus(credentialId);
            return new RecertificationStatusResponse { StatusId = (int)status };
        }

        public bool IsValidWorkPracticeAttachment(int workPracticeAttachmentId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidateWorkPracticeAttachment(workPracticeAttachmentId, naatiNumber);
        }

        public bool IsValidateWorkPractice(int workPracticeId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidateWorkPractice(workPracticeId, naatiNumber);
        }

        public bool IsValidActivityAttachment(int activityAttachmentId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidActivityAttachment(activityAttachmentId, naatiNumber);
        }

        public bool IsValidActivity(int activityId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidActivity(activityId, naatiNumber);
        }

        public bool IsValidWorkPracticeStoredFieldId(int storedFieldId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidWorkPracticeStoredFieldId(storedFieldId, naatiNumber);
        }

        public bool IsValidActivityStoredFieldId(int storedFieldId, int naatiNumber)
        {
            return mLogbookQueryService.IsValidActivityStoredFieldId(storedFieldId, naatiNumber);
        }

        public PdActivityPoints GetProfessionalActivityPoints(int naatiNumber, int certificationPeriodId)
        {
            return mActivityPointsService.CaluculatePointsFor(naatiNumber, certificationPeriodId);
        }

        public IEnumerable<ProfessionalDevelopmentActivityDto> GetProfessionalDevelopmentActivities(int naatiNumber, int certificationPeriodId)
        {
            if (naatiNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(naatiNumber));
            }

            return mActivityPointsService.GetAllCertificationPeriodActivities(naatiNumber, certificationPeriodId);
        }
        public void DeleteWorkPractice(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteWorkPractice(id);
        }

        public void DeleteProfessionalDevelopmentCategoryRequirement(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentCategoryRequirement(id);
        }

        public void DeleteProfessionalDevelopmentActivity(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentActivity(id);
        }

        public void DeleteProfessionalDevelopmentCategory(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentCategory(id);
        }

        public void DeleteProfessionalDevelopmentRequirement(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentRequirement(id);
        }

        public void DeleteProfessionalDevelopmentSection(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentSection(id);
        }

        public void DeleteWorkPracticeAttachment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteWorkPracticeAttachment(id);
        }

        public void DeleteProfessionalDevelopmentActivityAttachment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            mLogbookQueryService.DeleteProfessionalDevelopmentActivityAttachment(id);
        }

        public IEnumerable<WorkPracticeResponse> GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            if (credentialId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(credentialId));
            }

            return mCredentialPointsService.GetWorkPractices(credentialId, naatiNumber, certificationPeriodId);
        }

        public ProfessionalDevelopmentCategoryRequirementResponse GetProfessionalDevelopmentCategoryRequirement(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var model = mLogbookQueryService.GetProfessionalDevelopmentCategoryRequirement(id);
            var result = new ProfessionalDevelopmentCategoryRequirementResponse
            {
                Id = model.Id,
                Points = model.Points,
                ProfessionalDevelopmentCategory = model.ProfessionalDevelopmentCategory,
                ProfessionalDevelopmentRequirement = model.ProfessionalDevelopmentRequirement
            };
            return result;
        }

        public ProfessionalDevelopmentActivityDto GetProfessionalDevelopmentActivity(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var model = mLogbookQueryService.GetProfessionalDevelopmentActivity(id);
            var result = new ProfessionalDevelopmentActivityDto
            {
                Id = model.Id,
                ProfessionalDevelopmentCategoryId = model.ProfessionalDevelopmentCategoryId,
                ProfessionalDevelopmentRequirementId = model.ProfessionalDevelopmentRequirementId,
                Points = model.Points,
                Description = model.Description,
                DateCompleted = model.DateCompleted,
                Notes = model.Notes
            };
            return result;
        }

        public ProfessionalDevelopmentCategoryResponse GetProfessionalDevelopmentCategory(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var model = mLogbookQueryService.GetProfessionalDevelopmentCategory(id);
            var result = new ProfessionalDevelopmentCategoryResponse
            {
                Id = model.Id,
                Description = model.Description,
                Name = model.Name,
            };
            return result;
        }

        public ProfessionalDevelopmentRequirementResponse GetProfessionalDevelopmentRequirement(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var model = mLogbookQueryService.GetProfessionalDevelopmentRequirement(id);
            var result = new ProfessionalDevelopmentRequirementResponse
            {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName
            };
            return result;
        }

        public ProfessionalDevelopmentSectionResponse GetProfessionalDevelopmentSection(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var model = mLogbookQueryService.GetProfessionalDevelopmentSection(id);
            var result = new ProfessionalDevelopmentSectionResponse
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };
            return result;
        }

        public IEnumerable<WorkPracticeAttachmentResponse> GetWorkPracticeAttachments(int workPracticeId)
        {
            if (workPracticeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(workPracticeId));
            }

            return mLogbookQueryService.GetWorkPracticeAttachments(workPracticeId);
        }

        public IEnumerable<ProfessionalDevelopmentActivityAttachmentDto> GetProfessionalDevelopmentActivityAttachments(int activityId)
        {
            if (activityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(activityId));
            }

            return mLogbookQueryService.GetProfessionalDevelopmentActivityAttachments(activityId).List;
        }

        public CreateOrUpdateResponse CreateOrUpdateWorkPractice(WorkPracticeRequest model)
        {
            if (model.CredentialId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.CredentialId));

            return mLogbookQueryService.CreateOrUpdateWorkPractice(model);
        }

        public void CreateOrUpdateProfessionalDevelopmentCategoryRequirement(ProfessionalDevelopmentCategoryRequirementRequest model)
        {
            if (model.ProfessionalDevelopmentCategoryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentCategoryId));
            if (model.ProfessionalDevelopmentRequirementId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentRequirementId));

            mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentCategoryRequirement(model);
        }

        public CreateOrUpdateResponse CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model, int naatiNumber)
        {
            if (model.ProfessionalDevelopmentCategoryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentCategoryId));
            if (model.ProfessionalDevelopmentRequirementId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentRequirementId));

            return mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentActivity(model, naatiNumber);
        }

        public void CreateOrUpdateProfessionalDevelopmentCategory(ProfessionalDevelopmentCategoryRequest model)
        {
            if (model.ProfessionalDevelopmentSectionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentSectionId));

            mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentCategory(model);
        }

        public void CreateOrUpdateProfessionalDevelopmentRequirement(ProfessionalDevelopmentRequirementRequest model)
        {
            mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentRequirement(model);
        }

        public void CreateOrUpdateProfessionalDevelopmentSection(ProfessionalDevelopmentSectionRequest model)
        {
            mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentSection(model);
        }

        public AttachmentResponse CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest model)
        {
            if (model.WorkPracticeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.WorkPracticeId));

            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = model.UserName });
            model.UploadedByUserId = user.UserId.GetValueOrDefault();
            model.StoragePath = $@"{StoredFileType.WorkPractice}\{model.WorkPracticeId}\{model.FileName}";
            var response = mLogbookQueryService.CreateOrUpdateWorkPracticeAttachment(model);

            return new AttachmentResponse() { StoredFileId = response.StoredFileId };
        }

        public AttachmentResponse CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest model)
        {
            if (model.ProfessionalDevelopmentActivityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.ProfessionalDevelopmentActivityId));


            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = model.UserName });
            model.UploadedByUserId = user.UserId.GetValueOrDefault();
            model.StoragePath = $@"{StoredFileType.ProfessionalDevelopmentActivity}\{model.ProfessionalDevelopmentActivityId}\{model.FileName}";
            var response = mLogbookQueryService.CreateOrUpdateProfessionalDevelopmentActivityAttachment(model); ;

            return new AttachmentResponse() { StoredFileId = response.StoredFileId };
        }

        public GetFileResponse GetAttachment(GetFileRequest request)
        {
            return mFileStorageService.GetFile(request);
        }


        public ProfessionalDevelopmentCategoryRequirementListResponse GetProfessionalDevelopmentCategoriesRequirements(int sectionId)
        {
            if (sectionId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sectionId));
            }

            return mLogbookQueryService.GetProfessionalDevelopmentCategoriesRequirements(sectionId);
        }

        public IEnumerable<ProfessionalDevelopmentRequirementResponse> GetProfessionalDevelopmentRequirements(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(categoryId));
            }

            return mLogbookQueryService.GetProfessionalDevelopmentRequirements(categoryId);
        }

        public IEnumerable<ProfessionalDevelopmentCategoryResponse> GetProfessionalDevelopmentCategories()
        {
            return mLogbookQueryService.GetProfessionalDevelopmentCategories();
        }

    }
}
