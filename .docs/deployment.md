# Deployment process

## Preliminary statements

1. Use GitFlow to have separate `develop` and `master` branch.
2. Version of the next package is defined by tag on `master` branch.
3. Promotion on productions must happen manually, but automatically to private `nuget` hub.

## Release

1. Create PR from `develop` branch to `master`. Make sure that all checks is passed.
2. Merge RP (do not squash `develop`).
3. Wait for all builds to ensure that it works after the merge.
3. Add [release notes](https://github.com/sgaliamov/il-lighten-comparer/tags) and publish the release.
4. [Deploy](https://ci.appveyor.com/environment/40781/deployments/new) to [nuget.org](https://nuget.org) if need be.
