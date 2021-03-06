using Xunit;
using System.Collections.Generic;
using System.Windows;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    /// <summary>
    /// Holds test data definition for all tests. 
    /// First given item (string) is a Xaml content to be tested, 
    /// second second (bool) indicates if travelling from A to B is possible or not
    /// and third bool indicates if <see cref="ContentElement" /> object travelling is allowed or not.
    /// </summary>
    public class TestData : TheoryData<string, bool, bool>
    {
        #region Data
        // This contains a XAML representation of Dependency objects to travel through
        // Contains: items named 'A' and 'B' which are potential source/destination that are linked (can travel from parent to child and vice-versa).
        // May contain: 
        // - items named 'Similar' that represents a type to be encountered twice during travelling from A to B,
        // - items named 'SameSibling' that represents a similar type to A or B to be encountered among their direct siblings,
        // - items named 'ContentElementHolder' that clearly represents items holding a ContentElement within, which are special to travel through.
        // - items named 'C' which are aimed to be replaced in order to create a secondary 'end' point.
        // Keep in mind that names cannot be reproduced in the xaml' scope, except if defined in a template. 'A', 'B', 'C' must be unique in xaml's scope.
        private readonly List<string> _xamlValidTestData = new List<string>() {
            "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<TextBlock Name=\"B\" />" +
            "</Border>",

            "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<StackPanel>" +
                    "<Grid>" +
                        "<TextBlock Name=\"B\" />" +
                        "<TextBlock Name=\"SameSibling\" />" +
                    "</Grid>" +
                "</StackPanel>" +
            "</Border>",

            "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<Border>" +
                    "<TextBox />" +
                "</Border>" +
                "<Grid>" +
                    "<StackPanel>" +
                        "<TextBlock />" +
                        "<TextBlock />" +
                    "</StackPanel>" +
                    "<TextBlock />" +
                    "<DockPanel>" +
                        "<DockPanel>" +                            
                            "<TextBlock Name=\"SameSibling\" />" +
                            "<TextBlock Name=\"B\" />" +
                        "</DockPanel>" +
                    "</DockPanel>" +
                "</Grid>" +
            "</StackPanel>",

            "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<Button>" +
                    "<Button.Template>" +
                        "<ControlTemplate TargetType=\"Button\" >" +
                            "<TextBlock Name=\"B\" />" +
                        "</ControlTemplate>" +
                    "</Button.Template>" +
                "</Button>" +
            "</Grid>",

            "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<TextBlock Name=\"ContentElementHolder\" >" +
                    "<Run Name=\"B\" />" +
                "</TextBlock>" +
            "</Border>",

            "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<StackPanel Name=\"Similar1\" >" +
                    "<Grid>" +
                        "<DockPanel>" +
                            "<StackPanel Name=\"Similar2\" >" +
                                "<ComboBox Name=\"B\" >" +
                                    "<ComboBoxItem />" +
                                    "<ComboBoxItem />" +
                                "</ComboBox>" +
                            "</StackPanel>" +
                        "</DockPanel>" +
                    "</Grid>" +
                "</StackPanel>" +
            "</StackPanel>",

            "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" >" +
                "<ContentControl Name=\"A\">" +
                    "<ContentControl.ContentTemplate>" +                   
                        "<DataTemplate>" +
                            "<TextBlock Name=\"B\" Text=\"some text\" />" +
                        "</DataTemplate>" +
                    "</ContentControl.ContentTemplate>" +
                "</ContentControl>" + 
            "</Grid>",

            "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<TextBlock Name=\"SameSibling\" />" +
                "<ContentControl>" +
                    "<ContentControl.ContentTemplate>" +                   
                        "<DataTemplate>" +
                            "<TextBlock Name=\"C\" />" +
                        "</DataTemplate>" +
                    "</ContentControl.ContentTemplate>" +
                "</ContentControl>" + 
                "<TextBlock Name=\"B\" />" +
            "</StackPanel>",
        };

        // Contains a set of data that will always lead to failure as elements 
        // are not related with each other in path:
        private readonly List<string> _xamlInvalidTestData = new List<string>() {
            "<ComboBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                "<ComboBoxItem Name=\"A\" />" +
                "<ComboBoxItem Name=\"B\" />" +
            "</ComboBox>",

            "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" >" +
                "<Border>" + 
                    "<TextBox Name=\"A\"/>" + 
                "</Border>" +
                "<Grid>" + 
                    "<DockPanel>" + 
                        "<TextBlock Name=\"SameSibling\" />" + 
                        "<TextBlock Name=\"B\" />" + 
                    "</DockPanel>" + 
                    "<TextBlock />" + 
                "</Grid>" + 
            "</StackPanel>"
        };
        #endregion

        #region Utils
        /// <summary>
        /// Checks if a <see cref="ContentElement"/> might be encountered
        /// when travelling from start node to last one.
        /// </summary>
        /// <param name="rawXaml">Xaml as string to be inspected.</param>
        /// <returns>True if there is a <see cref="ContentElement"/> to be encountered.</returns>
        public static bool HasContentElementWithin(string rawXaml)
            => rawXaml.Contains("\"ContentElementHolder"); // can be suffixed (ex: 'ContentElementHolder2')

        /// <summary>
        /// Checks if a type similar to the start or end node will be found 
        /// when travelling from start to end and vice-versa.
        /// </summary>
        /// <param name="rawXaml">Xaml as string to be inspected.</param>
        /// <returns>True if there is an object of similar type to be encountered.</returns>
        public static bool HasSimilarTypeInDirectPath(string rawXaml)
            => rawXaml.Contains("\"Similar"); // can be suffixed (ex: 'Similar2')

        /// <summary>
        /// Checks if a type similar to the start or end node will be found 
        /// when travelling from start to end and vice-versa and if any siblings
        /// at the start or end level has a similar type.
        /// </summary>
        /// <param name="rawXaml">Xaml as string to be inspected.</param>
        /// <returns>True if there is an object of similar type to be encountered.</returns>
        public static bool HasSimilarTypeInDirectPathOrNearby(string rawXaml)
            => rawXaml.Contains("\"Similar") || rawXaml.Contains("\"SameSibling");

        /// <summary>
        /// Sets end point on any elements named "C".
        /// </summary>
        /// <param name="rawXaml">Xaml as string to be modified.</param>
        /// <returns>The raw xaml containing new end points if any predefined..</returns>
        public static string SetMultipleEnd(string rawXaml)
            => rawXaml.Replace("\"C\"", "\"End\"");
        #endregion

        protected virtual string SetStartEnd(string rawXaml)
            => rawXaml.Replace("\"A\"", "\"Start\"").Replace("\"B\"", "\"End\"");
        
        public TestData()
        {
            foreach(var xaml in _xamlValidTestData)
            {
                Add(SetStartEnd(xaml), true, false);  // one without content elements.
                Add(SetStartEnd(xaml), true, true);   // one with.
            }
            foreach(var xaml in _xamlInvalidTestData)
            {
                Add(SetStartEnd(xaml), false, false);
                Add(SetStartEnd(xaml), false, true);
            }
        }
    }
}
