using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using GetFileRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetFileRequest;

namespace MyNaati.Ui.Controllers
{
	[Authorize]
	public class LogbookController : NewtonsoftController
	{
		private readonly ILogbookService mLogbookService;
		private readonly IPersonalDetailsService mPersonalDetailsService;
		private readonly ILookupProvider mLookupProvider;
        private readonly ISecretsCacheQueryService mSecretsProvider;

        public LogbookController(ILogbookService logbookService, IPersonalDetailsService personalDetailsService, ILookupProvider lookupProvider, ISecretsCacheQueryService secretsProvider)
		{
			mLogbookService = logbookService;
			mPersonalDetailsService = personalDetailsService;
			mLookupProvider = lookupProvider;
            mSecretsProvider = secretsProvider;
        }

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Credentials()
		{
			var credentials = mLogbookService.GetCredentials(CurrentUserNaatiNumber).List;
			if (credentials == null || !credentials.Any())
			{
				return Json(new string[] { }, JsonRequestBehavior.AllowGet);
			}

			return Json(credentials, JsonRequestBehavior.AllowGet);
		}


		public ActionResult CredentialCertificationPeriods(int credentialId)
		{
			var certificationPeriods = mLogbookService.GetCredentialCertificationPeriodDetails(CurrentUserNaatiNumber, credentialId);
			if (certificationPeriods == null || !certificationPeriods.Any())
			{
				return Json(new string[] { }, JsonRequestBehavior.AllowGet);
			}

			return Json(certificationPeriods, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Credential(int id)
		{
			var credential = mLogbookService.GetCredentials(CurrentUserNaatiNumber).List.FirstOrDefault(c => c.Id == id);

			if (credential == null)
			{
				return Json(new string[] { }, JsonRequestBehavior.AllowGet);
			}

			return Json(credential, JsonRequestBehavior.AllowGet);
		}

		public ActionResult CertificationPeriodsRequests()
		{
			var credentialRequests = mLogbookService.GetCertificationPeriodRequests(CurrentUserNaatiNumber);

			return Json(credentialRequests, JsonRequestBehavior.AllowGet);
		}
		public ActionResult CertificationPeriodCredential(int credentialId, int certificationPeriodId)
		{
			var credential =
				mLogbookService.GetCertificationPeriodCredential(CurrentUserNaatiNumber, certificationPeriodId,
					credentialId);

			if (credential == null)
			{
				return Json(new string[] { }, JsonRequestBehavior.AllowGet);
			}

			return Json(credential, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Attachments()
		{
			return Json(mLogbookService.GetCredentials(CurrentUserNaatiNumber).List.Select(x => new
			{
				x.Id,
				x.Skill,
				x.Direction,
			}), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Activities(int certificationPeriodId)
		{
			return
				Json(
					new
					{
						List = mLogbookService.GetProfessionalDevelopmentActivities(CurrentUserNaatiNumber, certificationPeriodId),
						Catalogue = mLookupProvider.SystemValues.PdCatalogue
					}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult CredentialsRequests(int certificationPeriodId)
		{
			return
				Json(mLogbookService.GetProfessionalDevelopmentActivities(CurrentUserNaatiNumber, certificationPeriodId), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CertificationPeriods()
		{
			return
				Json(mLogbookService.GetCertificationPeriodDetails(CurrentUserNaatiNumber), JsonRequestBehavior.AllowGet);
		}


		public ActionResult GetCategoriesRequirements(int sectionId)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentCategoriesRequirements(sectionId), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Categories()
		{
			return Json(mLogbookService.GetProfessionalDevelopmentCategories(), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ActivityPoints(int certificationPeriodId)
		{
			return Json(mLogbookService.GetProfessionalActivityPoints(CurrentUserNaatiNumber, certificationPeriodId), JsonRequestBehavior.AllowGet);
		}

		public ActionResult Requirements(int categoryId)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentRequirements(categoryId), JsonRequestBehavior.AllowGet);
		}

		public void DeleteWorkPractice(int id)
		{
		    if (!mLogbookService.IsValidateWorkPractice(id, CurrentUserNaatiNumber))
		    {
		        throw new MyNaatiSecurityException();
		    }
		    mLogbookService.DeleteWorkPractice(id);
		}

		[HttpDelete]
		public void DeleteProfessionalDevelopmentCategoryRequirement(int id)
		{
			mLogbookService.DeleteProfessionalDevelopmentCategoryRequirement(id);
		}

		[HttpDelete]
		public void DeleteProfessionalDevelopmentActivity(int id)
		{
		    if (!mLogbookService.IsValidActivity(id, CurrentUserNaatiNumber))
		    {
		        throw new MyNaatiSecurityException();
		    }
            mLogbookService.DeleteProfessionalDevelopmentActivity(id);
        }

		[HttpDelete]
		public void DeleteProfessionalDevelopmentCategory(int id)
		{
			mLogbookService.DeleteProfessionalDevelopmentCategory(id);
		}

		[HttpDelete]
		public void DeleteProfessionalDevelopmentRequirement(int id)
		{
			mLogbookService.DeleteProfessionalDevelopmentRequirement(id);
		}

		[HttpDelete]
		public void DeleteProfessionalDevelopmentSection(int id)
		{
			mLogbookService.DeleteProfessionalDevelopmentSection(id);
		}

		[HttpDelete]
		public void DeleteWorkPracticeAttachment(int id)
		{
		    if (!mLogbookService.IsValidWorkPracticeAttachment(id, CurrentUserNaatiNumber))
		    {
		        throw new MyNaatiSecurityException();
            }

            mLogbookService.DeleteWorkPracticeAttachment(id);
		}
	    
        [HttpDelete]
		public void DeleteProfessionalDevelopmentActivityAttachment(int id)
		{
		    if (!mLogbookService.IsValidActivityAttachment(id, CurrentUserNaatiNumber))
		    {
		        throw new MyNaatiSecurityException();
            }

            mLogbookService.DeleteProfessionalDevelopmentActivityAttachment(id);
		}

		[HttpGet]
		public ActionResult GetWorkPractices(int credentialId, int certificationPeriodId)
		{
			return Json(mLogbookService.GetWorkPractices(credentialId, CurrentUserNaatiNumber, certificationPeriodId), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentCategoryRequirement(int id)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentCategoryRequirement(id), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentActivity(int id)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentActivity(id), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentCategory(int id)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentCategory(id), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentRequirement(int id)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentRequirement(id), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentSection(int id)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentSection(id), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetWorkPracticeAttachments(int workPracticeId)
		{
			return Json(mLogbookService.GetWorkPracticeAttachments(workPracticeId), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult GetProfessionalDevelopmentActivityAttachments(int activityId)
		{
			return Json(mLogbookService.GetProfessionalDevelopmentActivityAttachments(activityId), JsonRequestBehavior.AllowGet);
		}

		public ActionResult CreateOrUpdateWorkPractice(WorkPracticeRequest model)
		{
			model.Date = model.Date.Date;
			var response = mLogbookService.CreateOrUpdateWorkPractice(model);
			return Json(new { Success = true, Id = response.Id }, JsonRequestBehavior.AllowGet);
		}

		public void CreateOrUpdateProfessionalDevelopmentCategoryRequirement(ProfessionalDevelopmentCategoryRequirementRequest model)
		{
			mLogbookService.CreateOrUpdateProfessionalDevelopmentCategoryRequirement(model);
		}

		[HttpPost]
		public ActionResult CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model)
		{
			model.DateCompleted = model.DateCompleted.Date;
			var response = mLogbookService.CreateOrUpdateProfessionalDevelopmentActivity(model, CurrentUserNaatiNumber);
			return Json(new { Success = true, Id = response.Id }, JsonRequestBehavior.AllowGet);
		}

		public void CreateOrUpdateProfessionalDevelopmentCategory(ProfessionalDevelopmentCategoryRequest model)
		{
			mLogbookService.CreateOrUpdateProfessionalDevelopmentCategory(model);
		}

		public void CreateOrUpdateProfessionalDevelopmentRequirement(ProfessionalDevelopmentRequirementRequest model)
		{
			mLogbookService.CreateOrUpdateProfessionalDevelopmentRequirement(model);
		}

		public void CreateOrUpdateProfessionalDevelopmentSection(ProfessionalDevelopmentSectionRequest model)
		{
			mLogbookService.CreateOrUpdateProfessionalDevelopmentSection(model);
		}

		[HttpPost]
		public ActionResult CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest model)
		{
			var ids = new List<int>();

		    var random = new Random();
            for (var i = 0; i < Request.Files.Count; i++)
			{
				var file = Request.Files[i];
				if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
				{
					continue;
				}

				var randomNumber = random.Next(10000, 99999);
				var fileName = Path.GetFileNameWithoutExtension(file.FileName) + randomNumber + Path.GetExtension(file.FileName);
				var title = Path.GetFileNameWithoutExtension(file.FileName);

				var workPracticeId = Convert.ToInt32(Request["WorkPracticeId"]);
				var workPracticeAttachmentId = Convert.ToInt32(Request["WorkPracticeAttachmentId"]);

				string path;

				using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + fileName))
				{
					file.InputStream.Seek(0, SeekOrigin.Begin);
					file.InputStream.CopyTo(fileStream);
					path = fileStream.Name;
				}

                var defaultIdentity = mSecretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
				var request = new WorkPracticeAttachmentRequest()
				{
					WorkPracticeId = workPracticeId,
					Id = workPracticeAttachmentId,
					FileName = fileName,
					FilePath = path,
					UserName = defaultIdentity,
					Description = title,
					TokenToRemoveFromFilename = randomNumber.ToString()
				};

				ids.Add(mLogbookService.CreateOrUpdateWorkPracticeAttachment(request).StoredFileId);
			}

			return Json(new { success = true, ids });
		}

		[HttpPost]
		public ActionResult CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest model)
		{
			var ids = new List<int>();

            var random = new Random();
			for (var i = 0; i < Request.Files.Count; i++)
			{
				var file = Request.Files[i];
				if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
				{
					continue;
				}

				var randomNumber = random.Next(10000, 99999);
				var fileName = Path.GetFileNameWithoutExtension(file.FileName) + randomNumber + Path.GetExtension(file.FileName);
				var title = Path.GetFileNameWithoutExtension(file.FileName);

				var activityId = Convert.ToInt32(Request["ActivityId"]);
				var activityAttachmentId = Convert.ToInt32(Request["ActivityAttachmentId"]);

				string path;

				using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + fileName))
				{
					file.InputStream.Seek(0, SeekOrigin.Begin);
					file.InputStream.CopyTo(fileStream);
					path = fileStream.Name;
				}

                var defaultIdentity = mSecretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
                var request = new ProfessionalDevelopmentActivityAttachmentRequest()
				{
					ProfessionalDevelopmentActivityId = activityId,
					Id = activityAttachmentId,
					FileName = fileName,
					FilePath = path,
					UserName = defaultIdentity,
					Description = title,
					TokenToRemoveFromFilename = randomNumber.ToString()
				};

				ids.Add(mLogbookService.CreateOrUpdateProfessionalDevelopmentActivityAttachment(request).StoredFileId);
			}

			return Json(new { success = true, ids });
		}

		[HttpGet]
		public FileStreamResult DownloadActivityAttachment(int id)
		{

		    if (!mLogbookService.IsValidActivityStoredFieldId(id, CurrentUserNaatiNumber))
		    {
		        throw new MyNaatiSecurityException();
            }
		        

            var requestContract = new GetFileRequest
			{
				StoredFileId = id,
				TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
			};

			var file = mLogbookService.GetAttachment(requestContract);

			return File(OpenToMemoryAndDispose(file.FilePaths[0]), MimeMapping.GetMimeMapping(file.FileName), file.FileName);
		}

	    [HttpGet]
	    public FileStreamResult DownloadWorkPracticeAttachment(int id)
	    {

	        if (!mLogbookService.IsValidWorkPracticeStoredFieldId(id, CurrentUserNaatiNumber))
	        {
	            throw new MyNaatiSecurityException();
            }

	        var requestContract = new GetFileRequest
	        {
	            StoredFileId = id,
	            TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"]
	        };

	        var file = mLogbookService.GetAttachment(requestContract);

	        return File(OpenToMemoryAndDispose(file.FilePaths[0]), MimeMapping.GetMimeMapping(file.FileName), file.FileName);
	    }

        [HttpGet]
		public ActionResult RecertificationOptions()
		{
			var userName = User.Identity.Name;
			var naatiNumber = CurrentUserNaatiNumber;
			var isRecertification = false;
			var recertificationFormId = mLookupProvider.SystemValues.DefaultRecertificationForm;

			var credentials = mLogbookService.GetCredentials(naatiNumber).List;
			foreach (var credentialDto in credentials)
			{
				var response = mLogbookService.GetCredentialRecertificationStatus(credentialDto.Id);
				if (response.StatusId == (int)RecertificationStatus.EligibleForNew)
				{
					isRecertification = true;
					break;
				}
			}

			return Json(new
			{
				IsRecertification = isRecertification,
				RecertificationFormUrl = Url.Content($"apply/{recertificationFormId}")
			}, JsonRequestBehavior.AllowGet);
		}

		private static MemoryStream OpenToMemoryAndDispose(string path)
		{
			var fileData = new MemoryStream(System.IO.File.ReadAllBytes(path), false)
			{
				Position = 0
			};

			System.IO.File.Delete(path);

			return fileData;
		}
	}
}
