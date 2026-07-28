using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EMA.ExtendedWPFVisualTreeHelper.Tests.Utils;
using Xunit;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    public class RegressionTests
    {
        [Fact]
        public void RuntimeTypeSearchSupportsInterfaces()
        {
            const string xaml =
                "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                    "<TextBlock Name=\"Target\" />" +
                "</Border>";

            void Inspect(FrameworkElement tree)
            {
                var result = WpfVisualFinders.FindChildByType(tree, typeof(IInputElement), "Target");
                Assert.NotNull(result);
                Assert.IsAssignableFrom<IInputElement>(result);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }

        [Fact]
        public void RuntimeParentTypeSearchSupportsInterfaces()
        {
            const string xaml =
                "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Name=\"Target\">" +
                    "<TextBlock Name=\"Origin\" />" +
                "</Border>";

            void Inspect(FrameworkElement tree)
            {
                var origin = tree.FindChildByType(typeof(FrameworkElement), "Origin");
                var result = WpfVisualFinders.FindParentByType(origin, typeof(IInputElement), "Target");
                Assert.NotNull(result);
                Assert.IsAssignableFrom<IInputElement>(result);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }

        [Fact]
        public void NameMatchModeControlsPatternInterpretation()
        {
            const string xaml =
                "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                    "<TextBlock Name=\"Target\" />" +
                "</Border>";

            void Inspect(FrameworkElement tree)
            {
                var exactResult = tree.FindChild<FrameworkElement>(
                    "T.rget",
                    nameMatchMode: NameMatchMode.Exact);
                var regexResult = tree.FindChild<FrameworkElement>(
                    "T.rget",
                    nameMatchMode: NameMatchMode.Regex);

                Assert.Null(exactResult);
                Assert.NotNull(regexResult);
                Assert.Equal("Target", regexResult.Name);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }

        [Fact]
        public void NameRegexEvaluationHasATimeout()
        {
            void Inspect(FrameworkElement _)
            {
                var child = new FrameworkElement
                {
                    Name = new string('A', 20) + "B"
                };
                var root = new Border { Child = child };

                var stopwatch = Stopwatch.StartNew();
                var result = WpfVisualFinders.FindChild<FrameworkElement>(root, "^(A|AA)+$");
                stopwatch.Stop();

                Assert.Null(result);
                Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));
            }

            WpfAppTester.RunTestInWindow(
                Inspect,
                "<Border xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />",
                timeoutMs: 10000);
        }

        [Fact]
        public void GetParentExtendedReturnsNullForNonVisualDependencyObjects()
        {
            var node = new DependencyObject();

            var result = WpfVisualFinders.GetParentExtended(node);

            Assert.Null(result);
        }
    }
}
