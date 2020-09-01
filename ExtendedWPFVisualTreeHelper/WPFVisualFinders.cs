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
        /// Finds a child of in the visual tree using its type and (optionnaly) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T FindChild<T>(DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            if (node == null) return default;

            if (node is Visual || node is Visual3D)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(node, i);

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
                    var foundChild = FindChild<T>(child, name, allow_content_elements);
                    if (foundChild != null) return foundChild;
                }
            }

            if (allow_content_elements)
            {
                var children = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
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
                    var foundChild = FindChild<T>(child as DependencyObject, name, allow_content_elements);
                    if (foundChild != null) return foundChild;
                }
            }

            return default;
        }
                                
        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionnaly) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">Name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        public static object FindChildByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            if (node == null || type == null) return null;

            if (node is Visual || node is Visual3D)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(node, i);

                    // If the child if of the requested type:
                    if (child != null && (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                                return child;
                        }
                        else return child;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChildByType(child, type, name, allow_content_elements);
                    if (foundChild != null) return foundChild;
                }
            }

            if (allow_content_elements)
            {
                var children = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in children)
                {
                    // If the child if of the requested type:
                    if (child != null && (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (child is FrameworkContentElement frameworkContentElement && frameworkContentElement.Name == name)
                                return child;
                        }
                        else return child;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChildByType(child as DependencyObject, type, name, allow_content_elements);
                    if (foundChild != null) return foundChild;
                }
            }

            return null;
        } 

        /// <summary>
        /// Finds the first occurence of a typed child in the descendancy of a <see cref="DependencyObject"/> node 
        /// with optional name filtering and with the ability to travel through <see cref="ContentElement"/> objects 
        /// while exploring the visual tree.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <typeparam name="T">The type of the child to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">An optional name for filtering during search.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing in the direct path.</returns>
        public static T FindDirectChild<T>(DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            if (node == null) return default;
            var child = (object)null;

            if (node is Visual || node is Visual3D)
                if (VisualTreeHelper.GetChildrenCount(node) > 0)
                    child = VisualTreeHelper.GetChild(node, 0);
            
            if (allow_content_elements && child == null)
                child = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>().FirstOrDefault();

            if (child is T casted)
            {
                // If the child's name is set for search:
                if (!string.IsNullOrEmpty(name))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                        return casted;
                    else if (child is FrameworkContentElement frameworkContentElement && frameworkContentElement.Name == name)
                        return casted;
                    else return FindDirectChild<T>(child as DependencyObject, name, allow_content_elements);
                }
                else return casted;
            }
            else return child is DependencyObject asDO ? FindDirectChild<T>(asDO, name, allow_content_elements) : default;
        }

        /// <summary>
        /// Finds the first occurence of a typed child in the descendancy of a <see cref="DependencyObject"/> node 
        /// with optional name filtering and with the ability to travel through <see cref="ContentElement"/> objects 
        /// while exploring the visual tree.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">An optional name for filtering during search.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static object FindDirectChildByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            if (node == null || type == null) return null;
            var child = (object)null;

            if (node is Visual || node is Visual3D)
                if (VisualTreeHelper.GetChildrenCount(node) > 0)
                    child = VisualTreeHelper.GetChild(node, 0);
            
            if (allow_content_elements && child == null)
                child = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>().FirstOrDefault();

            if (child != null && (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
            {
                // If the child's name is set for search:
                if (!string.IsNullOrEmpty(name))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                        return child;
                    else if (child is FrameworkContentElement frameworkContentElement && frameworkContentElement.Name == name)
                        return child;
                    else return FindDirectChildByType(child as DependencyObject, type, name, allow_content_elements);
                }
                else return child;
            }
            else return child is DependencyObject asDO ? FindDirectChildByType(asDO, type, name, allow_content_elements) : null;
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildren<T>(DependencyObject node, bool allow_content_elements = true)
        {
            if (node == null)
                yield break;

            var queue = new Queue<DependencyObject>(new[] { node });

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

                if (allow_content_elements)
                {
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
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match passed type.</returns>
        public static IEnumerable<object> FindAllChildrenByType(DependencyObject node, Type type, bool allow_content_elements = true)
        {
            if (node == null || type == null)
                yield break;

            var queue = new Queue<DependencyObject>(new[] { node });

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
                        if (child != null && (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                            yield return child;

                        queue.Enqueue(child);
                    }
                }

                if (allow_content_elements)
                {
                    var children = LogicalTreeHelper.GetChildren(toProcess).OfType<ContentElement>();
                    foreach (var child in children)
                    {
                        if (child != null && (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                            yield return child;
                        if (child is DependencyObject castedDO)
                            queue.Enqueue(castedDO);
                    }
                }
            }
        }
        #endregion

        #region Find parents
        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name 
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T FindParent<T>(DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            // Get parent:
            var parent = allow_content_elements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;
            if (parent == null) return default;  // reached tree top.

            if (parent is T casted)
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return casted is FrameworkElement element && element.Name == name ? casted : FindParent<T>(parent, name);
                else return casted;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParent<T>(parent, name, allow_content_elements);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static object FindParentByType(DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            // Get parent:
            var parent = allow_content_elements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;
            if (parent == null) return default;  // reached tree top.

            if ((parent.GetType()).Equals(type) || (parent.GetType().GetTypeInfo().IsSubclassOf(type)))
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return parent is FrameworkElement element && element.Name == name ? parent : FindParentByType(parent, type, name, allow_content_elements);
                else return parent;  // case where no name is required: found typed parent then return result.
            }
            else
                return FindParentByType(parent, type, name, allow_content_elements);
        }

        /// <summary>
        /// Return a parent at a given ancestry level with the ability to travel through 
        /// <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed node.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevel(DependencyObject node, int level = 1, bool allow_content_elements = true)
        {
            if (level < 0) return null;
            int current_level = 0;

            while (current_level++ < level && node != null)
                node = allow_content_elements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;

            return node;
        }

        /// <summary>
        /// Alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not stictly speaking in the visual tree.</summary>
        /// <param name="node">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent. </remarks>
        public static DependencyObject GetParentExtended(DependencyObject node)
        {
            if (node == null) return null;  // tree root found.

            // Handle content elements separately:
            if (node is ContentElement contentElement)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce?.Parent;
            }

            // Also try searching for parent in framework elements (such as DockPanel, etc):
            if (node is FrameworkElement frameworkElement && frameworkElement.Parent is DependencyObject)
                return frameworkElement.Parent;

            // If it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper:
            return VisualTreeHelper.GetParent(node);
        }
        #endregion
    }
}
