# Repository Contribution Guidelines

## Initial Setup

When you clone the repository, run `yarn install` in the root of the repository to ensure the necessary development tools are configured.

Each library is organized in its own folder, and includes the library project, associated unit tests, and necessary documentation. Use the appropriate solution to make changes.

## Linting/Formatting

This repository uses [Dotnet Format][1] to apply formatting rules to the C#.

### Dotnet Format

To format your files prior to staging and committing, you can run `dotnet format` on any of the projects in the repository:

```powershell
dotnet format .\source\Spydersoft.Platform.Hosting.sln
```

## Automated Linting/Formatting

This repository is configured with `pre-commit` and `pre-push` hooks (installed via [Husky][2]) to enforce formatting standards. The hooks are stored in the [.husky](/.husky) folder in this repository

### Pre-Commit Linting

Before your files are committed, `lint-staged` will run linting commands on all staged files, formatting them as needed before they are committed. If a formatting or linting issue is unable to be fixed, an error will be displayed and your commit will not happen. Pre-commit linting commands are configured separately in the `.lintstagedrc` configuration files.

### Pre-Push Build and Test

Before a push is executed, the pre-push hook will execute a build of the solution as well as unit test execution on both the .Net projects (using `dotnet test`). If any unit tests fail, the push will not succeed.

## Repository Standards

Libraries in this repository are shared among several projects and are meant to contain basic functionality for these applications. As such, any proposed changes should be reviewed with a member of the Staff Engineering team.

A `CHANGELOG` and `README` file will be maintained in the `docs` folder for each project.

### Changelog Updates

Each Pull Request should have updates to the appropriate `CHANGELOG.MD` file in the `Unreleased` section. When a release is cut, the file will be updated with the new version number.

### Readme Updates

Any Pull Request with usage changes should update the appropriate README.md file if applicable.

### Versioning

GitVersion is enabled on this repository with `mainline` style formatting. By default, commits (including PRs) into main will increment the `Patch` version by 1.

If the changes are significant, you can use a commit message (put in the title of the PR):

- To increment the major version, use `+semver: major`
- To increment the minor version, use `+semver: minor`

#### When to Increment

As a guideline, the following guidelines should be used to determine which version to increment:

- Major versions represent breaking changes, significant functional changes (including additions), and any changes that require modification by consuming applications.
- Minor versions represent changes that can be made in a backward compatible manner. No changes are required by consuming applications.
- Patch versions represent backward compatible bug fixes or security enhancements.

### Testing

[1]: https://github.com/dotnet/format "Dotnet Format"
[2]: https://typicode.github.io/husky/ "Husky Git Hooks"
