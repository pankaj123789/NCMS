using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class ApplicationSearchTransformer : IResultTransformer
    {
        private readonly int mApplicationReferenceIndex;
        private readonly int mApplicationTypeIndex;
        private readonly int mApplicationTypeIdIndex;
        private readonly int mApplicationStatusIndex;
        private readonly int mCredentialApplicationIndex;
        private readonly int mNaatiNumberIndex;
        private readonly int mNameIndex;
        private readonly int mPrimaryContactNumberIndex;
        private readonly int mApplicationOwnerIndex;
        private readonly int mStatusChangeDateIndex;
        private readonly int mSponsorNameIndex;
        private readonly int mSponsorNaatiNumberIndex;
        private readonly int mEnteredDateIndex;
        private readonly int mPreferredTestLocationId;
        private readonly int mDisplayBillsIndex;
        private readonly int mAutoCreatedIndex;

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var applicationDictionary = new Dictionary<int, ApplicationSearchDto>();
            foreach (object[] tuple in collection)
            {
                ApplicationSearchDto applicationDto;
                if (!applicationDictionary.TryGetValue((int)tuple[mCredentialApplicationIndex], out applicationDto))
                {
                    applicationDto = new ApplicationSearchDto
                    {
                        Id = (int)tuple[mCredentialApplicationIndex],
                        Name = tuple[mNameIndex] == null ? "" : tuple[mNameIndex] as string,
                        ApplicationReference = tuple[mApplicationReferenceIndex] as string,
                        ApplicationType = tuple[mApplicationTypeIndex] as string,
                        ApplicationTypeId = (int)tuple[mApplicationTypeIdIndex],
                        NaatiNumber = tuple[mNaatiNumberIndex] == null ? 0 : (int)tuple[mNaatiNumberIndex],
                        ApplicationStatus = tuple[mApplicationStatusIndex] as string,
                        StatusChangeDate = (DateTime)tuple[mStatusChangeDateIndex],
                        ApplicationOwner = tuple[mApplicationOwnerIndex] as string,
                        SponsorName = tuple[mSponsorNameIndex] as string,
                        SponsorNaatiNumber = tuple[mSponsorNaatiNumberIndex] as int?,
                        EnteredDate = (DateTime)tuple[mEnteredDateIndex],
                        PreferredTestLocationId = tuple[mPreferredTestLocationId] as int?,
                        DisplayBills = (bool)tuple[mDisplayBillsIndex],
                        AutoCreated = (bool?)tuple[mAutoCreatedIndex]
                    };

                    applicationDictionary[applicationDto.Id] = applicationDto;
                }

                SetPrimaryContactNumber(applicationDto, tuple);
            }

            return applicationDictionary.Values.ToList();
        }

        public ApplicationSearchTransformer(IDictionary<string, int> projectionIndices)
        {
            mApplicationReferenceIndex = projectionIndices[nameof(ApplicationSearchDto.ApplicationReference)];
            mApplicationTypeIndex = projectionIndices[nameof(ApplicationSearchDto.ApplicationType)];
            mApplicationTypeIdIndex = projectionIndices[nameof(ApplicationSearchDto.ApplicationTypeId)];
            mApplicationStatusIndex = projectionIndices[nameof(ApplicationSearchDto.ApplicationStatus)];
            mCredentialApplicationIndex = projectionIndices[nameof(ApplicationSearchDto.Id)];
            mNaatiNumberIndex = projectionIndices[nameof(ApplicationSearchDto.NaatiNumber)];
            mNameIndex = projectionIndices[nameof(ApplicationSearchDto.Name)];
            mPrimaryContactNumberIndex = projectionIndices[nameof(ApplicationSearchDto.PrimaryContactNumber)];
            mStatusChangeDateIndex = projectionIndices[nameof(ApplicationSearchDto.StatusChangeDate)];
            mApplicationOwnerIndex = projectionIndices[nameof(ApplicationSearchDto.ApplicationOwner)];
            mSponsorNameIndex = projectionIndices[nameof(ApplicationSearchDto.SponsorName)];
            mSponsorNaatiNumberIndex = projectionIndices[nameof(ApplicationSearchDto.SponsorNaatiNumber)];
            mEnteredDateIndex = projectionIndices[nameof(ApplicationSearchDto.EnteredDate)];
            mPreferredTestLocationId = projectionIndices[nameof(ApplicationSearchDto.PreferredTestLocationId)];
            mDisplayBillsIndex = projectionIndices[nameof(ApplicationSearchDto.DisplayBills)];
            mAutoCreatedIndex = projectionIndices[nameof(ApplicationSearchDto.AutoCreated)];
        }

        private void SetPrimaryContactNumber(ApplicationSearchDto application, object[] tuple)
        {
            var primaryNumber = tuple[mPrimaryContactNumberIndex];
            if (primaryNumber != null)
            {
                application.PrimaryContactNumber = primaryNumber as string;
            }
        }
    }
}
