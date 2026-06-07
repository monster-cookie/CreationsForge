#ifndef ApplicationName
#define ApplicationName "Creations Forge"
#endif
#ifndef ApplicationPublisher
#define ApplicationPublisher "Venpi"
#endif
#define DesktopExecutable "CreationsForge.exe"
#define CliExecutable "CreationsForge.Console.exe"

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
SetupIconFile={#DesktopSourceDirectory}\Resources\AppIcon\CreationsForge.ico
UninstallDisplayIcon={app}\Desktop\Resources\AppIcon\CreationsForge.ico
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Files]
Source: "{#DesktopSourceDirectory}\*"; DestDir: "{app}\Desktop"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#CliSourceDirectory}\*"; DestDir: "{app}\Cli"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#ApplicationName}"; Filename: "{app}\Desktop\{#DesktopExecutable}"
Name: "{autoprograms}\{#ApplicationName} CLI"; Filename: "{cmd}"; Parameters: "/K ""{app}\Cli\{#CliExecutable}"""
