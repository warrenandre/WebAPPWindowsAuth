# Windows Authentication to SQL Server on .NET 8 Container App

This guide provides instructions on how to set up Windows Authentication to SQL Server using a .NET 8 container app running on a Red Hat server.

## Prerequisites

- Red Hat Enterprise Linux (RHEL) server
- Docker installed on RHEL
- SQL Server installed and configured
- .NET 8 SDK installed
- Keytab file for Kerberos authentication

## Steps

1. **Set Up SQL Server**: Ensure SQL Server is installed and configured on your Red Hat server[_{{{CITATION{{{_1{cs12dotnet8/docs/sql-server/README.md at main - GitHub](https://github.com/markjprice/cs12dotnet8/blob/main/docs/sql-server/README.md).

2. **Create a SQL Server Container**: Create a Docker container for SQL Server[_{{{CITATION{{{_2{labs/windows/sql-server/README.md at master - GitHub](https://github.com/docker/labs/blob/master/windows/sql-server/README.md).
   ```sh
   docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrongPassword' -p 1433:1433 --name sql_server_container -d mcr.microsoft.com/mssql/server:latest

3. **Copy Keytab File to Container**: Copy the keytab file to the SQL Server container.
   ```sh
    docker cp your-keytab-file.keytab sql_server_container:/tmp/your-keytab-file.keytab

4. **Configure Kerberos**: Set the necessary environment variables on the Redhat server for Kerberos authentication.

    docker exec -it sql_server_container /bin/bash
    export KRB5CCNAME=FILE:/tmp/krb5cc
    export KRB5_KDC_PROFILE=/etc/krb5.conf
    export KRB5_CONFIG=/etc/krb5.conf
    kinit -k -t /tmp/your-keytab-file.keytab your_username@YOUR.DOMAIN.COM

5. **Create a .Net 8 App**: Create a .NET 8 app that uses Windows Authentication to connect to SQL Server

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