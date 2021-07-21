namespace EMA.ExtendedWPFVisualTreeHelper.Tests
{    
    /// <summary>
    /// Inverted data are simply <see cref="TestData"/> for which start and end 
    /// nodes are inverted. See <see cref="TestData"/> for more details.
    /// </summary>
    public class InvertedTestData : TestData
    {
        protected override string SetStartEnd(string rawXaml)
            => rawXaml.Replace("\"A\"", "\"End\"").Replace("\"B\"", "\"Start\"");

        public InvertedTestData() : base()
        {   }
    }
}
