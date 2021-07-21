using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests.Utils
{
    /// <summary>
    /// Offers methods to get element by name.
    /// </summary>
    public static class TreeHelpers
    {
        /// <summary>
        /// Finds an element from a visual tree by its name.
        /// </summary>
        /// <param name="node">Starting node where to look for the named element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The named element, if found, null otherwise.</returns>
        public static DependencyObject FindElementByName(DependencyObject node, string name, bool allowContentElements = true)
        {
            if (node == null) return null;

            // Search in bare tree first:
            var result = (DependencyObject)null;
            if (node is FrameworkElement asFe)
                result = asFe.FindName(name) as DependencyObject;
            else if (node is FrameworkContentElement asFce)
                result = asFce.FindName(name) as DependencyObject;

            if (result != null) return result;
            
            // Go to children if not found:
            if (node is Visual || node is Visual3D)
            {
                var count = VisualTreeHelper.GetChildrenCount(node);
                for (var i = 0; i < count; i++)
                {
                    result = FindElementByName(VisualTreeHelper.GetChild(node, i), name, allowContentElements);
                    if (result != null) return result;
                }
            }

            if (allowContentElements)
            {
                var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in contentElements)
                {
                    result = FindElementByName(child, name);
                    if (result != null) return result;
                }
            }

            return null; // here if not found
        }

        /// <summary>
        /// Finds an element from a visual tree by its name. This element must be in the direct path 
        /// of the current element to be found otherwise null is returned. Check <see cref="FindElementByName"/> to 
        /// search element in all paths.
        /// </summary>
        /// <param name="node">Starting node where to look for the named element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The named element, if found, null otherwise.</returns>
        public static DependencyObject FindDirectElementByName(DependencyObject node, string name, bool allowContentElements = true)
        {
            if (node == null) return null;

            var nodeName = node is FrameworkElement asFe ? asFe.Name : (node is FrameworkContentElement asFce ? asFce.Name : null);
            var result = nodeName != null && nodeName == name ? node : null;
            if (result == null)
            {
                var count = 0;
                if (node is Visual || node is Visual3D)
                    count = VisualTreeHelper.GetChildrenCount(node);
                if (count > 0)
                    result = FindDirectElementByName(VisualTreeHelper.GetChild(node, 0), name, allowContentElements);
                else if (allowContentElements)
                {
                    var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                    if (contentElements.FirstOrDefault() is DependencyObject asDo)
                        result = FindDirectElementByName(asDo, name);
                }
            }

            return result;
        }

        /// <summary>
        /// Finds an element from a visual tree by its type. This element must be in the direct path 
        /// of the current element to be found otherwise null is returned.
        /// </summary>
        /// <param name="node">Starting node where to look for the named element.</param>
        /// <param name="type">Type of the element to find.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The named element, if found, null otherwise.</returns>
        public static DependencyObject FindDirectElementByType(DependencyObject node, Type type, bool allowContentElements = true)
        {
            if (node == null) return null;

            var result = node.GetType().Equals(type) || node.GetType().GetTypeInfo().IsSubclassOf(type) ? node : null;
            if (result == null)
            {
                int count = 0;
                if (node is Visual || node is Visual3D)
                    count = VisualTreeHelper.GetChildrenCount(node);
                if (count > 0)
                    result = FindDirectElementByType(VisualTreeHelper.GetChild(node, 0), type, allowContentElements);
                else if (allowContentElements)
                {
                    var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                    if (contentElements.FirstOrDefault() is DependencyObject asDo)
                        result = FindDirectElementByType(asDo, type);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the number of visual elements that are between a root element
        /// and a named element, if existing.
        /// </summary>
        /// <param name="node">Starting node where to look for the named element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>Depth of the named element.</returns>
        public static int GetElementDepthByName(DependencyObject node, string name, bool allowContentElements = true)
        {
            var element = FindElementByName(node, name, allowContentElements);
            if (element == null) return -1;
            else
            {
                var depth = 0;
                while (element != null && !element.Equals(node))
                {
                    if (allowContentElements)
                        element = element.GetParentExtended();
                    else if (element is Visual || element is Visual3D)
                        element = VisualTreeHelper.GetParent(element);
                    else element = null;

                    depth++;
                }
                return depth;
            }
        }

        /// <summary>
        /// Finds first-level visual children of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="node">The node to start looking from.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>A list containing the children of the passed <see cref="DependencyObject"/>.</returns>
        private static List<DependencyObject> FindVisualChildren(DependencyObject node, bool allowContentElements = true)
        {
            var results = new List<DependencyObject>();

            if (node == null)
                return results;

            if (node is Visual || node is Visual3D)
            {
                var count = VisualTreeHelper.GetChildrenCount(node);
                for (var i = 0; i < count; i++) // stack every children
                {
                    var current = VisualTreeHelper.GetChild(node, i);
                    results.Add(current);
                }
            }

            if (allowContentElements)
            {
                var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in contentElements)
                {
                    results.Add(child);
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves the complete lineage of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="node">The node to start looking from.</param>
        /// <param name="allowContentElements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The list of all descendants of the passed <see cref="DependencyObject"/>.</returns>
        public static List<DependencyObject> FindAllVisualChildren(DependencyObject node, bool allowContentElements = true)
        {
            if (node == null)
                return new List<DependencyObject>();

            var results = FindVisualChildren(node, allowContentElements);
            var currentCount = results.Count;
            while (currentCount > 0)
            {
                var total = results.Count;
                var currentCountTmp = 0;
                for (var i = total - currentCount; i < total; i++)
                {
                    var resultsTmp = FindVisualChildren(results[i], allowContentElements);
                    results.AddRange(resultsTmp);
                    currentCountTmp += resultsTmp.Count;
                }
                currentCount = currentCountTmp;
            }
            return results;
        }
    }
}
