# WPF Visual Tree Helper
Provides methods to travel a WPF visual tree and to find items of interest. 

Goes beyond framework's VisualTreeHelper [GetParent](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getparent) and [GetChild](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getchild) methods by allowing traversal of [ContentElement](https://docs.microsoft.com/en-us/dotnet/api/system.windows.contentelemen) objects. 

## Notions

 - Use 'FindParent' methods to travel the visual tree up and 'FindChildren' methods to walk it down
 - Use either static helpers or extension methods:
	 - ex: *WPFVisualFinders.FindParent(node)* or *node.FindParent()*
 - Most searches target a specific type and a optionally a name:
	 - Type can be given at compile time, or at runtime using the 'ByType' methods 
		 - ex: *FindParent{T}()* vs *FindParentByType(myType)*
	 - You can target the 'object' type to get rid or type filtering during search
	 - Named item search is activated when setting the optional 'name' arguments
	 - Exact naming is assessed for name searches


## About ContentElements
[ContentElements](https://docs.microsoft.com/en-us/dotnet/api/system.windows.contentelemen) - and more specifically their derived [ContentFrameworkElement](https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkcontentelement) counterparts - are special items that dictates rendering on screen for the part they describe but are not attachable to the visual tree and rely on the logical one. However, these items have many APIs in common with the objects we find on visual tree (specifically [FrameworkElement](https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement)) -including naming -, are directly visible in the XAML definitions, and might contain useful information about the visual they represent.

All these reasons make them important nodes to find or travel through while exploring the visual tree, although they are not part of it. As a consequence: 

> All provided methods allows [ContentElement](https://docs.microsoft.com/en-us/dotnet/api/system.windows.contentelemen) traversal by default. 
> This feature can disabled by setting **allow_content_elements = false** on any method call.

Some ContentElement examples:
- [ColumnDefinition](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.columndefinition?view=netcore-3.1)
- [Run](https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.run)
- [Section](https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.section)
- [List](https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.list)
- [TableCell](https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.tablecell)
- etc.

## Reference

### FindChild & FindChildByType
Finds a child of the specified type by walking down the visual tree from the passed node. Every leaves are explored until matching child is found.

    T FindChild<T>(DependencyObject node, string name = null, bool allow_content_elements = true)

    DependencyObject FindChildByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)

### FindDirectChild & FindDirectChildByType
Finds a child of a specific type by walking down the visual tree from the passed node and through first encountered children only. Search is stopped when a matching item is found or when the most accessible leaf is reached.

    T FindDirectChild<T>(DependencyObject node, string name = null, bool allow_content_elements = true)

    DependencyObject FindDirectChild(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)

### FindAllChildren & FindAllChildrenByType
Builds up an enumerable of all passed node's descendants that match target type and optional name. Looks at every paths from top node.

    IEnumerable<T> FindAllChildren<T>(DependencyObject node, string name = null, bool allow_content_elements = true)

    IEnumerable<DependencyObject> FindAllChildrenByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)

**Note**
This method explores and exposes the matching children level-by-level rather than branch-by-branch. Thus, all matching children are exposed, then all matching grandchildren, then all matching grand-grandchildren, etc.

### FindParent & FindParentByType
Finds first parent that matches specified type and optional name by walking up the visual tree from the passed node. Returns null after tree top is reached with no result.

    T FindParent<T>(DependencyObject node, string name = null, bool allow_content_elements = true)

    DependencyObject FindParentByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)

### FindParentByLevel
Finds parent taken at a given depth level from the passed node. Returns null if level to cross if too large or if cannot walk the tree up at some point.

    DependencyObject FindParentByLevel(DependencyObject node, int level = 1, bool allow_content_elements = true)

**Note**
When level defaults to 1, the method gets the immediate parent.

### GetParentExtended
An extension of the [VisualTreeHelper.GetParent()](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.visualtreehelper.getparent) method that supports travel through ContentElement objects.

    DependencyObject GetParentExtended(DependencyObject node)
