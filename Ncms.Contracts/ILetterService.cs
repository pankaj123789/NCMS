using System.Collections.Generic;
using Ncms.Contracts.Models.Letter;

namespace Ncms.Contracts
{
    public interface ILetterService
    {
        int GetDefaultLetterCategory();
        IEnumerable<LetterModel> ListLetters(int id);
        IEnumerable<LetterBatchModel> ListBatches();
        LetterBatchModel AddBatch(string batchName);
        void AddToBatch(AddToBatchRequestModel request);
    }
}
