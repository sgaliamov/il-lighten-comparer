# Deployment process

## Preliminary statements

1. Use GitFlow to have separate `develop` and `master` branches, and to be able trigger new release using `release` branch.
2. Name of `release` branch defines next version of the package.
3. Promotion on productions must happen manually, but automatically to private nuget hub.
