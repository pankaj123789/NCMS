using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Ncms.Contracts.Models;

namespace Ncms.Bl
{
    public interface ILogbookService
    {
        GenericResponse<IEnumerable<WorkPracticeResponse>> GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId);
        GenericResponse<bool> DeleteWorkPractice(int id);
        int CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest request);
        GenericResponse<IEnumerable<WorkPracticeAttachmentModel>> GetWorkPracticeAttachments(int id);
        GenericResponse<bool> DeleteWorkPracticeAttachment(int id);
        GenericResponse<CreateOrUpdateResponse> CreateOrUpdateWorkPractice(WorkPracticeRequest model);
		int CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest request);
		GenericResponse<IEnumerable<ProfessionalDevelopmentActivityAttachmentModel>> GetProfessionalDevelopmentActivityAttachments(int id);
		GenericResponse<CreateOrUpdateResponse> CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest request);
		GenericResponse<bool> DeleteProfessionalDevelopmentActivityAttachment(int id);
		GenericResponse<bool> DetachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId);
		GenericResponse<bool> AttachProfessionalDevelopmentActivity(AttachActivityRequest request);
		GenericResponse<bool> DeleteProfessionalDevelopmentActivity(int id);
        WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId,
            int credentialId);
        IEnumerable<CertificationPeriodDetailsDto> GetCredentialCertificationPeriodDetails(int naatiNumber,
            int credentialId);
        IEnumerable<WorkPracticeResponse> GetWorkPracticeActivities(int credentialId, int naatiNumber,
            int certificationPeriodId);
        void AttachWorkPractice(int workPracticeId, int credentialApplicationId, int credentialId);
        void DetachWorkPractice(int id);
    }

    public class LogbookService : ILogbookService
    {
        private readonly ILogbookQueryService _logbookQueryService ;
        private readonly ISystemQueryService _systemQueryService;
        private readonly ICredentialPointsCalculatorService _credentialPointsCalculatorService;

        public LogbookService(ILogbookQueryService logbookQueryService, ISystemQueryService systemQueryService, ICredentialPointsCalculatorService credentialPointsCalculatorService)
        {
            _logbookQueryService = logbookQueryService;
            _systemQueryService = systemQueryService;
            _credentialPointsCalculatorService = credentialPointsCalculatorService;
        }

        public GenericResponse<IEnumerable<WorkPracticeResponse>> GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            return new GenericResponse<IEnumerable<WorkPracticeResponse>>(_credentialPointsCalculatorService.GetWorkPractices(credentialId, naatiNumber, certificationPeriodId));
        }

        public GenericResponse<bool> DeleteWorkPractice(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            try
            {
                _logbookQueryService.DeleteWorkPractice(id);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return true;
        }

        public int CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest request)
        {
            try
            {
                var file = _logbookQueryService.CreateOrUpdateWorkPracticeAttachment(request);
                return file.StoredFileId;
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<bool> DeleteWorkPracticeAttachment(int id)
        {
            try
            {
                _logbookQueryService.DeleteWorkPracticeAttachment(id);
                return true;
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<CreateOrUpdateResponse> CreateOrUpdateWorkPractice(WorkPracticeRequest model)
        {
            try
            {
                if (model.CredentialId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(model.CredentialId));
                
                var certPeriod = _credentialPointsCalculatorService.GetCertificationPeriodDetails(model.NaatiNumber,
                    model.CredentialId).FirstOrDefault(x=>x.Id == model.CertificationPeriodId);

                if (certPeriod != null && certPeriod.IsCredentialSubmitted)
                {
                    model.CredentialApplicationId = certPeriod.SubmittedRecertificationApplicationId ?? 0;
                }
                else
                {
                    model.CredentialApplicationId = 0;
                }

                return _logbookQueryService.CreateOrUpdateWorkPractice(model);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<IEnumerable<WorkPracticeAttachmentModel>> GetWorkPracticeAttachments(int id)
        {
            var response = _logbookQueryService.GetWorkPracticeAttachments(id).Select(x=>new WorkPracticeAttachmentModel
            {
                WorkPracticeAttachmentId = x.Id,
                StoredFileId = x.StoredFile.Id,
                Title = x.Description,
                DocumentType = x.StoredFile.DocumentType.DisplayName,
                FileSize = x.StoredFile.FileSize,
                FileName = x.StoredFile.FileName,
                UploadedByName = x.StoredFile.UploadedByName,
                UploadedDateTime = x.StoredFile.UploadedDateTime,
                FileType = Path.GetExtension(x.StoredFile.FileName)?.Trim('.'),
                SoftDeleteDate = x.StoredFile.StoredFileStatusType != 1 ? x.StoredFile.StoredFileStatusChangedDate : (DateTime?)null
            });
            return new GenericResponse<IEnumerable<WorkPracticeAttachmentModel>>(response);
		}

		public GenericResponse<bool> DeleteProfessionalDevelopmentActivity(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id));
			}
			try
			{
				_logbookQueryService.DeleteProfessionalDevelopmentActivity(id);
			}
			catch (WebServiceException e)
			{
				throw new UserFriendlySamException(e.Message);
			}

			return true;
		}


		public int CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest request)
		{
			try
			{
				var file = _logbookQueryService.CreateOrUpdateProfessionalDevelopmentActivityAttachment(request);
				return file.StoredFileId;
			}
			catch (WebServiceException e)
			{
				throw new UserFriendlySamException(e.Message);
			}
		}

		public GenericResponse<bool> DeleteProfessionalDevelopmentActivityAttachment(int id)
		{
			try
			{
				_logbookQueryService.DeleteProfessionalDevelopmentActivityAttachment(id);
				return true;
			}
			catch (WebServiceException e)
			{
				throw new UserFriendlySamException(e.Message);
			}
		}

        public GenericResponse<bool> DetachProfessionalDevelopmentActivity(int activityId, int credentialAplicationId)
        {
            try
            {
                _logbookQueryService.DetachProfessionalDevelopmentActivity(activityId, credentialAplicationId);
                return true;
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<bool> AttachProfessionalDevelopmentActivity(AttachActivityRequest request)
        {
            try
            {
                _logbookQueryService.AttachProfessionalDevelopmentActivity(request.ActivityId, request.CredentialApplicationId);
                return true;
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<CreateOrUpdateResponse> CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model)
		{
			try
			{
				return _logbookQueryService.CreateOrUpdateProfessionalDevelopmentActivity(model, model.NaatiNumber);
			}
			catch (WebServiceException e)
			{
				throw new UserFriendlySamException(e.Message);
			}
		}

		public GenericResponse<IEnumerable<ProfessionalDevelopmentActivityAttachmentModel>> GetProfessionalDevelopmentActivityAttachments(int id)
		{
            var response = _logbookQueryService.GetProfessionalDevelopmentActivityAttachments(id).List.Select(x => new ProfessionalDevelopmentActivityAttachmentModel
            {
                ProfessionalDevelopmentActivityAttachmentId = x.Id,
                StoredFileId = x.StoredFile.Id,
                Title = x.Description,
                DocumentType = x.StoredFile.DocumentType.DisplayName,
                FileSize = x.StoredFile.FileSize,
                FileName = x.StoredFile.FileName,
                UploadedByName = x.StoredFile.UploadedByName,
                UploadedDateTime = x.StoredFile.UploadedDateTime,
                FileType = Path.GetExtension(x.StoredFile.FileName)?.Trim('.'),
                SoftDeleteDate = x.StoredFile.StoredFileStatusType != 1 ? x.StoredFile.StoredFileStatusChangedDate : (DateTime?)null
            });
			return new GenericResponse<IEnumerable<ProfessionalDevelopmentActivityAttachmentModel>>(response);
		}

        public WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId, int credentialId)
        {
            return _credentialPointsCalculatorService.GetCertificationPeriodCredential(naatiNumber, certificationPeriodId, credentialId);
        }

        public void AttachWorkPractice(int workPracticeId, int credentialApplicationId, int credentialId)
        {
            if (workPracticeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(workPracticeId));
            if (credentialApplicationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(credentialApplicationId));
            if (credentialId <= 0)
                throw new ArgumentOutOfRangeException(nameof(credentialId));

            _logbookQueryService.AttachWorkPractice(workPracticeId, credentialApplicationId, credentialId);
        }

        public void DetachWorkPractice(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            _logbookQueryService.DetachWorkPractice(id);
        }

        public IEnumerable<CertificationPeriodDetailsDto> GetCredentialCertificationPeriodDetails(int naatiNumber, int credentialId)
        {
            return _credentialPointsCalculatorService.GetCertificationPeriodDetails(naatiNumber, credentialId);
        }
        public IEnumerable<WorkPracticeResponse> GetWorkPracticeActivities(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            return _credentialPointsCalculatorService.GetWorkPractices(credentialId, naatiNumber, certificationPeriodId);
        }
    }

	public class BaseAttachmentModel
	{
		public int StoredFileId { get; set; }
		public string Title { get; set; }
		public string DocumentType { get; set; }
		public string UploadedByName { get; set; }
		public DateTime UploadedDateTime { get; set; }
		public long FileSize { get; set; }
		public string FileType { get; set; }
		public string FileName { get; set; }
        public DateTime? SoftDeleteDate { get; set; }

    }

    public class WorkPracticeAttachmentModel : BaseAttachmentModel
	{
		public int WorkPracticeAttachmentId { get; set; }
	}

	public class ProfessionalDevelopmentActivityAttachmentModel : BaseAttachmentModel
	{
		public int ProfessionalDevelopmentActivityAttachmentId { get; set; }
	}
}
