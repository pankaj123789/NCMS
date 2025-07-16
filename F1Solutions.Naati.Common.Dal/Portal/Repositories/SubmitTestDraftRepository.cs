using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface ISubmitTestDraftRepository : IRepository<SubmitTestDraft>
    {
        SubmitTestDraft GetByTestAttendance(int testAttendanceId, int naatiNumber);
    }

    public class SubmitTestDraftRepository : Repository<SubmitTestDraft>, ISubmitTestDraftRepository
    {
        public SubmitTestDraftRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public SubmitTestDraft GetByTestAttendance(int testAttendanceId, int naatiNumber)
        {
            return Session.Query<SubmitTestDraft>().SingleOrDefault(e => e.TestAttendanceID == testAttendanceId && e.NAATINumber == naatiNumber);
        }
    }
}
