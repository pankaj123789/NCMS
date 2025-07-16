Sanitisation instructions-
The running order of the whole process is:

1. Run MoveRefreshDB_ProdtoUAT.ps1 to use the restore function to get the last Prod backup to create the NCMS_UATRefresh DB
2. Run the UAT_Sanitisation_Script.sql
3. helm uninstall
4. We can then run the UAT_Sanitise_Securepay_Token.sql on the NCMS_UATRefresh DB, though the script has instructions to run script after deployment but this isnt necessary
5. Rename DBs
6. Deploy from Octopus
7. Run UAT_Sanitise_Securepay_Token.sql
8. Update User Mappings (ncuatdevadmin and bruce.mcleod)