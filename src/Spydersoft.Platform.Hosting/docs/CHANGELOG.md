# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Support for "none" as a valid telemetry exporter type for logs, metrics, and traces
- Support for `OTEL_EXPORTER_OTLP_HEADERS` environment variable for OTLP authentication headers
- Support for `http/protobuf` protocol in OTLP exporter configuration

### Changed

- Updated `LogOptions`, `MetricsOptions`, and `TraceOptions` documentation to include "none" as a valid type
- Modified telemetry configuration to properly handle "none" type without adding default exporters
- Fixed default behavior in `TelemetryExtensions` to not add console exporters when type is "none"
- OTLP headers now prioritize `OTEL_EXPORTER_OTLP_HEADERS` environment variable over configuration file values
- OTLP protocol configuration now supports both `http` and `http/protobuf` values (both map to HttpProtobuf)

### Removed

## [2.2.1] - 2026-01-12

### Added

- Support for OpenTelemetry environment variables (`OTEL:Exporter:Otlp:Endpoint` and `OTEL:Exporter:Otlp:Protocol`)

### Changed

- Updated OTLP configuration to prioritize environment variables over appsettings.json values
- Environment variables now override configuration file settings for OTLP endpoint and protocol

### Removed

## [2.2.0] - 2026-01-07

### Added

- Multi-targeting support for .NET 8.0, 9.0, and 10.0
- Centralized package management using Directory.Packages.props
- XML documentation generation for all public APIs
- `ConfigurationFunctions` class for advanced OpenTelemetry customization
- AspNetCore instrumentation enrichment support (filter, request, response, and exception enrichment)
- `Telemetry:Enabled` configuration option to enable/disable telemetry
- `Telemetry:Metrics:HistogramAggregation` configuration option for histogram aggregation strategy
- OTLP Headers support for custom authentication and metadata
- Unit tests for telemetry configuration functions and advanced scenarios
- Test configuration files for telemetry features (ConfigurationFunctions, ExponentialHistogram, OtlpHeaders, TelemetryDisabled)

### Changed

- Migrated to centralized package version management
- Enhanced TelemetryExtensions with additional configuration options
- Refactored `AddSpydersoftTelemetry` to use `ConfigurationFunctions` class instead of individual Action parameters
- Updated OpenTelemetry packages to version 1.14.0
- Updated Serilog.AspNetCore to version 10.0.0
- Updated FusionCache packages to version 2.5.0
- Updated Microsoft.Extensions packages to version 10.0.1
- Updated test packages (Microsoft.NET.Test.Sdk to 18.0.1, NUnit.Analyzers to 4.11.2, NUnit3TestAdapter to 6.0.1)
- Improved XML documentation across all classes and methods

### Removed

## [2.1.0] - 2025-08-21

### Added

- Exposed FusionCache configuration options for more granular control
- Support for OTLP headers in telemetry configuration

### Changed

- Simplified FusionCache options to allow better passthrough of fusion-specific configuration
- Improved FusionCache configuration flexibility

### Removed

- Removed unnecessary FusionCache configuration properties

## [2.0.0] - 2025-08-12

### Added

- `DependencyInjectionAttribute` for automatic service registration
- `InjectOptionsAttribute` for automatic options injection
- `HealthCheckAttribute` (renamed from `SpydersoftHealthCheckAttribute`)
- `DependencyInjectionExtensions` for scanning and registering services with attributes
- `OptionsExtensions` for scanning and registering options with attributes
- Comprehensive documentation for dependency injection and options patterns
- Additional FusionCache tests and configuration options
- File-scoped namespace support across the codebase

### Changed

- Renamed `SpydersoftHealthCheckAttribute` to `HealthCheckAttribute`
- Refactored dependency injection to use attribute-based registration
- Updated FusionCache extensions to support more configuration options
- Improved code formatting and organization (file-scoped namespaces, organized using statements)
- Updated package dependencies
- Enhanced health check testing

### Removed

- Removed unused test controllers

## [1.3.0] - 2025-07-30

### Added

- FusionCache integration with support for multi-layer caching (memory + distributed cache)
- `AddSpydersoftFusionCache` extension method for easy configuration
- `FusionCacheConfigOptions` for comprehensive cache configuration
- Support for Redis distributed cache with backplane for cache invalidation
- Support for in-memory distributed cache for development scenarios
- Fail-safe caching mechanisms with configurable timeouts
- FusionCache configuration documentation

### Changed

- Updated library dependencies to include FusionCache packages
- Enhanced telemetry configuration to support FusionCache instrumentation
- Split documentation into separate files for better organization

### Removed

## [1.2.1] - 2024-11-15

### Changed

- Updated README documentation
- Configured package licensing for GitHub Package feed

### Removed

## [1.2.0] - 2024-11-13

### Added

- Support for Serilog + OpenTelemetry logging integration
- Custom actions support for telemetry configuration
- Configuration split to allow for more flexible setup
- `LogOptions` for Serilog configuration
- `MetricsOptions` for OpenTelemetry metrics configuration
- `TraceOptions` for OpenTelemetry tracing configuration

### Changed

- Enhanced telemetry configuration with additional customization options
- Updated telemetry options structure for better organization
- Modified OTLP options to support additional configuration

### Removed

## [1.1.0] - 2024-11-11

### Added

- Health check framework with comprehensive support
- `HealthCheckExtensions` for easy health check registration
- `HealthCheckWriter` for custom health check response formatting
- `HealthCheckDataPropertyConvertor` for JSON serialization
- `TelemetryHealthCheck` for monitoring telemetry configuration
- `AppHealthCheckOptions` for health check configuration
- `IdentityExtensions` for JWT authentication setup
- Test project (Spydersoft.Platform.Hosting.ApiTests) for integration testing
- Comprehensive unit tests for health checks and configuration

### Changed

- Separated startup extensions into dedicated files (HealthCheckExtensions, IdentityExtensions, TelemetryExtensions)
- Updated telemetry options structure
- Enhanced exception handling

### Removed

## [1.0.1] - 2024-11-08

### Added

- `IdentityOptions` for JWT authentication configuration
- OpenTelemetry protocol (OTLP) support
- `TelemetryOptions` for comprehensive telemetry configuration
- Initial startup extensions for telemetry, identity, and configuration

### Changed

- Updated documentation
- Renamed project to "Platform"
- Configured SonarCloud scanning
- Updated NuGet publishing configuration

### Removed

## [1.0.0] - 2024-11-07

### Added

- Initial release of Spydersoft.Platform.Hosting
- Core hosting extensions and utilities
