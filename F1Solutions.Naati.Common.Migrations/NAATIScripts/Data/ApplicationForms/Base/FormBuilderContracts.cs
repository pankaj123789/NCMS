using System.Collections.Generic;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.Builder;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base
{

    public interface IFormBuilderHelper
    {
        void CreateForm();
    }

    public interface IFormUserTypeBuilder: IFormBuilder
    {
        string Name { get; }
        string DisplayName { get; }
        IApplicationFormBuilder AddForm(int applicationType, string name, string description, bool inactive, string url);
    }

    public interface IApplicationFormBuilder: IFormBuilder
    {
        int ApplicationTypeId { get; }
        string Name { get; }
        string Description { get; }
        bool Inactive { get; }
        IFormUserTypeBuilder UserType { get; }
        string Url { get; }
        IFormSectionBuilder AddSection(string name, string description);
        IApplicationFormBuilder DisplayOnlyCredentialTypes(params int[] credentialTypeIds);
    }

    public interface IFormSectionBuilder: IFormBuilder
    {
        string Name { get; }
        string Description { get; }
        int DisplayOrder { get; }
        IFormQuestionBuilder AddQuestion(IFormQuestionTypeBuilder questionTypeBuilder);
    }

    public interface IFormQuestionBuilder: IFormBuilder
    {
        IFormSectionBuilder SectionBuilder { get; }
        IFormQuestionTypeBuilder QuestionTypeBuilder { get; }
        int? Field { get; }
        int DisplayOrder { get; }
        IEnumerable<IFormQuestionAnswerOptionBuilder> Options { get; }
        IEnumerable<IFormQuestionLogicBuilder> Logics { get; }
        IFormQuestionBuilder StoreReponseOnField(int field);
        IFormQuestionAnswerOptionBuilder WhenSelectedOption(IFormAnswerOptionBuilder optionBuilder);
        ILogicSelector ShowOnlyIf();
    }

    public interface IFormAnswerOptionBuilder: IFormBuilder
    {
        IFormQuestionTypeBuilder QuestionTypeBuilder { get; }
        string Option { get; }
        string Description { get; }
        IFormAnswerOptionBuilder ExecuteActionWhenSelected(IFormActionTypeBuilder action, string parameter);
        IFormAnswerOptionBuilder RequestDocumentWhenSelected(int documentTypeId);
    }

    public interface IFormQuestionLogicBuilder : IFormBuilder
    {
        IFormQuestionBuilder Question { get; }
        bool NotLogic { get; }
        bool AndLogic { get; }
        int Group { get; }
        int Order { get; }
        IFormQuestionAnswerOptionBuilder OptionLogic { get; }
        int? CredentialTypeId { get; }
        int? CredentialRequestPathTypeId { get; }
        bool? PdPointsMetLogic { get; }
        bool? WorkPracticeMetLogic { get; }
    }


    public interface IFormQuestionTypeBuilder: IFormBuilder
    {
        string Text { get; }
        string Description { get; }
        IFormAnswerTypeBuilder Type { get; }
        IEnumerable<IFormAnswerOptionBuilder> Options { get; }
        IFormAnswerOptionBuilder AddOption(string option, string description);
    }

    public interface IFormQuestionAnswerOptionBuilder: IFormBuilder
    {
        string FieldData { get; }
        int? FieldOptionId { get; }
        int? Field { get; }
        IFormQuestionBuilder Question { get; }
        IFormAnswerOptionBuilder OptionBuilder { get; }
        int DisplayOrder { get; }
        IFormQuestionAnswerOptionBuilder Store(string fieldData);
        IFormQuestionAnswerOptionBuilder StoreOption(int fieldOptionId);
        IFormAnswerOptionBuilder DoNotStoreAnswer();
        IFormAnswerOptionBuilder OnField(int fieldId);
    }

    public interface IFormActionTypeBuilder: IFormBuilder
    {
        string Name { get; }
        string DisplayName { get; }
    }

    public interface IApplicationCredentialTypeBuilder : IFormBuilder
    {
        IApplicationFormBuilder ApplicationFormBuilder { get; }
        int CredentialTypeId { get; }
    }

    public interface IFormBuilder : IScriptBuilder
    {       
    }

    public interface ILogicSelector
    {
        ILogic CredentialType(int credentialTypeId);
        ILogic Skill(int skillId);
        ILogic CredentialRequestPathType(int credentialRequestPathTypeId);
        ILogic PdPointsMet();
        ILogic WorkPracticeMet();
        ILogic Option(IFormQuestionAnswerOptionBuilder option);
    }

    public interface ILogicSelectorConcatenator
    {
        ILogicSelector And();
        ILogicSelector Or();
        ILogicSelector AndGroupFormedBy();
        ILogicSelector OrGroupFormedBy();
    }

    public interface ILogic
    {
        ILogicSelectorConcatenator IsSelected();
        ILogicSelectorConcatenator IsNotSelected();
    }

    public interface IFormAnswerTypeBuilder : IFormBuilder
    {
        string Name { get; }
        string DisplayName { get; }
        bool AllowOptions { get; }
        bool AllowText { get; }
        IFormQuestionTypeBuilder CreateQuestionType(string text, string description);
    }
}
