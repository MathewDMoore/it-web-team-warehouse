using System;

namespace Common
{
    public interface ILogger
    {
        void LogAttempt(Type methodType, OperationType operationType, string message, string keyValues);
        void LogSuccess(Type methodType, OperationType operationType, string message, string keyValues);
        void LogWarning(Type methodType, OperationType operationType, string message, string keyValues);
        void LogFailure(Type methodType, OperationType operationType, string message, string keyValues);
        void LogException(Type methodType, OperationType operationType, Exception exception, string keyValues);
    }
}