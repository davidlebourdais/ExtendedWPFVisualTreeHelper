using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace EMA.ExtendedWPFVisualTreeHelper
{
    /// <summary>
    /// Provides a set of extensions on <see cref="DependencyObject"/> to navigate a visual tree 
    /// and either find a specific descendant or ancestor.
    /// </summary>
    public static class WPFVisualFindersExtensions
    {
        #region Find children
        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionnaly) its name.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="child_name">Name of the child to find.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type.</remarks>
        public static T FindChild<T>(this DependencyObject startNode, string child_name = null)
        {
            return WPFVisualFinders.FindChild<T>(startNode, child_name);
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
        public static T FindDirectChild<T>(this DependencyObject startNode, string child_name = "")
        {
            return WPFVisualFinders.FindDirectChild<T>(startNode, child_name);
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object. 
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type.</remarks>
        public static IEnumerable<T> FindAllChildren<T>(this DependencyObject startNode)
        {
            return WPFVisualFinders.FindAllChildren<T>(startNode);
        }
        #endregion

        #region Find parent
        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent.</remarks>
        public static T FindParent<T>(this DependencyObject child, string name = null)
        {
            return WPFVisualFinders.FindParent<T>(child, name);
        }

        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name 
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T FindParentExtended<T>(this DependencyObject child, string name = null)
        {
            return WPFVisualFinders.FindParentExtended<T>(child, name);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="targetType">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByType(this DependencyObject child, Type targetType, string name = null)
        {
             return WPFVisualFinders.FindParentByType(child, targetType, name);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="targetType">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByTypeExtended(this DependencyObject child, Type targetType, string name = null)
        {
             return WPFVisualFinders.FindParentByTypeExtended(child, targetType, name);
        }

        /// <summary>
        /// Return a parent at a given ancestry level.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed child.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevel(this DependencyObject child, int level = 1)
        {
            return WPFVisualFinders.FindParentByLevel(child, level);
        }

        /// <summary>
        /// Return a parent at a given ancestry level with the ability to travel through 
        /// <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed child.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevelExtended(this DependencyObject child, int level = 1)
        {
            return WPFVisualFinders.FindParentByLevelExtended(child, level);
        }

        /// <summary>
        /// Alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not stictly speaking in the visual tree.</summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent.</remarks>
        public static DependencyObject GetParentExtended(this DependencyObject child)
        {
            return WPFVisualFinders.GetParentExtended(child);
        }
        #endregion
    }
}
