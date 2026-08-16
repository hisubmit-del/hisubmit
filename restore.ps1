$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$solution = Join-Path $projectRoot "HiSubmit.sln"

# Avoid waiting for online certificate-revocation services when packages are
# already available in the local NuGet cache.
$previousRevocationMode = $env:NUGET_CERT_REVOCATION_MODE
$env:NUGET_CERT_REVOCATION_MODE = "offline"

try {
    dotnet restore $solution `
        --ignore-failed-sources `
        --disable-parallel `
        -p:NuGetAudit=false `
        -p:NuGetAuditMode=direct `
        -p:SignatureValidationMode=accept `
        --nologo

    if ($LASTEXITCODE -ne 0) {
        throw "NuGet restore failed."
    }
}
finally {
    $env:NUGET_CERT_REVOCATION_MODE = $previousRevocationMode
}
