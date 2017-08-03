param([string]$configFile = "")

function Get-Int32OrDefault {
    param([Int32] $Value, [Int32] $default)

    if (!$Value) {
        return $default
    }

    return $Value
}

Function Get-Version {
    param(
        $configFile
    )
    $nuspecFileNotSpecified = [string]::IsNullOrEmpty($configFile)
    if ($nuspecFileNotSpecified) {
        $nuspecFilesInDirectory = @(Get-ChildItem *.nuspec)
        
        if ($nuspecFilesInDirectory.length -eq 0) {
            throw "Expecting nuspec file on this build"
        }
        elseif ($nuspecFilesInDirectory.Length -gt 1) {
            throw "Expecting only 1 nuspec file"
        }

        $configFile = $nuspecFilesInDirectory[0];
    }
    
    $xmlContent = ([xml](Get-Content $configFile));
    if($configFile -like "*.nuspec"){
        return $xmlContent.package.metadata.version
    } 
    
    return $xmlContent.Project.PropertyGroup.Version
}

Function Set-BuildNumbers {
    param(
        $configFile
    )
    $version = Get-Version -configFile $configFile
    $offset = Get-Int32OrDefault -Value $env:BUILD_OFFSET -default 0
    [int32]$buildCounter = $env:Build_BuildNumber
    Write-Host "Build Counter from VSTS is " $buildCounter
    Write-Host "Build Offset is " $offset
    Write-Host "Version From nuspec is " $version
    $counter = $buildCounter + $offset
    Write-Host  "The Build Counter has been set to " $counter
    $suffix = $buildSuffix
    Write-Host "Build Suffix Set to " $buildSuffix
    $hash = $env:BUILD_SOURCEVERSION 
    Write-Host "Git Hash read as "  $hash
    $versionParts = $version.split(".")
    $longVersion = "$($versionParts[0]).$($versionParts[1])." + "$counter" + "$buildSuffix" + "+" + "$hash"
    $shortVersion = "$($versionParts[0]).$($versionParts[1])." + "$counter" + "$buildSuffix"

    Write-Host "##vso[build.updatebuildnumber]$longVersion"
    Write-Host "The Long Version is "  "$longVersion"
    Write-Host "##vso[task.setvariable variable=ShortVersion;]$ShortVersion"
    Write-Host "Short Version is "  "$shortVersion"
    
    return @{
        "shortVersion" = $shortVersion
        "longVersion"  = $longVersion
    }
}

Set-BuildNumbers -configFile $configFile