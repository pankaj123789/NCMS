--After a UAT DB refresh, run this script after the app is deployed, and pods are running
update tblSystemValue set Value='' where ValueKey='SecurePayAccessToken'