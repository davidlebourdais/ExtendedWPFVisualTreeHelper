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
    public static class WpfVisualFinders
    {
        private static readonly TimeSpan NameMatchTimeout = TimeSpan.FromMilliseconds(250);

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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>A matching child, or default if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T? FindChild<T>(
            DependencyObject? node,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null) return default;

            var toProcess = new Stack<DependencyObject>(GetChildren(node, allowContentElements).Reverse());
            while (toProcess.Count > 0)
            {
                var child = toProcess.Pop();
                if (child is T casted && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                    return casted;

                foreach (var descendant in GetChildren(child, allowContentElements).Reverse())
                    toProcess.Push(descendant);
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        public static DependencyObject? FindChildByType(
            DependencyObject? node,
            Type? type,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null || type == null) return null;

            var toProcess = new Stack<DependencyObject>(GetChildren(node, allowContentElements).Reverse());
            while (toProcess.Count > 0)
            {
                var child = toProcess.Pop();
                if (MatchesType(child, type) && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                    return child;

                foreach (var descendant in GetChildren(child, allowContentElements).Reverse())
                    toProcess.Push(descendant);
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>A matching child, or default if none existing in the direct path.</returns>
        public static T? FindDirectChild<T>(
            DependencyObject? node,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null) return default;

            var child = GetChildren(node, allowContentElements).FirstOrDefault();
            while (child != null)
            {
                if (child is T casted && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                    return casted;

                child = GetChildren(child, allowContentElements).FirstOrDefault();
            }

            return default;
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static DependencyObject? FindDirectChildByType(
            DependencyObject? node,
            Type? type,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null || type == null) return null;

            var child = GetChildren(node, allowContentElements).FirstOrDefault();
            while (child != null)
            {
                if (MatchesType(child, type) && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                    return child;

                child = GetChildren(child, allowContentElements).FirstOrDefault();
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildren<T>(
            DependencyObject? node,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
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
                foreach (var child in GetChildren(toProcess, allowContentElements))
                {
                    if (child is T casted && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                        yield return casted;

                    queue.Enqueue(child);
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>All found children elements that match passed type.</returns>
        public static IEnumerable<DependencyObject> FindAllChildrenByType(
            DependencyObject? node,
            Type? type,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null || type == null)
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
                foreach (var child in GetChildren(toProcess!, allowContentElements))
                {
                    if (MatchesType(child, type) && (name is null || name.Length == 0 || CheckNameMatch(child, name, nameMatchMode)))
                        yield return child;

                    queue.Enqueue(child);
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T? FindParent<T>(
            DependencyObject? node,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            var parent = GetParent(node, allowContentElements);
            while (parent != null)
            {
                if (parent is T casted && (name is null || name.Length == 0 || CheckNameMatch(parent, name, nameMatchMode)))
                    return casted;

                parent = GetParent(parent, allowContentElements);
            }

            return default;
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
        /// <param name="nameMatchMode">Defines how the optional name filter is interpreted.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject? FindParentByType(
            DependencyObject? node,
            Type? type,
            string? name = null,
            bool allowContentElements = true,
            NameMatchMode nameMatchMode = NameMatchMode.ExactOrRegex)
        {
            if (node == null || type == null) return null;

            var parent = GetParent(node, allowContentElements);
            while (parent != null)
            {
                if (MatchesType(parent, type) && (name is null || name.Length == 0 || CheckNameMatch(parent, name, nameMatchMode)))
                    return parent;

                parent = GetParent(parent, allowContentElements);
            }

            return null;
        }

        /// <summary>
        /// Return a parent at a given ancestry level with the ability to travel through 
        /// <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding passed node.</param>
        /// <param name="allowContentElements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The parent at given ancestry level, or null if none found at that level.</returns>
        public static DependencyObject? FindParentByLevel(DependencyObject? node, int level = 1, bool allowContentElements = true)
        {
            if (level < 0) return null;
            var currentLevel = 0;

            while (currentLevel++ < level && node != null)
                node = GetParent(node, allowContentElements);

            return node;
        }

        /// <summary>
        /// Alternative to WPF <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not strictly speaking in the visual tree.</summary>
        /// <param name="node">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent. </remarks>
        public static DependencyObject? GetParentExtended(DependencyObject? node)
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

            // Also try searching for parent in framework elements (such as DockPanel, etc.):
            if (node is FrameworkElement frameworkElement && frameworkElement.Parent != null)
                return frameworkElement.Parent;

            // If it's a visual then rely on VisualTreeHelper:
            return node is Visual || node is Visual3D ? VisualTreeHelper.GetParent(node) : null;
        }
        #endregion

        #region Tree navigation
        private static IEnumerable<DependencyObject> GetChildren(DependencyObject node, bool allowContentElements)
        {
            if (node is Visual || node is Visual3D)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(node);
                for (var i = 0; i < childrenCount; i++)
                    yield return VisualTreeHelper.GetChild(node, i);
            }

            if (allowContentElements)
                foreach (var child in LogicalTreeHelper.GetChildren(node).OfType<ContentElement>())
                    yield return child;
        }

        private static DependencyObject? GetParent(DependencyObject? node, bool allowContentElements)
            => allowContentElements
                ? GetParentExtended(node)
                : node is Visual || node is Visual3D
                    ? VisualTreeHelper.GetParent(node)
                    : null;

        private static bool MatchesType(DependencyObject node, Type type)
            => type.IsInstanceOfType(node);
        #endregion

        #region Check names
        /// <summary>
        /// Checks if a node's name matches exact passed name or regex.
        /// </summary>
        /// <param name="node">The node to check, must be a <see cref="FrameworkElement"/> 
        /// or <see cref="FrameworkContentElement"/> to read the 'Name' property.</param>
        /// <param name="name">The exact name or regex to assess.</param>
        /// <param name="nameMatchMode">Defines how the name filter is interpreted.</param>
        /// <returns>True if node's name matches passed name, false otherwise.</returns>
        private static bool CheckNameMatch(DependencyObject node, string name, NameMatchMode nameMatchMode)
        {
            if (string.IsNullOrEmpty(name)) return false;

            var nodeName = node is FrameworkElement asFe
                ? asFe.Name
                : node is FrameworkContentElement asFce
                    ? asFce.Name
                    : null;
            if (nodeName == null) return false;

            if (nameMatchMode != NameMatchMode.Regex && nodeName == name)
                return true;
            if (nameMatchMode == NameMatchMode.Exact)
                return false;

            try
            {
                return Regex.IsMatch(nodeName, name, RegexOptions.None, NameMatchTimeout);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is RegexMatchTimeoutException)
            { }

            return false;
        }
        #endregion
    }
}
