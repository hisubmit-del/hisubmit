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
        [string]$Expected = "2xx",
        [string]$Method = "Get"
    )

    $headers = @{}
    if (-not [string]::IsNullOrWhiteSpace($BearerToken)) {
        $headers.Authorization = "Bearer $BearerToken"
    }

    try {
        $requestArgs = @{
            Uri = "$base$Path"
            Headers = $headers
            Method = $Method
            UseBasicParsing = $true
        }
        if ($Method -in @("Post", "Put", "Patch")) {
            $requestArgs.ContentType = "application/json"
            $requestArgs.Body = "{}"
        }
        $response = Invoke-WebRequest @requestArgs
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
for ($attempt = 1; $attempt -le 10; $attempt++) {
    try {
        $probe = Invoke-WebRequest -Uri "$base/" -UseBasicParsing -TimeoutSec 2
        break
    }
    catch {
        if ($attempt -eq 10) {
            Write-Host "[FAIL] Local server did not become ready." -ForegroundColor Red
            exit 1
        }
        Start-Sleep -Seconds 1
    }
}

Test-Endpoint -Name "Public Gold pricing" -Path "/api/v1/Cart/SpecialAccountFee" -Expected "2xx"
Test-Endpoint -Name "Unauthenticated cart access is protected" -Path "/api/v1/Cart/GetAll" -Expected "401"
Test-Endpoint -Name "Unauthenticated Gold recommendations are protected" -Path "/api/v1/Project/GoldFestivalRecommendations" -Expected "401"
Test-Endpoint -Name "Unauthenticated project mutation is protected" -Path "/api/v1/Project/UpdateDetail" -Expected "401" -Method "Post"
Test-Endpoint -Name "Unauthenticated project specification is protected" -Path "/api/v1/ProjectSpecification/FilmSpecificationDetail" -Expected "401"
Test-Endpoint -Name "Unauthenticated festival payment information is protected" -Path "/api/v1/FestivalPayments/1/GetPaymentInformation" -Expected "401"
Test-Endpoint -Name "Unauthenticated festival files are protected" -Path "/api/v1/FestivalFile/1/GetAll" -Expected "401"

if ($failed) {
    exit 1
}

Write-Host "Smoke test completed." -ForegroundColor Cyan
