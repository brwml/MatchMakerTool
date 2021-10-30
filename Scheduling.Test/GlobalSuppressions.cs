using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Assertions",
    "xUnit2017:Do not use Contains() to check if a value exists in a collection",
    Justification = "This typically happens when we are checking the member of a hash set which is more efficient than the alternative.")]
