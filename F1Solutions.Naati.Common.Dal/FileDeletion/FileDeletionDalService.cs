using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F1Solutions.Naati.Common.Dal.FileDeletion
{
    public class FileDeletionDalService : IFileDeletionDalService
    {
        public FileDeletionDalService()
        {

        }

        public GenericResponse<StoredFile> GetStoredFileById(int storedFileId)
        {
            var result = new GenericResponse<StoredFile>();

            var storedFile = NHibernateSession.Current.Get<StoredFile>(storedFileId);

            if (storedFile.IsNull())
            {
                result = new GenericResponse<StoredFile>(null)
                {
                    Success = false
                };

                result.Errors.Add($"StoredFile does not exist for id {storedFileId}");

                return result;
            }

            return new GenericResponse<StoredFile>(storedFile)
            {
                Success = true
            };
        }

        public IEnumerable<int> GetExpiredStoredFiles(string documentType, string entity, int batchSize, int fileCount, int includeQueued, int documentTypeId)
        {
            // only take enough items so that maximum batch size is not exceeded
            var result = NHibernateSession.Current.CreateSQLQuery($"exec GetStoredFilesForFileDeletion {documentTypeId}, {entity}, {includeQueued}, {batchSize - fileCount}")
               .List<int>();

            return result.ToList();
        }

        public GenericResponse<StoredFile> MarkStoredFileAsQueuedForDelete(StoredFileDto storedFileDto)
        {
            var result = GetStoredFileById(storedFileDto.Id);

            if (result.Success)
            {
                var storedFile = result.Data;
                storedFile.StoredFileStatusType = NHibernateSession.Current.Get<StoredFileStatusType>(2); // "Queued" for delete

                if (storedFile.StoredFileStatusType.IsNull())
                {
                    result = new GenericResponse<StoredFile>(storedFile)
                    {
                        Success = false
                    };

                    result.Errors.Add($"Status type for {storedFile.Id} is NULL.");

                    return result;
                }

                storedFile.StoredFileStatusChangeDate = DateTime.Now;

                NHibernateSession.Current.SaveOrUpdate(storedFile);
                NHibernateSession.Current.Flush();

                result = new GenericResponse<StoredFile>()
                {
                    Success = true
                };

                return result;
            }

            return result;
        }

        public GenericResponse<StoredFile> MarkFileAsSoftDeleted(StoredFileDto storedFile)
        {
            var result = GetStoredFileById(storedFile.Id);

            if (result.Success)
            {
                var storedFileToDelete = result.Data;
                storedFileToDelete.StoredFileStatusType = NHibernateSession.Current.Get<StoredFileStatusType>(3); // "Soft Deleted"

                if (storedFileToDelete.StoredFileStatusType.IsNull())
                {
                    result = new GenericResponse<StoredFile>(storedFileToDelete)
                    {
                        Success = false
                    };

                    result.Errors.Add($"Status type for {storedFileToDelete.Id} is NULL.");

                    return result;
                }

                storedFileToDelete.StoredFileStatusChangeDate = DateTime.Now;

                NHibernateSession.Current.SaveOrUpdate(storedFileToDelete);
                NHibernateSession.Current.Flush();

                result = new GenericResponse<StoredFile>()
                {
                    Success = true
                };

                return result;
            }

            return result;
        }

        public GenericResponse<CredentialApplicationAttachment> GetCredentialApplicationAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<CredentialApplicationAttachment>();

            var attachment = (from credentialApplicationAttachment in NHibernateSession.Current.Query<CredentialApplicationAttachment>()
                              where credentialApplicationAttachment.StoredFile.Id == storedFileDto.Id
                              select credentialApplicationAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<CredentialApplicationAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Credential Application Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<CredentialApplicationAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<TestSittingDocument> GetTestSittingDocumentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<TestSittingDocument>();

            var attachment = (from testSittingDocument in NHibernateSession.Current.Query<TestSittingDocument>()
                              where testSittingDocument.StoredFile.Id == storedFileDto.Id
                              select testSittingDocument).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<TestSittingDocument>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Test Sitting Document exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<TestSittingDocument>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<NoteAttachment> GetNoteAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<NoteAttachment>();

            var attachment = (from noteAttachment in NHibernateSession.Current.Query<NoteAttachment>()
                              where noteAttachment.StoredFile.Id == storedFileDto.Id
                              select noteAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<NoteAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Note Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<NoteAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<PersonAttachment> GetPersonAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<PersonAttachment>();

            var attachment = (from personAttachment in NHibernateSession.Current.Query<PersonAttachment>()
                              where personAttachment.StoredFile.Id == storedFileDto.Id
                              select personAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<PersonAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Person Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<PersonAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<MaterialRequestRoundAttachment> GetMaterialRequestRoundAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<MaterialRequestRoundAttachment>();

            var attachment = (from materialRequestRoundAttachment in NHibernateSession.Current.Query<MaterialRequestRoundAttachment>()
                              where materialRequestRoundAttachment.StoredFile.Id == storedFileDto.Id
                              select materialRequestRoundAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<MaterialRequestRoundAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Material Request Round Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<MaterialRequestRoundAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<WorkPracticeAttachment> GetWorkPracticeAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<WorkPracticeAttachment>();

            var attachment = (from workPracticeAttachment in NHibernateSession.Current.Query<WorkPracticeAttachment>()
                              where workPracticeAttachment.StoredFile.Id == storedFileDto.Id
                              select workPracticeAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<WorkPracticeAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Work Practice Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<WorkPracticeAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public GenericResponse<ProfessionalDevelopmentActivityAttachment> GetProfessionalDevelopmentActivityAttachmentByStoredFileId(StoredFileDto storedFileDto)
        {
            var result = new GenericResponse<ProfessionalDevelopmentActivityAttachment>();

            var attachment = (from professionalDevelopmentActivityAttachment in NHibernateSession.Current.Query<ProfessionalDevelopmentActivityAttachment>()
                              where professionalDevelopmentActivityAttachment.StoredFile.Id == storedFileDto.Id
                              select professionalDevelopmentActivityAttachment).ToList().FirstOrDefault();

            if (attachment.IsNull())
            {
                result = new GenericResponse<ProfessionalDevelopmentActivityAttachment>(null)
                {
                    Success = false
                };

                result.Errors.Add($"No Professional Development Activity Attachment exists for StoredFile {storedFileDto.Id}.");

                return result;
            }

            result = new GenericResponse<ProfessionalDevelopmentActivityAttachment>(attachment)
            {
                Success = true
            };

            return result;
        }

        public void CreateCredentialApplicationNote(StoredFileDto storedFileDto, CredentialApplicationAttachment credentialApplicationAttachment, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);

            var credentialApplicationNoteToCreate = new CredentialApplicationNote()
            {
                CredentialApplication = NHibernateSession.Current.Get<CredentialApplication>(credentialApplicationAttachment.CredentialApplication.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(credentialApplicationNoteToCreate);
            NHibernateSession.Current.Flush();
        }

        public void CreateTestSittingNote(StoredFileDto storedFileDto, TestSittingDocument testSittingDocument, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);

            var testSittingNoteToCreate = new TestSittingNote()
            {
                TestSitting = NHibernateSession.Current.Get<TestSitting>(testSittingDocument.TestSitting.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(testSittingNoteToCreate);
            NHibernateSession.Current.Flush();
        }

        public void CreateEntityNote(StoredFileDto storedFileDto, PersonAttachment personAttachment, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);

            var entityNote = new NaatiEntityNote()
            {
                Entity = NHibernateSession.Current.Get<NaatiEntity>(personAttachment.Person.Entity.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();
        }

        public void CreateEntityNote(StoredFileDto storedFileDto, WorkPracticeAttachment workPracticeAttachment, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);
            var entity = workPracticeAttachment.WorkPractice.Credential.CredentialCredentialRequests.FirstOrDefault().CredentialRequest.CredentialApplication.Person.Entity;

            var entityNote = new NaatiEntityNote()
            {
                Entity = NHibernateSession.Current.Get<NaatiEntity>(entity.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();
        }

        public void CreateEntityNote(StoredFileDto storedFileDto, ProfessionalDevelopmentActivityAttachment professionalDevelopmentActivityAttachment, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);
            var entity = professionalDevelopmentActivityAttachment.ProfessionalDevelopmentActivity.Person.Entity;

            var entityNote = new NaatiEntityNote()
            {
                Entity = NHibernateSession.Current.Get<NaatiEntity>(entity.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();
        }

        public void CreateMaterialRequestNote(StoredFileDto storedFileDto, MaterialRequestRoundAttachment materialRequestRoundAttachment, int currentUserId)
        {
            var noteToCreate = CreateNote(storedFileDto, currentUserId);

            var materialRequestNoteToCreate = new MaterialRequestNote()
            {
                MaterialRequest = NHibernateSession.Current.Get<MaterialRequest>(materialRequestRoundAttachment.MaterialRequestRound.MaterialRequest.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(materialRequestNoteToCreate);
            NHibernateSession.Current.Flush();
        }

        public Note CreateNote(StoredFileDto storedFileDto, int currentUserId)
        {
            var noteToCreate = new Note()
            {
                User = NHibernateSession.Current.Get<User>(currentUserId),
                Description = $"File {storedFileDto.FileName} of type {storedFileDto.DocumentType.DisplayName} was deleted from {storedFileDto.Id} " +
                $"{storedFileDto.ExternalFileId} in batch {DateTime.Now:yyyyMMdd} at {DateTime.Now:HHmmss}.",
                CreatedDate = DateTime.Now,
                ModifiedDate = null,
                Highlight = false,
                ReadOnly = true
            };

            NHibernateSession.Current.Save(noteToCreate);
            NHibernateSession.Current.Flush();

            return noteToCreate;
        }

        public List<int> GetPersonImagesToDelete(int batchSize, int fileCount)
        {
            // get policy for person image
            var policy = NHibernateSession.Current.Get<StoredFileDeletePolicy>(4); // Person image policy

            //BL is in TFS214236

            var getImageIdsSql = $@"
            SELECT 
                TOP({batchSize - fileCount}) * 
            FROM
            (
                SELECT 
	                personImage.PersonImageId
                FROM
	                tblPersonImage personImage
                WHERE 
				    -- The person is a practitioner - have at least one active certification period 
					( 
						personImage.PersonId NOT IN 
						(
							SELECT DISTINCT certificationPeriod.personId
							FROM
								tblCertificationPeriod certificationPeriod 
							WHERE 
								certificationPeriod.EndDate > DATEADD(DAY, -180, GETDATE()) 
						)
					)
					AND
					(
						--There are no In progress applications
						personImage.PersonId NOT IN
						(
							select PersonId from 
							tblCredentialApplication
							where 
							CredentialApplicationStatusTypeId in 
							(
								select CredentialApplicationStatusTypeId from tblCredentialApplicationStatusType 
								where Name not in ('Draft','Rejected','Deleted','Completed')
							)
						)
					)
					AND
					(
						--At least one active non-certification credentials for e.g. CCL/CLA/Ethics/Intercultural/Knowledge tests
						personImage.PersonId NOT IN
						(
							select PersonId from 
							tblCredentialApplication a inner join
							tblCredentialRequest b on a.CredentialApplicationId = b.CredentialApplicationId inner join
							tblCredentialCredentialRequest c on c.CredentialRequestId = b.CredentialRequestId inner join
							tblCredential d on d.CredentialId = c.CredentialId
							where d.CertificationPeriodId IS NULL
							and d.ExpiryDate > GetDate()
						)
					)
					AND
					(
						personImage.PersonId NOT IN
						(							--At least one active exempted credential of the person.
						select personid from tblCredentialPrerequisiteExemption
						where EndDate IS NULL
						)
					)
	                AND
					--The photo was uploaded less than 3 years ago (compared to current date)
		                personImage.PhotoDate <= DATEADD(DAY, {-policy.DaysToKeep}, GETDATE())
             )B";

            //var getImageIdsSql = $@"
            //SELECT 
            //    TOP({batchSize - fileCount}) * 
            //FROM
            //(
            //    SELECT DISTINCT
            //     personImage.PersonImageId
            //    FROM
            //     tblPersonImage personImage INNER JOIN
            //     tblPerson person on personImage.PersonId = person.PersonId LEFT JOIN
            //     tblCertificationPeriod certificationPeriod on person.PersonId = certificationPeriod.PersonId LEFT JOIN
            //     tblCredential credential on certificationPeriod.CertificationPeriodId = credential.CertificationPeriodId
            //    WHERE
            //     (certificationPeriod.CertificationPeriodId IS NULL
            //      OR certificationPeriod.EndDate < GETDATE())
            //     AND (credential.CredentialId IS NULL
            //       OR (credential.StartDate >= GETDATE()
            //         AND (credential.TerminationDate IS NOT NULL OR credential.TerminationDate < GETDATE())))
            //     AND personImage.PhotoDate <= DATEADD(DAY, {-policy.DaysToKeep}, GETDATE())
            //) A";



            // get all person images older than days to keep
            var expiredPersonImages = NHibernateSession.Current.CreateSQLQuery(getImageIdsSql).List<int>().ToList();

            return expiredPersonImages;
        }

        public void DeletePersonImage(int personImageId, int currentUserId)
        {
            // store image id and person before the perosn image is deleted
            var personImage = NHibernateSession.Current.Get<PersonImage>(personImageId);
            var person = personImage.Person;

            NHibernateSession.Current.Delete(personImage);
            NHibernateSession.Current.Flush();

            CreatePersonImageDeletedNote(person, currentUserId);
        }

        public FileDeletionDetails GetStatusCount(FileDeletionDetails fileDeletionDetails)
        {
            foreach(var storedFileId in fileDeletionDetails.IdsToDelete)
            {
                var status = GetStoredFileStatus(storedFileId);

                if(status == 1)
                {
                    fileDeletionDetails.CurrentStatusCount++;
                }

                if (status == 2)
                {
                    fileDeletionDetails.QueuedStatusCount++;
                }
            }

            return fileDeletionDetails;
        }

        public GenericResponse<FileDeletionEstimationDetails> GetTotalStoredFilesLeftToDelete()
        {
            var estimationDetails = NHibernateSession.Current.TransformSqlQueryDataRowResult<FileDeletionEstimationDetails>($"exec GetRemainingStoredFilesForDeletionCount null, 0").FirstOrDefault();

            if (estimationDetails.IsNull())
            {
                return new GenericResponse<FileDeletionEstimationDetails>(null)
                {
                    Success = false,
                    Errors = new List<string>() { "Could not get count of files remaining for deletion from the GetRemainingStoredFilesForDeletionCount stored procedure" }
                };
            }

            return new GenericResponse<FileDeletionEstimationDetails>(estimationDetails);
        }

        public GenericResponse<IDictionary<string, int>> GetCountOfFilesDeletedEachDayLast7Days()
        {
            var processedFilesEachDayForWeekList = new Dictionary<string, int>();

            foreach(DateTime day in EachDay(DateTime.Today.Date.AddDays(-6), DateTime.Today.Date))
            {
                var processedFilesEachDayForWeekCount = NHibernateSession.Current.Query<StoredFile>()
                .Where(
                    x => x.StoredFileStatusType.Id == 3 &&
                    x.StoredFileStatusChangeDate.Date == day.Date
                ).ToList().Count;

                processedFilesEachDayForWeekList.Add(day.ToString("dd-MM-yyyy") ,processedFilesEachDayForWeekCount);
            }

            return processedFilesEachDayForWeekList;
        }

        public GenericResponse<List<FileDeletionGeneratorPolicy>> GetFileDeletionGeneratorPolicies()
        {
            var getFileDeletionGeneratorPoliciesSql = 
            $@"
            SELECT DISTINCT
                a.PolicyExecutionOrder,
                a.StoredFileDeletePolicyId,
                EntityType,
                c.Name,
                c.DocumentTypeId, 
                PolicyDescription, 
                DaysTokeep 
            FROM
                [dbo].[tblStoredFileDeletePolicy] a LEFT JOIN
                [dbo].[tblStoredFileDeletePolicyDocumentType] b on a.StoredFileDeletePolicyId = b.StoredFileDeletePolicyId LEFT JOIN
                tluDocumentType c on b.DocumentTypeId = c.DocumentTypeId
            ORDER BY 
                PolicyExecutionOrder
            ";

            var fileDeletionGeneratorPolicies = NHibernateSession.Current.TransformSqlQueryDataRowResult<FileDeletionGeneratorPolicy>(getFileDeletionGeneratorPoliciesSql).ToList();

            return fileDeletionGeneratorPolicies;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private void CreatePersonImageDeletedNote(Person person, int currentUserId)
        {
            // create entity note so the note shows on the person screen in ncms
            var entityNote = new NaatiEntityNote()
            {
                Entity = NHibernateSession.Current.Get<NaatiEntity>(person.Entity.Id),
                Note = CreateNote(currentUserId)
            };

            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();
        }

        private Note CreateNote(int currentUserId)
        {
            var note = new Note()
            {
                User = NHibernateSession.Current.Get<User>(currentUserId),
                Description = $"Person Image was permanently deleted from the system.",
                CreatedDate = DateTime.Now,
                ModifiedDate = null,
                Highlight = false,
                ReadOnly = true
            };

            NHibernateSession.Current.Save(note);
            NHibernateSession.Current.Flush();

            return note;
        }

        private int GetStoredFileStatus(int storedFileId)
        {
            var storedFile = NHibernateSession.Current.Get<StoredFile>(storedFileId);
            return storedFile.StoredFileStatusType.Id;
        } 
    }
}