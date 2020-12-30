namespace Google.Protobuf.WellKnownTypes
{
    // TODO: TBD: in a coming Ellumination.Protobuf deployment, allow for Duration deconstruction in seconds and/or nanos terms...
    // TODO: TBD: better still would be for such a thing to be deployed in the Google.Protobuf package.
    // TODO: TBD: at the time of this writing Google.Protobuf dependency in 3.11.2, whereas current release is at least 3.14.0
    /// <summary>
    /// Provides a set of helpful Duration based extension methods.
    /// </summary>
    internal static class InternalDurationExtensionMethods
    {
        // TODO: TBD: "Allow for Duration deconstruction into constituent member components"
        // TODO: TBD: https://github.com/protocolbuffers/protobuf/issues/8168
        /// <summary>
        /// Deconstructs the <paramref name="value"/> in terms of <paramref name="seconds"/>
        /// and, optionally, <paramref name="nanos"/>.
        /// </summary>
        /// <param name="value">The <see cref="Duration"/> value.</param>
        /// <param name="seconds">The <see cref="Duration.Seconds"/> component.</param>
        /// <param name="nanos">Optionally, the <see cref="Duration.Nanos"/> component.</param>
        /// <remarks>There is no value in deconstructing in only <paramref name="seconds"/>
        /// terms, from a language, or even simple POCO, perspective.</remarks>
        public static void Deconstruct(this Duration value, out long seconds, out int? nanos) =>
            (seconds, nanos) = (value.Seconds, value.Nanos == default ? (int?)null : value.Nanos);
    }
}
