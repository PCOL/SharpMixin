namespace MixinsUnitTests.Resources
{
    /// <summary>
    /// Defines a mixin interface.
    /// </summary>
    public interface IMixinWithMethods
    {
        /// <summary>
        /// Gets the test string.
        /// </summary>
        string TestStringProperty { get; }

        /// <summary>
        /// Gets the test integer.
        /// </summary>
        int TestIntegerProperty { get; }

        /// <summary>
        /// Sets the test string.
        /// </summary>
        /// <param name="value">The new value.</param>
        void SetTestStringProperty(string value);

        /// <summary>
        /// Sets the test integer.
        /// </summary>
        /// <param name="value">The new value.</param>
        void SetTestIntegerProperty(int value);
    }
}