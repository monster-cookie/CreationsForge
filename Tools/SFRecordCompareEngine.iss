#define ApplicationName "Starfield Record Compare Engine"
#define ApplicationPublisher "Venpi"
#define ApplicationExecutable "SFRecordCompareEngine.exe"

[Setup]
AppId={{BB63D3B0-E9B9-4C14-BE6A-F656C644524F}
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
AppPublisher={#ApplicationPublisher}
DefaultDirName={localappdata}\Programs\SFRecordCompareEngine
DefaultGroupName={#ApplicationName}
DisableProgramGroupPage=yes
OutputDir={#OutputDirectory}
OutputBaseFilename=SFRecordCompareEngine-Setup-{#ApplicationVersion}
SetupIconFile={#SourceDirectory}\Resources\AppIcon\sfrecordcompareengine.ico
UninstallDisplayIcon={app}\Resources\AppIcon\sfrecordcompareengine.ico
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Files]
Source: "{#SourceDirectory}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#ApplicationName}"; Filename: "{app}\{#ApplicationExecutable}"
