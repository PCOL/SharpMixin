namespace MixinsUnitTests.Resources
{
    public interface IMixinWithMethods
    {
        string TestStringProperty { get; }

        int TestIntegerProperty { get; }

        void SetTestStringProperty(string value);

        void SetTestIntegerProperty(int value);
    }
}