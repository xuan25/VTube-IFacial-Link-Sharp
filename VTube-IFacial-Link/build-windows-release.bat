::msbuild /restore /t:Publish /p:TargetFramework=net6.0-windows10.0.19041.0 /p:configuration=release /p:WindowsAppSDKSelfContained=true /p:Platform=x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:WindowsPackageType=None /p:RuntimeIdentifier=win10-x64

::dotnet publish -c Release -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true -p:Platform=x64 -p:TargetFramework=net6.0-windows10.0.19041.0 -p:PublishReadyToRun=true -p:PublishTrimmed=true -p:PublishSingleFile=true

dotnet publish -c Release -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true -p:Platform=x64 -p:TargetFramework=net6.0-windows10.0.19041.0 -p:PublishReadyToRun=true
