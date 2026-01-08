# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Removed

## [2.2.0] - 2026-01-07

### Added

- Centralized package management using Directory.Packages.props
- XML documentation generation for all public APIs

### Changed

- Migrated to centralized package version management
- Improved XML documentation for attributes and exceptions

### Removed

## [2.0.0] - 2025-08-12

### Added

- `DependencyInjectionAttribute` for automatic service registration
- `InjectOptionsAttribute` for automatic options injection
- File-scoped namespace support

### Changed

- Renamed `SpydersoftHealthCheckAttribute` to `HealthCheckAttribute`
- Improved code formatting and organization
- Enhanced XML documentation for all attributes
- Refactored exception classes for better clarity

### Removed

- `SpydersoftHealthCheckAttribute` (renamed to `HealthCheckAttribute`)

## [1.2.0] - 2024-11-13

### Added

- Initial Spydersoft.Platform library creation
- `SpydersoftHealthCheckAttribute` for health check registration
- `ConfigurationException` for configuration error handling
- .NET Standard 2.0 support for broad compatibility

### Changed

### Removed
