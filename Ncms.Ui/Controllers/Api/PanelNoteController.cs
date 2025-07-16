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
    public class PanelNoteController : BaseApiController
    {
        private readonly INoteService _noteService;

        public PanelNoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _noteService.ListPanelNotes(id).OrderByDescending(x => x.CreatedDate));
        }

        [HttpPost]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage Post(PanelNoteModel model)
        {
            model.UserId = CurrentUser.Id;

            return this.CreateResponse(() => _noteService.CreatePanelNote(model));
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes)]
        public HttpResponseMessage Delete(int id, [FromBody]int panelId)
        {
            return this.CreateResponse(() => _noteService.DeletePanelNote(panelId, id));
        }
    }
}
