
Get-ChildItem -Path "src" -Recurse -Filter "*.csproj" | ForEach-Object {
    (Get-Content $_.FullName).Replace('Version="10.0.1"', 'Version="9.0.0"') | Set-Content $_.FullName
}
write-host "Downgrade complete."
