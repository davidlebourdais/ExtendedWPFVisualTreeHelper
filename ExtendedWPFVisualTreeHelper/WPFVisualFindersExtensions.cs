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
        /// Finds a child of in the visual tree using its type and (optionnaly) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing.</returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type. </remarks>
        public static T FindChild<T>(this DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindChild<T>(node, name, allow_content_elements);
        }

        /// <summary>
        /// Finds a child of in the visual tree using its type and (optionnaly) its name and with
        /// the ability to travel through <see cref="ContentElement"/> objects while exploring the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing.</returns>
        public static DependencyObject FindChildByType(this DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindChildByType(node, type, name, allow_content_elements);
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
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or default if none existing in the direct path.</returns>
        public static T FindDirectChild<T>(this DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindDirectChild<T>(node, name, allow_content_elements);
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
        /// <param name="name">Optional name or regex that matches name of the child to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>A matching child, or null if none existing in the direct path.</returns>
        public static DependencyObject FindDirectChildByType(this DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindDirectChildByType(node, type, name, allow_content_elements);
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">An optional name or regex pattern to be used for filtering during search.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match method type.</returns>
        /// <remarks>Inspired from: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.visualtreehelper 
        /// and https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type. </remarks>
        public static IEnumerable<T> FindAllChildren<T>(this DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindAllChildren<T>(node, name, allow_content_elements);
        }

        /// <summary>
        /// Gets the filtered-by-type complete descendancy of a given dependency object with 
        /// the ability to travel through <see cref="ContentElement"/> objects while walking down the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">Type of the child to find.</param>
        /// <param name="name">An optional name or regex pattern to be used for filtering during search.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>All found children elements that match passed type.</returns>
        public static IEnumerable<DependencyObject> FindAllChildrenByType(this DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindAllChildrenByType(node, type, name, allow_content_elements);
        }
        #endregion

        #region Find parents
        /// <summary>
        /// Finds a parent that matches static type and (optionnaly) the passed name 
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <typeparam name="T">Type of the obect to find.</typeparam>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="name">Optional name or regex that matches name of the parent to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static T FindParent<T>(this DependencyObject node, string name = null, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindParent<T>(node, name, allow_content_elements);
        }

        /// <summary>
        /// Finds a parent that matches passed target (and dynamically defined) type and (optionnaly) a passed name
        /// with the ability to travel through <see cref="ContentElement"/> objects while walking up the visual tree.
        /// </summary>
        /// <param name="node">The node where to start looking from.</param>
        /// <param name="type">The explicit type the parent should have.</param>
        /// <param name="name">Optional name or regex that matches name of the parent to find.</param>
        /// <param name="allow_content_elements">Enables or disables the ability to go through <see cref="ContentElement"/> objects,
        /// thus allowing or forbidding logical tree travels for these items.</param>
        /// <returns>The matching parent, or null if none.</returns>
        public static DependencyObject FindParentByType(this DependencyObject node, Type type, string name = null, bool allow_content_elements = true)
        {
             return WPFVisualFinders.FindParentByType(node, type, name, allow_content_elements);
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
        public static DependencyObject FindParentByLevel(this DependencyObject node, int level = 1, bool allow_content_elements = true)
        {
            return WPFVisualFinders.FindParentByLevel(node, level, allow_content_elements);
        }

        /// <summary>
        /// Alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method, 
        /// which also supports navigation through <see cref="ContentElement"/> objects that
        /// are not stictly speaking in the visual tree.</summary>
        /// <param name="node">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available, null otherwise.</returns>
        /// <remarks>Adapted from http://www.hardcodet.net/2008/02/find-wpf-parent. </remarks>
        public static DependencyObject GetParentExtended(this DependencyObject node)
        {
            return WPFVisualFinders.GetParentExtended(node);
        }
        #endregion
    }
}
