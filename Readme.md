# Windows Authentication to SQL Server on .NET 8 Container App

This guide provides instructions on how to set up kerberos Windows Authentication to SQL Server using a .NET 8 container app running on a Red Hat server.

## Prerequisites

- Red Hat Enterprise Linux (RHEL) server
- Docker installed on RHEL
- SQL Server installed and configured
- .NET 8 SDK installed
- Keytab file for Kerberos authentication

## Steps


1. **Set Up SQL Server**: Ensure SQL Server is installed and configured.

2. ** Create SPN for SQL Server**: Create a Service Principal Name (SPN) for the SQL Server instance.

```sh
setspn -S MSSQLSvc/your-sql-server-hostname:1433 your_domain\your_sql_server_account
```

3. **Domain join Redhat Server to AD**: Join the Redhat server to the Active Directory domain.

```sh
#packaged needed to domain join redhat server to windows domain
yum install realmd oddjob oddjob-mkhomedir sssd adcli samba-common-tools krb5-workstation openldap-clients policycoreutils-python -y
realm join --
```

**https://learn.microsoft.com/en-us/azure-data-studio/enable-kerberos?view=sql-server-2017&tabs=ubuntu#join-your-os-to-the-active-directory-domain-controller&preserve-view=true**

4. **Test Domain integration on Redhat Server using kinit accountName@DOMAIN.CCOM**: Test the domain integration by running the kinit command with your account name.

```sh
kinit
```



5. **Configure Kerberos**: Set the necessary environment variables on the Redhat server for Kerberos authentication.

    
    export KRB5CCNAME=FILE:/tmp/krb5cc
    export KRB5_KDC_PROFILE=/etc/krb5.conf
    export KRB5_CONFIG=/etc/krb5.conf
    kinit your_username@YOUR.DOMAIN.COM

6. **Confirm kerberos ticket**: Confirm that the Kerberos ticket has been created successfully and the cachec location matches what was configured above.

```sh
klist
```


7. **Create a .Net 8 App**: Create a .NET 8 app that uses Windows Authentication to connect to SQL Server

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate(options =>
            {
                options.KerberosServicePrincipalName = "HTTP/your-container-hostname";
            });

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}

6. **Run the Container**: Run the .NET 8 container app with the necessary mounts and environment variables

docker run --rm -v /tmp/krb5cc:/tmp/krb5cc -e KRB5CCNAME=FILE:/tmp/krb5cc -p 8080:80 your_image_name


roubleshooting
Ensure the keytab file is correctly copied to the container.

Verify the Kerberos environment variables are set correctly.

Check the SQL Server container logs for any errors.

Feel free to customize this guide based on your specific requirements. If you have any questions or need further assistance, please let me know!