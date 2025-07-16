using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Data.NHibernate;
using F1Solutions.Naati.Common.ServiceContracts.DTO;
using D = NAATI.Domain;
using NHibernate.Linq;

namespace F1Solutions.NAATI.SAM.WebService.ExposedServices
{
    public static class LookupHelper
    {
        public static int ToSamId(this AccreditationCategory category)
        {
            var db = NHibernateSession.Current;
            switch (category)
            {
                case AccreditationCategory.Interpreter:
                    return db.Query<D.SystemParameter>().Single().InterpreterCategoryId;
                case AccreditationCategory.LanguageAide:
                    return db.Query<D.SystemParameter>().Single().LanguageAideCategoryId;
                case AccreditationCategory.Translator:
                    return db.Query<D.SystemParameter>().Single().TranslatorCategoryId;
                default:
                    throw new Exception("Unknown AccreditationCategory: " + category.ToString());
            }
        }

        public static int ToSamId(this AccreditationMethod method)
        {
            var db = NHibernateSession.Current;
            switch (method)
            {
                case AccreditationMethod.CourseApprovedByNaati:
                    return db.Query<D.SystemParameter>().Single().CourseMethodId;
                case AccreditationMethod.LanguageAide:
                    return db.Query<D.SystemParameter>().Single().LanguageAideMethodId;
                case AccreditationMethod.OverseasQualification:
                    return db.Query<D.SystemParameter>().Single().QualificationWithoutMigrationMethodId;
                case AccreditationMethod.Recognition:
                    return db.Query<D.SystemParameter>().Single().RecognitionMethodId;
                case AccreditationMethod.DeafInterpreterRecognition:
                    return db.Query<D.SystemParameter>().Single().DeafInterpreterRecognitionMethodId;
                case AccreditationMethod.SpecialOverseasQualification:
                    return db.Query<D.SystemParameter>().Single().QualificationWithMigrationMethodId;
                case AccreditationMethod.Testing:
                    return db.Query<D.SystemParameter>().Single().TestingMethodId;
                case AccreditationMethod.Revalidation:
                    return db.Query<D.SystemParameter>().Single().RevalidationMethodId;
                default:
                    throw new Exception("Unknown AccreditationMethod: " + method.ToString());
            }
        }

        public static int ToSamId(this AccreditationLevel level)
        {
            var db = NHibernateSession.Current;
            switch (level)
            {
                case AccreditationLevel.LanguageAide:
                    return db.Query<D.SystemParameter>().Single().RecognitionAndLanguageAideLevelId;
                case AccreditationLevel.Paraprofessional:
                    return db.Query<D.SystemParameter>().Single().ParaprofessionalLevelId;
                case AccreditationLevel.Professional:
                    return db.Query<D.SystemParameter>().Single().ProfessionalLevelId;
                case AccreditationLevel.AdvancedProfessional:
                    return db.Query<D.SystemParameter>().Single().AdvancedLevelId;
                case AccreditationLevel.SeniorAdvancedProfessional:
                    return 5;
                default:
                    throw new Exception("Unknown AccreditationLevel: " + level.ToString());
            }
        }

        /// <summary>
        /// Lowest to highest level
        /// </summary>
        public static IEnumerable<AccreditationLevel> LevelsBySeniority
        {
            get
            {
                yield return AccreditationLevel.LanguageAide;
                yield return AccreditationLevel.Paraprofessional;
                yield return AccreditationLevel.Professional;
                yield return AccreditationLevel.AdvancedProfessional;
                yield return AccreditationLevel.SeniorAdvancedProfessional;
            }
        }

        public static AccreditationLevel Max(this IEnumerable<AccreditationLevel> levels)
        {
            var orderedLevels = levels.OrderByDescending(l => LevelsBySeniority.ToList().IndexOf(l));
            return orderedLevels.First();
        }
    }
}
