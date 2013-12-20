; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Logboek"
#define MyAppVersion "1"
#define MyAppPublisher "GH"
#define MyAppExeName "logboek.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C30057B9-E885-4042-AF1E-ADB59D1BC90E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=X:\projects\logboek\logboek\install
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "X:\projects\logboek\logboek\logboek\bin\Release\logboek.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "X:\projects\logboek\logboek\logboek\bin\Release\7za.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "X:\projects\logboek\logboek\logboek\bin\Release\logboek.application"; DestDir: "{app}"; Flags: ignoreversion
Source: "X:\projects\logboek\logboek\logboek\bin\Release\logboek.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "X:\projects\logboek\logboek\logboek\bin\Release\logboek.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

