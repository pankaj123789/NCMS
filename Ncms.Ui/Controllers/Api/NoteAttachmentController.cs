using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/noteattachment")]
    public class NoteAttachmentController : BaseApiController
    {
        private readonly INoteService _note;
        private readonly IUserService _user;

        public NoteAttachmentController(INoteService note, IUserService user)
        {
            _note = note;
            _user = user;
        }

        public HttpResponseMessage Get(int id)
        {
            return this.CreateSearchResponse(() =>
            {
                return _note.ListAttachments(id);
            });
        }

        [Route("upload")]
        //SecurityNounName.Person Person->Note->Attachment
        [NcmsAuthorize(SecurityVerbName.Update, new SecurityNounName[] { SecurityNounName.Document, SecurityNounName.Person })]
        [HttpPost]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var noteId = Convert.ToInt32(provider.FormData["noteId"]);
                var id = Convert.ToInt32(provider.FormData["id"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);

                var user = _user.Get() ?? new UserModel();
                var request = new AttachmentModel
                {
                    FileName = fileName,
                    Title = title,
                    Type = type,
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{noteId}\{fileName}",
                    StoredFileId = storedFileId,
                    NoteId = noteId
                };

                return _note.CreateOrReplaceAttachment(request);
            }));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Document)]
        public HttpResponseMessage Post(AttachmentModel request)
        {
            var user = _user.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            return this.CreateResponse(() => _note.CreateOrReplaceAttachment(request));
        }

        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Document)]
        public HttpResponseMessage Delete(int id)
        {
            return this.CreateResponse(() => _note.DeleteAttachment(id));
        }
    }
}
