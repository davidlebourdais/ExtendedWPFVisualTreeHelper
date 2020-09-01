using Xunit;
using System.Windows;
using EMA.ExtendedWPFVisualTreeHelper.Tests.Utils;
using System.Collections.Generic;
using System.Linq;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    public class FindChildrenTests
    {
        #region FindChild
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindChild(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPathOrNearby(xaml);
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindChild");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindChild");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());

                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null });
                if (related_in_path && !has_content_element_in_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = extMethod.Invoke(origin, new object[] { origin, null });
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = method.Invoke(null, new object[] { origin, "End" });
                if (related_in_path && !has_content_element_in_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End" });
                Assert.Same(nammed_result, nammed_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindChildExtended
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindChildExtended(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPathOrNearby(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindChildExtended");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindChildExtended");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());

                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null });
                if (related_in_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = extMethod.Invoke(origin, new object[] { origin, null });
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = method.Invoke(null, new object[] { origin, "End" });
                if (related_in_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End" });
                Assert.Same(nammed_result, nammed_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindDirectChild
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindDirectChild(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expected = TreeHelpers.FindElementByName(tree, "End");

                var in_direct_path = related_in_path && TreeHelpers.FindDirectElementByName(origin, "End", false) != null;
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindDirectChild");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindDirectChild");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());

                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null });
                if (in_direct_path && !has_content_element_in_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = extMethod.Invoke(origin, new object[] { origin, null });
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = method.Invoke(null, new object[] { origin, "End" });
                if (in_direct_path && !has_content_element_in_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End" });
                Assert.Same(nammed_result, nammed_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindDirectChildExtended
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindDirectChildExtended(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expected = TreeHelpers.FindElementByName(tree, "End");

                var in_direct_path = related_in_path && TreeHelpers.FindDirectElementByName(origin, "End") != null;
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindDirectChildExtended");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindDirectChildExtended");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());

                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null });
                if (in_direct_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = extMethod.Invoke(origin, new object[] { origin, null });
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = method.Invoke(null, new object[] { origin, "End" });
                if (in_direct_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End" });
                Assert.Same(nammed_result, nammed_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindAllChildren
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindAllChildren(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expectedSpecificItem = TreeHelpers.FindElementByName(tree, "End");
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // For this test we will check manually that every children are caught,
                // so flatten the tree to get all children of the same type as expected one for the test:
                var flattenTree = TreeHelpers.FindAllVisualChildren(origin, false);  // (method is dissimilar to implementation)
                var targetType = expectedSpecificItem.GetType();
                var expected = flattenTree.Where(x => x.GetType() == targetType);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindAllChildren");
                var method = methodInfo.MakeGenericMethod(targetType);
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindAllChildren");
                var extMethod = extMethodInfo.MakeGenericMethod(targetType);

                // Test:
                var result = method.Invoke(null, new object[] { origin });
                Assert.Equal(expected, result);
                if (related_in_path && !has_content_element_in_path)
                    Assert.Contains(expectedSpecificItem, result as IEnumerable<DependencyObject>);
                else Assert.DoesNotContain(expectedSpecificItem, result as IEnumerable<DependencyObject>);

                // Test extension:
                var extresult = extMethod.Invoke(origin, new object[] { origin });
                Assert.Equal(result, extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindAllChildrenExtended
        [Theory]
        [ClassData(typeof(TestData))]
        public void CanFindAllChildrenExtended(string xaml, bool related_in_path)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start") as FrameworkElement;
                var expectedSpecificItem = TreeHelpers.FindElementByName(tree, "End");

                // For this test we will check manually that every children are caught,
                // so flatten the tree to get all children of the same type as expected one for the test:
                var flattenTree = TreeHelpers.FindAllVisualChildren(origin);  // (method is dissimilar to implementation)
                var targetType = expectedSpecificItem.GetType();
                var expected = flattenTree.Where(x => x.GetType() == targetType);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindAllChildrenExtended");
                var method = methodInfo.MakeGenericMethod(targetType);
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindAllChildrenExtended");
                var extMethod = extMethodInfo.MakeGenericMethod(targetType);

                // Test:
                var result = method.Invoke(null, new object[] { origin });
                Assert.Equal(expected, result);
                if (related_in_path)
                    Assert.Contains(expectedSpecificItem, result as IEnumerable<DependencyObject>);
                else Assert.DoesNotContain(expectedSpecificItem, result as IEnumerable<DependencyObject>);

                // Test extension:
                var extresult = extMethod.Invoke(origin, new object[] { origin });
                Assert.Equal(result, extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion
    }
}
