using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class PractitionerSearchTransformer : IResultTransformer
    {
        
        internal IDictionary<int, HashSet<int>> PractitionersByCredentialTypeId { get; }
        internal IDictionary<int, HashSet<int>> PractitionersByCountryId { get; }
        internal IDictionary<int, HashSet<int>> PractitionersByStateId { get; }

        private readonly int mAddressIdIndex;
        private readonly int mCountryIdIndex;
        private readonly int mCountryIndex;
        private readonly int mDeceasedIndex;
        private readonly int mPhonesIndPdIndex;
        private readonly int mGivenNameIndex;
        private readonly int mOtherNamesIndex;
        private readonly int mPersonIdIndex;
        private readonly int mEmailsInPdIndex;
        private readonly int mPostcodeIndex;
        private readonly int mShowPhotoOnlineIndex;
        private readonly int mCredentialTypeIdIndex;
        private readonly int mCredentialTypeExternalNameIndex;
        private readonly int mCredentialTypeUpgradePathIndex;
        private readonly int mStateIndex;
        private readonly int mStateIdIndex;
        private readonly int mStreetDetailsIndex;
        private readonly int mSuburbIndex;
        private readonly int mSurnameIndex;
        private readonly int mTitleIndex;
        private readonly int mIncludeOdAddressIndex;
        private readonly int mIsPrimaryAddressIndex;
        private readonly int mLanguage1IdsIndex;
        private readonly int mLanguage2IdsIndex;
        private readonly int mDirectionIndex;
        private readonly int mNaatiNumberIndex;
        private readonly int mSkillIdIndex;
        private readonly int mWebsiteIndex;
        private readonly int mHash;

        public PractitionerSearchTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mPersonIdIndex = projectionIndices[nameof(PractitionerSearchDto.PersonId)];
            mTitleIndex = projectionIndices[nameof(PractitionerSearchDto.Title)];
            mGivenNameIndex = projectionIndices[nameof(PractitionerSearchDto.GivenName)];
            mOtherNamesIndex = projectionIndices[nameof(PractitionerSearchDto.OtherNames)];
            mSurnameIndex = projectionIndices[nameof(PractitionerSearchDto.Surname)];
            mCredentialTypeIdIndex = projectionIndices[nameof(PractitionerCredentialTypeDto.CredentialTypeId)];
            mCredentialTypeExternalNameIndex = projectionIndices[nameof(PractitionerCredentialTypeDto.ExternalName)];
            mCredentialTypeUpgradePathIndex = projectionIndices[nameof(PractitionerCredentialTypeDto.DisplayOrder)];
            mDirectionIndex = projectionIndices[nameof(PractitionerCredentialTypeDto.Direction)];
            mAddressIdIndex = projectionIndices[nameof(PractitionerAddressDto.AddressId)];
            mCountryIdIndex = projectionIndices[nameof(PractitionerAddressDto.CountryId)];
            mStreetDetailsIndex = projectionIndices[nameof(PractitionerAddressDto.StreetDetails)];
            mCountryIndex = projectionIndices[nameof(PractitionerAddressDto.Country)];
            mSuburbIndex = projectionIndices[nameof(PractitionerAddressDto.Suburb)];
            mStateIndex = projectionIndices[nameof(PractitionerAddressDto.State)];
            mStateIdIndex = projectionIndices[nameof(PractitionerAddressDto.StateId)];
            mPostcodeIndex = projectionIndices[nameof(PractitionerAddressDto.Postcode)];
            mIncludeOdAddressIndex = projectionIndices[nameof(PractitionerAddressDto.OdAddressVisibilityTypeId)];
            mIsPrimaryAddressIndex = projectionIndices[nameof(PractitionerAddressDto.IsPrimaryAddress)];
            mPhonesIndPdIndex = projectionIndices[nameof(PractitionerSearchDto.PhonesInPd)];
            mEmailsInPdIndex = projectionIndices[nameof(PractitionerSearchDto.EmailsInPd)];
            mShowPhotoOnlineIndex = projectionIndices[nameof(PractitionerSearchDto.ShowPhotoOnline)];
            mDeceasedIndex = projectionIndices[nameof(PractitionerSearchDto.Deceased)];
            mLanguage1IdsIndex = projectionIndices[nameof(PractitionerSearchDto.Language1Ids)];
            mLanguage2IdsIndex = projectionIndices[nameof(PractitionerSearchDto.Language2Ids)];
            mNaatiNumberIndex = projectionIndices[nameof(PractitionerSearchDto.NaatiNumber)];
            mSkillIdIndex = projectionIndices[nameof(PractitionerCredentialTypeDto.SkillId)];
            mWebsiteIndex = projectionIndices[nameof(PractitionerSearchDto.Website)];
            mHash = projectionIndices[nameof(PractitionerSearchDto.Hash)];

            PractitionersByCredentialTypeId = new Dictionary<int, HashSet<int>>();
            PractitionersByCountryId = new Dictionary<int, HashSet<int>>();
            PractitionersByStateId = new Dictionary<int, HashSet<int>>();
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var addressComparer = new PractitionerAddressDtoComparer();
            var credentialTypeComparer = new PractitionerCredentialTypeComparerBySkill();
            var personDictionary = new Dictionary<int, PractitionerSearchDto>();
            foreach (object[] tuple in collection)
            {
                PractitionerSearchDto practitioner;
                if (!personDictionary.TryGetValue((int) tuple[mPersonIdIndex], out practitioner))
                {
                    practitioner = new PractitionerSearchDto
                    {
                        PersonId = (int) tuple[mPersonIdIndex],
                        Title = tuple[mTitleIndex] as string,
                        GivenName = tuple[mGivenNameIndex] as string,
                        OtherNames = tuple[mOtherNamesIndex] as string,
                        Surname = tuple[mSurnameIndex] as string,
                        ShowPhotoOnline = (bool) tuple[mShowPhotoOnlineIndex],
                        Deceased = (bool) tuple[mDeceasedIndex],
                        NaatiNumber = (int) tuple[mNaatiNumberIndex],
                        Addresses = new HashSet<PractitionerAddressDto>(addressComparer),
                        PhonesInPd = new HashSet<string>(),
                        EmailsInPd = new HashSet<string>(),
                        CredentialTypes = new HashSet<PractitionerCredentialTypeDto>(credentialTypeComparer),
                        Language1Ids = new HashSet<int>(),
                        Language2Ids = new HashSet<int>(),
                        Website = tuple[mWebsiteIndex] as string,
                        Hash = (int)(tuple[mHash]??0)
                    };

                    personDictionary[practitioner.PersonId] = practitioner;
                }

                practitioner.Language1Ids.Add((int)tuple[mLanguage1IdsIndex]);
                practitioner.Language2Ids.Add((int)tuple[mLanguage2IdsIndex]);
                AddCredentialType(practitioner, tuple);
                AddPhone(practitioner, tuple);
                AddEmail(practitioner, tuple);
                AddAddress(practitioner, tuple);
            }

            return personDictionary.Values.ToList();
        }

        public void AddPhone(PractitionerSearchDto practitioner, object[] tuple)
        {
            var phone = tuple[mPhonesIndPdIndex] as string;
            if (!string.IsNullOrWhiteSpace(phone))
            {
                practitioner.PhonesInPd.Add(phone);
            }
        }

        public void AddEmail(PractitionerSearchDto practitioner, object[] tuple)
        {
            var email = tuple[mEmailsInPdIndex] as string;
            if (!string.IsNullOrWhiteSpace(email))
            {
                practitioner.EmailsInPd.Add(email);
            }
        }

        public void AddAddress(PractitionerSearchDto practitioner, object[] tuple)
        {
            if (tuple[mAddressIdIndex] == null)
                return;

            var address = new PractitionerAddressDto
            {
                AddressId = (int) tuple[mAddressIdIndex],
                CountryId = (int) tuple[mCountryIdIndex],
                StreetDetails = tuple[mStreetDetailsIndex] as string,
                Country = tuple[mCountryIndex] as string,
                Suburb = tuple[mSuburbIndex] as string,
                State = tuple[mStateIndex] as string,
                StateId = tuple[mStateIdIndex] as int?,
                Postcode = tuple[mPostcodeIndex] as string,
                OdAddressVisibilityTypeId = (int) tuple[mIncludeOdAddressIndex],
                IsPrimaryAddress = (bool) tuple[mIsPrimaryAddressIndex]
            };

            practitioner.Addresses.Add(address);
        }

        public void AddCredentialType(PractitionerSearchDto practitioner, object[] tuple)
        {
            var credentialType = new PractitionerCredentialTypeDto
            {
                CredentialTypeId = (int) tuple[mCredentialTypeIdIndex],
                ExternalName = tuple[mCredentialTypeExternalNameIndex] as string,
                DisplayOrder = (int)tuple[mCredentialTypeUpgradePathIndex],
                Direction = tuple[mDirectionIndex] as string,
                SkillId = (int)tuple[mSkillIdIndex],
            };

            practitioner.CredentialTypes.Add(credentialType);
        }
    }

    internal class PractitionerAddressDtoComparer : IEqualityComparer<PractitionerAddressDto>
    {
        public bool Equals(PractitionerAddressDto x, PractitionerAddressDto y)
        {
            return x?.AddressId == y?.AddressId;
        }

        public int GetHashCode(PractitionerAddressDto obj)
        {
           return obj.AddressId;
        }
    }

    internal class PractitionerCredentialTypeComparerBySkill : IEqualityComparer<PractitionerCredentialTypeDto>
    {
        public bool Equals(PractitionerCredentialTypeDto x, PractitionerCredentialTypeDto y)
        {
            return x?.SkillId == y?.SkillId;
        }

        public int GetHashCode(PractitionerCredentialTypeDto obj)
        {
            return obj.SkillId;
        }
    }
}