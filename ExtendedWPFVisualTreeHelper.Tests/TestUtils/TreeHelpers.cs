using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Linq;
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
        /// <param name="allow_content_elements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The nammed element, if found, null otherwise.</returns>
        public static DependencyObject FindElementByName(DependencyObject node, string name, bool allow_content_elements = true)
        {
            if (node == null) return null;

            // Search in bare tree first:
            var result = (DependencyObject)null;
            if (node is FrameworkElement asFE)
                result = asFE.FindName(name) as DependencyObject;
            else if (node is FrameworkContentElement asFCE)
                result = asFCE.FindName(name) as DependencyObject;

            if (result != null) return result;
            
            // Go to children if not found:
            if (node is Visual || node is Visual3D)
            {
                int count = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < count; i++)
                {
                    result = FindElementByName(VisualTreeHelper.GetChild(node, i), name, allow_content_elements);
                    if (result != null) return result;
                }
            }

            if (allow_content_elements)
            {
                var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in contentElements)
                {
                    result = FindElementByName(child, name, allow_content_elements);
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
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <param name="allow_content_elements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The nammed element, if found, null otherwise.</returns>
        public static DependencyObject FindDirectElementByName(DependencyObject node, string name, bool allow_content_elements = true)
        {
            if (node == null) return null;

            var nodeName = node is FrameworkElement asFE ? asFE.Name : (node is FrameworkContentElement asFCE ? asFCE.Name : null);
            var result = nodeName != null && nodeName == name ? node as DependencyObject : null;
            if (result == null)
            {
                int count = 0;
                if (node is Visual || node is Visual3D)
                    count = VisualTreeHelper.GetChildrenCount(node);
                if (count > 0)
                    result = FindDirectElementByName(VisualTreeHelper.GetChild(node, 0), name, allow_content_elements);
                else if (allow_content_elements)
                {
                    var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                    if (contentElements.FirstOrDefault() is DependencyObject asDO)
                        result = FindDirectElementByName(asDO, name, allow_content_elements);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the number of visual elements that are between a root element
        /// and a nammed element, if existing.
        /// </summary>
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <param name="allow_content_elements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>Depth of the nammed element.</returns>
        public static int GetElementDepthByName(DependencyObject node, string name, bool allow_content_elements = true)
        {
            var element = FindElementByName(node, name, allow_content_elements);
            if (element == null) return -1;
            else
            {
                int depth = 0;
                while (element != null && !element.Equals(node))
                {
                    if (allow_content_elements)
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
        /// <param name="allow_content_elements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>A list containing the children of the passed <see cref="DependencyObject"/>.</returns>
        public static List<DependencyObject> FindVisualChildren(DependencyObject node, bool allow_content_elements = true)
        {
            var results = new List<DependencyObject>();

            if (node == null)
                return results;

            int actual_count = 0;

            if (node is Visual || node is Visual3D)
            {
                int count = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < count; i++) // stack every children
                {
                    var current = VisualTreeHelper.GetChild(node, i);
                    if (current != null)
                    {
                        results.Add(current);
                        actual_count++;
                    }
                }
            }

            if (allow_content_elements)
            {
                var contentElements = LogicalTreeHelper.GetChildren(node).OfType<ContentElement>();
                foreach (var child in contentElements)
                {
                    results.Add(child);
                    actual_count++;
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves the complete descendancy of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="node">The node to start looking from.</param>
        /// <param name="allow_content_elements">Allow travelling through <see cref="ContentElement"/> objects.</param>
        /// <returns>The list of all descendants of the passed <see cref="DependencyObject"/>.</returns>
        public static List<DependencyObject> FindAllVisualChildren(DependencyObject node, bool allow_content_elements = true)
        {
            if (node == null)
                return new List<DependencyObject>();

            var results = FindVisualChildren(node, allow_content_elements);
            int current_count = results.Count;
            while (current_count > 0)
            {
                int total = results.Count;
                int current_count_tmp = 0;
                for (int i = total - current_count; i < total; i++)
                {
                    var results_tmp = FindVisualChildren(results[i], allow_content_elements);
                    results.AddRange(results_tmp);
                    current_count_tmp += results_tmp.Count;
                }
                current_count = current_count_tmp;
            }
            return results;
        }
    }
}
