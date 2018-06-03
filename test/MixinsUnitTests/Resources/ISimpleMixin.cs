namespace MixinsUnitTests
{
    /// <summary>
    /// Defines a simple property mixin type.
    /// </summary>
    public interface ISimpleMixin
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Gets or sets the phone no.
        /// </summary>
        string PhoneNo { get; set; }
    }
}