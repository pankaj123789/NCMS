using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    internal class TableInfo
    {
        public static TableInfo[] Tables =
        {
            new TableInfo
            {
                TableName = "Accreditation",
                KeyColumn = "AccreditationResultId",
                Columns = new []
                {
                    "ApplicationId",
                    "PersonId",
                    "ResultDate",
                    "FailureReason",
                    "Days",
                    "CertificateNumber",
                    "IncludeInPD",
                    "AccreditationLevel",
                    "ExpiryDate",
                    "Expired",
                    "Notes",
                    "Result"
                }
            },
            new TableInfo
            {
                TableName = "Application",
                KeyColumn = "ApplicationId",
                Columns = new []
                {
                    "PersonId",
                    "LanguageId",
                    "LanguageName",
                    "LanguageIndigenous",
                    "ToLanguageId",
                    "ToLanguageName",
                    "ToEnglish",
                    "EnteredDate",
                    "AccreditationMethodDescription",
                    "AccreditationLevel",
                    "AccreditationCategoryDescription",
                    "ReceivingOfficeName",
                    "Status",
                    "StatusDate",
                    "StatusReason",
                    "EligibilityMeetsRequirements",
                    "EligibilityJustification",
                    "IncompleteApplication",
                    "LanguageToBoth",
                    "Direction",
                    "InviteToTesting",
                    "PreferredTestCentreName",
                    "MaxOpportunities",
                    "Prerequisite1",
                    "Prerequisite2",
                    "Prerequisite3",
                    "ForMigration",
                    "CountryOfTrainingName",
                    "ProjectName",
                    "EligibilityListedOnApplication",
                    "SponsorInstitutionName",
                    "ForCommunityLanguagePoints",
                    "ForEducationalAndSkillEmployment",
                    "SkillAssessmentResult",
                    "CourseCompletedDateInWizard"
                }
            },
            new TableInfo
            {
                TableName = "Invoice",
                KeyColumn = "InvoiceLineId",
                Columns = new []
                {
                    "InvoiceId",
                    "NaatiNumber",
                    "PersonId",
                    "InvoiceNote",
                    "InvoicePrivateNote",
                    "Amount",
                    "GST",
                    "ProductDescription",
                    "TotalAmount",
                    "TotalGST",
                    "TotalPaid",
                    "GLCode",
                    "ProductCode",
                    "DueDate",
                    "CreatedDate",
                    "Office",
                    "FullNameResponsible",
                    "RefundedInvoiceLineID"
                }
            },
            new TableInfo
            {
                TableName = "Payment",
                KeyColumn = "PaymentId",
                Columns = new []
                {
                    "InvoiceId",
                    "InvoiceLineId",
                    "PaymentDate",
                    "Office",
                    "PayerNAATINumber",
                    "PayerName",
                    "PaymentType",
                    "InvoiceLineTotalPaid",
                    "InvoiceTotalPaid"
                }
            },
            new TableInfo
            {
                TableName = "Person",
                KeyColumn = "PersonId",
                Columns = new []
                {
                    "EntityId",
                    "GivenName",
                    "OtherNames",
                    "Surname",
                    "Title",
                    "Gender",
                    "BirthDate",
                    "BirthCountry",
                    "Deceased",
                    "HighestEducationLevel",
                    "ReleaseDetails",
                    "DoNotInviteToDirectory",
                    "EnteredDate",
                    "ExpertiseFreeText",
                    "NameOnAccreditationProduct",
                    "DoNotSendCorrespondence",
                    "ScanRequired",
                    "IsEportalActive",
                    "PersonalDetailsLastUpdatedOnEportal",
                    "WebAccountCreateDate",
                    "AllowVerifyOnline",
                    "ABN",
                    "Note",
                    "NAATINumber",
                    "NAATINumberDisplay",
                    "EntityType",
                    "StreetDetails",
                    "Postcode",
                    "Country",
                    "State",
                    "MostRecentArchiveDate",
                    "MostRecentInvoiceDate",
                    "MostRecentApplicationDate",
                    "FullName"
                }
            },
            new TableInfo
            {
                TableName = "Revalidation",
                KeyColumn = "RevalidationId",
                CompositeKey = new []
                {
                    "ApplicationId"
                },
                Columns = new []
                {
                    "EnteredDate",
                    "ReceivingOffice",
                    "IncompleteApplication",
                    "Status",
                    "StatusDate",
                    "StatusReason",
                    "PersonId",
                    "EligibilityJustification",
                    "EligibilityMeetsRequirements",
                    "OldExpiryDate",
                    "NewExpiryDate"
                }
            },
            new TableInfo
            {
                TableName = "Test",
                KeyColumn = "TestInvitationId",
                CompositeKey = new []
                {
                    "TestAttendanceId",
                    "TestResultId"
                },
                Columns = new []
                {
                    "TestKey",
                    "ApplicationId",
                    "PersonId",
                    "ResponseDate",
                    "Accepted",
                    "WithdrawnDate",
                    "ConfirmedDate",
                    "TestEventId",
                    "Sat",
                    "ResultType",
                    "ThirdExaminerRequired",
                    "ProcessedDate",
                    "SatDate",
                    "ResultChecked"
                }
            }
        };

        // {0} = table name
        // {1} = abbreviation
        // {2} = key column
        // {3} = key column list
        // {4} = composite key expression
        // {5} = composite key join predicate
        // {6} = composite key where predicate
        private const string UpdateObsoletedDateFormat =
@"UPDATE [Internal].[{0}] SET [ObsoletedDate] = 
(
	SELECT [NextDate]
	FROM [Internal].[{0}] {1}
	INNER JOIN
	(
		SELECT
			{3}
			,[ModifiedDate]
			,LEAD({1}3.[ModifiedDate]) OVER (PARTITION BY {1}3.[{2}]{4} ORDER BY {1}3.[ModifiedDate]) AS [NextDate]
		FROM [Internal].[{0}] {1}3
	) {1}2 ON {1}.[{2}] = {1}2.[{2}]{5} AND {1}.[ModifiedDate] = {1}2.[ModifiedDate]
	WHERE [Internal].[{0}].[{2}] = {1}.[{2}]{6}
	AND [Internal].[{0}].[ModifiedDate] = {1}.[ModifiedDate]
)";

        // {0} = table name
        // {0} = column list
        private const string CreateObsoleteDateIndexFormat =
@"CREATE NONCLUSTERED INDEX IX_{0}_ObsoletedDate
ON [Internal].[{0}] ([ObsoletedDate])
INCLUDE
(
	{1}
)";
        public string TableName;
        public string KeyColumn;
        public string[] CompositeKey = new string[0];
        public string[] Columns;

        public string Abbreviation { get { return TableName.Substring(0, 1); } }

        public string CompositeKeyExpression
        {
            get
            {
                return string.Join(string.Empty, CompositeKey.Select(x => string.Format(", {0}3.[{1}]", Abbreviation, x)).ToArray());
            }
        }

        public string CompositeKeyJoinPredicate
        {
            get
            {
                return string.Join(" ", CompositeKey.Select(x => GetCompositeKeyPredicate(Abbreviation, string.Format("{0}2", Abbreviation), x)).ToArray());
            }
        }

        public string CompositeKeyWherePredicate
        {
            get
            {
                return string.Join(@"
    ", CompositeKey.Select(x => GetCompositeKeyPredicate(string.Format("[Internal].[{0}]", TableName), Abbreviation, x)).ToArray());
            }
        }

        public string[] KeyColumns
        {
            get
            {
                var keyColumns = new List<string>(CompositeKey.Length + 1)
                {
                    KeyColumn
                };

                keyColumns.AddRange(CompositeKey);

                return keyColumns.ToArray();
            }
        }

        public string KeyColumnList
        {
            get
            {
                return GetColumnList(KeyColumns);
            }
        }

        public string ColumnList
        {
            get
            {
                var keyColumns = KeyColumns;
                var columnList = new List<string>(keyColumns.Length + Columns.Length);

                columnList.AddRange(keyColumns);
                columnList.AddRange(Columns);

                return GetColumnList(columnList);
            }
        }

        public string UpdateObsoletedDateQuery
        {
            get
            {
                return string.Format(UpdateObsoletedDateFormat, TableName, Abbreviation,
                    KeyColumn, KeyColumnList, CompositeKeyExpression, CompositeKeyJoinPredicate, CompositeKeyWherePredicate);
            }
        }

        public string CreateObsoleteDateIndexQuery
        {
            get
            {
                return string.Format(CreateObsoleteDateIndexFormat, TableName, ColumnList);
            }
        }

        private string GetCompositeKeyPredicate(string id1, string id2, string column)
        {
            return string.Format("AND {0}.[{2}] = {1}.[{2}]", id1, id2, column);
        }

        private string GetColumnList(IEnumerable<string> columnNames)
        {
            return string.Join(@"
    ,", columnNames.Select(x => string.Format("[{0}]", x)).ToArray());
        }
    }
}
