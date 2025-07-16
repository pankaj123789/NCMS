using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSpecification : EntityBase
    {
        private IList<TestSpecificationStandardMarkingScheme> mTestSpecificationStandardMarkingSchemes = new List<TestSpecificationStandardMarkingScheme>();
        private IList<RubricQuestionPassRule> mRubricQuestionPassRules = new List<RubricQuestionPassRule>();
        private IList<RubricTestBandRule> mRubricTestBandRules = new List<RubricTestBandRule>();
        private IList<RubricTestQuestionRule> mRubricTestQuestionRules = new List<RubricTestQuestionRule>();
        private IList<TestComponentType> mTestComponentTypes = new List<TestComponentType>();
        private IList<TestComponent> mTestComponents = new List<TestComponent>();

        public virtual IEnumerable<TestComponentType> TestComponentTypes => mTestComponentTypes;

        public virtual string Description { get; set; }

        public virtual IEnumerable<TestSpecificationStandardMarkingScheme> TestSpecificationStandardMarkingSchemes => mTestSpecificationStandardMarkingSchemes;
        public virtual IEnumerable<RubricQuestionPassRule> RubricQuestionPassRules => mRubricQuestionPassRules;
        public virtual IEnumerable<RubricTestBandRule> RubricTestBandRules => mRubricTestBandRules;
        public virtual IEnumerable<RubricTestQuestionRule> RubricTestQuestionRules => mRubricTestQuestionRules;
        public virtual TestSpecificationStandardMarkingScheme ActiveTestSpecificationStandardMarkingScheme => mTestSpecificationStandardMarkingSchemes.FirstOrDefault();

        public virtual IEnumerable<TestComponent> TestComponents { get { return mTestComponents; } }

        public virtual void AddComponent(TestComponent component)
        {
            component.TestSpecification = this;
            mTestComponents.Add(component);
        }

        public virtual void RemoveComponent(TestComponent component)
        {
            var result = (from c in mTestComponents where c.Id == component.Id select c).FirstOrDefault();

            if (result != null)
            {
                result.TestSpecification = null;
                mTestComponents.Remove(result);
            }
        }

        public virtual CredentialType CredentialType { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool AutomaticIssuing { get; set; }
        public virtual double? MaxScoreDifference { get; set; }

        public virtual MarkingSchemaTypeName MarkingSchemaType()
        {

            return TestComponentTypes.Any(x => x.RubricMarkingCompetencies.Any()) ? MarkingSchemaTypeName.Rubric : MarkingSchemaTypeName.Standard;
        }
        public virtual void RemoveRubricQuestionPassRule(RubricQuestionPassRule rubricQuestionPassRule)
        {
            var result = (from pn in mRubricQuestionPassRules
                          where pn.Id == rubricQuestionPassRule.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mRubricQuestionPassRules.Remove(result);
            }
        }
        public virtual void RemoveRubricTestBandRule(RubricTestBandRule rubricTestBandRule)
        {
            var result = (from pn in mRubricTestBandRules
                          where pn.Id == rubricTestBandRule.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mRubricTestBandRules.Remove(result);
            }
        }
        public virtual void RemoveRubricTestQuestionRule(RubricTestQuestionRule rubricTestQuestionRule)
        {
            var result = (from pn in mRubricTestQuestionRules
                          where pn.Id == rubricTestQuestionRule.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mRubricTestQuestionRules.Remove(result);
            }
        }

        public virtual bool ResultAutoCalculation { get; set; }

        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

        public virtual bool TestMaterialReminder { get; set; }
        public virtual int TestMaterialReminderDays { get; set; }

    }

    public enum MarkingSchemaTypeName
    {
        Standard = 1,
        Rubric = 2,
    }
}
