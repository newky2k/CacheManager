# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`DSoft.CacheManager` — a managed .NET caching framework with pluggable storage backends, plus `DSoft.CacheManager.LiteDB`, the LiteDB backend. Two NuGet packages and a WPF test harness (`CacheManagerTest`).

- `CacheManager/` — core library (`DSoft.CacheManager`): `ICacheManager`, `ICacheStorageBackend`, `CacheManagerFactory` and the DI registration extensions.
- `DSoft.CacheManager.LiteDB/` — LiteDB backend (`DSoft.CacheManager.LiteDB`), depends on `LiteDB.Config`.
- `CacheManagerTest/` — WPF app used as a manual harness. There are no automated tests.

## Build

```bash
dotnet restore CacheManager.sln
dotnet build CacheManager.sln -c Release
```

- Both libraries multi-target `netstandard2.0;net8.0;net9.0;net10.0;net11.0`. The harness targets `net10.0-windows` and `net11.0-windows`. Build with the .NET 10 and .NET 11 SDKs (`global.json` rolls forward to the latest major); on Linux or macOS add `-p:EnableWindowsTargeting=true` (the WPF harness compiles but cannot run).
- `Microsoft.Extensions.*` is 11.0.0 for `net11.0` and 10.0.x for every other target (conditional `PackageReference` groups).
- `GeneratePackageOnBuild=true` (`Directory.Build.props`), so every build produces `.nupkg` files. Both are packed with `README.md` and `DSIcon.png`.
- Assemblies are strong-name signed with each project's `DSoft.snk`; do not remove those files.

## Branching

- `development` is the working branch: branch from it and open pull requests into it. Never commit or open a pull request directly against `main`.
- `main` is the release branch. Every push to `main` runs `release.yml`, which publishes the packages to nuget.org and creates a GitHub release, so only merge `development` into `main` when a release is intended, and only when asked to release.

## CI

GitHub Actions (`.github/workflows/`) on `windows-latest`:

- `ci.yml` builds Release for pull requests into `main` and `development`; it publishes nothing.
- `release.yml` runs on every push to `main` (changes only to Markdown or workflow files are skipped; run it by hand to test a workflow change). It builds Release as `1.1.<yyMM>.<run number>` plus `RELEASE_SUFFIX` (empty for a stable version), pushes `DSoft.CacheManager` and `DSoft.CacheManager.LiteDB` to nuget.org with Trusted Publishing (OIDC, `NUGET_USER` secret, `nuget` environment), then tags the commit `v<version>` and creates a GitHub release with the packages attached.
