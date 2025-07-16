using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.Builder;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base
{
    public abstract class BaseFormBuilder : BaseScriptBuilder<IFormBuilder>, IFormBuilder
    {
        protected BaseFormBuilder(int entityId) : base(entityId)
        {
        }
    }
    public class FormUserTypeBuilder : BaseFormBuilder, IFormUserTypeBuilder
    {
        private readonly IList<ApplicationFormBuilder> _applicationFormBuilders;
        public string Name { get; }
        public string DisplayName { get; }
        private static int _nextFormTypeId = 1;
        internal FormUserTypeBuilder(string name, string displayName) : base(_nextFormTypeId++)
        {
            Name = name;
            DisplayName = displayName;
            _applicationFormBuilders = new List<ApplicationFormBuilder>();
        }

        public static IFormUserTypeBuilder Create(string name, string displayName)
        {
            return new FormUserTypeBuilder(name, displayName);
        }

        public IApplicationFormBuilder AddForm(int applicationType, string name, string description, bool inactive, string url)
        {
            var applicationFormBuilder = new ApplicationFormBuilder(applicationType, name, description, inactive, this, url);
            _applicationFormBuilders.Add(applicationFormBuilder);
            return applicationFormBuilder;
        }

        protected override string TableName => "tblCredentialApplicationFormUserType";
        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormUserTypeId", "Name", "DisplayName" };
        protected override IEnumerable<string> Values => new[] { this.Name, this.DisplayName };


        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return _applicationFormBuilders;
        }
    }

    public class FormActionTypeBuilder : BaseFormBuilder, IFormActionTypeBuilder
    {
        public string Name { get; }
        public string DisplayName { get; }
        private static int _nextFormActionTypeId = 1;
        internal FormActionTypeBuilder(string name, string displayName) : base(_nextFormActionTypeId++)
        {
            Name = name;
            DisplayName = displayName;
        }

        public static IFormActionTypeBuilder Create(string name, string displayName)
        {
            return new FormActionTypeBuilder(name, displayName);
        }

        protected override string TableName => "tblCredentialApplicationFormActionType";
        protected override IEnumerable<string> Values => new[] { this.Name, this.DisplayName };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormActionTypeId", "Name", "DisplayName" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }
    
    public class FormAnswerTypeBuilder : BaseFormBuilder, IFormAnswerTypeBuilder
    {
        public string Name { get; }
        public string DisplayName { get; }
        public bool AllowOptions { get; }
        public bool AllowText { get; }
        private static int _nextFormAnswerTypeId = 1;

        private IList<IFormQuestionTypeBuilder> _questionTypes;
        internal FormAnswerTypeBuilder(string name, string displayName, bool allowOptions = false, bool allowText = false) : base(_nextFormAnswerTypeId++)
        {
            Name = name;
            DisplayName = displayName;
            AllowOptions = allowOptions;
            AllowText = allowText;
            _questionTypes = new List<IFormQuestionTypeBuilder>();
        }

        public static IFormAnswerTypeBuilder Create(string name, string displayName, bool allowOptions = false, bool allowText = false)
        {
            return new FormAnswerTypeBuilder(name, displayName, allowOptions, allowText);
        }

        public IFormQuestionTypeBuilder CreateQuestionType(string text, string description)
        {
            var questionType = new FormQuestionTypeBuilder(text, description, this);
            
            _questionTypes.Add(questionType);
            return questionType;
        }

        protected override string TableName => "tblCredentialApplicationFormAnswerType";

        protected override IEnumerable<string> Values => new[] { this.Name, this.DisplayName };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormAnswerTypeId", "Name", "DisplayName" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return _questionTypes;
        }
    }

    public class ApplicationCredentialTypeBuilder : BaseFormBuilder, IApplicationCredentialTypeBuilder
    {
        private static int _nextApplicationCredentialTypeId = 1;
        public IApplicationFormBuilder ApplicationFormBuilder { get; }
        public int CredentialTypeId { get; }
        public ApplicationCredentialTypeBuilder(IApplicationFormBuilder applicationFormBuilder, int credentialTypeId) : base(_nextApplicationCredentialTypeId++)
        {
            ApplicationFormBuilder = applicationFormBuilder;
            CredentialTypeId = credentialTypeId;
        }

        protected override string TableName => "tblCredentialApplicationFormCredentialType";

        protected override IEnumerable<string> Values => new[] { ApplicationFormBuilder.Id.ToString(), CredentialTypeId.ToString() };
        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormCredentialTypeId", "CredentialApplicationFormId", "CredentialTypeId" };
        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { ApplicationFormBuilder };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }
    
    public class ApplicationFormBuilder : BaseFormBuilder, IApplicationFormBuilder
    {
        private static int _nextFormId = 1;

        private int _nextSectionDisplayOrder;

        private int FormId { get; }
        public int ApplicationTypeId { get; }
        public string Name { get; }
        public string Description { get; }
        public bool Inactive { get; }
        public IFormUserTypeBuilder UserType { get; }
        public string Url { get; }

        private readonly IList<IFormSectionBuilder> _sectionBuilders;
        private readonly IList<IApplicationCredentialTypeBuilder> _applicationCredentialTypeBuilders;
        
        internal ApplicationFormBuilder(int applicationTypeId, string name, string description,
            bool inactive,
            FormUserTypeBuilder userType, string url) : base(_nextFormId++)
        {
            FormId = _nextFormId;
            ApplicationTypeId = applicationTypeId;
            Name = name;
            Description = description;
            Inactive = inactive;
            UserType = userType;
            Url = url;
            _sectionBuilders = new List<IFormSectionBuilder>();
            _applicationCredentialTypeBuilders = new List<IApplicationCredentialTypeBuilder>();
        }

        public IFormSectionBuilder AddSection(string name, string description)
        {
            var section = new FormSectionBuilder(this, name, description, _nextSectionDisplayOrder++);
            _sectionBuilders.Add(section);
            return section;
        }

        public IApplicationFormBuilder DisplayOnlyCredentialTypes(params int[] credentialTypeIds)
        {
            foreach (var credentialTypeId in credentialTypeIds)
            {
                var applicaitonCredentilType = new ApplicationCredentialTypeBuilder(this, credentialTypeId);
                _applicationCredentialTypeBuilders.Add(applicaitonCredentilType);
            }

            return this;
        }

        protected override string TableName => "tblCredentialApplicationForm";

        protected override IEnumerable<string> Values => new[] { this.ApplicationTypeId.ToString(), this.Name, this.Description, this.Inactive ? "1" : "0", this.UserType.Id.ToString(), this.Url };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormId", "CredentialApplicationTypeId", "Name", "Description", "Inactive", "CredentialApplicationFormUserTypeId", "Url" };
        
        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { UserType };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return _sectionBuilders.Cast<IFormBuilder>().Concat(_applicationCredentialTypeBuilders);
        }
    }

    public class FormSectionBuilder : BaseFormBuilder, IFormSectionBuilder
    {
        private static int _nextSectionId = 1;

        private int _nextQuestionDisplayOrder;
        private readonly IApplicationFormBuilder _formBuilder;

        private readonly IList<IFormQuestionBuilder> _questionBuilders;
        public string Name { get; }
        public string Description { get; }
        public int DisplayOrder { get; }
        
        internal FormSectionBuilder(IApplicationFormBuilder formBuilder, string name, string description, int displayOrder) : base(_nextSectionId++)
        {
            Name = name;
            Description = description;
            _formBuilder = formBuilder;
            DisplayOrder = displayOrder;
            _questionBuilders = new List<IFormQuestionBuilder>();
        }
        public IFormQuestionBuilder AddQuestion(IFormQuestionTypeBuilder questionTypeBuilder)
        {
            var question = new FormQuestionBuilder(this, questionTypeBuilder, _nextQuestionDisplayOrder++);
            _questionBuilders.Add(question);
            return question;
        }
        
        protected override string TableName => "tblCredentialApplicationFormSection";

        protected override IEnumerable<string> Values => new[] { this._formBuilder.Id.ToString(), this.Name, this.DisplayOrder.ToString(), this.Description };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormSectionId", "CredentialApplicationFormId", "Name", "DisplayOrder", "Description" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { _formBuilder };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return _questionBuilders;
        }
    }
    
    public class FormQuestionBuilder : BaseFormBuilder, IFormQuestionBuilder
    {
        public IFormSectionBuilder SectionBuilder { get; }
        public IFormQuestionTypeBuilder QuestionTypeBuilder { get; }

        public int? Field { get; private set; }
        public int DisplayOrder { get; }
        private static int _nextQuestionId = 1;
        private int _nextQuestionAnswerOrder;
     
        private readonly IList<IFormQuestionAnswerOptionBuilder> _options;

        private readonly BaseLogicHelper _logicBuilderHelper;
        public IEnumerable<IFormQuestionAnswerOptionBuilder> Options => _options;

        public IEnumerable<IFormQuestionLogicBuilder> Logics => _logicBuilderHelper.Logics;

        public FormQuestionBuilder(IFormSectionBuilder sectionBuilder, IFormQuestionTypeBuilder questionTypeBuilder, int displayOrder) : base(_nextQuestionId++)
        {
            SectionBuilder = sectionBuilder;
            QuestionTypeBuilder = questionTypeBuilder;
            DisplayOrder = displayOrder;
            _options = new List<IFormQuestionAnswerOptionBuilder>();
            _logicBuilderHelper = new BaseLogicHelper(this);
        }

        public IFormQuestionBuilder StoreReponseOnField(int field)
        {
            if (!this.QuestionTypeBuilder.Type.AllowText)
            {
                throw new NotSupportedException($"Text storage cant be configured for question type {this.QuestionTypeBuilder.Type.Name}");
            }
            this.Field = field;
            return this;
        }

        public IFormQuestionAnswerOptionBuilder WhenSelectedOption(IFormAnswerOptionBuilder optionBuilder)
        {
            if (optionBuilder.QuestionTypeBuilder != this.QuestionTypeBuilder)
            {
                throw new ArgumentException($"Option is not available in the question");
            }

            var questionAnswer = new FormQuestionAnswerOptionBuilder(this, optionBuilder, _nextQuestionAnswerOrder++);
            _options.Add(questionAnswer);

            return questionAnswer;
        }

        public ILogicSelector ShowOnlyIf()
        {
            return _logicBuilderHelper;
        }

        protected override string TableName => "tblCredentialApplicationFormQuestion";

        protected override IEnumerable<string> Values => new[] { this.SectionBuilder.Id.ToString(), this.QuestionTypeBuilder.Id.ToString(), this.Field?.ToString(), this.DisplayOrder.ToString() };
        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormQuestionId", "CredentialApplicationFormSectionId", "CredentialApplicationFormQuestionTypeId", "CredentialApplicationFieldId", "DisplayOrder" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new IFormBuilder[] { SectionBuilder, QuestionTypeBuilder };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return _options.Cast<IFormBuilder>().Concat(_logicBuilderHelper.Logics);
        }
    }
    
    public class FormQuestionAnswerOptionBuilder : BaseFormBuilder, IFormQuestionAnswerOptionBuilder
    {
        private static int _nextQuestionAnswerId = 1;

        private bool _valueSet;
        private bool _fieldSet;
        private bool _fielOptionIdSet;
        private bool _doNotStoreAnswerSet;
        public string FieldData { get; private set; }
        public int? FieldOptionId { get; private set; }
        public bool IsDefaultAnswer { get; private set; }
        public int? Field { get; private set; }
        public IFormQuestionBuilder Question { get; }
        public IFormAnswerOptionBuilder OptionBuilder { get; }
        public int DisplayOrder { get; }

        internal FormQuestionAnswerOptionBuilder(IFormQuestionBuilder question, IFormAnswerOptionBuilder optionBuilder, int displayOrder) : base(_nextQuestionAnswerId++)
        {
            Question = question;
            OptionBuilder = optionBuilder;
            DisplayOrder = displayOrder;
        }

        public IFormQuestionAnswerOptionBuilder Store(string fieldData)
        {
            if (_valueSet || _fielOptionIdSet || _doNotStoreAnswerSet)
            {
                throw new ArgumentException($"Value has been already configured for option {Question.QuestionTypeBuilder.Text}");
            }
            FieldData = fieldData;
            _valueSet = true;
            return this;
        }

        public IFormQuestionAnswerOptionBuilder StoreOption(int fieldOptionId)
        {
            if (_valueSet || _fielOptionIdSet || _doNotStoreAnswerSet)
            {
                throw new ArgumentException($"Value has been already configured for option {Question.QuestionTypeBuilder.Text}");
            }
            FieldOptionId = fieldOptionId;
            _fielOptionIdSet = true;
            return this;
        }

        public IFormAnswerOptionBuilder DoNotStoreAnswer()
        {
            if (_valueSet || _fielOptionIdSet || _doNotStoreAnswerSet)
            {
                throw new ArgumentException($"Value has been already configured for option {Question.QuestionTypeBuilder.Text}");
            }
            _doNotStoreAnswerSet = true;
            return OptionBuilder;
        }

        public IFormAnswerOptionBuilder OnField(int fieldId)
        {
            if (_fieldSet)
            {
                throw new ArgumentException($"Field has been already configured for question {Question.QuestionTypeBuilder.Text}");
            }
            Field = fieldId;
            _fieldSet = true;
            return OptionBuilder;
        }

        public void AsDefaultAnswer()
        {
            this.IsDefaultAnswer = true;
        }

        protected override string TableName => "tblCredentialApplicationFormQuestionAnswerOption";

        protected override IEnumerable<string> Values => new[] { this.Question.Id.ToString(), this.OptionBuilder.Id.ToString(), (this.IsDefaultAnswer ? 1 : 0).ToString(), this.Field?.ToString(), this.DisplayOrder.ToString(), this.FieldOptionId?.ToString(), this.FieldData };
        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormQuestionAnswerOptionId", "CredentialApplicationFormQuestionId", "CredentialApplicationFormAnswerOptionId", "DefaultAnswer", "CredentialApplicationFieldId", "DisplayOrder", "CredentialApplicationFieldOptionOptionId", "FieldData" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new IFormBuilder[] { Question, OptionBuilder };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }
    
    public class FormQuestionTypeBuilder : BaseFormBuilder, IFormQuestionTypeBuilder
    {
        private static int _nextQuestionTypeId = 1;
        public string Text { get; }
        public string Description { get; }
        public IFormAnswerTypeBuilder Type { get; }

        private readonly IList<IFormAnswerOptionBuilder> _options;
        public IEnumerable<IFormAnswerOptionBuilder> Options => _options;

        internal FormQuestionTypeBuilder(string text, string description, IFormAnswerTypeBuilder type) : base(_nextQuestionTypeId++)
        {
            Text = text;
            Description = description;
            Type = type;
            _options = new List<IFormAnswerOptionBuilder>();
        }
        
        public FormQuestionTypeBuilder Clone()
        {
            return new FormQuestionTypeBuilder(this.Text, this.Description, this.Type);
        }

        public IFormAnswerOptionBuilder AddOption(string option, string description)
        {
            if (!this.Type.AllowOptions)
            {
                throw new NotSupportedException($"Answer type {Type.Name} does not support options");
            }
            var optionBuilder = new FormAnswerOptionBuilder(this, option, description);
            _options.Add(optionBuilder);
            return optionBuilder;
        }

        protected override string TableName => "tblCredentialApplicationFormQuestionType";
        protected override IEnumerable<string> Values => new[] { this.Text, this.Type.Id.ToString(), this.Description, };
        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormQuestionTypeId", "Text", "CredentialApplicationFormAnswerTypeId", "Description" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { this.Type };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return this.Options;
        }
    }
    
    public class FormAnswerOptionBuilder : BaseFormBuilder, IFormAnswerOptionBuilder
    {
        public IFormQuestionTypeBuilder QuestionTypeBuilder { get; }
        public string Option { get; }
        public string Description { get; }
        private static int _nextAnswerOptionId = 1;

        private int _nextActionOrderId;

        private IList<FormAnswerOptionActionBuilder> _optionActions;
        private IList<FormAnswerOptionDocumentTypeBuilder> _optionDocuments;

        internal FormAnswerOptionBuilder(FormQuestionTypeBuilder questionTypeBuilder, string option, string description) : base(_nextAnswerOptionId++)
        {
            QuestionTypeBuilder = questionTypeBuilder;
            Option = option;
            Description = description;
            _optionActions = new List<FormAnswerOptionActionBuilder>();
            _optionDocuments = new List<FormAnswerOptionDocumentTypeBuilder>();
        }

        public IFormAnswerOptionBuilder ExecuteActionWhenSelected(IFormActionTypeBuilder action, string parameter)
        {
            var optionAction = new FormAnswerOptionActionBuilder(this, action, parameter, _nextActionOrderId++);
            _optionActions.Add(optionAction);
            return this;
        }

        public IFormAnswerOptionBuilder RequestDocumentWhenSelected(int documentTypeId)
        {
            var optionDocument = new FormAnswerOptionDocumentTypeBuilder(this, documentTypeId);
            _optionDocuments.Add(optionDocument);
            return this;
        }

        protected override string TableName => "tblCredentialApplicationFormAnswerOption";

        protected override IEnumerable<string> Values => new[] { this.Option, this.QuestionTypeBuilder.Id.ToString(), this.Description };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormAnswerOptionId", "Option", "CredentialApplicationFormQuestionTypeId", "Description" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { this.QuestionTypeBuilder };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return this._optionActions.Cast<IFormBuilder>().Concat(this._optionDocuments);
        }
    }

    public class FormAnswerOptionDocumentTypeBuilder : BaseFormBuilder
    {
        public FormAnswerOptionBuilder Option { get; }
        public int DocumentTypeId { get; }

        private static int _nextFormOptionDocumentId = 1;
        public FormAnswerOptionDocumentTypeBuilder(FormAnswerOptionBuilder option, int documentTypeId) : base(_nextFormOptionDocumentId++)
        {
            Option = option;
            DocumentTypeId = documentTypeId;
        }

        protected override string TableName => "tblCredentialApplicationFormAnswerOptionDocumentType";

        protected override IEnumerable<string> Values => new[]
            {this.Option.Id.ToString(), this.DocumentTypeId.ToString()};

        protected override IEnumerable<string> Columns => new[] {"CredentialApplicationFormAnswerOptionDocumentTypeId", "CredentialApplicationFormAnswerOptionId","DocumentTypeId"
        };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new[] { this.Option };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }

    public class FormAnswerOptionActionBuilder : BaseFormBuilder
    {
        public IFormAnswerOptionBuilder Option { get; }
        public IFormActionTypeBuilder Action { get; }
        public string Parameter { get; }
        public int Order { get; }

        private static int _nextFormAnswerOptionActionBuilder = 1;

        internal FormAnswerOptionActionBuilder(IFormAnswerOptionBuilder option, IFormActionTypeBuilder action, string parameter, int order) : base(_nextFormAnswerOptionActionBuilder++)
        {
            Option = option;
            Action = action;
            Parameter = parameter;
            Order = order;
        }

        protected override string TableName => "tblCredentialApplicationFormAnswerOptionActionType";

        protected override IEnumerable<string> Values => new[]
            {this.Option.Id.ToString(), this.Action.Id.ToString(), this.Parameter, this.Order.ToString()};

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormAnswerOptionActionTypeId", "CredentialApplicationFormAnswerOptionId", "CredentialApplicationFormActionTypeId", "Parameter", "Order" };

        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            return new IFormBuilder[] { Option, Action };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }

  

    public class FormQuestionLogicBuilder : BaseFormBuilder, IFormQuestionLogicBuilder
    {
        private static int _nextFormQuestionLogicId = 1;
        public IFormQuestionBuilder Question { get; }
        public bool NotLogic { get; protected set; }
        public bool AndLogic { get; protected set; }
        public int Group { get; protected set; }
        public int Order { get; protected set; }

        public IFormQuestionAnswerOptionBuilder OptionLogic { get; protected set; }
        public int? CredentialTypeId { get; protected set; }
       
        public int? CredentialRequestPathTypeId { get; protected set; }
        public bool? PdPointsMetLogic { get; protected set; }
        public bool? WorkPracticeMetLogic { get; protected set; }

        public int? SkillId { get; protected set; }

        internal FormQuestionLogicBuilder(IFormQuestionBuilder question, bool notLogic, bool andLogic, int groupId, int order, IFormQuestionAnswerOptionBuilder option = null, int? credentialTypeId = null, int? credentialRequestPathTypeId = null, bool? pdPointsMet = null, bool? workPracticeMet = null, int? skillId = null) : base(_nextFormQuestionLogicId++)
        {
            Question = question;
            NotLogic = notLogic;
            AndLogic = andLogic;
            Group = groupId;
            Order = order;
            OptionLogic = option;
            CredentialTypeId = credentialTypeId;
            CredentialRequestPathTypeId = credentialRequestPathTypeId;
            PdPointsMetLogic = pdPointsMet;
            WorkPracticeMetLogic = workPracticeMet;
            SkillId = skillId;
        }

        protected override string TableName => "tblCredentialApplicationFormQuestionLogic";

        protected override IEnumerable<string> Values => new[]
        {
            this.Question.Id.ToString(), this.OptionLogic?.Id.ToString(), (this.NotLogic ? 1 : 0).ToString(),
            (this.AndLogic ? 1 : 0).ToString(), this.CredentialTypeId?.ToString(), this.Group.ToString(),
            this.Order.ToString(),
            this.CredentialRequestPathTypeId?.ToString(),
            (this.PdPointsMetLogic.GetValueOrDefault() ? 1.ToString() : null),
            (this.WorkPracticeMetLogic.GetValueOrDefault() ? 1.ToString() : null),
            this.SkillId?.ToString()
        };

        protected override IEnumerable<string> Columns => new[] { "CredentialApplicationFormQuestionLogicId", "CredentialApplicationFormQuestionId", "CredentialApplicationFormQuestionAnswerOptionId", "Not", "And", "CredentialTypeId", "Group", "Order", "CredentialRequestPathTypeId", "PdPointsMet", "WorkPracticeMet", "SkillId" };
        protected override IEnumerable<IFormBuilder> GetParentBuilders()
        {
            if (this.OptionLogic != null)
            {
                return new IFormBuilder[] { this.Question, this.OptionLogic };
            }

            return new IFormBuilder[] { this.Question };
        }

        protected override IEnumerable<IFormBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IFormBuilder>();
        }
    }

    public class BaseLogicHelper : FormQuestionLogicBuilder, ILogicSelector, ILogicSelectorConcatenator, ILogic
    {
        private IList<IFormQuestionLogicBuilder> _logics;
        public IEnumerable<IFormQuestionLogicBuilder> Logics => _logics;
        internal BaseLogicHelper(IFormQuestionBuilder question) : base(question, false, false, 0, 0)
        {

            _logics = new List<IFormQuestionLogicBuilder>();
        }


        private void Reset()
        {
            this.OptionLogic = null;
            this.CredentialTypeId = null;
            this.CredentialRequestPathTypeId = null;
            this.WorkPracticeMetLogic = null;
            this.PdPointsMetLogic = null;
            this.SkillId = null;
        }
        public ILogic CredentialType(int credentialTypeId)
        {
            this.Reset();
            this.CredentialTypeId = credentialTypeId;
            return this;
        }

        public ILogic Skill(int skillId)
        {
            this.Reset();
            this.SkillId = skillId;
            return this;
        }

        public ILogic CredentialRequestPathType(int credentialRequestPathTypeId)
        {
            this.Reset();
            this.CredentialRequestPathTypeId = credentialRequestPathTypeId;
            return this;

        }

        public ILogic PdPointsMet()
        {
            this.Reset();
            this.PdPointsMetLogic = true;
            return this;
        }

        public ILogic WorkPracticeMet()
        {
            this.Reset();
            this.WorkPracticeMetLogic = true;
            return this;
        }

        public ILogic Option(IFormQuestionAnswerOptionBuilder option)
        {
            this.Reset();
            this.OptionLogic = option;
            return this;
        }

        public ILogicSelector And()
        {
            this.AndLogic = true;
            return this;
        }

        public ILogicSelector Or()
        {
            this.AndLogic = false;
            return this;
        }

        public ILogicSelector AndGroupFormedBy()
        {
            this.Group++;
            this.AndLogic = true;
            return this;
        }

        public ILogicSelector OrGroupFormedBy()
        {
            this.Group++;
            this.AndLogic = false;
            return this;
        }

        public ILogicSelectorConcatenator IsSelected()
        {
            this.NotLogic = false;
            AddLogic();
            return this;
        }

        public ILogicSelectorConcatenator IsNotSelected()
        {
            this.NotLogic = true;
            AddLogic();
            return this;
        }

        private void AddLogic()
        {
            var logic = new FormQuestionLogicBuilder(this.Question, this.NotLogic, this.AndLogic, this.Group, this.Order, this.OptionLogic, this.CredentialTypeId, this.CredentialRequestPathTypeId, this.PdPointsMetLogic, this.WorkPracticeMetLogic, this.SkillId);
            this.Order++;
            this._logics.Add(logic);
        }
    }

}
