using F1Solutions.Naati.Common.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ncms.Contracts;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.Extensions
{
    public static class ApiPermissions
    {
        public static ApiAdminSearchResultModel ToSearchResultModel(this ApiAdminSearchResultDto result)
        {
            return new ApiAdminSearchResultModel()
            {
                ApiAccessId = result.ApiAccessId,
                Active = !result.InActive,
                Name = result.Name,
                InstitutionId = result.InstitutionId,
                PrivateKey = result.PrivateKey,
                PublicKey = result.PublicKey,
                PermissionLabels = ApiPermissions.ToString(result.Permissions),
                Permissions = ApiPermissions.ToDictionary(result.Permissions),
                PermissionIds = ApiPermissions.ToPermissionListofInt(result.Permissions).ToList(),
                OrgNaatiNumber = result.OrgNaatiNumber
            };
        }

        public static string ToString(this int permissions)
        {
            var result = new StringBuilder();

            var permissionFlags = Enum.GetValues(typeof(EndpointPermission)).Cast<EndpointPermission>().ToList();

            foreach (var flag in permissionFlags)
            {
                var permissionEnabled = permissions & (int)flag;

                if (permissionEnabled != 0)
                {
                    var name = Enum.GetName(typeof(EndpointPermission),flag);
                    if (name == "None")
                    {
                        name += " (V1 Practitioners)";
                    }
                    result.AppendLine($"- {name}");
                }
            }

            return result.ToString();
        }

        public static Dictionary<string,bool> ToDictionary(this int permissions)
        {
            var result = new Dictionary<string, bool>();

            var permissionFlags = Enum.GetValues(typeof(EndpointPermission)).Cast<EndpointPermission>().ToList();

            foreach (var flag in permissionFlags)
            {
                var permissionEnabled = permissions & (int)flag;

                var name = Enum.GetName(typeof(EndpointPermission), flag);

                if (name == "None")
                {
                    name += " (V1 Practitioners)";
                }

                result.Add(name, permissionEnabled != 0);
            }

            return result;
        }

        public static IEnumerable<int> ToPermissionListofInt(this int permissions)
        {
            List<int> list = new List<int>();

            int remainder;
            string result = string.Empty;

            //put the valuie into string form
            while (permissions > 0)
            {
                remainder = permissions % 2;
                permissions /= 2;
                result = remainder.ToString() + result;
            }

            //take each part of the string, mask it, and then add the resaulting binary value to the list
            var offset = 1;
            foreach (char x in result)
            {
                if (x == '1')
                {
                    var stringVersion = $"1{new String('0', result.Length - offset)}";
                    var mask = Convert.ToInt32(stringVersion, 2);

                    list.Add(mask);
                }
                offset++;
            }

            return list;
        }

        public static int ToPermissionInt(this Dictionary<string, bool> permissionDictionary)
        {
            var result = 0;
            foreach (KeyValuePair<string, bool> permissionEntry in permissionDictionary)
            {                
                if (Enum.TryParse(permissionEntry.Key, out EndpointPermission permission))
                {
                    result = result + (int)permission;
                }
            }
            return result;
        }


        public static GenericResponse<IEnumerable<LookupTypeModel>> GetApiPermissions()
        {
            var result = new List<LookupTypeModel>();

            var permissionFlags = Enum.GetValues(typeof(EndpointPermission)).Cast<EndpointPermission>().ToList();

            foreach (var flag in permissionFlags)
            {
                var name = Enum.GetName(typeof(EndpointPermission), flag);

                if (name == "None")
                {
                    name += " (V1 Practitioners)";
                }
                result.Add(new LookupTypeModel() {Id =  (int)flag, DisplayName =  name });
            }

            return result;
        }


        /// <summary>
        /// these are duplicated in MyNaati.Contracts.BackOffice.IApiPublicService. Need to maintain both
        /// </summary>
        public enum EndpointPermission
        {
            None = 0x1,
            PractitionersCount = 0x2,
            Practitioners = 0x4,
            Lookups = 0x8,
            Languages = 0x10,
            LegacyAccreditations = 0x20,
            TestSessions = 0x40,
            EndorsedQualifications = 0x80,
            TestSessionAvailability = 0x100,
            Certifications = 0x200,
            PersonPhoto = 0x400
        }
    }
}
