# Migrations Status

## Migration 100 - Unreleased Reset

All current database work uses the reset migration script:

REF: CreationsForge.Migrations\Sql\100_ResetSchema.sql

This repository is in a pre-release reset window for the local cache database. Existing local SQLite databases must be
deleted manually and rebuilt from migration 100; released migration-history continuity is intentionally not preserved
for this reset.
