using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

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
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>The nammed element, if found, null otherwise.</returns>
        public static DependencyObject FindElementByName(FrameworkElement node, string name)
        {
            if (node == null) return null;

            var result = (DependencyObject)node.FindName(name); // search in bare tree
            if (result == null)
            {
                int count = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < count; i++)
                {
                    result = FindElementByName(VisualTreeHelper.GetChild(node, i) as FrameworkElement, name);
                    if (result != null)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Finds an element from a visual tree by its name. This element must be in the direct path 
        /// of the current element to be found otherwise null is returned. Check <see cref="FindElementByName"/> to 
        /// search element in all paths.
        /// </summary>
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>The nammed element, if found, null otherwise.</returns>
        public static DependencyObject FindDirectElementByName(FrameworkElement node, string name)
        {
            if (node == null) return null;

            var result = node.Name == name ? node as DependencyObject : null;
            if (result == null)
            {
                int count = VisualTreeHelper.GetChildrenCount(node);
                if (count > 0)
                    result = FindDirectElementByName(VisualTreeHelper.GetChild(node, 0) as FrameworkElement, name);
            }

            return result;
        }

        /// <summary>
        /// Returns the number of visual elements that are between a root element
        /// and a nammed element, if existing.
        /// </summary>
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>Depth of the nammed element.</returns>
        public static int GetElementDepthByName(FrameworkElement node, string name)
        {
            var element = FindElementByName(node, name);
            if (element == null) return -1;
            else
            {
                int depth = 0;
                while (element != null && !element.Equals(node))
                {
                    element = element.GetParentExtended();
                    depth++;
                }
                return depth;
            }
        }

        /// <summary>
        /// Finds first-level visual children of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="startNode">The node to start looking from.</param>
        /// <returns>A list containing the children of the passed <see cref="DependencyObject"/>.</returns>
        public static List<DependencyObject> FindVisualChildren(DependencyObject startNode)
        {
            var results = new List<DependencyObject>();

            if (startNode == null)
                return results;

            int count = VisualTreeHelper.GetChildrenCount(startNode);
            int actual_count = 0;
            for (int i = 0; i < count; i++) // stack every children
            {
                var current = VisualTreeHelper.GetChild(startNode, i);
                if (current != null)
                {
                    results.Add(current);
                    actual_count++;
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves the complete descendancy of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="startNode">The node to start looking from.</param>
        /// <returns>The list of all descendants of the passed <see cref="DependencyObject"/>.</returns>
        public static List<DependencyObject> FindAllVisualChildren(DependencyObject startNode)
        {
            if (startNode == null)
                return new List<DependencyObject>();

            var results = FindVisualChildren(startNode);
            int current_count = results.Count;
            while (current_count > 0)
            {
                int total = results.Count;
                int current_count_tmp = 0;
                for (int i = total - current_count; i < total; i++)
                {
                    var results_tmp = FindVisualChildren(results[i]);
                    results.AddRange(results_tmp);
                    current_count_tmp += results_tmp.Count;
                }
                current_count = current_count_tmp;
            }
            return results;
        }
    }
}
