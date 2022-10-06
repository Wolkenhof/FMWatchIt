; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "FMFuckIt"
#define MyAppVersion "1.0"
#define MyAppPublisher "valnoxy"
#define MyAppURL "https://valnoxy.dev"
#define MyAppExeName "FMFuckIt.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{9AC3B10C-177B-4E42-BBB2-8F0A9960D238}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\Users\jgu\Desktop\FMFuckIt-Setup
OutputBaseFilename=FMFuckIt_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\BouncyCastle.Crypto.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\FMFuckIt.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\MailKit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\Microsoft.Win32.SystemEvents.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\MimeKit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Configuration.ConfigurationManager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Diagnostics.EventLog.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Drawing.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Security.Cryptography.Pkcs.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Security.Cryptography.ProtectedData.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Security.Permissions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.ServiceProcess.ServiceController.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jgu\Documents\GitHub\CrackishHWID\FMFuckIt\FMFuckIt\bin\Debug\net6.0\System.Windows.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
