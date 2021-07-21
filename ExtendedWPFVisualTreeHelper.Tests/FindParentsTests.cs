using Xunit;
using System.Windows;
using EMA.ExtendedWPFVisualTreeHelper.Tests.Utils;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    public class FindParentsTests
    {
        #region FindParent
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParent(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var hasSimilarTypeInPath = TestData.HasSimilarTypeInDirectPath(xaml);
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Build generic methods manually, since the type to seek might change for each data set:
                var methodInfo = typeof(WpfVisualFinders).GetMethod("FindParent");
                var method = methodInfo?.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WpfVisualFindersExtensions).GetMethod("FindParent");
                var extMethod = extMethodInfo?.MakeGenericMethod(expected.GetType());

                // Test unnamed:
                var result = method?.Invoke(null, new object[] { origin, null, allowContentElements });
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                {
                    if (!hasSimilarTypeInPath) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnamed target:
                var extensionResult = extMethod?.Invoke(origin, new object[] { origin, null, allowContentElements });
                Assert.Same(result, extensionResult);

                // Test named:
                var namedResult = method?.Invoke(null, new object[] { origin, "End", allowContentElements });
                if (relatedInPath && (allowContentElements || !hasContentElementInPath)) // should always find if related in path
                    Assert.Same(expected, namedResult);
                else Assert.Null(result);

                // Test with regex:
                var regexResult = method?.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allowContentElements });
                Assert.Same(namedResult, regexResult);

                // Test extension method with named target and regex:
                var namedExtensionResult = extMethod?.Invoke(null, new object[] { origin, "End", allowContentElements });
                var regexExtensionResult = method?.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allowContentElements });
                Assert.Same(namedResult, namedExtensionResult);
                Assert.Same(namedResult, regexExtensionResult);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region FindParentByType
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByType(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var hasSimilarTypeInPath = TestData.HasSimilarTypeInDirectPath(xaml);
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Test unnamed:
                var result = WpfVisualFinders.FindParentByType(origin, expected.GetType(), allowContentElements: allowContentElements);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                {
                    if (!hasSimilarTypeInPath) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnamed target:
                var extensionResult = origin.FindParentByType(expected.GetType(), allowContentElements: allowContentElements);
                Assert.Same(result, extensionResult);

                // Test named:
                var namedResult = WpfVisualFinders.FindParentByType(origin, expected.GetType(), "End", allowContentElements);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath)) // should always find if related in path
                    Assert.Same(expected, namedResult);
                else Assert.Null(result);

                // Test with regex:
                var regexResult = WpfVisualFinders.FindParentByType(origin, expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, regexResult);

                // Test extension method with named target and regex:
                var namedExtensionResult = origin.FindParentByType(expected.GetType(), "End", allowContentElements);
                var regexExtensionResult = origin.FindParentByType(expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, namedExtensionResult);
                Assert.Same(namedResult, regexExtensionResult);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region FindParentByLevel
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByLevel(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var originDepth = TreeHelpers.GetElementDepthByName(tree, "Start", allowContentElements);
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var expectedDepth = TreeHelpers.GetElementDepthByName(tree, "End", allowContentElements);
                var level = originDepth - expectedDepth;
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Test bare method:
                var result = WpfVisualFinders.FindParentByLevel(origin, level, allowContentElements);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                    Assert.Equal(expected, result);
                else Assert.NotEqual(expected, result); // may find something close but not our target.

                // Test extension method:
                var extensionResult = origin.FindParentByLevel(level, allowContentElements);
                Assert.Same(result, extensionResult);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region GetParentExtended
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanGetParentExtended(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                if (!allowContentElements) return; // same result if activated or not.

                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");

                // Run up until reaching top:
                var parent = origin;
                var foundExpected = false;
                do
                {
                    var currentNode = parent;
                    parent =  WpfVisualFinders.GetParentExtended(currentNode);

                    var parentExt = currentNode.GetParentExtended();
                    Assert.Equal(parent, parentExt);

                    if (expected.Equals(parent))
                        foundExpected = true;
                } while (parent != null);

                Assert.Equal(relatedInPath, foundExpected);
            }

            WpfAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion
    }
}
