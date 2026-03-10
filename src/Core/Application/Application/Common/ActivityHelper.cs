using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common;

internal static class ActivityHelper
{
    /// <summary>
    /// Registra a exceção no span do OpenTelemetry. Excluído da cobertura pois é
    /// infraestrutura de observabilidade — coberto pelos testes de integração.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static void RecordExceptionSpan(Activity? activity, Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.StackTrace }
        }));
    }
}
