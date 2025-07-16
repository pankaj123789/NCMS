using System.Linq;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/naatientity")]
    public class NaatiEntityController : BaseApiController
    {
        private readonly INoteService _noteService;

        public NaatiEntityController(INoteService note)
        {
            _noteService = note;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        [Route("documents/{naatiNumber}")]
        public HttpResponseMessage DocumentsGet([FromUri]int naatiNumber)
        {
            return this.CreateResponse(() => new object[] { });
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Document)]
        [Route("documents")]
        public HttpResponseMessage DocumentsPost()
        {
            return this.CreateResponse(() => { });
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Document)]
        [Route("documents/{naatiNumber}")]
        public HttpResponseMessage DocumentsDelete([FromUri]int naatiNumber)
        {
            return this.CreateResponse(() => { });
        }

        [Route("{entityId}/notes")]
        //SecurityNounName.Person -> View Person->Notes
        [NcmsAuthorize(SecurityVerbName.Read, new SecurityNounName[] { SecurityNounName.Document, SecurityNounName.Person })]
        public HttpResponseMessage GetNotes([FromUri]int entityId)
        {
            return this.CreateResponse(() => _noteService.ListNaatiEntityNotes(entityId).OrderByDescending(x => x.CreatedDate));
        }

        [Route("{entityId}/notes/{showAll}")]
        //SecurityNounName.Person -> View Person->Notes
        [NcmsAuthorize(SecurityVerbName.Read,new SecurityNounName[] {SecurityNounName.Document,SecurityNounName.Person })]
        public HttpResponseMessage GetNotes([FromUri]int entityId, [FromUri]bool showAll)
        {
            if (showAll)
            {
                return this.CreateResponse(
                    () => _noteService.GetNotesByEntityId(entityId).OrderByDescending(x => x.CreatedDate));
            }
            return
                this.CreateResponse(
                    () => _noteService.ListNaatiEntityNotes(entityId).OrderByDescending(x => x.CreatedDate));
        }

        [HttpPost]
        //SecurityNounName.Person -> View Person->Notes->AddNote
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        [Route("notes")]
        public HttpResponseMessage PostNotes(NaatiEntityNoteModel model)
        {
            model.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _noteService.CreateNaatiEntityNote(model));
        }

        [HttpDelete]
        //SecurityNounName.Person -> View Person->Notes->DeleteNote
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes )]
        [Route("notes/{entityId}/{id}")]
        public HttpResponseMessage DeleteNotes([FromUri]int entityId, [FromUri]int id)
        {
            return this.CreateResponse(() => _noteService.DeleteNaatiEntityNote(entityId, id));
        }
    }
}
