namespace Mixins
{
    /// <summary>
    /// Defines the <see cref="IMixinObject"/> interface.
    /// </summary>
    public interface IMixinObject
    {
        /// <summary>
        /// Gets the objects which make up the mixin.
        /// </summary>
        object[] MixinObjects { get; }
    }
}
