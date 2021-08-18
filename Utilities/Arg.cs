namespace MatchMaker.Utilities
{
    using System;

    /// <summary>
    /// Defines the <see cref="Arg" /> utilities.
    /// </summary>
    public static class Arg
    {
        /// <summary>
        /// Determines whether the argument is null. When it is, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The argument type</typeparam>
        /// <param name="arg">The argument to test</param>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument</returns>
        public static T NotNull<T>([ValidatedNotNull] T arg, string name)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }

            return arg;
        }

        /// <summary>
        /// Determines whether the argument is null or whitespace. When it is, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="name">The name.</param>
        /// <returns>The argument</returns>
        public static string NotNullOrWhiteSpace([ValidatedNotNull] string arg, string name)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                throw new ArgumentException("The argument cannot be null or whitespace.", name);
            }

            return arg;
        }
    }
}
