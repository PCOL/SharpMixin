namespace MixinsUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mixins;
    using MixinsUnitTests.Resources;

    [TestClass]
    public class MixinUnitTests
    {
        [TestMethod]
        public void CreateSimpleMixin()
        {
            var simpleType1 = new SimpleType1()
            {
                Name = "Mr Mixin",
                Address = "Somewhere Street"
            };

            var simpleType2 = new SimpleType2()
            {
                PhoneNo = "0800 123456"
            };

            var mixin = new object[] { simpleType1, simpleType2 }
                .CreateMixin<ISimpleMixin>();

            Assert.IsNotNull(mixin);
            Assert.AreEqual("Mr Mixin", mixin.Name);
            Assert.AreEqual("Somewhere Street", mixin.Address);
            Assert.AreEqual("0800 123456", mixin.PhoneNo);
        }
    }
}
