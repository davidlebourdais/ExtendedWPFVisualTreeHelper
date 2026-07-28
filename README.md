# Extended WPF Visual Tree Helper

[![Build Status](https://dev.azure.com/davidlebourdais/ExtendedWPFVisualTreeHelper/_apis/build/status/davidlebourdais.ExtendedWPFVisualTreeHelper?branchName=master)](https://dev.azure.com/davidlebourdais/ExtendedWPFVisualTreeHelper/_build/latest?definitionId=11&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/ExtendedWPFVisualTreeHelper.svg)](https://www.nuget.org/packages/ExtendedWPFVisualTreeHelper)
[![Issues](https://img.shields.io/github/issues/davidlebourdais/ExtendedWPFVisualTreeHelper.svg)](https://github.com/davidlebourdais/ExtendedWPFVisualTreeHelper/issues)

Provides static and extension methods for navigating WPF visual and logical trees and finding descendants or ancestors by type and name.

It extends WPF's [`VisualTreeHelper.GetParent`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getparent) and [`VisualTreeHelper.GetChild`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getchild) capabilities with optional traversal through [`ContentElement`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.contentelement) objects.

## Getting started

Install the package from NuGet:

```powershell
dotnet add package ExtendedWPFVisualTreeHelper
```

Then import the namespace and use either the extension methods or static helpers:

```csharp
using EMA.ExtendedWPFVisualTreeHelper;

var button = root.FindChild<Button>("SubmitButton");
var window = button.FindParent<Window>();
```

The supported compatibility baselines are .NET Framework 4.6.2 and the supported LTS .NET Windows targets.

## Notions

- Use `FindParent` methods to travel up the tree and `FindChild` methods to walk down it.
- Use either static helpers or extension methods:
  - For example: `WpfVisualFinders.FindParent<Window>(node)` or `node.FindParent<Window>()`.
- Most searches target a specific type and optionally a name or a regex filter:
  - Type can be given at compile time or at runtime using the `ByType` methods.
    - For example: `FindParent<MyType>(node)` versus `FindParentByType(node, typeof(MyType))`.
  - Runtime type searches support classes and interfaces.
  - You can target `object` to disable type filtering during a generic search.
  - Name filtering is enabled by setting the optional `name` argument.
  - A name can be an exact value or a [regular expression](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions). Regex evaluation uses a bounded timeout.
  - Use `NameMatchMode.Exact` or `NameMatchMode.Regex` when the interpretation must be explicit. `ExactOrRegex` preserves the original exact-first behavior.


## Reference

### FindChild & FindChildByType
Finds a child of the specified type by walking down the visual tree from the supplied node. Every branch is explored until a matching child is found.

    T? FindChild<T>(DependencyObject? node, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

    DependencyObject? FindChildByType(DependencyObject? node, Type? type, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

### FindDirectChild & FindDirectChildByType
Finds a child of a specific type by walking down the visual tree from the passed node and through first encountered children only. Search is stopped when a matching item is found or when the most accessible leaf is reached.

    T? FindDirectChild<T>(DependencyObject? node, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

    DependencyObject? FindDirectChildByType(DependencyObject? node, Type? type, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

### FindAllChildren & FindAllChildrenByType
Builds an enumerable of all descendants that match the target type and optional name or regex. Every path below the starting node is explored.

    IEnumerable<T> FindAllChildren<T>(DependencyObject? node, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

    IEnumerable<DependencyObject> FindAllChildrenByType(DependencyObject? node, Type? type, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

**Note**
This method explores and exposes the matching children level-by-level rather than branch-by-branch. Thus, all matching children are exposed, then all matching grandchildren, then all matching grand-grandchildren, etc.

### FindParent & FindParentByType
Finds first parent that matches specified type and optional name or regex by walking up the visual tree from the passed node. Returns null after tree top is reached with no result.

    T? FindParent<T>(DependencyObject? node, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

    DependencyObject? FindParentByType(DependencyObject? node, Type? type, string? name = null, bool allowContentElements = true, NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)

### FindParentByLevel
Finds the parent at a given depth from the supplied node. Returns null if that level does not exist.

    DependencyObject? FindParentByLevel(DependencyObject? node, int level = 1, bool allowContentElements = true)

**Note**
When level defaults to 1, the method gets the immediate parent.

### GetParentExtended
An extension of the [`VisualTreeHelper.GetParent()`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getparent) method that supports travel through `ContentElement` objects.

    DependencyObject? GetParentExtended(DependencyObject? node)

## About ContentElements
[`ContentElement`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.contentelement) objects—and more specifically their derived [`FrameworkContentElement`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkcontentelement) counterparts—participate in WPF content and logical trees but cannot be attached directly to the visual tree. They share useful APIs, including naming, with visual-tree objects such as [`FrameworkElement`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement).

All these reasons make them important nodes to find or travel through while exploring the visual tree, although they are not part of it. As a consequence: 

> All provided methods allow [`ContentElement`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.contentelement) traversal by default.
> Disable this behavior by setting `allowContentElements: false`.

Some ContentElement examples:
- [Run](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.run)
- [Section](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.section)
- [List](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.list)
- [TableCell](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.tablecell)
- etc.

## License
This work is licensed under the [MIT License](LICENSE).
