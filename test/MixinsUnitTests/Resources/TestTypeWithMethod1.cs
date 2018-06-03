namespace MixinsUnitTests.Resources
{
    /// <summary>
    /// A test type with property and method.
    /// </summary>
    public class TestTypeWithMethod1
    {
        /// <summary>
        /// Gets a test string property.
        /// </summary>
        public string TestStringProperty { get; private set; }

        /// <summary>
        /// Sets a test string.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetTestStringProperty(string value)
        {
            this.TestStringProperty = value;
        }
    }
}
