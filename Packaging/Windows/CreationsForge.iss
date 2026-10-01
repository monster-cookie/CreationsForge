#ifndef ApplicationName
#define ApplicationName "Creations Forge"
#endif
#ifndef ApplicationPublisher
#define ApplicationPublisher "Venpi"
#endif
#ifndef InstallerIconFile
#define InstallerIconFile "..\..\CreationsForge.Workbench\Resources\AppIcon\CreationsForge.ico"
#endif
; Reserve CreationsForge.exe for the final GUI. It will launch this Workbench later.
#define DesktopExecutable "CreationsForgeWorkbench.exe"
[Setup]
AppId={{BB63D3B0-E9B9-4C14-BE6A-F656C644524F}
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
AppPublisher={#ApplicationPublisher}
DefaultDirName={localappdata}\Programs\CreationsForge
DefaultGroupName={#ApplicationName}
DisableProgramGroupPage=yes
OutputDir={#OutputDirectory}
OutputBaseFilename=CreationsForge-Setup-{#ApplicationVersion}
SetupIconFile={#InstallerIconFile}
UninstallDisplayIcon={app}\Desktop\{#DesktopExecutable}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Files]
Source: "{#DesktopSourceDirectory}\*"; DestDir: "{app}\Desktop"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#McpSourceDirectory}\*"; DestDir: "{app}\Mcp"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#ApplicationName}"; Filename: "{app}\Desktop\{#DesktopExecutable}"
