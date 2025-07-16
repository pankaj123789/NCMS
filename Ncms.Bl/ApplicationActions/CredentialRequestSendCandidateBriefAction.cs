using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestSendCandidateBriefAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Email;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Send;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSessionAccepted };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestSessionAccepted;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidatePedingBriefs

        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            MarkDocumentsAsSent,
            AddAttachments,
            SetExitState,
        };


        private void MarkDocumentsAsSent()
        {
            var emailedDate = DateTime.Now;
            foreach (var brief in ApplicationBriefsModel.Briefs)
            {
                CredentialRequestModel.Briefs.Add(new CandidateBriefModel
                {
                    CandidateBriefId = brief.CandidateBriefId.GetValueOrDefault(),
                    EmailedDate = emailedDate,
                    TestMaterialAttachmentId = brief.TestMaterialAttachmentId,
                    TestSittingId = TestSessionModel.CredentialTestSessionId
                });

            }
        }

        protected override string GetNote()
        {
            return Naati.Resources.Application.DocumentBriefSentNote;
        }

        private string GetBriefDocumentName(CandidateBriefFileInfoModel brief)
        {
            var briefFileName = $"{ApplicationModel.ApplicationInfo.NaatiNumber} - Candidate Brief - #{brief.TestMaterialId} - {brief.TaskTypeLabel}{brief.TaskLabel}";
            return briefFileName;
        }
        public override IEnumerable<DocumentData> GetDocumentsPreview(DocumentsPreviewRequestModel request)
        {
            var pendingBriefDetails = ApplicationBriefsModel;

            var documents = new List<DocumentData>();
            foreach (var brief in pendingBriefDetails.Briefs)
            {
                var fileInfo = FileService.GetFile(brief.StorageFileId);
                var directory = Path.GetDirectoryName(fileInfo.FilePath);
                var fileType = Path.GetExtension(fileInfo.FilePath);

                var briefFileName = $"{GetBriefDocumentName(brief)}{fileType}";
                var briefPath = Path.Combine(directory, briefFileName);

                if (File.Exists(briefPath))
                {
                    File.Delete(briefPath);
                }

                var briefInfo = new FileInfo(briefPath);

                File.Move(fileInfo.FilePath, briefPath);

                var briefDocument = new DocumentData
                {
                    DocumentNumber = string.Empty,
                    DocumentTypeName = ((StoredFileType)fileInfo.StoredFileTypeId).ToString(),
                    FilePath = briefPath,
                    FileType = fileType,
                    FileSize = briefInfo.Length,
                    StoredFileType = (StoredFileType)fileInfo.StoredFileTypeId
                };

                var existingDocumentsWithSameName = documents.Count(x => x.FileName.StartsWith(briefDocument.FileName));
                if (existingDocumentsWithSameName >0 )
                {
                    briefDocument.FilePath = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(briefDocument.FileName)} ({existingDocumentsWithSameName}){Path.GetExtension(briefDocument.FileName)}");
                }
                documents.Add(briefDocument);

            }

            return documents;
        }

        private void ValidatePedingBriefs()
        {
            if (ApplicationBriefsModel == null)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.NoPendingCandidateBriefs);
            }

        }

        public void AddAttachments()
        {
            var briefDetails = ApplicationBriefsModel;

            foreach (var brief in briefDetails.Briefs)
            {
                var fileName = GetBriefDocumentName(brief);
                var existingDocumentsWithSameName = Attachments.Count(x => x.FileName.StartsWith(fileName));
                if (existingDocumentsWithSameName > 0)
                {
                    fileName =  $"{fileName} ({existingDocumentsWithSameName})";
                }

                Attachments.Add(new EmailMessageAttachmentModel() { StoredFileId = brief.StorageFileId, AttachmentType = AttachmentType.CandidateBrief, FileName = fileName });
            }
          
        }
    }
}
