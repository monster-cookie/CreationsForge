CREATE TABLE Global (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Data REAL NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Global_FormKey_ID ON Global (FormKey_ID);

CREATE TABLE MiscObject (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, ShortName TEXT NULL, Value INTEGER NULL, Weight REAL NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_MiscObject_FormKey_ID ON MiscObject (FormKey_ID);

CREATE TABLE Keyword (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, Color TEXT NOT NULL, Type TEXT NOT NULL, Notes TEXT NULL, FlashLinkageName TEXT NULL, AttractionRuleFormKey TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Keyword_FormKey_ID ON Keyword (FormKey_ID);

CREATE TABLE NPC (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, ShortName TEXT NULL, LongName TEXT NULL, DispositionBase INTEGER NOT NULL, Aggression TEXT NOT NULL, Confidence TEXT NOT NULL, EnergyLevel INTEGER NOT NULL, Responsibility TEXT NOT NULL, Assistance TEXT NOT NULL, GearedUpWeapons INTEGER NOT NULL, HeightMin REAL NOT NULL, HeightMax REAL NOT NULL, SkinToneIndex INTEGER NULL, Pronoun TEXT NULL,
    VoiceFormKey TEXT NULL, RaceFormKey TEXT NULL, CombatOverridePackageListFormKey TEXT NULL, CombatStyleFormKey TEXT NULL, DefaultPackageListFormKey TEXT NULL, CrimeFactionFormKey TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_NPC_FormKey_ID ON NPC (FormKey_ID);

CREATE TABLE ActorValueInformation (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, Abbreviation TEXT NULL, ContextNotes TEXT NULL, DefaultValue REAL NULL, Flags TEXT NULL, Type TEXT NULL, Min REAL NULL, Max REAL NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_ActorValueInformation_FormKey_ID ON ActorValueInformation (FormKey_ID);

CREATE TABLE MagicEffect (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, Description TEXT NULL, Flags TEXT NOT NULL, CastType TEXT NULL, TargetType TEXT NULL, ActorValue2FormKey TEXT NULL, ResistValueFormKey TEXT NULL, PerkToApplyFormKey TEXT NULL, EquipAbilityFormKey TEXT NULL, ExplosionFormKey TEXT NULL, CastingArtFormKey TEXT NULL, HitEffectArtFormKey TEXT NULL, HitShaderFormKey TEXT NULL, ImageSpaceModifierFormKey TEXT NULL, ImpactDataFormKey TEXT NULL, ProjectileFormKey TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_MagicEffect_FormKey_ID ON MagicEffect (FormKey_ID);

CREATE TABLE Perk (
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL, FormVersion INTEGER NOT NULL, StarfieldMajorRecordFlags INTEGER NOT NULL, Version2 INTEGER NOT NULL, VersionControl INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    Name TEXT NULL, Description TEXT NULL, Flags TEXT NOT NULL, SkillGroup TEXT NULL, CrewAssignment TEXT NULL, PerkIcon TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Perk_FormKey_ID ON Perk (FormKey_ID);
