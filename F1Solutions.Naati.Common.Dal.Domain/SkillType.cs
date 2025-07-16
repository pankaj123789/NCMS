using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SkillType : EntityBase, IDynamicLookupType
    {
        private IList<Skill> mSkills = new List<Skill>();
        public virtual IEnumerable<Skill> Skills => mSkills;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public override string ToString()
        {
            return this.DisplayName;
        }
    }

    public enum SkillTypeName
    {
        RecognisedPractisingTranslator = 1,
        CertifiedTranslator,
        CertifiedAdvancedTranslator,
        CertifiedTranslatorLoteToLote,
        RecognisedPractisingInterpreter,
        CertifiedProvisionalInterpreter,
        CertifiedInterpreter,
        CertifiedConferenceInterpreter,
        CertifiedSpecialistInterpreterHealth,
        CertifiedSpecialistInterpreterLegal,
        CertifiedConferenceInterpreterLoteToLote,
        CertifiedProvisionalDeafInterpreter,
        RecognisedPractisingDeafInterpreter,
        CredentialedCommunityLanguage,
        CommunityLanguageAide,
        Ethics,
        Intercultural,
        MigrationAssessment
    }
}
