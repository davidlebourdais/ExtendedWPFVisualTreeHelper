using System.Windows;
using System.Windows.Media;

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
        /// <returns>The nammed element, if found.</returns>
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
        /// Returns the number of visual elements that are between a root element
        /// and a nammed element, if existing.
        /// </summary>
        /// <param name="node">Starting node where to look for the nammed element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>Depth of the nammed element.</returns>
        public static int GetElementDepthByName(FrameworkElement root, string name)
        {
            var element = FindElementByName(root, name);
            if (element == null) return -1;
            else
            {
                int depth = 0;
                while (element != null && !element.Equals(root))
                {
                    element = element.GetParentExtended();
                    depth++;
                }
                return depth;
            }
        }
    }
}
