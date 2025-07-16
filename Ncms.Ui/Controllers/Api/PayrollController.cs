using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/payroll")]
    public class PayrollController : BaseApiController
    {
        private const int PreviousResultLimit = 26;

        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payroll)
        {
            _payrollService = payroll;
        }

        [HttpGet]
        //called on main test page
        [NcmsAuthorize(SecurityVerbName.Read, new SecurityNounName[] { SecurityNounName.TestSitting })]
        [Route("productsForMarkingClaims")]
        public HttpResponseMessage ProductsForMarkingClaims()
        {
            return this.CreateResponse(() => _payrollService.GetProductSpecificationsForMarkingClaimsLookupList().OrderBy(x => x.DisplayNameWithGlCode));
        }

        [HttpGet]
        [Route("getMarkingsForValidation")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PayRun)]
        public HttpResponseMessage GetMarkingsForValidation()
        {
            var markings = _payrollService.GetMarkingsForValidation()
                .OrderBy(x => x.ProductSpecificationChangedUser)
                .GroupBy(x => x.ProductSpecificationChangedUser)
                .Select(userGroup => new
                {
                    User = userGroup.Key,
                    NoCode = userGroup.Key == null,
                    Groups = userGroup
                        .OrderBy(x => x.TestType)
                        .GroupBy(x => x.TestType)
                        .Select(testTypeGroup => new
                        {
                            TestType = testTypeGroup.Key,
                            Markings = testTypeGroup
                                .OrderBy(m => m.Examiner)
                                .ThenBy(m => m.Language).Select(TransformExaminerName),
                            OldestReceivedDays = (DateTime.Today - testTypeGroup.Min(r => r.ResultReceivedDate)).Days
                        })
                });

            return this.CreateResponse(() => markings);
        }

        private static MarkingForPayrollModel TransformExaminerName(MarkingForPayrollModel input)
        {
            input.Examiner = string.Join(" ", Regex.Split(input.Examiner, "(?<=[a-z])(?=[A-Z])"));

            return input;
        }

        [HttpGet]
        [Route("getMarkingsForNewPayroll")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PayRun)]
        public HttpResponseMessage GetMarkingsForNewPayroll()
        {
            var markings = _payrollService.GetMarkingsForNewPayroll();
            var vm = GetMarkingsForPayroll(markings, true);

            return this.CreateResponse(() => vm);
        }

        [HttpGet]
        [Route("getMarkingsForInProgressPayroll")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PayRun)]
        public HttpResponseMessage GetMarkingsForInProgressPayroll()
        {
            object vm = null;

            var markings = _payrollService.GetMarkingsForInProgressPayroll().ToArray();

            if (markings.Any())
            {
                vm = new
                {
                    markings.First().PayrollId,
                    Markings = GetMarkingsForPayroll(markings, true)
                };
            }

            return this.CreateResponse(() => vm);
        }

        [HttpPost]
        [Route("getMarkingsForPreviousPayroll")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PayRun)]
        public HttpResponseMessage GetMarkingsForPreviousPayroll(PreviousMarkingFilterForPayrollModel filter)
        {
            var markings = _payrollService.GetMarkingsForPreviousPayroll(filter);

            var vm = markings
                .GroupBy(x => new { x.PayrollId, x.PayrollModifiedDate, x.PayrollModifiedUser })
                .OrderByDescending(x => x.Key.PayrollModifiedDate)
                .Select(payrollGroup => new
                {
                    payrollGroup.Key.PayrollModifiedDate,
                    payrollGroup.Key.PayrollModifiedUser,
                    TotalCost = payrollGroup.Sum(x => x.ExaminerCost),
                    Count = payrollGroup.Count(),
                    Examiners = payrollGroup
                        .GroupBy(x => new { x.ExaminerPersonId, x.Examiner, x.AccountingReference })
                        .OrderBy(x => x.Key.Examiner)
                        .Select(examinerGroup => new
                        {
                            examinerGroup.Key.Examiner,
                            examinerGroup.Key.AccountingReference,
                            TotalCost = examinerGroup.Sum(x => x.ExaminerCost),
                            Count = examinerGroup.Count(),
                            Markings = examinerGroup
                                .GroupBy(y => new { y.ProductSpecificationCode, CostPerUnit = y.ExaminerCost })
                                .OrderBy(x => x.Key.ProductSpecificationCode)
                                .Select(codeGroup =>
                                {
                                    var firstCode = codeGroup.First();
                                    return new
                                    {
                                        firstCode.ProductSpecificationCode,
                                        firstCode.GlCode,
                                        Count = codeGroup.Count(),
                                        firstCode.ExaminerCost,
                                        TotalCost = codeGroup.Sum(x => x.ExaminerCost),
                                        Tests = codeGroup.Select(x => new
                                        {
                                            x.JobExaminerId,
                                            x.Language,
                                            x.TestAttendanceId,
                                            x.ValidatedDate,
                                            x.ValidatedUser,
                                            x.Office,
                                            x.Supplementary
                                        })
                                    };
                                })
                        })
                });

            if (filter.from != null && filter.to != null)
            {
                vm = vm.Where(x => x.PayrollModifiedDate.GetValueOrDefault().Date <= filter.to &&
                                   x.PayrollModifiedDate.GetValueOrDefault().Date >= filter.from);
            }

            return this.CreateSearchResponse(PreviousResultLimit, () => vm);
        }

        private static object GetMarkingsForPayroll(IEnumerable<MarkingForPayrollModel> markings, bool includeTestsLevel)
        {
            return markings
                .GroupBy(x => new
                {
                    x.Examiner,
                    x.ExaminerPersonId,
                    x.ExaminerEntityId,
                    x.ExaminerNaatiNumber,
                    x.ExaminerAccountNumber,
                    x.AccountingReference
                })
                .OrderBy(x => x.Key.Examiner)
                .Select(examinerGroup => new
                {
                    examinerGroup.Key.Examiner,
                    examinerGroup.Key.ExaminerEntityId,
                    examinerGroup.Key.ExaminerNaatiNumber,
                    examinerGroup.Key.ExaminerAccountNumber,
                    examinerGroup.Key.AccountingReference,
                    TotalCost = examinerGroup.Sum(x => x.ExaminerCost),
                    Count = examinerGroup.Count(),
                    OldestReceivedDays = (DateTime.Today - examinerGroup.Min(r => r.ResultReceivedDate)).Days,
                    JobExaminerIds = examinerGroup.Select(j => j.JobExaminerId),
                    Markings = examinerGroup
                        .GroupBy(y => new { y.ProductSpecificationId, y.ProductSpecificationCode, CostPerUnit = y.ExaminerCost })
                        .OrderBy(x => x.Key.ProductSpecificationCode)
                        .Select(codeGroup =>
                        {
                            var first = codeGroup.First();
                            return new
                            {
                                first.ProductSpecificationId,
                                first.ProductSpecificationCode,
                                first.GlCode,
                                Count = codeGroup.Count(),
                                first.ExaminerCost,
                                TotalCost = codeGroup.Sum(x => x.ExaminerCost),
                                Tests = includeTestsLevel
                                        ? codeGroup.Select(x => new
                                        {
                                            x.JobExaminerId,
                                            x.Language,
                                            x.TestAttendanceId,
                                            x.ValidatedDate,
                                            x.ValidatedUser,
                                            x.Office,
                                            x.Supplementary
                                        })
                                        : null
                            };
                        })
                });
        }

        [HttpPost]
        [Route("validate")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.PayRun)]
        public HttpResponseMessage ValidateMarking([FromBody]IEnumerable<int> jobExaminerIds)
        {
            return this.CreateResponse(() => _payrollService.ValidateMarkingForPayroll(jobExaminerIds));
        }

        [HttpPost]
        [Route("createPayroll")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.PayRun)]
        public HttpResponseMessage CreatePayroll([FromBody] int[] jobExaminerIds)
        {
            return this.CreateResponse(() => _payrollService.CreatePayroll(jobExaminerIds));
        }

        [HttpPost]
        [Route("saveInvoiceNumber")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.PayRun)]
        public HttpResponseMessage SaveInvoiceNumber([FromBody] JobExaminerPayrollModel model)
        {
            return this.CreateResponse(_payrollService.AssignInvoiceNumber(model));
        }

        [HttpPost]
        [Route("removeExaminer")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.PayRun)]
        public HttpResponseMessage RemoveExaminer([FromBody] JobExaminerPayrollModel model)
        {
            return this.CreateResponse(_payrollService.RemoveExaminerFromPayroll(model));
        }

        [HttpPost]
        [Route("revertMarking")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.PayRun)]
        public HttpResponseMessage RevertMarking([FromBody]int jobExaminerId)
        {
            return this.CreateResponse(() => _payrollService.UnvalidateMarkingForPayroll(jobExaminerId));
        }

        [HttpPost]
        [Route("finalisePayroll")]
        [NcmsAuthorize(SecurityVerbName.Finalise, SecurityNounName.PayRun)]
        public HttpResponseMessage FinalisePayroll([FromBody] int payrollId)
        {
            return this.CreateResponse(() => _payrollService.FinalisePayroll(payrollId));
        }
    }
}
