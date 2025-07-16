using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestMaterialApplicantDtoTransformer: IResultTransformer
    {
        private readonly int mMaterialIdIndex;
        private readonly int mTestSittingIdIndex;
        private readonly int mPreviousSessionIdIndex;
        private readonly int mPreviousSessionNameIndex;
        private readonly int mPreviousSessionDateIndex;
        private readonly int mNaatiNumberIndex;
        private readonly int mNameIndex;
        private readonly int mPersonIdIndex;
        public TestMaterialApplicantDtoTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mMaterialIdIndex = projectionIndices[nameof(TestMaterialApplicantDto.ConflictingTestMaterialsIds)];
            mTestSittingIdIndex = projectionIndices[nameof(TestMaterialApplicantDto.TestSittingId)];
            mPreviousSessionIdIndex = projectionIndices[nameof(TestMaterialApplicantDto.PreviousTestSessionId)];
            mNaatiNumberIndex = projectionIndices[nameof(TestMaterialApplicantDto.NaatiNumber)];
            mNameIndex = projectionIndices[nameof(TestMaterialApplicantDto.Name)];
            mPreviousSessionDateIndex = projectionIndices[nameof(TestMaterialApplicantDto.PreviousTestSessionDate)];
            mPreviousSessionNameIndex = projectionIndices[nameof(TestMaterialApplicantDto.PreviousTestSessionName)];
            mPersonIdIndex = projectionIndices[nameof(TestMaterialApplicantDto.PersonId)];
        }
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var personDictionary = new Dictionary<int, TestMaterialApplicantDto>();
            foreach (object[] tuple in collection)
            {
                TestMaterialApplicantDto dto;
                if (!personDictionary.TryGetValue((int)tuple[mTestSittingIdIndex], out dto))
                {
                    dto = new TestMaterialApplicantDto
                    {
                        TestSittingId = (int)tuple[mTestSittingIdIndex],
                        NaatiNumber = (int)tuple[mNaatiNumberIndex],
                        ConflictingTestMaterialsIds = new HashSet<int>(),
                        PreviousTestSessionId = (int)tuple[mPreviousSessionIdIndex],
                        Name = tuple[mNameIndex] as string,
                        PreviousTestSessionDate =(DateTime) tuple[mPreviousSessionDateIndex],
                        PreviousTestSessionName = tuple[mPreviousSessionNameIndex] as string,
                        PersonId = (int)tuple[mPersonIdIndex]
                    };

                    personDictionary[dto.TestSittingId] = dto;
                }
                dto.ConflictingTestMaterialsIds.Add((int)tuple[mMaterialIdIndex]);
                
            }

            return personDictionary.Values.ToList();
        }
    }
}
