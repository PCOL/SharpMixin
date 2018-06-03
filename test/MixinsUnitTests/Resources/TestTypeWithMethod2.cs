namespace MixinsUnitTests.Resources
{
    /// <summary>
    /// A test type with property and method.
    /// </summary>
    public class TestTypeWithMethod2
    {
        /// <summary>
        /// Gets the test integer property.`
        /// </summary>
        public int TestIntegerProperty { get; private set; }

        /// <summary>
        /// Sets the test integer property.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetTestIntegerProperty(int value)
        {
            this.TestIntegerProperty = value;
        }
    }
}
