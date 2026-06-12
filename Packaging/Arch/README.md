# Arch Packaging

`package-root.tar.gz` is a release artifact and is not committed to the repository. Before building or publishing the
Arch package, generate the release tarball and replace the placeholder in `PKGBUILD` with its SHA-256 checksum.

The GitHub release workflow does this automatically for the staged `PKGBUILD` after it creates
`package-root.tar.gz`. Manual packaging must do the same before running `makepkg`.

From `Packaging/Arch`:

```bash
sha256sum package-root.tar.gz
```

Copy the checksum into `sha256sums` in `PKGBUILD`, then run the normal Arch package validation/build. `SKIP` must not be
used for release packages.
