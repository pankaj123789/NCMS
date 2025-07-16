using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface INoteQueryService : IQueryService
    {
        
        GetNotesResponse GetNotes(GetNotesRequest request);

        
        CreateNoteResponse CreateOrUpdateNote(CreateNoteRequest request);

        
        DeleteNoteResponse DeleteNote(DeleteNoteRequest request);

        
        GetAttachmentsResponse GetAttachments(GetAttachmentsRequest request);

        
        CreateOrReplaceNoteAttachmentResponse CreateOrReplaceNoteAttachment(CreateOrReplaceNoteAttachmentRequest request);

        
        DeleteNoteAttachmentResponse DeleteNoteAttachment(DeleteNoteAttachmentRequest request);

        
        IEnumerable<NoteDto> GetNotesByEntityId(int entityId);
    }
}
