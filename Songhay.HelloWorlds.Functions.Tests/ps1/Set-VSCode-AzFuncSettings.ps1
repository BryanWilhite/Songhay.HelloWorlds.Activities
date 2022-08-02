$settingsName = $args[0]

if([System.String]::IsNullOrWhiteSpace($settingsName)) {
    $settingsName = "C#"
}

Set-Location $PSScriptRoot

$settingsFile = Resolve-Path -Path "..\..\.vscode\settings.json"

if (-not(Test-Path $settingsFile)) {
    Write-Warning "Cannot find file $settingsFile. Exiting script."
    exit
}

$settings = Get-Content -Path $settingsFile | ConvertFrom-Json

$settingsForCSharp =
@{
    deploySubpath = "Songhay.HelloWorlds.Functions/bin/Release/net6.0/publish";
    projectLanguage = "C#";
    projectSubpath = "Songhay.HelloWorlds.Functions";
}

$settingsForPowerShell =
@{
    deploySubpath = ".";
    projectLanguage = "PowerShell";
    projectSubpath = "Songhay.HelloWorlds.IO.Functions";
}

switch ($settingsName)
{
    "C#" {
        $settings."azureFunctions.deploySubpath" = $settingsForCSharp.deploySubpath
        $settings."azureFunctions.projectLanguage" = $settingsForCSharp.projectLanguage
        $settings."azureFunctions.projectSubpath" = $settingsForCSharp.projectSubpath
    }

    "PowerShell" {
        $settings."azureFunctions.deploySubpath" = $settingsForPowerShell.deploySubpath
        $settings."azureFunctions.projectLanguage" = $settingsForPowerShell.projectLanguage
        $settings."azureFunctions.projectSubpath" = $settingsForPowerShell.projectSubpath
    }
}

$settings | ConvertTo-Json -Depth 1 | Out-File $settingsFile
