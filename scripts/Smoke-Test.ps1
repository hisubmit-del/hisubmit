param(
    [string]$BaseUrl = "http://localhost:5120",
    [string]$BearerToken = ""
)

$ErrorActionPreference = "Stop"
$base = $BaseUrl.TrimEnd("/")

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Expected = "2xx"
    )

    $headers = @{}
    if (-not [string]::IsNullOrWhiteSpace($BearerToken)) {
        $headers.Authorization = "Bearer $BearerToken"
    }

    try {
        $response = Invoke-WebRequest -Uri "$base$Path" -Headers $headers -UseBasicParsing
        $status = [int]$response.StatusCode
        if ($Expected -eq "2xx" -and ($status -lt 200 -or $status -ge 300)) {
            throw "Expected 2xx but received $status"
        }
        Write-Host "[PASS] $Name ($status)" -ForegroundColor Green
    }
    catch {
        $status = $_.Exception.Response.StatusCode.value__
        if ($Expected -eq "401" -and $status -eq 401) {
            Write-Host "[PASS] $Name (401 as expected)" -ForegroundColor Green
            return
        }
        Write-Host "[FAIL] $Name ($($_.Exception.Message))" -ForegroundColor Red
        $script:failed = $true
    }
}

$failed = $false
Test-Endpoint -Name "Public Gold pricing" -Path "/api/v1/Cart/SpecialAccountFee" -Expected "2xx"
Test-Endpoint -Name "Unauthenticated cart access is protected" -Path "/api/v1/Cart/GetAll" -Expected "401"
Test-Endpoint -Name "Unauthenticated Gold recommendations are protected" -Path "/api/v1/Project/GoldFestivalRecommendations" -Expected "401"

if ($failed) {
    exit 1
}

Write-Host "Smoke test completed." -ForegroundColor Cyan
