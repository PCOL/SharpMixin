namespace MixinsUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mixins;
    using MixinsUnitTests.Resources;

    [TestClass]
    public class MixinUnitTests
    {
        [TestMethod]
        public void CreateSimplePropertyMixin()
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
            Assert.IsTrue(mixin.IsMixin());
            Assert.AreEqual("Mr Mixin", mixin.Name);
            Assert.AreEqual("Somewhere Street", mixin.Address);
            Assert.AreEqual("0800 123456", mixin.PhoneNo);

            var mixinObjects = ((IMixinObject)mixin).MixinObjects;
            Assert.IsNotNull(mixinObjects);
            Assert.AreEqual(simpleType1, mixinObjects[0]);
            Assert.AreEqual(simpleType2, mixinObjects[1]);
        }

        [TestMethod]
        public void CreateMixinWithMethods()
        {
            var testType1 = new TestTypeWithMethod1();
            testType1.SetTestStringProperty("Type1");

            var testType2 = new TestTypeWithMethod2();
            testType2.SetTestIntegerProperty(100);

            var mixin = new object[] { testType1, testType2 }
                .CreateMixin<IMixinWithMethods>();

            Assert.IsNotNull(mixin);
            Assert.IsTrue(mixin.IsMixin());
            Assert.AreEqual("Type1", mixin.TestStringProperty);
            Assert.AreEqual(100, mixin.TestIntegerProperty);

            mixin.SetTestStringProperty("MixinType1");
            mixin.SetTestIntegerProperty(200);

            Assert.AreEqual("MixinType1", mixin.TestStringProperty);
            Assert.AreEqual(200, mixin.TestIntegerProperty);
        }
    }
}
