namespace Ncms.Bl
{
    //public class LetterService : ILetterService
    //{
    //    private readonly ITestQueryService _testQueryService;

    //    public LetterService(ITestQueryService testQueryService)
    //    {
    //        _testQueryService = testQueryService;
    //    }
    //    public IEnumerable<LetterModel> ListLetters(int id)
    //    {
    //        var letters = ClientGlobal.ClientCache["StandardLetter"];

    //        var mapper = new DataRowToEntity<LetterModel>();
    //        return letters.AsEnumerable()
    //            .Select(mapper.Map)
    //            .Where(r => r.LetterVisible.GetValueOrDefault() && r.StandardLetterCategoryId == id);
    //    }

    //    public IEnumerable<LetterBatchModel> ListBatches()
    //    {
    //        var correspondence = new Correspondence(null);
    //        correspondence.SyncLoad();

    //        var mapper = new DataRowToEntity<LetterBatchModel>();
    //        return correspondence
    //            .LetterBatch
    //            .AsEnumerable()
    //            .Select(mapper.Map);
    //    }

    //    public LetterBatchModel AddBatch(string batchName)
    //    {
    //        var correspondence = new Correspondence(null);
    //        correspondence.SyncLoad();
    //        var row = correspondence.AddBatch(batchName);
    //        correspondence.SyncUpdate();

    //        var mapper = new DataRowToEntity<LetterBatchModel>();
    //        return mapper.Map(row);
    //    }

    //    public void AddToBatch(AddToBatchRequestModel request)
    //    {
    //        var correspondence = new Correspondence(null);
    //        correspondence.SyncLoad();

    //        var newStandardLetter = correspondence.AddStandardLetter();

    //        newStandardLetter.LetterBatchId = request.LetterBatchId;
    //        newStandardLetter.EntityId = request.EntityId;

    //        var standardLetter = ClientGlobal.ClientCache["StandardLetter"].Select("StandardLetterID = " + request.StandardLetterId, "", DataViewRowState.CurrentRows);

    //        var mailCategoryId = 0;
    //        if (standardLetter.Length > 0)
    //        {
    //            mailCategoryId = Convert.ToInt32(standardLetter[0]["MailCategoryID"]);
    //            newStandardLetter.StandardLetterId = request.StandardLetterId;
    //        }

    //        correspondence.LoadEntity(new ArrayList { request.EntityId }, mailCategoryId);

    //        var correspondenceEntity = correspondence.CorrespondenceEntity.Select("", "", DataViewRowState.CurrentRows)[0];
    //        if (!correspondenceEntity.IsNull("AddressId"))
    //        {
    //            newStandardLetter.AddressId = Convert.ToInt32(correspondenceEntity["AddressId"]);
    //        }
    //        else
    //        {
    //            throw new UserFriendlySamException(Test.PrimaryAddressIsInvalid);
    //        }

    //        if (request.TestAttendanceId.HasValue)
    //        {
    //            GetApplicationIdResponse response = null;
    //            response = _testQueryService.GetApplicationId(new GetApplicationIdRequest
    //            {
    //                TestAttendanceId = request.TestAttendanceId.Value
    //            });

    //            correspondence.AddTestAttendance(newStandardLetter, request.TestAttendanceId.Value);
    //        }

    //        correspondence.SyncUpdate();
    //    }

    //    public int GetDefaultLetterCategory()
    //    {
    //        return ClientGlobal.ClientCache.SystemParameter.ResultStandardLetterCategoryId;
    //    }
    //}
}
