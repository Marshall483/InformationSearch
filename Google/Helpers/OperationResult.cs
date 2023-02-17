namespace Google.Helpers;

/// <summary>
/// Результат операции обобщенный по TResult и TError
/// </summary>
/// <typeparam name="TResult">Тип результата.</typeparam>
/// <typeparam name="TError">Тип ошибки.</typeparam>
public class OperationResult<TResult, TError>
{
    public bool Success { get; set; }
    public virtual TResult? Result { get; set; }
    public virtual TError? Error { get; set; }
}

/// <summary>
/// Успешный результат выполнения операции.
/// </summary>
/// <typeparam name="TResult">Type of result.</typeparam>
/// <typeparam name="TError">Type of error.</typeparam>
public sealed class SucceededOperationResult<TResult, TError> : OperationResult<TResult, TError>
{
    public override TError? Error => default;

    public SucceededOperationResult(TResult result)
    {
        Success = true;
        Result = result;
    }
}

/// <summary>
/// Не Успешный результат выполнения операции.
/// </summary>
/// <typeparam name="TResult">Type of result.</typeparam>
/// <typeparam name="TError">Type of error.</typeparam>
public sealed class FailedOperationResult<TResult, TError> : OperationResult<TResult, TError>
{
    public override TResult? Result => default;

    public FailedOperationResult(TError error)
    {
        Success = false;
        Error = error;
    }
}