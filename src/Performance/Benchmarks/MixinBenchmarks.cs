namespace Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    using Benchmarks.Resources;
    using Mixins;

    public class MixinBenchmarks
    {
        [Benchmark]
        public bool SimpleMixin()
        {
            var a = new SimpleTypeA()
            {
                Name = "Name",
                Address = "Address"
            };

            var b = new SimpleTypeB()
            {
                Telephone = "Telephone"
            };

            var mixin = new object[] { a, b }.CreateMixin<ISimpleTypeAB>();

            return
                mixin.Name == a.Name &&
                mixin.Address == a.Address &&
                mixin.Telephone == b.Telephone;
        }
    }
}