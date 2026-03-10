using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests.Helpers;

/// <summary>
/// Extensões para verificar chamadas de ILogger em testes com NSubstitute.
/// LogInformation/LogError são métodos de extensão que chamam ILogger.Log internamente.
/// </summary>
public static class LoggerTestExtensions
{
    public static void ShouldHaveLoggedInformation<T>(this ILogger<T> logger)
    {
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public static void ShouldHaveLoggedError<T>(this ILogger<T> logger, Exception exception)
    {
        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            exception,
            Arg.Any<Func<object, Exception?, string>>());
    }

    public static void ShouldHaveLoggedWarning<T>(this ILogger<T> logger)
    {
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
