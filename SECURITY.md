# Security Policy

## Supported Versions

Creations Forge only receives security updates for actively maintained releases.

| Version | Supported          |
| ------- | ------------------ |
| 1.1.x   | :white_check_mark: |
| < 1.1   | :x:                |

Version 1.1.x is currently maintained and may receive security fixes, compatibility fixes, and critical bug fixes.

Versions older than 1.1.x are no longer maintained. Security fixes are not backported to unsupported versions unless specifically noted by the maintainers.

For a full version history, see `Documentation/CHANGE_LOG.md`.

## Reporting a Vulnerability

If you believe you have found a security vulnerability in Creations Forge, please report it using one of the following public support channels:

* Open a GitHub issue:
  [GitHub Issue Tracker](https://github.com/monster-cookie/CreationsForge/issues)

* Post a comment on the Nexus Mods page:
  [Nexus Discussion Board](https://www.nexusmods.com/starfield/mods/17323)

Because these channels are public, please do not include passwords, tokens, private keys, personal information, or sensitive exploit details in the initial report.

When reporting a vulnerability, please include as much of the following information as possible:

* The Creations Forge version affected.
* Your operating system and runtime environment.
* A clear description of the issue.
* Steps to reproduce the issue.
* The expected behavior.
* The actual behavior.
* Any relevant logs, screenshots, or error messages.
* Whether the issue affects local files, plugin parsing, exported data, application settings, or external dependencies.

If the issue may expose sensitive information or could be used to harm users, please provide a high-level summary first and state that more details are available privately if needed.

## Response Expectations

Security reports are reviewed on a best-effort basis.

After a report is received, the maintainers will attempt to:

1. Confirm whether the issue affects a supported version.
2. Reproduce and assess the impact of the issue.
3. Determine whether the report is a security vulnerability, a general bug, or expected behavior.
4. Provide a fix, mitigation, or explanation when practical.

If a vulnerability is accepted, it may be fixed in a future 1.1.x release or later supported release. The fix may also be documented in `Documentation/CHANGE_LOG.md`.

If a report is declined, the maintainers may explain why it is not considered a vulnerability, such as when the issue only affects unsupported versions, requires unsafe local configuration, or is outside the scope of Creations Forge.

## Scope

Security issues may include, but are not limited to:

* Unsafe handling of local files.
* Unexpected modification or deletion of user data.
* Unsafe parsing of plugin, archive, YAML, JSON, or other project files.
* Exposure of sensitive local paths, environment details, or configuration data.
* Dependency vulnerabilities that directly affect Creations Forge.
* Crashes or denial-of-service issues caused by malformed input files.

The following are generally not considered security vulnerabilities unless they create a concrete security impact:

* General application crashes without data exposure or unsafe behavior.
* Bugs affecting unsupported versions.
* Compatibility issues with specific mods, plugins, or game files.
* Feature requests or design concerns.
* Issues caused by manually editing application files in an unsupported way.

## Unsupported Versions

Versions older than 1.1.x are unsupported.

Users running unsupported versions should upgrade to a supported release before reporting security issues. Reports for unsupported versions may be closed unless the issue also affects a supported version.

## Disclosure

Please avoid publicly posting detailed exploit steps until the maintainers have had a reasonable opportunity to review the report.

Security fixes may be released with a general changelog entry first, followed by additional details later if needed to protect users.
