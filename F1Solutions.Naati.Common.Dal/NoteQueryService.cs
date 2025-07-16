using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal
{
    public class NoteQueryService : INoteQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public NoteQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }
        private abstract class NoteTransform
        {
            public abstract IEnumerable<Note> GetNotes(GetNotesRequest request);
            public abstract int CreateOrUpdateNote(CreateNoteRequest request);
            public abstract Note DeleteNote(DeleteNoteRequest request);
        }

        private class NoteTransform<T, TU> : NoteTransform where T : EntityBase
        {
            public Func<T, IEnumerable<Note>> SelectNotes;
            public Func<T, Note, TU> NewOrExistNote;
            public Func<T, IEnumerable<TU>> TransformNotes;
            public Func<TU, Note> TransformNote;
            public Action<T, TU> RemoveNote;

            public override IEnumerable<Note> GetNotes(GetNotesRequest request)
            {
                return NoteQueryService.GetNotes(request.EntityId, SelectNotes);
            }

            public override int CreateOrUpdateNote(CreateNoteRequest request)
            {
                return NoteQueryService.CreateOrUpdateNote(request.NoteId, request.EntityId, request.UserId, request.Note, request.Highlight, request.ReadOnly, TransformNotes, TransformNote, NewOrExistNote);
            }

            public override Note DeleteNote(DeleteNoteRequest request)
            {
                return NoteQueryService.DeleteNote(request.EntityId, request.NoteId, TransformNotes, TransformNote, RemoveNote);
            }
        }

        private readonly Dictionary<NoteType, NoteTransform> _noteTransforms = new Dictionary<NoteType, NoteTransform>
        {
            {
                NoteType.Panel,
                new NoteTransform<Panel, PanelNote>
                {
                    SelectNotes = panel => panel.PanelNotes.Select(panelNote => panelNote.Note),
                    NewOrExistNote = (panel, note) => new PanelNote { Panel = panel, Note = note },
                    TransformNotes = panel => panel.PanelNotes,
                    TransformNote = panelNote => panelNote.Note,
                    RemoveNote = (panel, panelNote) => panel.RemovePanelNote(panelNote)
                }
            },
            {
                NoteType.Test,
                new NoteTransform<TestSitting, TestSittingNote>
                {
                    SelectNotes = test => test.TestSittingNotes.Select(testNote => testNote.Note),
                    NewOrExistNote = (test, note) => new TestSittingNote { TestSitting = test, Note = note },
                    TransformNotes = test => test.TestSittingNotes,
                    TransformNote = testNote => testNote.Note,
                    RemoveNote = (test, testNote) => test.RemoveTestNote(testNote)
                }
            },
            {
                NoteType.NaatiEntity,
                new NoteTransform<NaatiEntity, NaatiEntityNote>
                {
                    SelectNotes = entity => entity.NaatiEntityNotes.Select(note => note.Note),
                    NewOrExistNote = (entity, note) => new NaatiEntityNote { Entity = entity, Note = note },
                    TransformNotes = entity => entity.NaatiEntityNotes,
                    TransformNote = note => note.Note,
                    RemoveNote = (entity, note) => entity.RemoveNaatiEntityNote(note)
                }
            },
            {
                NoteType.Application,
                new NoteTransform<CredentialApplication, CredentialApplicationNote>
                {
                    SelectNotes = application => application.CredentialApplicationNotes.Select(applicationNote => applicationNote.Note),
                    NewOrExistNote = (application, note) => new CredentialApplicationNote { CredentialApplication = application, Note = note },
                    TransformNotes = application => application.CredentialApplicationNotes,
                    TransformNote = applicationNote => applicationNote.Note,
                    RemoveNote = (application, applicationNote) => application.RemoveApplicationNote(applicationNote)
                }
            },
            {
                NoteType.MaterialRequest,
                new NoteTransform<Domain.MaterialRequest, MaterialRequestNote>
                {
                    SelectNotes = materialRequest => materialRequest.MaterialRequestNotes.Select(materialRequestNote => materialRequestNote.Note),
                    NewOrExistNote = (materialRequest, note) => new MaterialRequestNote() { MaterialRequest = materialRequest, Note = note },
                    TransformNotes = materialRequest => materialRequest.MaterialRequestNotes,
                    TransformNote = materialRequest => materialRequest.Note,
                    RemoveNote = (materialRequest, materialRequestNote) => materialRequest.RemoveMaterialRequestNote(materialRequestNote)
                }
            },
            {
                NoteType.MaterialRequestPublic,
                new NoteTransform<Domain.MaterialRequest, MaterialRequestPublicNote>
                {
                    SelectNotes = materialRequest => materialRequest.MaterialRequestPublicNotes.Select(materialRequestPublicNote => materialRequestPublicNote.Note),
                    NewOrExistNote = (materialRequest, note) => new MaterialRequestPublicNote() { MaterialRequest = materialRequest, Note = note },
                    TransformNotes = materialRequest => materialRequest.MaterialRequestPublicNotes,
                    TransformNote = materialRequest => materialRequest.Note,
                    RemoveNote = (materialRequest, materialRequestPublicNote) => materialRequest.RemoveMaterialRequestPublicNote(materialRequestPublicNote)
                }
            }
        };

        public GetNotesResponse GetNotes(GetNotesRequest request)
        {
            return new GetNotesResponse
            {
                Notes = _noteTransforms[request.NoteType].GetNotes(request).Select(MapNote).ToArray()
            };
        }

        private static IEnumerable<Note> GetNotes<T>(int entityId, Func<T, IEnumerable<Note>> transform) where T : EntityBase
        {
            var query = NHibernateSession.Current.Query<T>();
            var entity = query.SingleOrDefault(x => x.Id == entityId);

            if (entity == null)
            {
                throw new WebServiceException($"Referenced {typeof(T).Name.ToLower()} does not exist");
            }

            return transform(entity);
        }

        public CreateNoteResponse CreateOrUpdateNote(CreateNoteRequest request)
        {
            return new CreateNoteResponse
            {
                NoteId = _noteTransforms[request.NoteType].CreateOrUpdateNote(request)
            };
        }

        private static int CreateOrUpdateNote<T, TU>(int? noteId, int entityId, int userId, string noteDescription, bool highlight, bool readOnly, Func<T, IEnumerable<TU>> entityTransform, Func<TU, Note> noteTransform, Func<T, Note, TU> transform) where T : EntityBase
        {
            var query = NHibernateSession.Current.Query<T>();
            var entity = query.SingleOrDefault(x => x.Id == entityId);

            if (entity == null)
            {
                throw new WebServiceException(string.Format("Referenced {0} does not exist", typeof(T).Name.ToLower()));
            }

            var userQuery = NHibernateSession.Current.Query<User>();
            var user = userQuery.SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new WebServiceException("Referenced user does not exist");
            }

            Note note;
            TU entityNote;

            if (noteId.GetValueOrDefault() != default(int))
            {
                entityNote = entityTransform(entity).Single(x => noteTransform(x).Id == noteId);

                if (entityNote == null)
                {
                    throw new WebServiceException("Referenced note does not exist");
                }

                note = noteTransform(entityNote);
                note.ModifiedDate = DateTime.Now;
            }
            else
            {
                note = new Note
                {
                    CreatedDate = DateTime.Now
                };

                entityNote = transform(entity, note);
            }

            note.User = user;
            note.Description = noteDescription;
            note.Highlight = highlight;
            note.ReadOnly = readOnly;

            NHibernateSession.Current.Save(note);
            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();

            return note.Id;
        }

        public DeleteNoteResponse DeleteNote(DeleteNoteRequest request)
        {
            return new DeleteNoteResponse
            {
                Note = MapNote(_noteTransforms[request.NoteType].DeleteNote(request))
            };
        }

        private static Note DeleteNote<T, TU>(int entityId, int noteId, Func<T, IEnumerable<TU>> entityTransform, Func<TU, Note> noteTransform, Action<T, TU> removeAction) where T : EntityBase
        {
            var query = NHibernateSession.Current.Query<T>();
            var entity = query.SingleOrDefault(x => x.Id == entityId);

            if (entity == null)
            {
                throw new WebServiceException(string.Format("Referenced {0} does not exist", typeof(T).Name.ToLower()));
            }

            var entityNote = entityTransform(entity).Single(x => noteTransform(x).Id == noteId);

            if (entityNote == null)
            {
                throw new WebServiceException("Referenced note does not exist");
            }

            removeAction(entity, entityNote);

            var note = noteTransform(entityNote);

            NHibernateSession.Current.Delete(entityNote);
            NHibernateSession.Current.Delete(note);
            NHibernateSession.Current.Flush();

            return note;
        }
        public IEnumerable<NoteDto> GetNotesByEntityId(int entityId)
        {
            var applications =
                NHibernateSession.Current.Get<NaatiEntity>(entityId)
                    .People.FirstOrDefault()?.CredentialApplications.ToList()
                    .Select(x => x.Id)
                    .ToArray() ?? new[] {0};

            var list = new List<NoteDto>();

            if (applications.Length > 0)
            {
                list = NHibernateSession.Current.Query<CredentialApplicationNote>()
                    .Where(x => applications.Contains(x.CredentialApplication.Id))
                    .Select(x => MapNoteIncludeReference(x.Note, NoteReferenceType.Application, x.CredentialApplication.Id.ToString())).ToList();

                var query = from testSittingNote in NHibernateSession.Current.Query<TestSittingNote>()
                                join testSitting in NHibernateSession.Current.Query<TestSitting>() on testSittingNote.TestSitting.Id equals testSitting.Id
                                join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on testSitting.CredentialRequest.Id equals credentialRequest.Id
                                join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id

                                where applications.Contains(credentialApplication.Id)
                                select MapNoteIncludeReference(testSittingNote.Note, NoteReferenceType.Test, testSitting.Id.ToString());

                var testNotes = query.ToList();
                list.AddRange(testNotes);
            }

            var naatiEntityNotes = NHibernateSession.Current.Get<NaatiEntity>(entityId).NaatiEntityNotes.Select(x => MapNoteIncludeReference(x.Note));
            list.AddRange(naatiEntityNotes);

            return list;
        }

        private static NoteDto MapNoteIncludeReference(Note note, NoteReferenceType referenceType = NoteReferenceType.None, string reference = null)
        {
            return new NoteDto
            {
                NoteId = note.Id,
                UserId = note.User.Id,
                User = note.User.FullName,
                Description = note.Description,
                ModifiedDate = note.ModifiedDate,
                CreatedDate = note.CreatedDate,
                Highlight = note.Highlight,
                ReadOnly = referenceType != NoteReferenceType.None || note.ReadOnly,
                Reference = reference ?? string.Empty,
                ReferenceType = (int)referenceType
            };
        }

        private static NoteDto MapNote(Note note)
        {
            return new NoteDto
            {
                NoteId = note.Id,
                UserId = note.User.Id,
                User = note.User.FullName,
                Description = note.Description,
                ModifiedDate = note.ModifiedDate,
                CreatedDate = note.CreatedDate,
                Highlight = note.Highlight,
                ReadOnly = note.ReadOnly
            };
        }

        public GetAttachmentsResponse GetAttachments(GetAttachmentsRequest request)
        {
            var query = NHibernateSession.Current.Query<NoteAttachment>();
            var attachments = query.Where(n => n.Note.Id == request.NoteId).ToList().Select(n => new NoteAttachmentDto
            {
                NoteId = n.Note.Id,
                StoredFileId = n.StoredFile.Id,
                FileName = n.StoredFile.FileName,
                Description = n.Description,
                DocumentType = n.StoredFile.DocumentType.DisplayName,
                UploadedByName = n.StoredFile.UploadedByUser.FullName,
                UploadedDateTime = n.StoredFile.UploadedDateTime,
                FileSize = n.StoredFile.FileSize,
                SoftDeleteDate = n.StoredFile.StoredFileStatusType.Id != 1 ? n.StoredFile.StoredFileStatusChangeDate : null
            }) ;

            return new GetAttachmentsResponse
            {
                Attachments = attachments.ToArray()
            };
        }

        public CreateOrReplaceNoteAttachmentResponse CreateOrReplaceNoteAttachment(CreateOrReplaceNoteAttachmentRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = request.StoredFileId != 0 ? (int?)request.StoredFileId : null,
                UpdateFileName = request.StoredFileId != 0 ? request.FileName : null,
                Type = request.Type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
            });

            var storeFile = NHibernateSession.Current.Load<StoredFile>(response.StoredFileId);
            var note = NHibernateSession.Current.Load<Note>(request.NoteId);
            var noteAttachment = NHibernateSession.Current.Query<NoteAttachment>().SingleOrDefault(n => n.StoredFile.Id == response.StoredFileId);
            if (noteAttachment == null)
            {
                noteAttachment = new NoteAttachment
                {
                    StoredFile = storeFile,
                    Note = note
                };
            }
            noteAttachment.Description = request.Title;

            NHibernateSession.Current.Save(noteAttachment);
            NHibernateSession.Current.Flush();

            return new CreateOrReplaceNoteAttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public DeleteNoteAttachmentResponse DeleteNoteAttachment(DeleteNoteAttachmentRequest request)
        {
            var noteAttachment = NHibernateSession.Current.Query<NoteAttachment>().SingleOrDefault(n => n.StoredFile.Id == request.StoredFileId);
            NHibernateSession.Current.Delete(noteAttachment);
            NHibernateSession.Current.Flush();

            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            fileService.DeleteFile(new DeleteFileRequest
            {
                StoredFileId = request.StoredFileId
            });

            return new DeleteNoteAttachmentResponse();
        }
    }
}
