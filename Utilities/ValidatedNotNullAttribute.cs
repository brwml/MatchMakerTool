namespace MatchMaker.Utilities
{
    using System;

    /// <summary>
    /// Defines the <see cref="ValidatedNotNullAttribute" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter |
                    AttributeTargets.Property | AttributeTargets.Delegate |
                    AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
