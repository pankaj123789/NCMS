using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class ApplicationFormQuestionModel
    {
        public int Id { get; set; }
   
        public string Question { get; set; }
       
        public string Description { get; set; }
 
        public int DisplayOrder { get; set; }
    
        public int Type { get; set; }
       
        public string AnswerTypeName { get; set; }
    
        public int? CredentialApplicationFieldId { get; set; }
       
        public IEnumerable<ApplicationFormAnswerModel> Answers { get; set; }
  
        public IEnumerable<ApplicationFormQuestionLogicModel> Logics { get; set; }

        public object Response { get; set; }

        public List<object> Responses { get; set; }

        public bool HasTokens { get; set; }
    }
}