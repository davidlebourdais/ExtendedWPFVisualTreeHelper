
using Xunit;
using System.Collections.Generic;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    /// <summary>
    /// Holds test data definition for all tests. 
    /// First given item (string) is a Xaml content to be tested, 
    /// second second (bool) indicates if travelling from A to B is possible or not.
    /// </summary>
    public class TestData : TheoryData<string, bool>
    {
        #region Data
        // This contains a XAML representation of Dependency objects to travel through
        // Contains: items nammed 'A' and 'B' which are potential source/destination that are linked (can travel from parent to child and vice-versa).
        // May contain: 
        // - items nammed 'Similar' that represents a type to be encountered twice during travelling from A to B,
        // - items nammed 'ContentElementHolder' that clearly represents items holding a ContentElement within, which are special to travel through.
        private readonly List<string> xaml_valid_test_data = new List<string>() {
            "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<TextBlock Name=\"B\" />" +
            "</Border>",

            "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<StackPanel>" +
                    "<Grid>" +
                        "<TextBlock Name=\"B\" />" +
                        "<TextBlock />" +
                    "</Grid>" +
                "</StackPanel>" +
            "</Border>",

            "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<Border>" +
                    "<TextBox />" +
                "</Border>" +
                "<Grid>" +
                    "<DockPanel>" +
                        "<TextBlock />" +
                        "<TextBlock Name=\"B\" />" +
                    "</DockPanel>" +
                    "<TextBlock />" +
                "</Grid>" +
            "</StackPanel>",

            "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<Button >" +
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

            "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"A\" >" +
                "<ContentControl>" +
                    "<ContentControl.ContentTemplate>" +                   
                        "<DataTemplate>" +
                            "<TextBlock Name=\"B\" Text=\"some text\" />" +
                        "</DataTemplate>" +
                    "</ContentControl.ContentTemplate>" +
                "</ContentControl>" + 
            "</Grid>"
        };

        // Contains a set of data that will always lead to failure as elements 
        // are not related with each other in path:
        private readonly List<string> xaml_invalid_test_data = new List<string>() {
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
                        "<TextBlock />" + 
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
        /// <param name="raw_xaml">Xaml as string to be inspected.</param>
        /// <returns>True if there is a <see cref="ContentElement"/> to be encountered.</returns>
        public static bool HasContentElementWithin(string raw_xaml)
            => raw_xaml.Contains("\"ContentElementHolder"); // can be suffixed (ex: 'ContentElementHolder2')

        /// <summary>
        /// Checks if a type similar to the start node one will be found 
        /// when travelling from start node to last one.
        /// </summary>
        /// <param name="raw_xaml">Xaml as string to be inspected.</param>
        /// <returns>True if there is an object of similar type to be encountered.</returns>
        public static bool HasSimilarTypeInDirectPath(string raw_xaml)
            => raw_xaml.Contains("\"Similar"); // can be suffixed (ex: 'Similar2')
        #endregion

        protected virtual string SetStartEnd(string raw_xml)
            => raw_xml.Replace("\"A\"", "\"Start\"").Replace("\"B\"", "\"End\"");

        public TestData()
        {
            foreach(var xaml in xaml_valid_test_data)
                Add(SetStartEnd(xaml), true);
            foreach(var xaml in xaml_invalid_test_data)
                Add(SetStartEnd(xaml), false);  
        }
    }
}
