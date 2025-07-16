using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class TestSittingProfile : Profile
    {
        public TestSittingProfile()
        {
            CreateMap<TestSitting, CredentialRequestTestSessionDto>()
                .ForMember(x => x.CredentialTestSessionId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.TestSession.Name))
                .ForMember(x => x.TestSessionId, y => y.MapFrom(z => z.TestSession.Id))
                .ForMember(x => x.TestDate, y => y.MapFrom(z => z.TestSession.TestDateTime))
                .ForMember(x => x.Rejected, y => y.MapFrom(z => z.Rejected))
                .ForMember(x => x.Supplementary, y => y.MapFrom(z => z.Supplementary))
                .ForMember(x => x.Sat, y => y.MapFrom(z => z.Sat))
                .ForMember(x => x.Completed, y => y.MapFrom(z => z.TestSession.Completed))
                .ForMember(x => x.VenueName, y => y.MapFrom(z => z.TestSession.Venue.Name))
                .ForMember(x => x.HasAssets, y => y.MapFrom(z => Util.GetValueOrNull(z.TestStatus, w => w.HasAssets)))
                .ForMember(x => x.HasExaminers, y => y.MapFrom(z => Util.GetValueOrNull(z.TestStatus, w => w.HasExaminers)))
                .ForMember(x => x.HasPaidReviewExaminers, y => y.MapFrom(z => Util.GetValueOrNull(z.TestStatus, w => w.HasPaidReviewExaminers)))
                .ForMember(x => x.TestStatusTypeId, y => y.MapFrom(z => Util.GetValueOrNull(z.TestStatus, w => w.TestStatusType.Id)))
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.Id)))
                .ForMember(x => x.TestResultStatusId, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.ResultType.Id)))
                .ForMember(x => x.TotalTestResults, y => y.MapFrom(z => z.TestResults.Count))
                .ForMember(x => x.TestResultChecked, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.ResultChecked)))
                .ForMember(x => x.AllowIssue, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.AllowIssue)))
                .ForMember(x => x.MarkingSchemaTypeId, y => y.MapFrom(z => z.TestSpecification.MarkingSchemaType()))
                .ForMember(x => x.TestSpecificationId, y => y.MapFrom(z => z.TestSpecification.Id))
                .ForMember(x => x.EligibleForSupplementary, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.EligibleForSupplementary)))
                .ForMember(x => x.EligibleForConcededPass, y => y.MapFrom(z => Util.GetValueOrNull(z.TestResults.OrderByDescending(x => x.Id).FirstOrDefault(), w => w.EligibleForConcededPass)))
                .ForMember(x => x.AutomaticIssuing, y => y.MapFrom(z => z.TestSpecification.AutomaticIssuing))
                .ForMember(x => x.MaxScoreDifference, y => y.MapFrom(z => z.TestSpecification.MaxScoreDifference))
                .ForMember(x => x.TestLocation, y => y.Ignore())
                .ForMember(x => x.TestLocationState, y => y.Ignore())
                .ForMember(x => x.Materials, y => y.Ignore())
                .ForMember(x => x.ShowNonPreferredTestLocationInfo, y => y.Ignore());
        }
    }
}
