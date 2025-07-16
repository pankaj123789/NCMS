using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class ApiPublicPractitionerSearchTransformer : IResultTransformer
    {
        private readonly int mAddressIdIndex;
        private readonly int mCountryIndex;
        private readonly int mDeceasedIndex;
        private readonly int mPhonesIndPdIndex;
        private readonly int mGivenNameIndex;
        private readonly int mOtherNamesIndex;
        private readonly int mPersonIdIndex;
        private readonly int mEmailsInPdIndex;
        private readonly int mPostcodeIndex;
        
        private readonly int mCredentialTypeExternalNameIndex;
        private readonly int mCredentialTypeUpgradePathIndex;
        private readonly int mStateIndex;
        
        private readonly int mStreetDetailsIndex;
        private readonly int mSuburbIndex;
        private readonly int mSurnameIndex;
        private readonly int mTitleIndex;
        private readonly int mIncludeOdAddressIndex;
        private readonly int mIsPrimaryAddressIndex;
        
        private readonly int mSkillIdIndex;
        private readonly int mSkillIndex;

        //private readonly int mShowPhotoOnlineIndex;
        //private readonly int mStateIdIndex;
        //private readonly int mLanguage1IdsIndex;
        //private readonly int mLanguage2IdsIndex;
        //private readonly int mDirectionIndex;
        //private readonly int mNaatiNumberIndex;
        //private readonly int mCredentialTypeIdIndex;
        private readonly int mWebsiteIndex;
        //private readonly int mHash;
        internal ApiPublicPractitionerSearchTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mPersonIdIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.PersonId)];
            mTitleIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.Title)];
            mGivenNameIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.GivenName)];
            mOtherNamesIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.OtherNames)];
            mSurnameIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.Surname)];
            mCredentialTypeExternalNameIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.ExternalName)];
            mCredentialTypeUpgradePathIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.DisplayOrder)];
            mStreetDetailsIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.StreetDetails)];
            mCountryIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.Country)];
            mSuburbIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.Suburb)];
            mStateIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.State)];
            mPostcodeIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.Postcode)];
            mIncludeOdAddressIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.OdAddressVisibilityTypeId)];
            mIsPrimaryAddressIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.IsPrimaryAddress)];
            mPhonesIndPdIndex = projectionIndices[nameof(ApiPublicContactDetailsDto.PhonesInPd)];
            mWebsiteIndex = projectionIndices[nameof(ApiPublicContactDetailsDto.WebsiteUrlInPd)];
            mEmailsInPdIndex = projectionIndices[nameof(ApiPublicContactDetailsDto.EmailsInPd)];
            mDeceasedIndex = projectionIndices[nameof(ApiPublicPractitionerSearchDto.Deceased)];
            mSkillIdIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.SkillId)];
            mSkillIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.Skill)];
            mAddressIdIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.AddressId)];

            //mCredentialTypeIdIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.CredentialTypeId)];
            //mDirectionIndex = projectionIndices[nameof(ApiPubicPractitionerCredentialTypeDto.Direction)];
            //mCountryIdIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.CountryId)];
            //mStateIdIndex = projectionIndices[nameof(ApiPublicPractitionerAddressDto.StateId)];
            //mShowPhotoOnlineIndex = projectionIndices[nameof(ApiPubicPractitionerSearchDto.ShowPhotoOnline)];
            //mLanguage1IdsIndex = projectionIndices[nameof(ApiPubicPractitionerSearchDto.Language1Ids)];
            //mLanguage2IdsIndex = projectionIndices[nameof(ApiPubicPractitionerSearchDto.Language2Ids)];
            //mNaatiNumberIndex = projectionIndices[nameof(ApiPubicPractitionerSearchDto.NaatiNumber)];
            //mWebsiteIndex = projectionIndices[nameof(ApiPubicPractitionerSearchDto.Website)];
            //mHash = projectionIndices[nameof(ApiPubicPractitionerSearchDto.Hash)];
        }
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var credentialTypeComparer = new ApiPublicPractitionerCredentialTypeComparerBySkill();
            var contactDetailsComparer = new ApiPublicContactDetailsComparer();
            var personDictionary = new Dictionary<int, ApiPublicPractitionerSearchDto>();
            foreach (object[] tuple in collection)
            {
                if (!personDictionary.TryGetValue((int)tuple[mPersonIdIndex], out ApiPublicPractitionerSearchDto practitioner))
                {
                    practitioner = new ApiPublicPractitionerSearchDto
                    {
                        PersonId = (int)tuple[mPersonIdIndex],
                        Deceased = (bool)tuple[mDeceasedIndex],
                        Surname = tuple[mSurnameIndex] as string,
                        GivenName = tuple[mGivenNameIndex] as string,
                        OtherNames = tuple[mOtherNamesIndex] as string,
                        Title = tuple[mTitleIndex] as string,
                        CredentialTypes = new HashSet<ApiPubicPractitionerCredentialTypeDto>(credentialTypeComparer),
                        Address = new ApiPublicPractitionerAddressDto(),
                        ContactDetails = new HashSet<ApiPublicContactDetailsDto>(contactDetailsComparer)
                    };

                    personDictionary[practitioner.PersonId] = practitioner;
                }

                AddCredentialType(practitioner, tuple);
                AddAddress(practitioner, tuple);
                AddContactDetails(practitioner, tuple);
            }
            return personDictionary.Values.ToList();
        }

        public void AddCredentialType(ApiPublicPractitionerSearchDto practitioner, object[] tuple)
        {
            var credentialType = new ApiPubicPractitionerCredentialTypeDto
            {
                ExternalName = tuple[mCredentialTypeExternalNameIndex] as string,
                SkillId = (int)tuple[mSkillIdIndex],
                DisplayOrder = (int)tuple[mCredentialTypeUpgradePathIndex],
                Skill = tuple[mSkillIndex] as string,

            };
            practitioner.CredentialTypes.Add(credentialType);
        }
        public void AddContactDetails(ApiPublicPractitionerSearchDto practitioner, object[] tuple)
        {
            var phone = tuple[mPhonesIndPdIndex] as string;

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var contactDetail = new ApiPublicContactDetailsDto
                {
                    Type = "Phone",
                    Contact = phone.Trim()
                };
                practitioner.ContactDetails.Add(contactDetail);
            }

            var email = tuple[mEmailsInPdIndex] as string;

            if (!string.IsNullOrWhiteSpace(email))
            {
                var contactDetail = new ApiPublicContactDetailsDto
                {
                    Type = "Email",
                    Contact = email?.Trim()
                };
                practitioner.ContactDetails.Add(contactDetail);
            }

            var webUrl = tuple[mWebsiteIndex] as string;

            if (!string.IsNullOrWhiteSpace(webUrl))
            {
                var contactDetail = new ApiPublicContactDetailsDto
                {
                    Type = "WebsiteUrl",
                    Contact = webUrl?.Trim()
                };
                practitioner.ContactDetails.Add(contactDetail);
            }
        }

        public void AddAddress(ApiPublicPractitionerSearchDto practitioner, object[] tuple)
        {
            if (tuple[mAddressIdIndex] == null)
            {
                return;
            }  

            var visibilityTypeId = (int) tuple[mIncludeOdAddressIndex];
            var isPrimaryAddress = (bool)tuple[mIsPrimaryAddressIndex];

            if (practitioner.Address.Country != null  && !isPrimaryAddress)
            {
                return;
            }

            practitioner.Address.Country = string.Empty;
            practitioner.Address.State = string.Empty;
            practitioner.Address.Suburb = string.Empty;
            practitioner.Address.Postcode = string.Empty;
            practitioner.Address.StreetDetails = string.Empty;

            if (visibilityTypeId == (int)OdAddressVisibilityTypeName.DoNotShow)
            {
                return;
            }

            if (visibilityTypeId == (int)OdAddressVisibilityTypeName.StateOnly)
            {
                practitioner.Address.Country = tuple[mCountryIndex] as string;
                practitioner.Address.State = tuple[mStateIndex] as string;
            }
            else if (visibilityTypeId == (int)OdAddressVisibilityTypeName.StateAndSuburb)
            {
                practitioner.Address.Country = tuple[mCountryIndex] as string;
                practitioner.Address.State = tuple[mStateIndex] as string;
                practitioner.Address.Suburb = tuple[mSuburbIndex] as string;
                practitioner.Address.Postcode = tuple[mPostcodeIndex] as string;
            }
            else if (visibilityTypeId == (int)OdAddressVisibilityTypeName.FullAddress)
            {
                practitioner.Address.Country = tuple[mCountryIndex] as string;
                practitioner.Address.State = tuple[mStateIndex] as string;
                practitioner.Address.Suburb = tuple[mSuburbIndex] as string;
                practitioner.Address.Postcode = tuple[mPostcodeIndex] as string;
                practitioner.Address.StreetDetails = tuple[mStreetDetailsIndex] as string;
            }
            
            //Need to remove
            practitioner.Address.AddressId = int.Parse(tuple[mAddressIdIndex].ToString());
            practitioner.Address.OdAddressVisibilityTypeId = int.Parse(tuple[mIncludeOdAddressIndex].ToString());
            practitioner.Address.IsPrimaryAddress = (bool) tuple[mIsPrimaryAddressIndex];
        }


    }

    internal class ApiPublicPractitionerCredentialTypeComparerBySkill : IEqualityComparer<ApiPubicPractitionerCredentialTypeDto>
    {
        public bool Equals(ApiPubicPractitionerCredentialTypeDto x, ApiPubicPractitionerCredentialTypeDto y)
        {
            return x?.SkillId == y?.SkillId;
        }

        public int GetHashCode(ApiPubicPractitionerCredentialTypeDto obj)
        {
            return obj.SkillId;
        }
    }

    internal class ApiPublicContactDetailsComparer : IEqualityComparer<ApiPublicContactDetailsDto>
    {
        public bool Equals(ApiPublicContactDetailsDto x, ApiPublicContactDetailsDto y)
        {
            return $"{x.Type ?? string.Empty} {x.Contact ?? string.Empty}" == $"{y.Type ?? string.Empty} {y.Contact ?? string.Empty}";
        }

        public int GetHashCode(ApiPublicContactDetailsDto obj)
        {
            return $"{obj.Type ?? string.Empty} {obj.Contact ?? string.Empty}".GetHashCode();
        }
    }
}
