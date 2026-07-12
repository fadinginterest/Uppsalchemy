param($ProjectDir, $TargetPath)

try {
    Add-Type -AssemblyName System.IO.Compression -ErrorAction Stop
    Add-Type -AssemblyName System.IO.Compression.FileSystem -ErrorAction Stop
}
catch {
    Write-Error "Failed to load compression assemblies: $_"
    exit 1
}

$modinfo = Get-Content "$ProjectDir\modinfo.json" | ConvertFrom-Json
$version = $modinfo.version
$modid = $modinfo.modid
$releasesDir = "$ProjectDir\Releases"
$zipPath = "$releasesDir\${modid}_${version}.zip"
if (!(Test-Path $releasesDir)) { New-Item -ItemType Directory -Path $releasesDir | Out-Null }
if (Test-Path $zipPath) { Remove-Item $zipPath }

try {
    $zip = [System.IO.Compression.ZipFile]::Open($zipPath, [System.IO.Compression.ZipArchiveMode]::Create)
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, "$ProjectDir\modinfo.json", "modinfo.json") | Out-Null
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $TargetPath, [System.IO.Path]::GetFileName($TargetPath)) | Out-Null
    Get-ChildItem -Path "$ProjectDir\assets" -Recurse -File | ForEach-Object {
        $entryName = ($_.FullName.Substring($ProjectDir.Length).TrimStart('\') -replace '\\','/')
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $_.FullName, $entryName) | Out-Null
    }
}
catch {
    Write-Error "Packaging failed: $_"
    exit 1
}
finally {
    if ($zip) { $zip.Dispose() }
}

Write-Host "Packaged: $zipPath"

$modsFolder = "$env:APPDATA\VintagestoryData\Mods"
Get-ChildItem -Path $modsFolder -Filter "${modid}_*.zip" -ErrorAction SilentlyContinue | Remove-Item -Force
Copy-Item -Path $zipPath -Destination $modsFolder -Force
Write-Host "Copied to mods folder: $modsFolder"