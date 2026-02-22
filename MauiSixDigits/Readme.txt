publish the application using the following command:

dotnet publish -c Release -f net8.0-windows10.0.19041.0 `
  -p:RuntimeIdentifier=win-x64 `
  -p:SelfContained=true `
  -p:PublishSingleFile=true `
  -p:PublishTrimmed=false


find the published application in the following directory:

explorer .\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\