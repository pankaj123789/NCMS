using System;
using System.IO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.Models;

namespace Ncms.Bl.Mappers
{
    public class NoteDtoMapper : BaseMapper<NoteDto, NoteModel>
    {
        public override NoteModel Map(NoteDto source)
        {
            return new NoteModel
            {
                NoteId = source.NoteId,
                UserId = source.UserId,
                User = source.User,
                Note = source.Description,
                CreatedDate = source.CreatedDate,
                ModifiedDate = source.ModifiedDate,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly,

                Reference = source.Reference,
                ReferenceType = source.ReferenceType == 0 ? (int)NoteReferenceType.None : source.ReferenceType
            };
        }

        public override NoteDto MapInverse(NoteModel source)
        {
            throw new NotImplementedException();
        }
    }

    public class CreatePanelNoteMapper : BaseMapper<PanelNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(PanelNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.Panel,
                EntityId = source.PanelId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly
            };
        }

        public override PanelNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class CreateTestNoteMapper : BaseMapper<TestNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(TestNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.Test,
                EntityId = source.TestSittingId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly
            };
        }

        public override TestNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class CreatePersonNoteMapper : BaseMapper<NaatiEntityNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(NaatiEntityNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.NaatiEntity,
                EntityId = source.NaatiEntityId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly,
            };
        }

        public override NaatiEntityNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class CreateApplicationNoteMapper : BaseMapper<ApplicationNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(ApplicationNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.Application,
                EntityId = source.ApplicationId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly
            };
        }

        public override ApplicationNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class MaterialRequestNoteMapper : BaseMapper<MaterialRequestNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(MaterialRequestNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.MaterialRequest,
                EntityId = source.MaterialRequestId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly
            };
        }

        public override MaterialRequestNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class MaterialRequestPublicNoteMapper : BaseMapper<MaterialRequestPublicNoteModel, CreateNoteRequest>
    {
        public override CreateNoteRequest Map(MaterialRequestPublicNoteModel source)
        {
            return new CreateNoteRequest
            {
                NoteType = NoteType.MaterialRequestPublic,
                EntityId = source.MaterialRequestId,
                NoteId = source.NoteId,
                UserId = source.UserId,
                Note = source.Note,
                Highlight = source.Highlight,
                ReadOnly = source.ReadOnly
            };
        }

        public override MaterialRequestPublicNoteModel MapInverse(CreateNoteRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class NoteAttachmentDtoMapper : BaseMapper<NoteAttachmentDto, NoteAttachmentModel>
    {
        public override NoteAttachmentModel Map(NoteAttachmentDto source)
        {
            return new NoteAttachmentModel
            {
                NoteId = source.NoteId,
                StoredFileId = source.StoredFileId,
                FileName = source.FileName,
                Title = source.Description,
                DocumentType = source.DocumentType,
                UploadedByName = source.UploadedByName,
                UploadedDateTime = source.UploadedDateTime,
                FileSize = source.FileSize,
                FileType = Path.GetExtension(source.FileName)?.Trim('.'),
                SoftDeleteDate = source.SoftDeleteDate
            };
        }

        public override NoteAttachmentDto MapInverse(NoteAttachmentModel source)
        {
            throw new NotImplementedException();
        }
    }
}
