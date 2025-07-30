# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- FusionCache integration with support for multi-layer caching (memory + distributed cache)
- `AddSpydersoftFusionCache` extension method for easy configuration
- `FusionCacheConfigOptions` for comprehensive cache configuration
- Support for Redis distributed cache with backplane for cache invalidation
- Support for in-memory distributed cache for development scenarios
- Fail-safe caching mechanisms with configurable timeouts
- FusionCache configuration documentation
- Cache test controller in ApiTests project for testing different cache scenarios
- Unit tests for FusionCache configurations (L1, L2, Redis, disabled scenarios)
- Test configuration files for various FusionCache setups

### Changed

- Updated project dependencies to include FusionCache and Redis caching packages
- Enhanced TelemetryExtensions with additional configuration options

### Removed

## [1.2.0]

### Added

- Standardized HealthCheck options.
- Test Project (Spydersoft.Platform.Hosting.ApiTests).
- Add OpenTelemetry protocol support (`grpc` and `http`).

### Changed

- Update Serilog Extension to support writing to other providers.
- Modified configuration options to better support multiple providers.

## [1.0.0]

### Added

- Initial Release
