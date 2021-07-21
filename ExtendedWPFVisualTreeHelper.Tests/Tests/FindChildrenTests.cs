using Xunit;
using System.Windows;
using EMA.ExtendedWPFVisualTreeHelper.Tests.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    public class FindChildrenTests
    {
        #region FindChild
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindChild(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var hasSimilarTypeInPath = TestData.HasSimilarTypeInDirectPathOrNearby(xaml);
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Build generic methods manually, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindChild");
                var method = methodInfo?.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindChild");
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

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region FindChildByType
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindChildByType(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var hasSimilarTypeInPath = TestData.HasSimilarTypeInDirectPathOrNearby(xaml);
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Test unnamed:
                var result = WPFVisualFinders.FindChildByType(origin, expected.GetType(), allowContentElements: allowContentElements);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                {
                    if (!hasSimilarTypeInPath) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnamed target:
                var extensionResult = origin.FindChildByType(expected.GetType(), allowContentElements: allowContentElements);
                Assert.Same(result, extensionResult);

                // Test named:
                var namedResult = WPFVisualFinders.FindChildByType(origin, expected.GetType(), "End", allowContentElements);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath)) // should always find if related in path
                    Assert.Same(expected, namedResult);
                else Assert.Null(namedResult);

                // Test with regex:
                var regexResult = WPFVisualFinders.FindChildByType(origin, expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, regexResult);

                // Test extension method with named target and regex:
                var namedExtensionResult = origin.FindChildByType(expected.GetType(), "End", allowContentElements);
                var regexExtensionResult = origin.FindChildByType(expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, namedExtensionResult);
                Assert.Same(namedResult, regexExtensionResult);
            }

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region FindDirectChild
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindDirectChild(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expected = TreeHelpers.FindElementByName(tree, "End");

                var inDirectPath = relatedInPath && TreeHelpers.FindDirectElementByName(origin, "End", allowContentElements) != null;
                var directSimilar = TreeHelpers.FindDirectElementByType(origin, expected.GetType(), allowContentElements);
                var hasSimilarTypeInDirectPath = directSimilar != null && directSimilar != origin;
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Build generic methods manually, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindDirectChild");
                var method = methodInfo?.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindDirectChild");
                var extMethod = extMethodInfo?.MakeGenericMethod(expected.GetType());

                // Test unnamed:
                var result = method?.Invoke(null, new object[] { origin, null, allowContentElements });
                if (hasSimilarTypeInDirectPath && (allowContentElements || !hasContentElementInPath)) // here if caught an intermediary node
                     Assert.Equal(expected.GetType(), result?.GetType());
                else if (inDirectPath && (allowContentElements || !hasContentElementInPath)) // should find destination if not caught a similar type.
                    Assert.Same(expected, result);
                else Assert.Null(result);

                // Test extension method with unnamed target:
                var extensionResult = extMethod?.Invoke(origin, new object[] { origin, null, allowContentElements });
                Assert.Same(result, extensionResult);

                // Test named:
                var namedResult = method?.Invoke(null, new object[] { origin, "End", allowContentElements });
                if (inDirectPath && (allowContentElements || !hasContentElementInPath)) // should always find if related in path
                    Assert.Same(expected, namedResult);
                else Assert.Null(namedResult);
                
                // Test with regex:
                var regexResult = method?.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allowContentElements });
                Assert.Same(namedResult, regexResult);

                // Test extension method with named target and regex:
                var namedExtensionResult = extMethod?.Invoke(null, new object[] { origin, "End", allowContentElements });
                var regexExtensionResult = method?.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allowContentElements });
                Assert.Same(namedResult, namedExtensionResult);
                Assert.Same(namedResult, regexExtensionResult);
            }

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion
        
        #region FindDirectChildByType
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindDirectChildByType(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expected = TreeHelpers.FindElementByName(tree, "End");

                var inDirectPath = relatedInPath && TreeHelpers.FindDirectElementByName(origin, "End", allowContentElements) != null;
                var directSimilar = TreeHelpers.FindDirectElementByType(origin, expected.GetType(), allowContentElements);
                var hasSimilarTypeInDirectPath = directSimilar != null && directSimilar != origin;
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // Test unnamed:
                var result = WPFVisualFinders.FindDirectChildByType(origin, expected.GetType(), allowContentElements: allowContentElements);
                if (hasSimilarTypeInDirectPath && (allowContentElements || !hasContentElementInPath)) // here if caught an intermediary node
                     Assert.Equal(expected.GetType(), result?.GetType());
                else if (inDirectPath && (allowContentElements || !hasContentElementInPath)) // should find destination if not caught a similar type.
                    Assert.Same(expected, result);
                else Assert.Null(result);

                // Test extension method with unnamed target:
                var extensionResult = origin.FindDirectChildByType(expected.GetType(), allowContentElements: allowContentElements);
                Assert.Same(result, extensionResult);

                // Test named:
                var namedResult = WPFVisualFinders.FindDirectChildByType(origin, expected.GetType(), "End", allowContentElements);
                if (inDirectPath && (allowContentElements || !hasContentElementInPath)) // should always find if related in path
                    Assert.Same(expected, namedResult);
                else Assert.Null(namedResult);

                // Test with regex:
                var regexResult = WPFVisualFinders.FindDirectChildByType(origin, expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, regexResult);

                // Test extension method with named target and regex:
                var namedExtensionResult = origin.FindDirectChildByType(expected.GetType(), "End", allowContentElements);
                var regexExtensionResult = origin.FindDirectChildByType(expected.GetType(), @"E[a-z]\D{1}", allowContentElements);
                Assert.Same(namedResult, namedExtensionResult);
                Assert.Same(namedResult, regexExtensionResult);
            }

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion

        #region FindAllChildren
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindAllChildren(string xaml, bool relatedInPath, bool allowContentElements)
        {
            // For this test, allow multiple end-points:
            xaml = TestData.SetMultipleEnd(xaml);

            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expectedSpecificItem = TreeHelpers.FindElementByName(tree, "End");
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // For this test we will check manually that every children are caught,
                // so flatten the tree to get all children of the same type as expected one for the test:
                var flattenTree = TreeHelpers.FindAllVisualChildren(origin, allowContentElements); // (method is dissimilar to implementation)
                var targetType = expectedSpecificItem.GetType();
                var expected = flattenTree.Where(x => x.GetType() == targetType).ToArray();
                var expectedNamed = expected.Where(x => 
                                                       x is FrameworkElement asFe && asFe.Name == "End" ||
                                                       x is FrameworkContentElement asFce && asFce.Name == "End");

                // For this test, target nodes marked as 'siblings' as we may find more than ones with 'End':
                var expectedRegex = expected.Where(x => 
                                                       x is FrameworkElement asFe && Regex.IsMatch(asFe.Name, ".*[A-Z]ibling.*") || 
                                                       x is FrameworkContentElement asFce && Regex.IsMatch(asFce.Name, ".*[A-Z]ibling.*"));

                // Build generic methods manually, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindAllChildren");
                var method = methodInfo?.MakeGenericMethod(targetType);
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindAllChildren");
                var extMethod = extMethodInfo?.MakeGenericMethod(targetType);

                // Test unnamed:
                var result = method?.Invoke(null, new object[] { origin, null, allowContentElements });
                Assert.Equal(expected, result);
                Assert.NotNull(result as IEnumerable<DependencyObject>);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                    Assert.Contains(expectedSpecificItem, result as IEnumerable<DependencyObject>);
                else Assert.DoesNotContain(expectedSpecificItem, result as IEnumerable<DependencyObject>);

                // Test extension with unnamed targets:
                var extensionResult = extMethod?.Invoke(origin, new object[] { origin, null, allowContentElements });
                Assert.Equal(result, extensionResult);

                // Test named:
                result = method.Invoke(null, new object[] { origin, "End", allowContentElements });
                Assert.Equal(expectedNamed, result);
                Assert.NotNull(result as IEnumerable<DependencyObject>);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                    Assert.Contains(expectedSpecificItem, result as IEnumerable<DependencyObject>);
                else Assert.DoesNotContain(expectedSpecificItem, result as IEnumerable<DependencyObject>);

                // Test extension with named targets:
                extensionResult = extMethod?.Invoke(origin, new object[] { origin, "End", allowContentElements });
                Assert.Equal(result, extensionResult);
                
                // Test regex pattern on similar types named "SimilarSiblings":
                result = WPFVisualFinders.FindAllChildrenByType(origin, targetType, ".*[A-Z]ibling.*", allowContentElements);
                Assert.Equal(expectedRegex, result);

                // Test extension with regex pattern:
                extensionResult = origin.FindAllChildrenByType(targetType, ".*[A-Z]ibling.*", allowContentElements);
                Assert.Equal(result, extensionResult);
            }

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion
        
        #region FindAllChildrenByType
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindAllChildrenByType(string xaml, bool relatedInPath, bool allowContentElements)
        {
            void Inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expectedSpecificItem = TreeHelpers.FindElementByName(tree, "End");
                var hasContentElementInPath = TestData.HasContentElementWithin(xaml);

                // For this test we will check manually that every children are caught,
                // so flatten the tree to get all children of the same type as expected one for the test:
                var flattenTree = TreeHelpers.FindAllVisualChildren(origin, allowContentElements); // (method is dissimilar to implementation)
                var targetType = expectedSpecificItem.GetType();
                var expected = flattenTree.Where(x => x.GetType() == targetType).ToArray();
                var expectedNamed = expected.Where(x =>
                                                       x is FrameworkElement asFe && asFe.Name == "End" ||
                                                       x is FrameworkContentElement asFce && asFce.Name == "End");

                // For this test, target nodes marked as 'siblings' as we may find more than ones with 'End':
                var expectedRegex = expected.Where(x => 
                                                       x is FrameworkElement asFe && Regex.IsMatch(asFe.Name, ".*[A-Z]ibling.*") ||
                                                       x is FrameworkContentElement asFce && Regex.IsMatch(asFce.Name, ".*[A-Z]ibling.*"));

                // Test unnamed:
                var result = WPFVisualFinders.FindAllChildrenByType(origin, targetType, allowContentElements: allowContentElements).ToArray();
                Assert.Equal(expected, result);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                    Assert.Contains(expectedSpecificItem, result);
                else Assert.DoesNotContain(expectedSpecificItem, result);

                // Test extension with unnamed targets:
                var extensionResult = origin.FindAllChildrenByType(targetType, allowContentElements: allowContentElements);
                Assert.Equal(result, extensionResult);

                // Test named:
                result = WPFVisualFinders.FindAllChildrenByType(origin, targetType, "End", allowContentElements).ToArray();
                Assert.Equal(expectedNamed, result);
                if (relatedInPath && (allowContentElements || !hasContentElementInPath))
                    Assert.Contains(expectedSpecificItem, result);
                else Assert.DoesNotContain(expectedSpecificItem, result);

                // Test extension with named targets:
                extensionResult = origin.FindAllChildrenByType(targetType, "End", allowContentElements);
                Assert.Equal(result, extensionResult);

                // Test regex pattern on similar types named "SameSibling":
                result = WPFVisualFinders.FindAllChildrenByType(origin, targetType, ".*[A-Z]ibling.*", allowContentElements).ToArray();
                Assert.Equal(expectedRegex, result);

                // Test extension with regex pattern:
                extensionResult = origin.FindAllChildrenByType(targetType, ".*[A-Z]ibling.*", allowContentElements);
                Assert.Equal(result, extensionResult);
            }

            WPFAppTester.RunTestInWindow(Inspect, xaml);
        }
        #endregion
    }
}
