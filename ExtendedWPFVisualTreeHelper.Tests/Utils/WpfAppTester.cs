using Xunit;
using System;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests.Utils
{
    /// <summary>
    /// Offers ways to run a test from a dedicated <see cref="Window"/>.
    /// </summary>
    public static class WpfAppTester
    {
        /// <summary>
        /// Parses a Xaml content.
        /// </summary>
        /// <param name="xaml">Raw xaml to be parsed.</param>
        /// <returns>The result of the Xaml parsing.</returns>
        public static object LoadFromXaml(string xaml)
        {
            using var stringReader = new StringReader(xaml);
            using var xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader);
        }

        /// <summary>
        /// Runs a test in a <see cref="Window"/> launched from a separate thread.
        /// </summary>
        /// <param name="testMethod">Test method to be invoked. The window content will be injected as <see cref="FrameworkElement"/> parameter.</param>
        /// <param name="windowContent">Content of the window, should be a raw Xaml content as string or a valid content for the window.</param>
        /// <param name="showWindow">Optionally shows generated window for the tests.</param>
        /// <param name="timeoutMs">Optionally sets a timeout in milliseconds to be triggered for abnormally pending tests.</param>
        public static void RunTestInWindow(Action<FrameworkElement> testMethod, object windowContent, bool showWindow = false, int timeoutMs = 200000)
        {
            var exceptionInfo = (ExceptionDispatchInfo)null;

            var worker = new ThreadStart(() =>
            {
                var window = new Window
                {
                    // Load content into our window:
                    Content = windowContent is string xaml ? LoadFromXaml(xaml) : windowContent
                };

                if (!showWindow)
                {
                    window.Height = window.Width = 0;
                    window.ShowInTaskbar = false;
                    window.WindowStyle = WindowStyle.None;
                }

                // Once window with our content is loaded, execute test:
                window.Loaded += (_, _) =>
                {
                    try
                    {
                        testMethod.Invoke(window);
                    }
                    catch (Exception e) // register any exception during test execution
                    {
                        exceptionInfo = ExceptionDispatchInfo.Capture(e);
                    }

                    window.Close();
                };

                window.Show();
            });

            // Start worker:
            var testThread = new Thread(worker);
            testThread.SetApartmentState(ApartmentState.STA);
            testThread.Start();

            // Timeout if worker is not ending quickly enough:
            var joined = testThread.Join(TimeSpan.FromMilliseconds(timeoutMs));
            Assert.True(joined, "Test thread did not respond");

            // Rethrow exception caught in our worker if any: 
            if (exceptionInfo != null)
                exceptionInfo.Throw();
        }
    }
}
