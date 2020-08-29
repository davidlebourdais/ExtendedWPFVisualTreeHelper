using Xunit;
using System;
using System.Windows;
using EMA.ExtendedWPFVisualTreeHelper.Tests.Utils;

namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{
    public class FindParentsTests
    {
        #region FindParent
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParent(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);
                var has_content_control_in_path = TestData.HasContentElementWithin(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindParent");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindParent");
                var extMethod = extMethodInfo.MakeGenericMethod(expected.GetType());
                
                // Test unnammed:
                var result = method.Invoke(null, new object[] { origin, null });
                if (related_in_path && !has_content_control_in_path)
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
                var nammed_result = method.Invoke(null, new object[] { origin, "End"});
                if (related_in_path && !has_content_control_in_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);                
                
                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End"});
                Assert.Same(nammed_result, nammed_extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentExtended
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentExtended(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);

                // Build generic methods manualy, since the type to seek might change for each data set:
                var methodInfo = typeof(WPFVisualFinders).GetMethod("FindParentExtended");
                var method = methodInfo.MakeGenericMethod(expected.GetType());
                var extMethodInfo = typeof(WPFVisualFindersExtensions).GetMethod("FindParentExtended");
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
                var nammed_result = method.Invoke(null, new object[] { origin, "End"});
                if (related_in_path)
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);                
                
                // Test extension method with nammed target:
                var nammed_extresult = extMethod.Invoke(null, new object[] { origin, "End"});
                Assert.Same(nammed_result, nammed_extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByType
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByType(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);
                var has_content_control_in_path = TestData.HasContentElementWithin(xaml);

                // Test unnammed:
                var result = WPFVisualFinders.FindParentByType(origin, expected.GetType());
                if (related_in_path && !has_content_control_in_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = origin.FindParentByType(expected.GetType());
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = WPFVisualFinders.FindParentByType(origin, expected.GetType(), "End");
                if (related_in_path && !has_content_control_in_path)  // should always find if related in path
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);                      
                
                // Test extension method with nammed target:
                var nammed_extresult = origin.FindParentByType(expected.GetType(), "End");
                Assert.Same(nammed_result, nammed_extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByTypeExtended
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByTypeExtended(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var has_similar_type_in_path = TestData.HasSimilarTypeInDirectPath(xaml);

                // Test unnammed:
                var result = WPFVisualFinders.FindParentByTypeExtended(origin, expected.GetType());
                if (related_in_path)
                {
                    if (!has_similar_type_in_path) // should find destination if not caught a similar type.
                        Assert.Same(expected, result);
                    else  // here if caught an intermediary node
                        Assert.Equal(expected?.GetType(), result?.GetType());
                }
                else Assert.Null(result);

                // Test extension method with unnammed target:
                var extresult = origin.FindParentByTypeExtended(expected.GetType());
                Assert.Same(result, extresult);

                // Test nammed:
                var nammed_result = WPFVisualFinders.FindParentByTypeExtended(origin, expected.GetType(), "End");
                if (related_in_path)
                    Assert.Same(expected, nammed_result);
                else Assert.Null(result);                
                
                // Test extension method with nammed target:
                var nammed_extresult = origin.FindParentByTypeExtended(expected.GetType(), "End");
                Assert.Same(nammed_result, nammed_extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByLevel
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByLevel(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var origin_depth = TreeHelpers.GetElementDepthByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var expected_depth = TreeHelpers.GetElementDepthByName(tree, "End");
                var level = origin_depth - expected_depth;
                var has_content_control_in_path = TestData.HasContentElementWithin(xaml);

                // Test bare method:
                var result = WPFVisualFinders.FindParentByLevel(origin, level);
                if (related_in_path && !has_content_control_in_path)
                    Assert.Equal(expected?.GetType(), result?.GetType());
                else Assert.NotEqual(expected, result); // may find something close but not our target.

                // Test extension method:
                var extresult = origin.FindParentByLevel(level);
                Assert.Same(result, extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion

        #region FindParentByLevelExtended
        [Theory]
        [ClassData(typeof(InvertedTestData))]
        public void CanFindParentByLevelExtended(string xaml, bool related_in_path)
        {
            Action<FrameworkElement> inspect = (tree) => 
            {
                var origin = TreeHelpers.FindElementByName(tree, "Start");
                var origin_depth = TreeHelpers.GetElementDepthByName(tree, "Start");
                var expected = TreeHelpers.FindElementByName(tree, "End");
                var expected_depth = TreeHelpers.GetElementDepthByName(tree, "End");
                var level = origin_depth - expected_depth;

                // Test bare method:
                var result = WPFVisualFinders.FindParentByLevelExtended(origin, level);
                if (related_in_path)
                    Assert.Equal(expected?.GetType(), result?.GetType());
                else Assert.NotEqual(expected, result); // may find something close but not our target.

                // Test extension method:
                var extresult = origin.FindParentByLevelExtended(level);
                Assert.Same(result, extresult);
            };

            WPFAppTester.RunTestInWindow(inspect, xaml);
        }
        #endregion
    }
}
