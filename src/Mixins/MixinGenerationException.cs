namespace Mixins
{
    using System;

    /// <summary>
    /// Represents a mixin generation exception.
    /// </summary>
    [Serializable]
    public class MixinGenerationException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MixinGenerationException"/> class.
        /// </summary>
        public MixinGenerationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MixinGenerationException"/> class.
        /// </summary>
        /// <param name="message">A message for the exception.</param>
        public MixinGenerationException(string message)
            : base(message)
        {
        }
    }
}
