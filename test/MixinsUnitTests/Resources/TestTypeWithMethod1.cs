namespace MixinsUnitTests.Resources
{
    public class TestTypeWithMethod1
    {
        public string TestStringProperty { get; private set; }


        public void SetTestStringProperty(string value)
        {
            this.TestStringProperty = value;
        }
    }
}
