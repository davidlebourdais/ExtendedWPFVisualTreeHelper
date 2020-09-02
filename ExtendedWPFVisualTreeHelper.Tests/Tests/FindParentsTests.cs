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
        public void CanFindParent(string xaml, bool related_in_path, bool allow_content_elements)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindParent");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindParent");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());

                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null, allow_content_elements });
                if (related_in_path && (allow_content_elements || !has_content_element_in_path))
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = extMethod.Invoke(origin, new object[] { origin, null, allow_content_elements });
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = method.Invoke(null, new object[] { origin, "End", allow_content_elements });
                if (related_in_path && (allow_content_elements || !has_content_element_in_path)) // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test with regex:
                var regex_result = method.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allow_content_elements });
                Assert.Same(nammed_result, regex_result);

                // Test extension method with nammed target and regex:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End", allow_content_elements });
                var regex_extresult = method.Invoke(null, new object[] { origin, @"E[a-z]\D{1}", allow_content_elements });
                Assert.Same(nammed_result, nammed_extresult);
                Assert.Same(nammed_result, regex_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByType
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByType(string xaml, bool related_in_path, bool allow_content_elements)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // Test unnammed:
                var result = WPFVisualFinders.FindParentByType(origin, expected.GetType(), allow_content_elements: allow_content_elements);
                if (related_in_path && (allow_content_elements || !has_content_element_in_path))
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = origin.FindParentByType(expected.GetType(), allow_content_elements: allow_content_elements);
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = WPFVisualFinders.FindParentByType(origin, expected.GetType(), "End", allow_content_elements);
                if (related_in_path && (allow_content_elements || !has_content_element_in_path)) // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);

                // Test with regex:
                var regex_result = WPFVisualFinders.FindParentByType(origin, expected.GetType(), @"E[a-z]\D{1}", allow_content_elements);
                Assert.Same(nammed_result, regex_result);

                // Test extension method with nammed target and regex:
                var nammed_extresult = origin.FindParentByType(expected.GetType(), "End", allow_content_elements);
                var regex_extresult = origin.FindParentByType(expected.GetType(), @"E[a-z]\D{1}", allow_content_elements);
                Assert.Same(nammed_result, nammed_extresult);
                Assert.Same(nammed_result, regex_extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByLevel
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByLevel(string xaml, bool related_in_path, bool allow_content_elements)
        {
            void inspect(FrameworkElement tree)
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var origin_depth = TreeHelpers.GetElementDepthByName(tree, "Start", allow_content_elements);
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var expected_depth = TreeHelpers.GetElementDepthByName(tree, "End", allow_content_elements);
                var level = origin_depth - expected_depth;
                var has_content_element_in_path = TestData.HasContentElementWithin(xaml);

                // Test bare method:
                var result = WPFVisualFinders.FindParentByLevel(origin, level, allow_content_elements);
                if (related_in_path && (allow_content_elements || !has_content_element_in_path))
                    Assert.Equal(expected, result);
                else Assert.NotEqual(expected, result); // may find something close but not our target.

                // Test extension method:
                var extresult = origin.FindParentByLevel(level, allow_content_elements);
                Assert.Same(result, extresult);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region GetParentExtended
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanGetParentExtended(string xaml, bool related_in_path, bool allow_content_elements)
        {
            void inspect(FrameworkElement tree)
            {
                if (!allow_content_elements) return; // same result if activated or not.

                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");

                // Run up until reaching top:
                var parent = origin;
                var found_expected = false;
                do
                {
                    var current_node = parent;
                    parent =  WPFVisualFinders.GetParentExtended(current_node) as DependencyObject;

                    var parent_ext = current_node.GetParentExtended() as DependencyObject;
                    Assert.Equal(parent, parent_ext);

                    if (expected.Equals(parent))
                        found_expected = true;
                } while (parent != null);

                Assert.Equal(related_in_path, found_expected);
            }

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion
    }
}
