# Deployment process

## Preliminary statements

1. Use GitFlow to have separate `develop` and `master` branches, and to be able trigger new release using `release` branch.
2. Name of `release` branch defines next version of the package.
3. Promotion on productions must happen manually, but automatically to private nuget hub.

## Release

1. Create `release` branch with next version as a name.
2. Create PR from the release branch to `master`. Make sure that all checks is passed.
3. Squash and merge RP.
4. Set version tag for the merge commit and push it to `origin/master`.
5. Add [release notes](https://github.com/sgaliamov/il-lighten-comparer/tags) and publish the release.
6. Create pull request from `master` to `develop` and complete it.
7. [Deploy](https://ci.appveyor.com/environment/40781/deployments/new) to [nuget.org](https://nuget.org) if need be.
