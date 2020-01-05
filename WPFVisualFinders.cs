using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

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
        /// <param name="child_name">Name of the child to find.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type </remarks>
        public static T FindChild<T>(DependencyObject startNode, string child_name = null)
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
                    var foundChild = FindChild<T>(child, child_name);

                    // If the child is found, just return:
                    if (foundChild != null) return foundChild;
                }
                // Child of correct type, now check name if required:
                else if (!string.IsNullOrEmpty(child_name))
                {
                    // If the child's name is correct, return result:
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == child_name)
                        return typedChild;
                    else
                    {
                        // Continue to explore:
                        var foundChild = FindChild<T>(child, child_name);

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
        /// Finds the first occurence of a child of a given typein descendance of 
        /// a dependency object with an optional name filtering. 
        /// Direct as it only goes through the first child of visual elements, 
        /// contrary to <see cref="FindChild{T}"/> which looks to every children
        /// at every nodes to look for first matching result.
        /// </summary>
        /// <typeparam name="T">The type of the child to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="child_name">An optional name to give further filtering during search.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static T FindDirectChild<T>(DependencyObject startNode, string child_name = "")
        {
            if (startNode == null) return default;

            int children_count = VisualTreeHelper.GetChildrenCount(startNode);
            if (children_count > 0)
            {
                var child = VisualTreeHelper.GetChild(startNode, 0);
                if (child != null)
                {
                    if (child is T typedChild)
                    {
                        // If the child's name is set for search:
                        if (!string.IsNullOrEmpty(child_name))
                        {
                            if (child is FrameworkElement frameworkElement && frameworkElement.Name == child_name)
                                return typedChild;
                            else return FindDirectChild<T>(child, child_name);
                        }
                        else
                            return typedChild;
                    }
                    else
                        return FindDirectChild<T>(child, child_name);
                }
            }
            return default;
        }

        /// <summary>
        /// Gets a list of all descendance of given type from a dependency object. 
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="results">A list of all found children elements.</param>
        /// <remarks>From: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper</remarks>
        public static void FindAllChildren<T>(DependencyObject startNode, ref IList<T> results)
        {
            if (results == null)
                results = new List<T>();

            if (startNode == null)
                return;

            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                var current = VisualTreeHelper.GetChild(startNode, i);
                if (current != null)
                {
                    if (current is T casted)
                        results.Add(casted);
                    FindAllChildren<T>(current, ref results);
                }
            }
        }
        #endregion

        #region Find parents
        /// <summary>
        /// Finds a parent that matches method type and (optionnaly) the passed name.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent </remarks>
        public static T FindParent<T>(DependencyObject child, string name = null)
        {
            // Get parent:
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
        /// Finds a parent that matches method type and (optionnaly) the passed name 
        /// by also travelling the logical tree when necessary (i.e. when child is a content).
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
        /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports content element navigation. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!</summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, and null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent </remarks>
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
