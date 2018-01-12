namespace MixinsUnitTests.Resources
{
    public class TestTypeWithMethod2
    {
        public int TestIntegerProperty { get; private set; }


        public void SetTestIntegerProperty(int value)
        {
            this.TestIntegerProperty = value;
        }
    }
}
