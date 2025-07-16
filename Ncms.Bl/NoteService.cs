using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;

namespace Ncms.Bl
{
    public class NoteService : INoteService
    {
        private readonly INoteQueryService _noteQueryService;
        private readonly IFileService _fileService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public NoteService(INoteQueryService noteQueryService, 
            IFileService fileService, 
            IFileStorageService fileStorageService, IAutoMapperHelper autoMapperHelper)
        {
            _noteQueryService = noteQueryService;
            _fileService = fileService;
            _fileStorageService = fileStorageService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IEnumerable<NoteModel> ListPanelNotes(int panelId)
        {
            return ListNotes(NoteType.Panel, panelId);
        }

        public IEnumerable<NoteModel> ListTestNotes(int testId)
        {
            return ListNotes(NoteType.Test, testId);
        }

        public IEnumerable<NoteModel> ListApplicationNotes(int applicationId)
        {
            return ListNotes(NoteType.Application, applicationId);
        }

        public IEnumerable<NoteModel> ListMaterialRequestNotes(int materialRequestId)
        {
            return ListNotes(NoteType.MaterialRequest, materialRequestId);
        }

        public IEnumerable<NoteModel> ListMaterialRequestPublicNotes(int materialRequestId)
        {
            return ListNotes(NoteType.MaterialRequestPublic, materialRequestId);
        }

        public IEnumerable<NoteModel> ListNaatiEntityNotes(int entityId)
        {
            return ListNotes(NoteType.NaatiEntity, entityId);
        }

        private  IEnumerable<NoteModel> ListNotes(NoteType noteType, int entityId)
        {
            var notesRequest = new GetNotesRequest
            {
                NoteType = noteType,
                EntityId = entityId
            };

            GetNotesResponse notesResponse = null;

            try
            {
                notesResponse = _noteQueryService.GetNotes(notesRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var noteDtoMapper = new NoteDtoMapper();
            return notesResponse?.Notes.Select(noteDtoMapper.Map).ToArray() ?? new NoteModel[0];
        }

        public int CreatePanelNote(PanelNoteModel model)
        {
            return CreateNote<PanelNoteModel, CreatePanelNoteMapper>(model);
        }

        public int CreateTestNote(TestNoteModel model)
        {
            return CreateNote<TestNoteModel, CreateTestNoteMapper>(model);
        }

        public int CreateApplicationNote(ApplicationNoteModel model)
        {
            return CreateNote<ApplicationNoteModel, CreateApplicationNoteMapper>(model);
        }

        public int CreateMaterialRequestNote(MaterialRequestNoteModel model)
        {
            return CreateNote<MaterialRequestNoteModel, MaterialRequestNoteMapper>(model);
        }

        public int CreateMaterialRequestPublicNote(MaterialRequestPublicNoteModel model)
        {
            return CreateNote<MaterialRequestPublicNoteModel, MaterialRequestPublicNoteMapper>(model);
        }

        public int CreateNaatiEntityNote(NaatiEntityNoteModel model)
        {
            return CreateNote<NaatiEntityNoteModel, CreatePersonNoteMapper>(model);
        }

        private int CreateNote<T, TU>(T model) where TU : IMapper<T, CreateNoteRequest>, new()
        {
            var createNoteMapper = new TU();
            var noteRequest = createNoteMapper.Map(model);

            CreateNoteResponse noteResponse = null;

            try
            {
               noteResponse = _noteQueryService.CreateOrUpdateNote(noteRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return noteResponse.NoteId;
        }

        public NoteModel DeletePanelNote(int panelId, int noteId)
        {
            return DeleteNote(NoteType.Panel, panelId, noteId);
        }

        public NoteModel DeleteTestNote(int testSittingId, int noteId)
        {
            return DeleteNote(NoteType.Test, testSittingId, noteId);
        }

        public NoteModel DeleteApplicationNote(int applicationId, int noteId)
        {
            return DeleteNote(NoteType.Application, applicationId, noteId);
        }

        public NoteModel DeleteMaterialRequestNote(int materialRequestId, int noteId)
        {
            return DeleteNote(NoteType.MaterialRequest, materialRequestId, noteId);
        }
        
        public NoteModel DeleteMaterialRequestPublicNote(int materialRequestId, int noteId)
        {
            return DeleteNote(NoteType.MaterialRequestPublic, materialRequestId, noteId);
        }

        public NoteModel DeleteNaatiEntityNote(int naatiNumber, int noteId)
        {
            return DeleteNote(NoteType.NaatiEntity, naatiNumber, noteId);
        }

        private NoteModel DeleteNote(NoteType noteType, int entityId, int noteId)
        {
            var noteRequest = new DeleteNoteRequest
            {
                NoteType = noteType,
                EntityId = entityId,
                NoteId = noteId
            };

            DeleteNoteResponse noteResponse = null;

            try
            {
                noteResponse = _noteQueryService.DeleteNote(noteRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var noteDtoMapper = new NoteDtoMapper();
            return noteDtoMapper.Map(noteResponse.Note);
        }

        public IEnumerable<NoteAttachmentModel> ListAttachments(int noteId)
        {
            var request = new GetAttachmentsRequest
            {
                NoteId = noteId,
            };

            GetAttachmentsResponse response = null;

            try
            {
                 response = _noteQueryService.GetAttachments(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var mapper = new NoteAttachmentDtoMapper();
            return response?.Attachments.Select(mapper.Map).ToArray() ?? new NoteAttachmentModel[0];
        }

        public int CreateOrReplaceAttachment(AttachmentModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<CreateOrReplaceNoteAttachmentRequest>(request);

            CreateOrReplaceNoteAttachmentResponse response = null;

            try
            {
                 response = _noteQueryService.CreateOrReplaceNoteAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response.StoredFileId;
        }

        public void DeleteAttachment(int storedFileId)
        {
            var serviceRequest = new DeleteNoteAttachmentRequest
            {
                StoredFileId = storedFileId
            };

            DeleteNoteAttachmentResponse response = null;

            var allowedDocumentTypes = _fileService.GetAllowedDocumentTypesToUpload();
            var documentToBeDeleted = _fileStorageService.GetStoredFileInfo(storedFileId);

            if (allowedDocumentTypes.All(x => x.Id != documentToBeDeleted.DocumentTypeId))
            {
                throw new UserFriendlySamException("You do not have permission to delete this document");
            }

            try
            {
                response = _noteQueryService.DeleteNoteAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public IEnumerable<NoteModel> GetNotesByEntityId(int entityId)
        {
            var noteDtoMapper = new NoteDtoMapper();
            return _noteQueryService.GetNotesByEntityId(entityId).Select(noteDtoMapper.Map);
        }
    }
}
