# Changelog

## Unreleased

## v3.0.0
- Added interface support to runtime type searches.
- Made child and parent traversal iterative to support deeply nested trees.
- Added a bounded timeout for regular-expression name filters.
- Added `NameMatchMode` to make exact and regular-expression filters explicit.
- Made `GetParentExtended` safe for nonvisual dependency objects.
- Added nullable API annotations and raised the .NET Framework compatibility floor to 4.6.2.
- Added multi-target tests, current test dependencies, analyzers and reproducible SDK selection.
- Added a NuGet package README, Source Link metadata and symbol packages.
- Improved CI coverage publishing, prerelease versioning and source archives.

## v2.0.4
Updated to .NET 10

## v2.0.3
Updated to .NET 9

## v2.0.2
Updated to .NET 5
Breaking changes: WPFVisualFinders -> WpfVisualFinders and WPFVisualFindersExtensions -> WpfVisualFindersExtensions

## v2.0.1
Updated minimum .Net Core version and package info for better visibility in Nuget feeds.

## v2.0.0
Initial version for public release, comprising the following visual tree traveling methods:
- FindChild
- FindChildByType
- FindDirectChild
- FindDirectChildByType
- FindAllChildren
- FindAllChildrenByType
- FindParent
- FindParentByType
- FindParentByLevel
- GetParentExtended

[ReadMe Documentation](https://github.com/davidlebourdais/ExtendedWPFVisualTreeHelper/blob/v2.0.0/README.md).
