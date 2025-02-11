#define MyAppName "ScreenSketcher"
#define MyAppVersion "0.0.1"
#define MyAppPublisher "J-E"
#define MyAppExeName "ScreenSketcher.exe"
#define MyAppIcon "..\src\ScreenSketcher\ScreenSketcher\Resources\Icons\Icon.ico"
#define MyAppId "com.je.screensketcher"

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=
OutputDir=.
OutputBaseFilename=ScreenSketcher_Setup_{#MyAppVersion}
SetupIconFile={#MyAppIcon}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; 
Name: "startup"; Description: "Start automatically at Windows startup"; GroupDescription: "Windows Startup";

[Files]
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\ColorPicker.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\MvvmHelpers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\Ookii.Dialogs.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\ScreenSketcher.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\ScreenSketcher.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\bin\Release\net8.0-windows\ScreenSketcher.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\ScreenSketcher\ScreenSketcher\Resources\Icons\Icon.ico"; DestDir: "{app}\Resources\Icons"; Flags: ignoreversion

[Dirs]
Name: "{app}\Resources"
Name: "{app}\Resources\Icons"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startup; Check: not IsAdminInstallMode
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startup; Check: IsAdminInstallMode

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent