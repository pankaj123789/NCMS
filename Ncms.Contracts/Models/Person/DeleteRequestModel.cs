using System.Collections.Generic;

namespace Ncms.Contracts.Models.Person
{
    public class DeleteRequestModel
    {
        public int ObjectId { get; set; }
        public List<KeyValuePair<string, bool>> FlowAnswers { get; set; }
    }
}
