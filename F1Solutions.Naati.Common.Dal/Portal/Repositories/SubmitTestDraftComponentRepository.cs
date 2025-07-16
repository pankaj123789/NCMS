using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface ISubmitTestDraftComponentRepository : IRepository<SubmitTestDraftComponent>
    {
        List<SubmitTestDraftComponent> List(int submitTestDraftID);
        void Save(SubmitTestDraftComponent data);
        void DeleteAll(int submitTestDraftId);
    }

    public class SubmitTestDraftComponentRepository : Repository<SubmitTestDraftComponent>, ISubmitTestDraftComponentRepository
    {
        public SubmitTestDraftComponentRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public List<SubmitTestDraftComponent> List(int submitTestDraftID)
        {
            return Session.Query<SubmitTestDraftComponent>().Where(e => e.SubmitTestDraftID == submitTestDraftID).ToList();
        }

        public void Save(SubmitTestDraftComponent data)
        {
            var draft = Session.Query<SubmitTestDraftComponent>().SingleOrDefault(d => d.Id == data.SubmitTestDraftID && d.ComponentID == data.ComponentID);
            if (draft != null)
            {
                Session.Delete(draft);
            }
            Session.SaveOrUpdate(data);
        }


        public void DeleteAll(int submitTestDraftId)
        {
            Session.CreateQuery("DELETE SubmitTestDraftComponent s WHERE s.SubmitTestDraftID = :submitTestDraftId")
                .SetParameter("submitTestDraftId", submitTestDraftId)
                .ExecuteUpdate();
        }
    }
}
