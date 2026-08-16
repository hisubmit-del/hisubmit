# HiSubmit deployment

The production database password is intentionally not stored in the repository.
The application reads the normal .NET configuration key
`ConnectionStrings__DefaultConnection`, which overrides the value in
`Web/appsettings.json`.

On the production server, configure the environment variable (or the hosting
panel's equivalent application setting) with the complete SQL Server connection
string:

```text
ConnectionStrings__DefaultConnection=Server=31.25.90.195\MSSQLSERVER2022;Initial Catalog=hisubmi1_MainDb;User Id=hisubmi1_user;Password=<production-password>;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Do not put the real password in `appsettings.Production.template.json`, GitHub
Actions logs, commits, or pull requests. After publishing, verify the
application setting is present before starting the site.

The release candidate was publish-tested on 2026-08-16 with the local
`HiSubmitDB50` database. The published output returned HTTP 200 for `/`,
`/faq`, `/terms`, and the public festival deadline endpoint. The production
database credential still requires a secure host-side configuration step.

## Publish

```powershell
dotnet publish Web\Web.csproj --configuration Release --output .\publish
```

Copy the contents of `publish` to the production application directory and
restart the application pool/service. The runtime-created `Logs` and `Files`
directories must remain writable by the application identity.

## Send changes to GitHub

From the repository root:

```powershell
git status
git add .
git commit -m "Fix runtime errors and public navigation"
git push origin main
```

If the default branch is `master`, replace `main` with `master`. Review
`git status` before committing so deleted legacy folders and generated files
are included only when they are intentional.
