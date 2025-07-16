using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal
{
	public class ExaminerToolsService : IExaminerToolsService
	{
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ExaminerToolsService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        private class JobMaterialDto
		{
			public string RequestStatus { get; set; }
		}



	    public int? GetTestSittingIdByTestResult(int testResultId)
	    {
	        var testResult = NHibernateSession.Current.Get<TestResult>(testResultId);
	        return testResult?.TestSitting.Id;
	    }

        public GetTestDetailsResponse GetTestDetails(GetTestDetailsRequest request)
		{
			var testResult = NHibernateSession.Current.Get<TestResult>(request.TestResultId);
			testResult.NotNull($"Test Result with ID {request.TestResultId} not found.");

			var test = testResult.TestSitting;
			var specification = test.TestSpecification;
			var attachments = new List<TestAttendanceDocumentContract>();
			var person = request.NaatiNumber.HasValue ? NHibernateSession.Current.Query<Person>().SingleOrDefault(p => p.Entity.NaatiNumber == request.NaatiNumber) : null;

			if (person != null)
			{
				var testResultAttachments = NHibernateSession.Current.Query<TestSittingDocument>().Where(tr => tr.StoredFile.UploadedByPerson.Id == person.Id && tr.TestSitting.Id == test.Id);
				attachments = testResultAttachments.Select(a => new TestAttendanceDocumentContract
				{
					TestAttendanceDocumentId = a.Id,
					Title = a.Title,
					Type = a.StoredFile.DocumentType.DisplayName
				}).ToList();
			}

			var components = GetComponents(testResult, specification, request.UseOriginalResultMark);
			var feedback = "";
			
			if(request.NaatiNumber.HasValue)
            {
				LoggingHelper.LogInfo("Getting job examiner for feedback...");

				LoggingHelper.LogInfo($"Examiner NAATI number from request: {request.NaatiNumber.Value}");

				JobExaminer matchedJobExaminer = null;

				var count = 1;
				foreach(var jobExaminer in testResult.CurrentJob.JobExaminers)
                {
					LoggingHelper.LogInfo($"Current Job's Job Examiner #{count}'s PanelMembership ID: {jobExaminer.PanelMembership.Id}");
					LoggingHelper.LogInfo($"Current Job's Job Examiner #{count}'s Person ID: {jobExaminer.PanelMembership.Person.Id}");
					LoggingHelper.LogInfo($"Current Job's Job Examiner #{count}'s Entity ID: {jobExaminer.PanelMembership.Person.Entity.Id}");
					LoggingHelper.LogInfo($"Current Job's Job Examiner #{count}'s NAATI Number from Entity: {jobExaminer.PanelMembership.Person.Entity.NaatiNumber}");

					if (jobExaminer.PanelMembership.Person.Entity.NaatiNumber == request.NaatiNumber.Value)
                    {
						LoggingHelper.LogInfo($"Request Naati Number {request.NaatiNumber} matched Entity NaatiNumber {jobExaminer.PanelMembership.Person.Entity.NaatiNumber}");
						matchedJobExaminer = jobExaminer;
                    }

					count++;
				}

				if (matchedJobExaminer.IsNull() && request.NaatiNumber.HasValue && request.TestResultId > 0)
				{
					throw new Exception($"Job Examiner did not exist for Examiner (NAATI Number) {request.NaatiNumber} and Test Result {request.TestResultId}.");
				}

				if (matchedJobExaminer.IsNotNull() && matchedJobExaminer.Feedback.IsNotNullOrEmpty())
                {
					LoggingHelper.LogInfo($"Setting feedback as job examiner feedback: {matchedJobExaminer.Feedback}");
					feedback = matchedJobExaminer.Feedback;
				}
			}

			return new GetTestDetailsResponse
			{
				OverAllPassMark = new TestSpecificationPassMarkContract
				{
					OverAllPassMark = specification.TestSpecificationStandardMarkingSchemes.FirstOrDefault()?.OverallPassMark ?? 0
				},
				Components = components.ToArray(),
				Attachments = attachments.ToArray(),
				CommentsGeneral = testResult.CommentsGeneral,
				TestMarkingTypeId = (int)specification.MarkingSchemaType(),
				Feedback = feedback,
			};
		}

		private List<StandardTestComponentContract> GetComponents(TestResult testResult, TestSpecification specification, bool? useOriginalResultMark)
		{
			var components = new List<StandardTestComponentContract>();
			var originalTestResult = testResult.TestSitting.TestResults.OrderBy(tr => tr.Id).First();

			if (originalTestResult.TestComponentResults != null && originalTestResult.TestComponentResults.Any())
			{
				foreach (var c in originalTestResult.TestComponentResults)
				{
					var com = MapTestComponentResult(c, specification);

					if (!useOriginalResultMark.HasValue)
					{
						if (com.MarkingResultTypeId != (int)MarkingResultTypeName.FromOriginal)
						{
							com.Mark = null;
						}
					}
					else if (!useOriginalResultMark.Value)
					{
						com.Mark = null;
						var componentResult = testResult.TestComponentResults.FirstOrDefault(x => x.ComponentNumber == com.ComponentNumber);
						if (componentResult != null)
						{
							com = MapTestComponentResult(componentResult, specification);
						}
					}

					if (string.IsNullOrWhiteSpace(com.Name))
					{
						com.Name = c.Type.Name;
					}

					components.Add(com);
				}
			}
			else
			{
				foreach (var component in specification.TestComponents)
				{
					var testComponenteTypeStandardMarkingScheme = component.Type.ActiveTestComponentTypeStandardMarkingScheme;

					var com = _autoMapperHelper.Mapper.Map<StandardTestComponentContract>(component);
					com.PassMark = testComponenteTypeStandardMarkingScheme?.PassMark ?? 0;
					com.TotalMarks = testComponenteTypeStandardMarkingScheme?.TotalMarks ?? 0;
					com.Name = GetComponentIdNameAndLabel(component).Item2;
					com.Label = component.Label;
					com.TypeId = component.Type.Id;

					if (string.IsNullOrWhiteSpace(com.Name))
					{
						com.Name = component.Type.Name;
					}

					components.Add(com);
				}
			}

			return components;
		}


		private StandardTestComponentContract MapTestComponentResult(TestComponentResult c, TestSpecification specification)
		{
			if (c == null)
			{
				return null;
			}

            

			var com = _autoMapperHelper.Mapper.Map<StandardTestComponentContract>(c);

			com.TestComponentResultId = c.Id;
			com.PassMark = c.Type.ActiveTestComponentTypeStandardMarkingScheme?.PassMark ?? 0;
			com.TotalMarks = c.Type.ActiveTestComponentTypeStandardMarkingScheme?.TotalMarks ?? 0;
			com.MarkingResultTypeId = c.MarkingResultType.Id;
			com.TypeId = c.Type.Id;
			com.Label = c.ComponentNumber.ToString();
			com.TypeName = c.Type.Name;
			com.TypeLabel = c.Type.Label;
			var idNameAndLabel = GetComponentIdNameAndLabel(com.ComponentNumber, specification);

			if (idNameAndLabel != default(System.Tuple<int, string, string>))
			{
				com.Id = idNameAndLabel.Item1;
				com.Name = idNameAndLabel.Item2;
				com.Label = idNameAndLabel.Item3;
			}

			return com;
		}

		private static Job GetParentJob(Job job)
		{
			var parentJob = job;

			while (true)
			{
				if (!parentJob.ReviewFromJobId.HasValue)
				{
					return parentJob;
				}

				parentJob = NHibernateSession.Current.Load<Job>(parentJob.ReviewFromJobId.Value);
			}
		}

		private static IEnumerable<TestComponentResult> GetTestComponentResults(Job job, TestResult result)
		{
			return result.TestSitting.TestResults.First(tr => tr.CurrentJobId == job.Id).TestComponentResults;
		}

		private static System.Tuple<int, string, string> GetComponentIdNameAndLabel(TestComponent c)
		{
			if (c == null) return default(System.Tuple<int, string, string>);
			var name = c.Name;

			return new System.Tuple<int, string, string>(c.Id, name, c.Label);
		}

		private static System.Tuple<int, string, string> GetComponentIdNameAndLabel(int componentNumber, TestSpecification specification)
		{
			var testComponent = specification.TestComponents.FirstOrDefault(tc => tc.ComponentNumber == componentNumber);

			return GetComponentIdNameAndLabel(testComponent);
		}

		public GetTestsResponse GetTests(GetTestsRequest request)
		{
			return request.AsChair
				? SearchAsChair(request)
				: SearchAsExaminer(request);
		}

		private static GetTestsResponse SearchAsExaminer(GetTestsRequest request)
		{
			var queryPanelMembership = NHibernateSession.Current.Query<PanelMembership>();
			var panels = from p in queryPanelMembership
						 where p.Person.Entity.NaatiNumber == request.UserId
						 select p;

			if ((request.PanelMembershipIds ?? new int[0]).Any())
			{
				panels = panels.Where(p => request.PanelMembershipIds.Contains(p.Id));
			}

			return new GetTestsResponse { Tests = GetTests(panels, request, false).ToArray() };
		}

		private static GetTestsResponse SearchAsChair(GetTestsRequest request)
		{
			var testList = new List<TestContract>();

			var queryPanelMembership = NHibernateSession.Current.Query<PanelMembership>();
			var panelMembers = from p in queryPanelMembership
							   where p.Person.Entity.NaatiNumber == request.UserId
							   select p;

			var hasRoles = request.RoleCategories != null && request.RoleCategories.Any();
			var roleIds = request.RoleCategories?.Select(x => (int)x).ToList() ?? new List<int>();


			if (hasRoles)
			{
				panelMembers = panelMembers.Where(p => roleIds.Contains(p.PanelRole.Id) && p.PanelRole.Chair);
			}

			var panels = panelMembers.Select(p => p.Panel);

			if (request.PanelId != null && request.PanelId.Any())
			{
				panels = panels.Where(p => request.PanelId.Contains(p.Id)).Distinct();
			}
			else
			{
				panels = panels.Distinct();
			}

			var hasPeople = request.NAATINumber != null && request.NAATINumber.Any();
			foreach (var p in panels)
			{
				var panelsMembership = from pm in p.PanelMemberships
									   where
										   pm.StartDate <= DateTime.Now.Date
										   && DateTime.Now.Date <= pm.EndDate
									   select pm;

				if (hasRoles)
				{
					panelsMembership = panelsMembership.Where(pm => roleIds.Contains(pm.PanelRole.Id) && pm.PanelRole.Chair);
				}

				if (hasPeople)
				{
					panelsMembership = panelsMembership.Where(pm => request.NAATINumber.Contains(pm.Person.Entity.NaatiNumber));
				}

				testList.AddRange(GetTests(panelsMembership, request, true));
			}

			return new GetTestsResponse { Tests = testList.ToArray() };
		}

		private static List<TestContract> GetTests(IEnumerable<PanelMembership> panelsMembership, GetTestsRequest request, bool asChair)
		{
			var testList = new List<TestContract>();
			var queryTestSitting = NHibernateSession.Current.Query<TestSitting>();
		    var queryTestSittingTestMaterial = NHibernateSession.Current.Query<TestSittingTestMaterial>();
            var queryJobs = NHibernateSession.Current.Query<Job>();
			var queryTestAssets = NHibernateSession.Current.Query<TestSittingDocument>();
		    var querytestSpecificationAttachment = NHibernateSession.Current.Query<TestSpecificationAttachment>();

		    var totalDaysToShow = Convert.ToInt32(NHibernateSession.Current.Query<SystemValue>()
		        .First(x => x.ValueKey == "ShowLastSubmittedExaminerMarkDays")
		        .Value);

            foreach (var pm in panelsMembership)
			{
				var testQuery = from ts in queryTestSitting
								from tr in ts.TestResults join j in queryJobs on tr.CurrentJob.Id equals j.Id
								from je in j.JobExaminers
                                
								where je.PanelMembership.Id == pm.Id
									&& ts.Sat
									&& !ts.Rejected
									&& (tr.ResultChecked == false || je.ExaminerReceivedDate >= DateTime.Now.Date.AddDays(-totalDaysToShow))
								select new
								{
									Job = j,
									TestSitting = ts,
									JobExaminer = je,
									TestResult = tr,
									HasAssets = queryTestAssets.Any(q => q.TestSitting.Id == ts.Id && q.EportalDownload)  
									            ||
									            queryTestSittingTestMaterial.Any(tstm => tstm.TestSitting.Id == ts.Id && tstm.TestMaterial.TestMaterialAttachments.Any(tma=> tma.ExaminerToolsDownload))
									            ||	
									            querytestSpecificationAttachment.Any(tsa => tsa.TestSpecification.Id == ts.TestSpecification.Id && tsa.ExaminerToolsDownload)
                                };

				if (request.DateAllocatedFrom.HasValue)
				{
					testQuery = testQuery.Where(t => t.JobExaminer.DateAllocated >= request.DateAllocatedFrom.Value.Date);
				}

				if (request.DateAllocatedTo.HasValue)
				{
					testQuery = testQuery.Where(t => t.JobExaminer.DateAllocated <= request.DateAllocatedTo.Value.Date.AddDays(1).AddMilliseconds(-1));
				}

				if (request.DateDueFrom.HasValue)
				{
					testQuery = testQuery.Where(t => t.Job.DueDate >= request.DateDueFrom.Value.Date);
				}

				if (request.DateDueTo.HasValue)
				{
					testQuery = testQuery.Where(t => t.Job.DueDate <= request.DateDueTo.Value.Date.AddDays(1).AddMilliseconds(-1));
				}

				var tests = testQuery.ToList();
				var hasStatus = request.TestStatus != null && request.TestStatus.Any();

				foreach (var t in tests)
				{
					var ts = t.TestSitting;
					var tr = t.TestResult;
					var status = "InProgress";
					var job = t.Job;

					if (asChair && t.JobExaminer.ExaminerReceivedDate.HasValue)
					{
						status = "Submitted";
					}
					else if (!asChair && (tr.ExaminerMarkings.Count(em => em.JobExaminer.PanelMembership.Person.Entity.NaatiNumber == t.JobExaminer.PanelMembership.Person.Entity.NaatiNumber && em.Status == ExaminerMarkingStatus.Submitted) > 0 || t.JobExaminer.ExaminerReceivedDate != null))
					{
						status = "Submitted";
					}

					// it's not Overdue until after the DAY of the due date (ignore the time, which is always midnight)
					if (status == "InProgress" && job.DueDate.GetValueOrDefault().Date.AddDays(1) <= DateTime.Now)
					{
						status = "Overdue";
					}

					if (hasStatus && !request.TestStatus.Contains(status))
					{
						continue;
					}

					var credentialRequest = ts.CredentialRequest ?? new CredentialRequest();
					var app = credentialRequest.CredentialApplication ?? new CredentialApplication();
					var person = ts.CredentialRequest.CredentialApplication.Person;

					testList.Add(new TestContract
					{
						JobID = job.Id,
						TestSittingId = ts.Id,
						TestResultID = tr.Id,
						SkillDisplayName = credentialRequest.Skill.DisplayName,
						CredentialTypeExternalName = credentialRequest.CredentialType.ExternalName,
						CredentialTypeInternalName = credentialRequest.CredentialType.InternalName,
						TestDate = ts.TestSession.TestDateTime,
						DueDate = job.DueDate,
						//MaterialID = (ta.TestMaterial ?? new D.TestMaterial(0)).Id,
						Status = status,
						Examiner = (pm.Person ?? new Person()).FullName,
						DateAllocated = (t.JobExaminer ?? new JobExaminer()).DateAllocated,
						Description = $"{tr.TestSitting.CredentialRequest.CredentialType.ExternalName} - {tr.TestSitting.CredentialRequest.Skill.DisplayName}",
					    HasAssets = t.HasAssets,
						Applicant = person.FullName,
						NaatiNumber = person.Entity?.NaatiNumber ?? 0,
						Supplementary = ts.Supplementary,
						TestMarkingTypeId = (int)ts.TestSpecification.MarkingSchemaType(),
					    HasDefaultSpecification = ts.HasDefaultSpecification()

                    });
				}
			}

			return testList;
		}

		public SubmitTestResponse SubmitTest(SubmitTestRequest request)
		{

			using (var transaction = NHibernateSession.Current.BeginTransaction())
			{
				var queryTestResults = NHibernateSession.Current.Query<TestResult>();
				var queryJobs = NHibernateSession.Current.Query<Job>();
				var queryComponents = NHibernateSession.Current.Query<TestComponent>();

				var result = (from tr in queryTestResults
							  where tr.Id == request.TestResultID
							  select tr).First();

				var jobExaminer = (from j in queryJobs
								   where j.Id == result.CurrentJob.Id
								   from je in j.JobExaminers
								   where je.PanelMembership.Person.Entity.NaatiNumber == request.UserId
								   select je).First();

				jobExaminer.Feedback = request.Feedback;

                var examinerMarking = result.ExaminerMarkings.FirstOrDefault(x => x.JobExaminer.Id == jobExaminer.Id);
                if (examinerMarking == null)
                {
                    examinerMarking = new ExaminerMarking
                    {
                        JobExaminer = jobExaminer,
                        TestResult = result
                    };
                    result.AddExaminerMarking(examinerMarking);
                }

                if (examinerMarking.ExaminerTestComponentResults.Any())
                {
                    throw new Exception("Results for this test have already been submitted");
                }

				examinerMarking.Comments = request.Comments;
				examinerMarking.Status = ExaminerMarkingStatus.Submitted;
				examinerMarking.ReasonsForPoorPerformance =
					string.Join(",", (request.ReasonsForPoorPerformance ?? new List<string>()).ToArray());
				examinerMarking.PrimaryReasonForFailure = request.PrimaryReasonForFailure;
				examinerMarking.SubmittedDate = DateTime.Now;
				examinerMarking.CountMarks = true;

				NHibernateSession.Current.Save(examinerMarking);

				jobExaminer.ExaminerReceivedDate = examinerMarking.SubmittedDate;
				NHibernateSession.Current.Save(jobExaminer);

				var job = NHibernateSession.Current.Load<Job>(result.CurrentJobId.GetValueOrDefault());
				var parentJob = GetParentJob(job);
				var unknownResultType = NHibernateSession.Current.Load<MarkingResultType>((int)MarkingResultTypeName.Original);

				foreach (var requestComponent in request.Components)
				{
					var component = (from co in queryComponents
									 where co.Id == requestComponent.Id
									 select co).FirstOrDefault()
									?? GetTestComponentResults(parentJob, result).Where(cr => cr.ComponentNumber == requestComponent.ComponentNumber).Select(cr => new TestComponent
									{
										Type = cr.Type,
										ComponentNumber = cr.ComponentNumber.GetValueOrDefault(),
										GroupNumber = cr.GroupNumber.GetValueOrDefault()
									}).First();

					var testComponenteTypeStandardMarkingScheme = component.Type.ActiveTestComponentTypeStandardMarkingScheme;
					var mark = requestComponent.Mark.GetValueOrDefault();

					var examinerTestComponentResult = new ExaminerTestComponentResult
					{
						Mark = mark,
						Type = component.Type,
						TotalMarks = testComponenteTypeStandardMarkingScheme?.TotalMarks ?? 0,
						PassMark = testComponenteTypeStandardMarkingScheme?.PassMark ?? 0,
						ComponentNumber = component.ComponentNumber,
						GroupNumber = component.GroupNumber
					};

					examinerMarking.AddExaminerTestComponentResult(examinerTestComponentResult);

					var componentResult = GetTestComponentResults(job, result).FirstOrDefault(cr => cr.ComponentNumber == component.ComponentNumber)
										  ?? new TestComponentResult
										  {
											  TestResult = result,
											  Mark = mark,
											  Type = component.Type,
											  TotalMarks = testComponenteTypeStandardMarkingScheme?.TotalMarks ?? 0,
											  PassMark = testComponenteTypeStandardMarkingScheme?.PassMark ?? 0,
											  ComponentNumber = component.ComponentNumber,
											  GroupNumber = component.GroupNumber,
											  MarkingResultType = unknownResultType
										  };


					NHibernateSession.Current.Save(componentResult);
					NHibernateSession.Current.Save(examinerTestComponentResult);
				}

				NHibernateSession.Current.Flush();
				UpdateMarks(result, transaction);
				transaction.Commit();
				return new SubmitTestResponse();
			}

		}

		private void UpdateMarks(TestResult result, ITransaction transaction)
		{
			var examinerQueryService = new ExaminerQueryService(_autoMapperHelper);
		    var examinerIds = result.ExaminerMarkings.Where(x => x.CountMarks).Select(y => y.JobExaminer.Id);
		    examinerQueryService.UpdateCountMarks(new UpdateCountMarksRequest()
		    {
		        IncludePreviousMarks = result.IncludePreviousMarks,
		        JobExaminersId = examinerIds.ToArray(),
		        TestResultId = result.Id
		    }, transaction);
        }

		public ListUnavailabilityResponse ListUnavailability(ListUnavailabilityRequest request)
		{
			var queryExaminerUnavailable = NHibernateSession.Current.Query<ExaminerUnavailable>();
			var list = queryExaminerUnavailable.Where(e => e.Person.Entity.NaatiNumber == request.NAATINumber).ToList();

			return new ListUnavailabilityResponse
			{
				Periods = list.Select(_autoMapperHelper.Mapper.Map<ExaminerUnavailableContract>).ToArray()
			};
		}

		public SaveUnavailabilityResponse SaveUnavailability(SaveUnavailabilityRequest request)
		{
			var unavailability = new ExaminerUnavailable(KeyAllocation.GetSingleKey("ExaminerUnavailable"));

			if (request.Id.HasValue)
			{
				unavailability = NHibernateSession.Current.Load<ExaminerUnavailable>(request.Id.Value);
			}
			else
			{
				var queryPerson = NHibernateSession.Current.Query<Person>();
				var person = queryPerson.Single(p => p.Entity.NaatiNumber == request.NAATINumber);
				unavailability.Person = person;
			}

			unavailability.StartDate = request.StartDate;
			unavailability.EndDate = request.EndDate;

			NHibernateSession.Current.Save(unavailability);
			NHibernateSession.Current.Flush();

			return new SaveUnavailabilityResponse();
		}

		public DeleteUnavailabilityResponse DeleteUnavailability(DeleteUnavailabilityRequest request)
		{
			var unavailability = NHibernateSession.Current.Get<ExaminerUnavailable>(request.Id);
			NHibernateSession.Current.Delete(unavailability);
			NHibernateSession.Current.Flush();
			return new DeleteUnavailabilityResponse();
		}

	    public bool IsValidExaminerForAvailability(int examinerUnavailableId, int naatinumber)
	    {
	        var unavailability = NHibernateSession.Current.Get<ExaminerUnavailable>(examinerUnavailableId);
	        if (unavailability != null)
	        {
	            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == naatinumber);
	            if (person != null && unavailability.Person.Id == person.Id)
	            {
	                return true;
                }
	        }
            return false;
	    }

        public GetTestsMaterialResponse GetTestsMaterial(GetTestsMaterialRequest request)
		{
			var roleCategoryIds = request.RoleCategories.Select(x => (int)x).ToList();
			var queryPanelMembership = NHibernateSession.Current.Query<PanelMembership>();
			var panelsMembership = (from p in queryPanelMembership
									where p.Person.Entity.NaatiNumber == request.UserId
									   && p.StartDate <= DateTime.Now.Date
									   && DateTime.Now.Date <= p.EndDate
										&& roleCategoryIds.Contains(p.PanelRole.Id)
									select p).ToList();

			var testList = new List<TestMaterialContract>();
			var queryJobs = NHibernateSession.Current.Query<Job>();
			var queryMaterials = NHibernateSession.Current.Query<TestMaterialAttachment>();

			foreach (var pm in panelsMembership)
			{
				var tests = from j in queryJobs
							from je in j.JobExaminers
							where j.JobCategory == 0
								&& je.PanelMembership.Id == pm.Id
								// If JobExaminer.PrimaryContact is null, treat as true
								&& (!request.PrimaryContact || je.PrimaryContact == null || je.PrimaryContact.Value)
								&& (request.ListAllStatuses || je.Status != JobExaminerStatus.Submitted)
								&& j.SettingMaterial != null
							select new
							{
								Job = j,
								TestMaterial = j.SettingMaterial,
								JobExaminer = je
							};

				foreach (var t in tests)
				{
					var ta = t.TestMaterial;
					var job = t.Job;
					var materials = from m in queryMaterials
									where m.TestMaterial.Id == ta.Id &&
										m.Deleted == false
									select m;

					var jobMaterial = NHibernateSession.Current.TransformSqlQueryDataRowResult<JobMaterialDto>("exec JobMaterialSelect " + job.Id).FirstOrDefault();
					var status = jobMaterial?.RequestStatus;

					var direction = "B";



					testList.Add(new TestMaterialContract
					{
						TestMaterialID = ta.Id,
						JobExaminerID = t.JobExaminer.Id,
						JobID = job.Id,
						Language = ta.Language.Name,

						Direction = direction,

						DueDate = job.DueDate,
						Materials = materials.Select(m => new MaterialContract { MaterialId = m.Id, Title = m.Title }).ToList(),
						DateReceived = t.JobExaminer.ExaminerReceivedDate,
						Cost = t.JobExaminer.ExaminerCost,
						Approved = status == "Approved"
					});
				}
			}

			return new GetTestsMaterialResponse { Tests = testList.ToArray() };
		}

		public TestMaterialContract GetTestMaterial(GetTestMaterialRequest request)
		{
			var queryTests = NHibernateSession.Current.Query<TestMaterial>();
			var queryMaterials = NHibernateSession.Current.Query<TestMaterialAttachment>();

			var ta = queryTests.First(t => t.Id == request.TestMaterialId);

			var materials = from m in queryMaterials
							where m.TestMaterial.Id == ta.Id &&
								m.StoredFile.UploadedByPerson.Entity.NaatiNumber == request.NAATINumber &&
								m.Deleted == false
							select m;

			var direction = "B";



			return new TestMaterialContract
			{
				TestMaterialID = ta.Id,
				Language = ta.Language.Name,

				Direction = direction,

				Materials = materials.Select(m => new MaterialContract { MaterialId = m.Id, Title = m.Title }).ToList()
			};
		}

		public SaveMaterialResponse SaveMaterial(SaveMaterialRequest request)
		{
			var person = NHibernateSession.Current.Query<Person>().Single(x => x.Entity.NaatiNumber == request.NAATINumber);

			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
			var fileResponse = mFileStorageService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
			{
				Type = StoredFileType.TestMaterial,
				FilePath = request.FilePath,
				UploadedByPersonId = person.Id,
				UploadedDateTime = DateTime.Now,
				StoragePath = $"{StoredFileType.TestMaterial}\\{request.TestMaterialId}\\{request.Title}"
			});

			var material = SaveMaterial(request, fileResponse.StoredFileId);

			return new SaveMaterialResponse
			{
				StoredFileId = fileResponse.StoredFileId,
				TestMaterialId = material.Id
			};
		}

		private static TestMaterialAttachment SaveMaterial(SaveMaterialRequest request, int storedFileId)
		{
			var material = new TestMaterialAttachment();

			var storedFile = NHibernateSession.Current.Query<StoredFile>()
				.SingleOrDefault(x => x.Id == storedFileId);

			var testMaterial = NHibernateSession.Current.Query<TestMaterial>()
				.SingleOrDefault(x => x.Id == request.TestMaterialId);

			if (storedFile == null)
			{
				throw new Exception("Referenced StoredFile does not exist: " + storedFileId);
			}

			material.StoredFile = storedFile;

			material.Title = request.Title;
			material.Deleted = false;
			material.TestMaterial = testMaterial;

			NHibernateSession.Current.Save(material);
			NHibernateSession.Current.Flush();

			return material;
		}

		public DeleteMaterialResponse DeleteMaterial(DeleteMaterialRequest request)
		{
			var material = NHibernateSession.Current.Query<TestMaterialAttachment>().Single(x => x.Id == request.MaterialId);

			material.Deleted = true;

			NHibernateSession.Current.Save(material);
			NHibernateSession.Current.Flush();

			return new DeleteMaterialResponse();
		}

		public GetMaterialFileResponse GetMaterialFile(GetMaterialFileRequest request)
		{
			var material = NHibernateSession.Current.Query<TestMaterialAttachment>().Single(x => x.Id == request.MaterialId);

			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
			var storedFile = mFileStorageService.GetFile(new GetFileRequest { StoredFileId = material.StoredFile.Id, TempFileStorePath = request.TempFileStorePath });

			return new GetMaterialFileResponse
			{
				FileName = material.StoredFile.FileName,
				FilePaths = storedFile.FilePaths
			};
		}

		public GetTestAttendanceDocumentResponse GetTestAttendanceDocument(GetTestAttendanceDocumentRequest request)
		{
			var material = NHibernateSession.Current.Query<TestSittingDocument>().Single(x => x.Id == request.TestAttendanceDocumentId);

			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
			var storedFile = mFileStorageService.GetFile(new GetFileRequest { StoredFileId = material.StoredFile.Id, TempFileStorePath = request.TempFileStorePath });

			return new GetTestAttendanceDocumentResponse
			{
				FileName = material.StoredFile.FileName,
				FilePaths = storedFile.FilePaths
			};
		}

		public GetDocumentTypesResponse GetDocumentTypes()
		{
			var response = new GetDocumentTypesResponse
			{
				DocumentTypes = NHibernateSession.Current.Query<DocumentType>().Where(dt => dt.ExaminerToolsUpload).Select(dt => new DocumentTypeContract { Id = dt.Id, DisplayName = dt.DisplayName }).ToArray()
			};
			return response;
		}

		public SubmitMaterialResponse SubmitMaterial(SubmitMaterialRequest request)
		{
			var jobExaminer = NHibernateSession.Current.Get<JobExaminer>(request.JobExaminerId);
			jobExaminer.Status = JobExaminerStatus.Submitted;
			jobExaminer.ExaminerReceivedDate = DateTime.Now;

			NHibernateSession.Current.Save(jobExaminer);
			NHibernateSession.Current.Flush();

			return new SubmitMaterialResponse();
		}

		public SaveAttachmentResponse SaveAttachment(SaveAttachmentRequest request)
		{
			var queryTestResults = NHibernateSession.Current.Query<TestResult>();
			var queryJobs = NHibernateSession.Current.Query<Job>();
			var queryStoredFile = NHibernateSession.Current.Query<StoredFile>();

			var result = (from tr in queryTestResults
						  where tr.Id == request.TestResultId
						  select tr).First();

			var jobExaminer = (from j in queryJobs
							   where j.Id == result.CurrentJob.Id
							   from je in j.JobExaminers
							   where je.PanelMembership.Person.Entity.NaatiNumber == request.NAATINumber
							   select je).First();

			var person = jobExaminer.PanelMembership.Person;
			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
			var fileResponse = mFileStorageService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
			{
				Type = (StoredFileType)request.Type,
				FilePath = request.FilePath,
				UploadedByPersonId = person.Id,
				UploadedDateTime = DateTime.Now,
				StoragePath = $"{(StoredFileType)request.Type}\\{result.TestSitting.Id}\\{request.Title}"
			});

			var storedFile = queryStoredFile.Single(x => x.Id == fileResponse.StoredFileId);

			var document = new TestSittingDocument
			{
				TestSitting = result.TestSitting,
				Title = request.Title,
				StoredFile = storedFile
			};

			NHibernateSession.Current.Save(document);
			NHibernateSession.Current.Flush();

			return new SaveAttachmentResponse
			{
				TestAttendanceDocumentId = document.Id,
				StoredFileId = storedFile.Id
			};
		}

		public DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request)
		{
			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);

			var attachment = NHibernateSession.Current.Query<TestSittingDocument>()
				.Single(x => x.Id == request.TestAttendanceDocumentId);

			var storedFileId = attachment.StoredFile.Id;

			NHibernateSession.Current.Delete(attachment);
			NHibernateSession.Current.Flush();

			mFileStorageService.DeleteFile(new DeleteFileRequest { StoredFileId = storedFileId });

			return new DeleteAttachmentResponse();
		}

		public GetTestAssetsFileResponse GetTestAssetsFile(GetTestAssetsFileRequest request)
		{
			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);

			var assets = NHibernateSession.Current.Query<TestSittingDocument>()
				.Where(x => x.TestSitting.Id == request.TestSittingId && x.EportalDownload);

			var file = mFileStorageService.GetFiles(new GetFilesRequest { StoredFileIds = assets.Select(a => a.StoredFile.Id).ToArray(), TempFileStorePath = request.TempFileStorePath });

			return new GetTestAssetsFileResponse
			{
				FileName = file.FileName,
				FilePaths = file.FilePaths
			};
		}

		public GetAttendeesTestSpecificationTestMaterialResponse GetTestMaterialsByAttendaceId(int attendanceId)
		{
			var mTestMaterialQueryService = new TestMaterialQueryService(_autoMapperHelper);

			var testSpecificationTestMaterials = mTestMaterialQueryService.GetTestSpecificationTestMaterialsByAttendanceId(attendanceId, true);

			return testSpecificationTestMaterials;
		}

		public GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest request)
		{
			var testSitting = NHibernateSession.Current.Get<TestSitting>(request.AttendanceId);

			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);

			var response = mFileStorageService.GetFileStoreTestSpecificationTestMaterialList(request);
			response.AttandanceId = request.AttendanceId;
			response.NaatiNumber = testSitting.CredentialRequest.CredentialApplication.Person.GetNaatiNumber();
			response.TestSessionId = testSitting.TestSession.Id;

			return response;
		}

		public GetTestMaterialsFileResponse GetTestMaterialsFile(GetTestMaterialsFileRequest request)
		{
			// note the following code avoids using certain methods which could drastically simplify it (Any<>(), Count<>()) because they create a method not supported exception
			// also avoids expressions reliant on short circuiting as I found this to also throw an exception which is somehow eaten and the containing WHERE statement yeilds no results
			var testMaterials = NHibernateSession.Current.Query<TestMaterialAttachment>()
				.Where(x => x.TestMaterial.Id == request.TestMaterialId).ToList();

			var valid = new List<int>();

			foreach (var testMaterial in testMaterials)
			{
				if (testMaterial.StoredFile.UploadedByUser != null)
				{
					valid.Add(testMaterial.TestMaterial.Id);
				}
				else if (testMaterial.StoredFile.UploadedByPerson != null && testMaterial.StoredFile.UploadedByPerson.Entity.NaatiNumber == request.NAATINumber)
				{
					valid.Add(testMaterial.TestMaterial.Id);
				}
			}

			var materials = NHibernateSession.Current.Query<TestMaterialAttachment>().Where(x => valid.Contains(x.TestMaterial.Id));

			var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
			var file = mFileStorageService.GetFiles(new GetFilesRequest { StoredFileIds = materials.Select(a => a.StoredFile.Id).ToArray(), TempFileStorePath = request.TempFileStorePath });

			return new GetTestMaterialsFileResponse
			{
				FileName = file.FileName,
				FilePaths = file.FilePaths
			};
		}

		public DocumentAdditionalTokenValueDto GetDocumentAdditionalTokens(int attendanceId)
		{
			var mTestMaterialQueryService = new TestMaterialQueryService(_autoMapperHelper);
			var documentAdditionalTokenValue = mTestMaterialQueryService.GetDocumentAdditionalTokens(attendanceId);
			return documentAdditionalTokenValue;
		}

		public void SaveRolePlayerSettings(RolePlayerSettingsRequest request)
		{
			using (var transaction = NHibernateSession.Current.BeginTransaction())
			{

				SaveRolePlayerSettings(request, transaction);
				transaction.Commit();
			}
		}

		public void SaveRolePlayerSettings(RolePlayerSettingsRequest request, ITransaction transaction)
		{
			var settings = request.Settings;
			var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == settings.NaatiNumber);
			if (person == null)
			{
				throw new NullReferenceException("Person doesn't found.");
			}

			var rolePlayer = NHibernateSession.Current.Query<RolePlayer>().FirstOrDefault(r => r.Person == person);
			if (rolePlayer == null)
			{
				rolePlayer = new RolePlayer { Person = person };
			}

			rolePlayer.SessionLimit = settings.MaximumRolePlaySessions;
			rolePlayer.Rating = settings.Rating;
			rolePlayer.Senior = settings.Senior;

			if (rolePlayer.Locations != null)
			{
				while (rolePlayer.Locations.Any())
				{
					var l = rolePlayer.Locations.First();
					NHibernateSession.Current.Delete(l);
					rolePlayer.RemoveLocation(l);
				}
			}

			NHibernateSession.Current.Save(rolePlayer);

			foreach (var l in settings.RolePlayLocations)
			{
				var testLocation = new RolePlayerTestLocation
				{
					TestLocation = NHibernateSession.Current.Load<TestLocation>(l),
					RolePlayer = rolePlayer
				};
				NHibernateSession.Current.Save(testLocation);
			}

		}

		public GetRolePlayerSettingsResponse GetRolePlayerSettings(GetRolePlayerSettingsRequest request)
		{
			var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == request.NaatiNumber);
			if (person == null)
			{
				throw new NullReferenceException("Person not found.");
			}

			var rolePlayer = NHibernateSession.Current.Query<RolePlayer>().FirstOrDefault(r => r.Person == person);
			if (rolePlayer == null)
			{
				return new GetRolePlayerSettingsResponse() { };
			}

			return new GetRolePlayerSettingsResponse
			{
				Settings = new RolePlayerSettingsDto
				{
					NaatiNumber = request.NaatiNumber,

					MaximumRolePlaySessions = rolePlayer.SessionLimit,
					Rating = rolePlayer.Rating ?? 0,
					Senior = rolePlayer.Senior,

					RolePlayLocations = rolePlayer.Locations?.Select(l => l.TestLocation.Id).ToArray()
				}
			};
		}   

	    public bool IsValidAttendeeDeleteAttachment(int testAttendanceDocumentId, int naatiNumber)
	    {
	        var testSittingDocument = NHibernateSession.Current.Get<TestSittingDocument>(testAttendanceDocumentId);
	        if (testSittingDocument == null) return false;
	        var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == naatiNumber);
	        if (person != null && testSittingDocument.StoredFile.UploadedByPerson.Id == person.Id)
	        {
	            return true;
	        }

	        return false;
	    }

	    private RolePlaySessionContract MapTestSessionRolePlayer(TestSessionRolePlayer session)
		{
			return new RolePlaySessionContract
			{
				Id = session.Id,
				TestSessionId = session.TestSession.Id,
				TestSessionName = session.TestSession.Name,
				CredentialType = session.TestSession.CredentialType.ExternalName,
				RolePlay = String.Join(", ", session?.Details?.Select(d => d.RolePlayerRoleType?.DisplayName).Distinct()),
				TestSessionDate = session.TestSession.TestDateTime,
				TestLocationName = session.TestSession.Venue.TestLocation.Name,
				LanguageName = String.Join(", ", session.TestSession.TestSessionSkills.Select(s => s.Skill.DisplayName)),
				RehearsalDate = session.TestSession.RehearsalDateTime,
				RehearsalNotes = session?.TestSession.RehearsalNotes,
				Attended = session.Attended,
				Rehearsed = session.Rehearsed,
				Rejected = session.Rejected
			};
		}
		
	}
}
