using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class CclUnavailableApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public CclUnavailableApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "NonPractitionerUser");
            var application = userType.AddForm(7, "Credentialed Community Language (CCL) Testing", "Credentialed Community Language (CCL) Testing", true, "ccl-unavailable");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var applicationSectionSection = ApplicationSection(application);
            var question1 = Question1(applicationSectionSection);

        }

        private IFormSectionBuilder ApplicationSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Application", string.Empty);
            return section;
        }

        private IFormQuestionBuilder Question1(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Due to high volumes, new Credentialed Community Language (CCL) applications are temporarily unavailable. We apologize for the inconvenience and encourage you to check back in 24 hours. Please note that calling the NAATI office will not help to expedite your application as this is a system related delay.", @"");
            var option1B = Option1B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Redirect"), @"[[ExternalUrl_NAATI_URL]]/other-information/ccl-testing/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option1B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option1B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Ok", @"");
        }


    }
}
