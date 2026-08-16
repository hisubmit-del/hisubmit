param(
    [ValidateSet("http", "https")]
    [string]$Profile = "http"
)

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$webRoot = Join-Path $projectRoot "Web"
$webProject = Join-Path $webRoot "Web.csproj"

Write-Host "Building Web without contacting NuGet..."
dotnet build $webProject --no-restore --nologo
if ($LASTEXITCODE -ne 0) {
    throw "Web build failed."
}

Push-Location $webRoot
try {
    $previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    try {
        dotnet run --no-build --no-restore --launch-profile $Profile
    }
    finally {
        $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    }
}
finally {
    Pop-Location
}
