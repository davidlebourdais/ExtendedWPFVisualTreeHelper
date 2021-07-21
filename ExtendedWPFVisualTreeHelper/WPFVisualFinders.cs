using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;

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
        /// Finds a child of in the visual tree using its type and (optionally) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T FindChild<T>(DependencyObject node, string name = null, bool allowContentElements = true)
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
                            if (CheckNameMatch(child, name))
                                return casted;
                        }
                        else return casted;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChild<T>(child, name, allowContentElements);
                    if (foundChild != null) return foundChild;
                }
            }

            if (allowContentElements)
            {
                var children = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in children)
                {
                    // If the child if of the requested type:
                    if (child is T casted)
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (CheckNameMatch(child, name))
                                return casted;
                        }
                        else return casted;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChild<T>(child, name);
                    if (foundChild != null) return foundChild;
                }
            }

            return default;
        }
                                
        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionally) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        public static DependencyObject FindChildByType(DependencyObject node, Type type, string name = null, bool allowContentElements = true)
        {
            if (node == null || type == null) return null;

            if (node is Visual || node is Visual3D)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(node);
                for (var i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(node, i);

                    // If the child if of the requested type:
                    if ((child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (CheckNameMatch(child, name))
                                return child;
                        }
                        else return child;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChildByType(child, type, name, allowContentElements);
                    if (foundChild != null) return foundChild;
                }
            }

            if (allowContentElements)
            {
                var children = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in children)
                {
                    // If the child if of the requested type:
                    if ((child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                    {
                        if (!string.IsNullOrEmpty(name)) // if the child's name is set for search
                        {
                            if (CheckNameMatch(child, name))
                                return child;
                        }
                        else return child;
                    }

                    // If here, no child found so far so keep digging:
                    var foundChild = FindChildByType(child, type, name);
                    if (foundChild != null) return foundChild;
                }
            }

            return null;
        } 

        /// <summary>
        /// Finds the first occurence of a typed child in the lineage of a <see cref="DependencyObject"/> node 
        /// with optional name filtering and with the ability to travel through <see cref="ContentElement"/> objects 
        /// while exploring the visual tree.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <typeparam name="T">The type of the child to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing in the direct path.</returns>
        public static T FindDirectChild<T>(DependencyObject node, string name = null, bool allowContentElements = true)
        {
            if (node == null) return default;
            var child = (object)null;

            if (node is Visual || node is Visual3D)
                if (VisualTreeHelper.GetChildrenCount(node) > 0)
                    child = VisualTreeHelper.GetChild(node, 0);
            
            if (allowContentElements && child == null)
                child = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>().FirstOrDefault();

            if (child is T casted)
            {
                // If the child's name is set for search:
                if (string.IsNullOrEmpty(name))
                    return casted;
                
                if (CheckNameMatch(child as DependencyObject, name))
                    return casted;
                
                return FindDirectChild<T>(child as DependencyObject, name, allowContentElements);

            }

            return child is DependencyObject asDo ? FindDirectChild<T>(asDo, name, allowContentElements) : default;
        }

        /// <summary>
        /// Finds the first occurence of a typed child in the lineage of a <see cref="DependencyObject"/> node 
        /// with optional name filtering and with the ability to travel through <see cref="ContentElement"/> objects 
        /// while exploring the visual tree.
        /// Direct as it only goes through the first child of visual elements, contrary to <see cref="FindChild{T}"/> which looks 
        /// searches any children of a node to find the first matching result.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static DependencyObject FindDirectChildByType(DependencyObject node, Type type, string name = null, bool allowContentElements = true)
        {
            if (node == null || type == null) return null;
            var child = (object)null;

            if (node is Visual || node is Visual3D)
                if (VisualTreeHelper.GetChildrenCount(node) > 0)
                    child = VisualTreeHelper.GetChild(node, 0);
            
            if (allowContentElements && child == null)
                child = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>().FirstOrDefault();

            if (child is DependencyObject casted)
            {
                if (child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type))
                {
                    // If the child's name is set for search:
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (CheckNameMatch(casted, name))
                            return casted;
                        return FindDirectChildByType(casted, type, name, allowContentElements);
                    }
                    return casted;
                }
                return FindDirectChildByType(casted, type, name, allowContentElements);
            }
            return null;
        }

        /// <summary>
        /// Gets the filtered-by-type complete lineage of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">An optional name or regex pattern to be used for filtering during search.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildren<T>(DependencyObject node, string name = null, bool allowContentElements = true)
        {
            if (node == null)
                yield break;

            var queue = new Queue<DependencyObject>(new[] { node });

            #if NETFRAMEWORK
            while (queue.Count > 0)
            {
                var toProcess = queue.Dequeue();
            #else
            while (queue.TryDequeue(out var toProcess))
            { 
            #endif
                if (toProcess is Visual || toProcess is Visual3D)
                {
                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(toProcess); i++)
                    {
                        var child = VisualTreeHelper.GetChild(toProcess, i);
                        if (child is T casted)
                            if (string.IsNullOrEmpty(name) || CheckNameMatch(child, name))
                                yield return casted;

                        queue.Enqueue(child);
                    }
                }

                if (allowContentElements)
                {
                    var children = LogicalTreeHelper.GetChildren(toProcess).OfType<ContentElement>();
                    foreach (var child in children)
                    {
                        if (child is T casted)
                            if (string.IsNullOrEmpty(name) || CheckNameMatch(child, name))
                                yield return casted;
                        if (child is DependencyObject castedDo)
                            queue.Enqueue(castedDo);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the filtered-by-type complete lineage of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">An optional name or regex pattern to be used for filtering during search.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match passed type.</returns>
        public static IEnumerable<DependencyObject> FindAllChildrenByType(DependencyObject node, Type type, string name = null, bool allowContentElements = true)
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
                        if ((child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                            if (string.IsNullOrEmpty(name) || CheckNameMatch(child, name))
                                yield return child;

                        queue.Enqueue(child);
                    }
                }

                if (allowContentElements)
                {
                    var children = LogicalTreeHelper.GetChildren(toProcess).OfType<ContentElement>();
                    foreach (var child in children)
                    {
                        if ((child.GetType().Equals(type) || child.GetType().GetTypeInfo().IsSubclassOf(type)))
                            if (string.IsNullOrEmpty(name) || CheckNameMatch(child, name))
                                yield return child;
                        if (child is DependencyObject castedDo)
                            queue.Enqueue(castedDo);
                    }
                }
            }
        }
        #endregion

        #region Find parents
        /// <summary>
        /// Finds a parent that matches static type and (optionally) the passed name 
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of the object to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name or regex that matches name of the parent to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T FindParent<T>(DependencyObject node, string name = null, bool allowContentElements = true)
        {
            // Get parent:
            var parent = allowContentElements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;
            if (parent == null) return default;  // reached tree top.

            if (parent is T casted)
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return CheckNameMatch(casted as DependencyObject, name) ? casted : FindParent<T>(parent, name);
                return casted;  // case where no name is required: found typed parent then return result.
            }
            
            return FindParent<T>(parent, name, allowContentElements);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionally) a passed name
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">The explicit type the parent should have.</param>
        /// <param name="name">Optional name or regex that matches name of the parent to find.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByType(DependencyObject node, Type type, string name = null, bool allowContentElements = true)
        {
            // Get parent:
            var parent = allowContentElements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;
            if (parent == null) return default;  // reached tree top.

            if ((parent.GetType()).Equals(type) || (parent.GetType().GetTypeInfo().IsSubclassOf(type)))
            {
                if (!string.IsNullOrEmpty(name))  // case where search by name is enabled.
                    return CheckNameMatch(parent, name) ? parent : FindParentByType(parent, type, name, allowContentElements);
                return parent;  // case where no name is required: found typed parent then return result.
            }
            
            return FindParentByType(parent, type, name, allowContentElements);
        }

        /// <summary>
        /// Return a parent at a given ancestry level with the ability to travel through 
        /// <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed node.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The parent at given ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevel(DependencyObject node, int level = 1, bool allowContentElements = true)
        {
            if (level < 0) return null;
            var currentLevel = 0;

            while (currentLevel++ < level && node != null)
                node = allowContentElements ? GetParentExtended(node) : (node is Visual || node is Visual3D) ? VisualTreeHelper.GetParent(node) : null;

            return node;
        }

        /// <summary>
        /// Alternative to WPF <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not strictly speaking in the visual tree.</summary>
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
            if (node is FrameworkElement frameworkElement && frameworkElement.Parent != null)
                return frameworkElement.Parent;

            // If it's not a ContentElement or FrameworkElement then rely on VisualTreeHelper:
            return VisualTreeHelper.GetParent(node);
        }
        #endregion

        #region Check names
        /// <summary>
        /// Checks if a node's name matches exact passed name or regex.
        /// </summary>
        /// <param name="node">The node to check, must be a <see cref="FrameworkElement"/> 
        /// or <see cref="FrameworkContentElement"/> to read the 'Name' property.</param>
        /// <param name="name">The exact name or regex to assess.</param>
        /// <returns>True if node's name matches passed name, false otherwise.</returns>
        private static bool CheckNameMatch(DependencyObject node, string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            try
            {
                if (node is FrameworkElement asFe)
                    return asFe.Name == name || Regex.IsMatch(asFe.Name, name); // default regex options
                if (node is FrameworkContentElement asFce)
                    return asFce.Name == name || Regex.IsMatch(asFce.Name, name);
            }
            catch (Exception ex) when(ex is ArgumentException || ex is RegexMatchTimeoutException)
            {   }

            return false;
        }
        #endregion
    }
}
