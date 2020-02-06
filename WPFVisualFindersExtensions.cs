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
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type </remarks>
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
        /// Gets a list of all descendance of given type from a dependency object. 
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="startNode">The node where to start looking from.</param>
        /// <param name="results">A list of all found children elements.</param>
        /// <remarks>From: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper</remarks>
        public static void FindAllChildren<T>(this DependencyObject startNode, ref IList<T> results)
        {
            WPFVisualFinders.FindAllChildren<T>(startNode, ref results);
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
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent </remarks>
        public static T FindParent<T>(this DependencyObject child, string name = null)
        {
            return WPFVisualFinders.FindParent<T>(child, name);
        }

        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name 
        /// by also travelling the logical tree when necessary (i.e. when child is a content).
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
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="targetType">The explicit type the parent should have.</param>
        /// <param name="name">Optional name of the parent to find.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByTypeExtended(DependencyObject child, Type targetType, string name = null)
        {
             return WPFVisualFinders.FindParentByTypeExtended(child, targetType, name);
        }

        /// <summary>
        /// Return a parent at a given ancestry level.
        /// </summary>
        /// <param name="child">The node where to start looking from.</param>
        /// <param name="level">The ancestry level the parent is at regarding to passed child.</param>
        /// <returns>The parent at fiven ancestry level, or null if none found at that level.</returns>
        public static DependencyObject FindParentByLevel(DependencyObject child, int level = 1)
        {
            return WPFVisualFinders.FindParentByLevel(child, level);
        }

        /// <summary>
        /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports content element navigation. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!</summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, and null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent </remarks>
        public static DependencyObject GetParentExtended(this DependencyObject child)
        {
            return WPFVisualFinders.GetParentExtended(child);
        }
        #endregion
    }
}
