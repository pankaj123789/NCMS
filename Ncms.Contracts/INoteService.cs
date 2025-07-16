using System.Collections.Generic;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;

namespace Ncms.Contracts
{
    public interface INoteService
    {
        IEnumerable<NoteModel> ListPanelNotes(int panelId);
        IEnumerable<NoteModel> ListTestNotes(int testId);
        IEnumerable<NoteModel> ListApplicationNotes(int applicationId);
        IEnumerable<NoteModel> ListMaterialRequestNotes(int materialRequestId);
        IEnumerable<NoteModel> ListMaterialRequestPublicNotes(int materialRequestId);
        IEnumerable<NoteModel> ListNaatiEntityNotes(int naatiEntity);
        int CreatePanelNote(PanelNoteModel model);
        int CreateTestNote(TestNoteModel model);
        int CreateApplicationNote(ApplicationNoteModel model);
        int CreateMaterialRequestNote(MaterialRequestNoteModel model);
        int CreateMaterialRequestPublicNote(MaterialRequestPublicNoteModel model);
        int CreateNaatiEntityNote(NaatiEntityNoteModel model);
        NoteModel DeletePanelNote(int panelId, int noteId);
        NoteModel DeleteTestNote(int testSittingId, int noteId);
        IEnumerable<NoteAttachmentModel> ListAttachments(int noteId);
        NoteModel DeleteApplicationNote(int applicationId, int noteId);
        NoteModel DeleteMaterialRequestNote(int materialRequestId, int noteId);
        NoteModel DeleteMaterialRequestPublicNote(int materialRequestId, int noteId);
        NoteModel DeleteNaatiEntityNote(int naatiNumber, int noteId);
        int CreateOrReplaceAttachment(AttachmentModel request);
        void DeleteAttachment(int storedFileId);
        IEnumerable<NoteModel> GetNotesByEntityId(int entityId);
    }
}
