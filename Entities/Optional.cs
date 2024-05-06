namespace ScoreHubAPI.Entities;

public class Optional<T>
{
    private readonly T _value;
    private readonly bool _hasValue;

    private Optional(T value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

#pragma warning disable CS8604 // Possible null reference argument.
    public static Optional<T> None => new(default, false);
#pragma warning restore CS8604 // Possible null reference argument.

    public static Optional<T> Some(T value) => new(value, true);

    public static Optional<T> FromNullable(T value)
    {
        return value == null ? None : Some(value);
    }

    public bool HasValue => _hasValue;

#pragma warning disable CS8601 // Possible null reference assignment.
    public T GetValueOrDefault(T defaultValue = default) => _hasValue ? _value : defaultValue;
#pragma warning restore CS8601 // Possible null reference assignment.

    public T GetValueOrThrow(string errorMessage = "Value not found")
    {
        if (_hasValue)
            return _value;
        throw new NullReferenceException(errorMessage);
    }
}
