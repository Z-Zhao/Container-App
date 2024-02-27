# script to call the api
$count = 0
while ($true) {
    $body = @{
        "{{param a}}" = ((Get-Random -Minimum -100 -Maximum 100.0) + 0.01 * $count)
        "{{param b}}" = ((Get-Random -Minimum -100 -Maximum 100.0) + 0.01 * $count)
        "{{param c}}" = ((Get-Random -Minimum 0.1 -Maximum 100.0) + 0.01 * $count)
    } 
    $baseuri = "https://containerappapi-app-feb2024.proudmushroom-2f7dc7fc.australiaeast.azurecontainerapps.io"
    $formula = ''
    $formula = [System.Web.HttpUtility]::UrlEncode("{{param a}}+{{param b}}/({{param c}}+{{param c}})")
    $response = $null
    $response = Invoke-WebRequest -Uri "$baseuri/formularesult?formula=$formula"   -ContentType 'application/json' -Method Post -Body ($body | ConvertTo-Json)
    Write-Output $response.Content
    Write-Output ($count = $count + 1)
    #Start-Sleep -Milliseconds 10
}
