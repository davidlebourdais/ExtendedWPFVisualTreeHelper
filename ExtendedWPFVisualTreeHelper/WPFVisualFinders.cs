using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace EMA.ExtendedWPFVisualTreeHelper
{
    /// <summary>
    /// Provides a set of helpers to navigate a visual tree and either find 
    /// a specific child or parent of a dependency object.
    /// </summary>
    public static class WPFVisualFinders
    {
        #region Find children
        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionnaly) its name.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="name">Name of the child to find.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T FindChild<T>(DependencyObject startNode, string name = null)
        {
            if (startNode == null) return default;

            var childrenCount = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(startNode, i);

                // If the child is not of the requested type:
                if (!(child is T typedChild))
                {
                    // Continue to explore:
                    var foundChild = FindChild<T>(child, name);

                    // If the child is found, just return:
                    if (foundChild != null) return foundChild;
                }
                // Child of correct type, now check name if required:
                else if (!string.IsNullOrEmpty(name))
                {
                    // If the child's name is correct, return result:
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                        return typedChild;
                    else
                    {
                        // Continue to explore:
                        var foundChild = FindChild<T>(child, name);

                        // If the child is found, just return:
                        if (foundChild != null) return foundChild;
                    }
                }
                else // if no name required and type is correct:
                {
                    return typedChild;  // just return child result.
                }
            }

            return default;  // been at the end and nothing interesting came back.
        }

        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionnaly) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="name">Name of the child to find.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T FindChildExtended<T>(DependencyObject startNode, string name = null)
        {
            if (startNode == null) return default;

            if (startNode is Visual || startNode is Visual3D)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(startNode);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(startNode, i);

                    // If the child if of the requested type:
                    if (child is T casted)
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                                return casted;
                        }
                        else return casted;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChildExtended<T>(child, name);
                    if (foundChild != null) return foundChild;
                }
            }

            var children = LogicalTreeHelper.GetChildren(startNode).OfType<ContentElement>();
            foreach (var child in children)
            {
                // If the child if of the requested type:
                if (child is T casted)
                {
                    if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                    {
                        if (child is FrameworkContentElement frameworkContentElement && frameworkContentElement.Name == name)
                            return casted;
                    }
                    else return casted;
                }

                // If here, no child found so far so keep digging:
                var foundChild = FindChildExtended<T>(child as DependencyObject, name);
                if (foundChild != null) return foundChild;
            }

            return default;
        }

        /// <summary>
        /// Finds the first occurence of a typed child in the descendancy of a <see cref="DependencyObject"/> node 
        /// with optional name filtering.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <typeparam name="T">The type of the child to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="name">An optional name for filtering during search.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static T FindDirectChild<T>(DependencyObject startNode, string name = "")
        {
            if (startNode == null) return default;

            int children_count = VisualTreeHelper.GetChildrenCount(startNode);
            if (children_count > 0)
            {
                var child = VisualTreeHelper.GetChild(startNode, 0);
                if (child is T typedChild)
                {
                    // If the child's name is set for search:
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                            return typedChild;
                        else return FindDirectChild<T>(child, name);
                    }
                    else return typedChild;
                }
                else return FindDirectChild<T>(child, name);
            }
            return default;
        }

        /// <summary>
        /// Finds the first occurence of a typed child in the descendancy of a <see cref="DependencyObject"/> node 
        /// with optional name filtering and with the ability to travel through <see cref="ContentElement"/> objects 
        /// while exploring the visual tree.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <typeparam name="T">The type of the child to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="name">An optional name for filtering during search.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static T FindDirectChildExtended<T>(DependencyObject startNode, string name = "")
        {
            if (startNode == null) return default;
            var child = (object)null;

            if (startNode is Visual || startNode is Visual3D)
                if (VisualTreeHelper.GetChildrenCount(startNode) > 0)
                    child = VisualTreeHelper.GetChild(startNode, 0);
            
            if (child == null)
                child = LogicalTreeHelper.GetChildren(startNode).OfType<ContentElement>().FirstOrDefault();

            if (child is T casted)
            {
                // If the child's name is set for search:
                if (!string.IsNullOrEmpty(name))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                        return casted;
                    else if (child is FrameworkContentElement frameworkContentElement && frameworkContentElement.Name == name)
                        return casted;
                    else return FindDirectChildExtended<T>(child as DependencyObject, name);
                }
                else return casted;
            }
            else return child is DependencyObject asDO ? FindDirectChildExtended<T>(asDO, name) : default;
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildren<T>(DependencyObject startNode)
        {
            if (startNode == null)
                yield break;

            var queue = new Queue<DependencyObject>(new[] { startNode });

            #if NETFRAMEWORK
            while (queue.Count > 0)
            {
                var toProcess = queue.Dequeue();
            #else
            while (queue.TryDequeue(out DependencyObject toProcess))
            { 
            #endif
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(toProcess); i++)
                {
                    var child = VisualTreeHelper.GetChild(toProcess, i);
                    if (child is T casted)
                        yield return casted;

                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildrenExtended<T>(DependencyObject startNode)
        {
            if (startNode == null)
                yield break;

            var queue = new Queue<DependencyObject>(new[] { startNode });

            #if NETFRAMEWORK
            while (queue.Count > 0)
            {
                var toProcess = queue.Dequeue();
            #else
            while (queue.TryDequeue(out DependencyObject toProcess))
            { 
            #endif
                if (toProcess is Visual || toProcess is Visual3D)
                {
                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(toProcess); i++)
                    {
                        var child = VisualTreeHelper.GetChild(toProcess, i);
                        if (child is T casted)
                            yield return casted;

                        queue.Enqueue(child);
                    }
                }

                var children = LogicalTreeHelper.GetChildren(toProcess).OfType<ContentElement>();
                foreach (var child in children)
                {
                    if (child is T casted)
                        yield return casted;
                    if (child is DependencyObject castedDO)
                        queue.Enqueue(castedDO);
                }
            }
        }
        #endregion

        #region Find parents
        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent. </remarks>
        public static T FindParent<T>(DependencyObject child, string name = null)
        {
            // Get parent:
            if (child is ContentElement) return default;  // cannot find child for ContentElement, use FindParentExtended instead.

            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return default;  // reached tree top.

            if (parent is T casted)
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return casted is FrameworkElement element && element.Name == name ? casted : FindParent<T>(parent, name);
                else return casted;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParent<T>(parent, name);
        }

        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name 
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T FindParentExtended<T>(DependencyObject child, string name = null)
        {
            // Get parent:
            var parent = GetParentExtended(child);
            if (parent == null) return default;  // reached tree top.

            if (parent is T casted)
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return casted is FrameworkElement element && element.Name == name ? casted : FindParentExtended<T>(parent, name);
                else return casted;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParentExtended<T>(parent, name);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="targetType">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByType(DependencyObject child, Type targetType, string name = null)
        {
            // Get parent:
            if (child is ContentElement) return default;  // cannot find child for ContentElement, use FindParentExtended instead.
            
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return default;  // reached tree top.

            if ((parent.GetType()).Equals(targetType) || (parent.GetType().GetTypeInfo().IsSubclassOf(targetType)))
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return parent is FrameworkElement element && element.Name == name ? parent : FindParentByType(parent, targetType, name);
                else return parent;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParentByType(parent, targetType, name);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="targetType">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByTypeExtended(DependencyObject child, Type targetType, string name = null)
        {
            // Get parent:
            var parent = GetParentExtended(child);
            if (parent == null) return default;  // reached tree top.

            if ((parent.GetType()).Equals(targetType) || (parent.GetType().GetTypeInfo().IsSubclassOf(targetType)))
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return parent is FrameworkElement element && element.Name == name ? parent : FindParentByTypeExtended(parent, targetType, name);
                else return parent;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParentByTypeExtended(parent, targetType, name);
        }

        /// <summary>
        /// Return a parent at a given ancestry level.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed child.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevel(DependencyObject child, int level = 1)
        {
            if (level < 0) return null;
            int current_level = 0;
            var result = child;

            while (current_level++ < level && result != null)
                result = result is ContentElement ? null : VisualTreeHelper.GetParent(result);

            return result;
        }

        /// <summary>
        /// Return a parent at a given ancestry level with the ability to travel through 
        /// <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed child.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevelExtended(DependencyObject child, int level = 1)
        {
            if (level < 0) return null;
            int current_level = 0;
            var result = child;

            while (current_level++ < level && result != null)
                result = GetParentExtended(result);

            return result;
        }

        /// <summary>
        /// Alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not stictly speaking in the visual tree.</summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent. </remarks>
        public static DependencyObject GetParentExtended(DependencyObject child)
        {
            if (child == null) return null;  // tree root found.

            // Handle content elements separately:
            if (child is ContentElement contentElement)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce?.Parent;
            }

            // Also try searching for parent in framework elements (such as DockPanel, etc):
            if (child is FrameworkElement frameworkElement && frameworkElement.Parent is DependencyObject)
                return frameworkElement.Parent;

            // If it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper:
            return VisualTreeHelper.GetParent(child);
        }
        #endregion
    }
}
