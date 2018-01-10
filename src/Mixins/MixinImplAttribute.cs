namespace Mixins
{
    using System;

    /// <summary>
    /// An attribute used to influence the mixins implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class MixinImplAttribute
        : Attribute
    {
        /// <summary>
        /// Gets or sets the type that the method, property, or event should target a static member on.
        /// </summary>
        public Type TargetStaticType { get; set; }

        /// <summary>
        /// Gets or sets the name of the member being targeted.
        /// </summary>
        public string TargetMemberName { get; set; }
    }
}
