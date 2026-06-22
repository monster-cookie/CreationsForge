# Migrations Status

## Migration 001 - Unreleased Reset

All current database work uses the reset migration script:

REF: CreationsForge.Migrations\Sql\001_ResetSchemaForV2.sql

This repository is in a pre-release reset window for the local cache database. Existing local SQLite databases must be
deleted manually and rebuilt from migration 001; released migration-history continuity is intentionally not preserved
for this reset.
