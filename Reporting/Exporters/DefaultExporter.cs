﻿namespace MatchMaker.Reporting.Exporters;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="DefaultExporter" />
/// </summary>
public class DefaultExporter : IExporter
{
    /// <summary>
    /// Exports the <see cref="Summary"/> instance using the default serializer.
    /// </summary>
    /// <param name="summary">The summary</param>
    /// <param name="folder">The folder</param>
    public void Export(Summary summary, string folder)
    {
        Guard.Against.Null(summary, nameof(summary));
        Guard.Against.NullOrWhiteSpace(folder, nameof(folder));

        var path = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.summary.json"));

        var options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            WriteIndented = true
        };
        options.Converters.Add(new DateOnlyConverter());
        options.Converters.Add(new TimeOnlyConverter());
        options.Converters.Add(new DecimalConverter());

        File.WriteAllText(path, JsonSerializer.Serialize(summary, options));
    }
}

/// <summary>
/// A JSON serialization converter for <see cref="DateOnly"/> objects.
/// </summary>
internal class DateOnlyConverter : JsonConverter<DateOnly>
{
    /// <summary>
    /// Reads the <see cref="DateOnly"/> value from the JSON payload.
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">The type of convert.</param>
    /// <param name="options">The serialization options</param>
    /// <returns>The <see cref="DateOnly"/> instance</returns>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString() ?? string.Empty);
    }

    /// <summary>
    /// Writes the <see cref="DateOnly"/> value to the JSON payload.
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The date value</param>
    /// <param name="options">The serialization options</param>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        Guard.Against.Null(writer, nameof(writer));
        Guard.Against.Null(value, nameof(value));

        writer.WriteStringValue(value.ToString("o"));
    }
}

/// <summary>
/// A JSON serialization converter for <see cref="TimeOnly"/> objects.
/// </summary>
internal class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    /// <summary>
    /// Reads the <see cref="TimeOnly"/> value from the JSON payload.
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">The type of convert.</param>
    /// <param name="options">The serialization options</param>
    /// <returns>The <see cref="DateOnly"/> instance</returns>
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeOnly.Parse(reader.GetString() ?? string.Empty);
    }

    /// <summary>
    /// Writes the <see cref="TimeOnly"/> value to the JSON payload.
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The time value</param>
    /// <param name="options">The serialization options</param>
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        Guard.Against.Null(writer, nameof(writer));
        Guard.Against.Null(value, nameof(value));

        writer.WriteStringValue(value.ToString("o"));
    }
}

/// <summary>
/// A JSON serialization converter for <see cref="decimal"/> objects.
/// </summary>
internal class DecimalConverter : JsonConverter<decimal>
{
    /// <summary>
    /// Reads the <see cref="decimal"/> value from the JSON payload.
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">The type of convert.</param>
    /// <param name="options">The serialization options</param>
    /// <returns>The <see cref="DateOnly"/> instance</returns>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return decimal.Parse(reader.GetString() ?? string.Empty);
    }

    /// <summary>
    /// Writes the <see cref="decimal"/> value to the JSON payload.
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The decimal value</param>
    /// <param name="options">The serialization options</param>
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        Guard.Against.Null(writer, nameof(writer));
        Guard.Against.Null(value, nameof(value));

        writer.WriteStringValue(value.ToString("F3"));
    }
}
