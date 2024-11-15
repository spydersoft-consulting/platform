# Spydersoft Platform Libraries

![GitHub License](https://img.shields.io/github/license/spydersoft-consulting/platform)
![Azure DevOps builds](https://img.shields.io/azure-devops/build/spydersoft/97a7b4ed-e5c1-4999-b697-36114643d28c/52)
![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/spydersoft_platform?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/spydersoft_platform?server=https%3A%2F%2Fsonarcloud.io)

This repository contains libraries with common tasks for Spydersoft projects.

## Libraries

- [Spydersoft.Platform](./src/Spydersoft.Platform/)
- [Spydersoft.Platform.Hosting](./src/Spydersoft.Platform.Hosting//)

## Directory Structure

- `./.devops` - Contains any pipelines for Azure DevOps
- `./docs` - Contains all markdown documentation, including relevant Architecture Decision Records
- `./src` - Contains source content for all libraries.

Each library has the following structure:

- `./docs` - `README.md` and `CHANGELOG.md` files for the package
- Source project(s)
- Unit Test Project(s)
- A single solution file

## Contribution Guidelines

See the [Contribution Guidelines](docs/CONTRIBUTING.md) for details on contributing.
