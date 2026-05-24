ALTER TABLE Faction ADD COLUMN KeywordFormKey TEXT NULL;
ALTER TABLE Faction ADD COLUMN Flags TEXT NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesArrest INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesMurder INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesAssault INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesTrespass INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesPickpocket INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesStealMultiplier REAL NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesEscape INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesPiracy INTEGER NULL;
ALTER TABLE Faction ADD COLUMN CrimeValuesSmuggleMultiplier REAL NULL;
ALTER TABLE Faction ADD COLUMN VendorValuesStartHour INTEGER NULL;
ALTER TABLE Faction ADD COLUMN VendorValuesEndHour INTEGER NULL;
ALTER TABLE Faction ADD COLUMN VendorValuesBuysStolenItems INTEGER NULL;
ALTER TABLE Faction ADD COLUMN VendorValuesBuysNonStolenItems INTEGER NULL;

CREATE INDEX IF NOT EXISTS IX_Faction_KeywordFormKey
ON Faction(KeywordFormKey);

CREATE TABLE IF NOT EXISTS FactionRelation (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    TargetFormKey TEXT NOT NULL,
    Reaction TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES Faction(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_FactionRelation_TargetFormKey
ON FactionRelation(TargetFormKey);
