
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180730_ConvertTablesToIdentity
{

    [NaatiMigration(201807301601)]
    public class ConvertTablesToIdentity: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Resource.ConvertAddressToIdentity);
            Execute.Sql(Resource.ConvertCountryToIdentity);
            Execute.Sql(Resource.ConvertEFTMachineToIdentity);
            Execute.Sql(Resource.ConvertEmailToIdentity);
            Execute.Sql(Resource.ConvertEntityToIdentity);
            Execute.Sql(Resource.ConvertFailureReasonToIdentity);
            Execute.Sql(Resource.ConvertInstitutionNameToIdentity);
            Execute.Sql(Resource.ConvertInstitutionToIdentity);
            Execute.Sql(Resource.ConvertOfficeToIdentity);
            Execute.Sql(Resource.ConvertPanelMembershipToIdentity);
            Execute.Sql(Resource.ConvertPanelToIdentity);
            Execute.Sql(Resource.ConvertPanelTypeToIdentity);
           // Execute.Sql(Resource.ConvertPersonImageToIdentity);
            Execute.Sql(Resource.ConvertPersonNameToIdentity);
            Execute.Sql(Resource.ConvertPersonToIdentity);
            Execute.Sql(Resource.ConvertPhoneToIdentity);
            Execute.Sql(Resource.ConvertPoscodeToIdentity);
            Execute.Sql(Resource.ConvertResultTypeToIdentity);
            Execute.Sql(Resource.ConvertRoleScreenToIdentity);
            Execute.Sql(Resource.ConvertRoleToIdentity);
            Execute.Sql(Resource.ConvertRoleTypeToIdentity);
            Execute.Sql(Resource.ConvertStateToIdentity);
            Execute.Sql(Resource.ConvertSuburbToIdentity);

            Execute.Sql(Resource.ConvertTestComponentTypeToIdentity);
            Execute.Sql(Resource.ConvertTestFailureReasonToIdentity);
            Execute.Sql(Resource.ConvertUserRoleToIdentity);
            Execute.Sql(Resource.ConvertUserToIdentity);
            Execute.Sql(Resource.ConvertLanguageToIdentity);
            Execute.Sql(Resource.ConvertTitleToIdentity);
       
        }
    }
}
