using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    public class ExaminerToolsInternalService : IExaminerToolsInternalService
    {
        private readonly ISubmitTestDraftRepository mDraftRepository;
        private readonly ISubmitTestDraftComponentRepository mComponentRepository;

        public ExaminerToolsInternalService(
            ISubmitTestDraftRepository draftRepository,
            ISubmitTestDraftComponentRepository componentRepository)
        {
            mDraftRepository = draftRepository;
            mComponentRepository = componentRepository;
        }

        public SaveTestResponse SaveTest(SaveTestRequest request)
        {
            using (var transaction = mDraftRepository.CreateSyncTransaction())
            {
                var draft = SaveDraft(request);
                SaveComponents(request, draft);
                transaction.Commit();
            }

            return new SaveTestResponse();
        }

        private void SaveComponents(SaveTestRequest request, SubmitTestDraft draft)
        {
            mComponentRepository.DeleteAll(draft.Id);

            foreach (var c in request.Components ?? new List<TestComponentContract>())
            {
                if (!c.Mark.HasValue) continue;

                var component = new SubmitTestDraftComponent();

                component.SubmitTestDraftID = draft.Id;
                component.ComponentID = c.Id;
                component.Mark = c.Mark;

                mComponentRepository.Save(component);
            }
        }

        private SubmitTestDraft SaveDraft(SaveTestRequest request)
        {
            var draft = mDraftRepository.GetByTestAttendance(request.TestID, request.UserId);

            if (draft == null)
            {
                draft = new SubmitTestDraft
                {
                    TestAttendanceID = request.TestID,
                    NAATINumber = request.UserId,
                };
            }

            draft.PrimaryReasonForFailure = request.PrimaryReasonForFailure;
            draft.Comments = request.Comments;
            draft.Feedback = request.Feedback;
            draft.Updated = DateTime.Now;
            draft.Letters = String.Join(",", (request.ReasonsForPoorPerformance ?? new List<string>()).ToArray());

            mDraftRepository.SaveOrUpdate(draft);
            return draft;
        }

        public GetTestResponse GetTest(GetTestRequest request)
        {
            var draft = mDraftRepository.GetByTestAttendance(request.TestID, request.UserID);
            if (draft == null)
            {
                return null;
            }

            var response = new GetTestResponse();

            response.TestID = draft.TestAttendanceID;
            response.Comments = draft.Comments;
            response.Feedback = draft.Feedback;
            response.ReasonsForPoorPerformance = draft.Letters.Split(',').ToList();
            response.PrimaryReasonForFailure = draft.PrimaryReasonForFailure;
            response.Components = new List<TestComponentContract>();

            var components = mComponentRepository.List(draft.Id);
            foreach (var c in components)
            {
                var component = new TestComponentContract();

                component.Id = c.ComponentID;
                component.Mark = c.Mark;

                response.Components.Add(component);
            }

            return response;
        }
    }
}
