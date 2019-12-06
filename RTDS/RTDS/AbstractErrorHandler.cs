namespace RTDS
{
    public interface IErrorHandler
    {
        void OnFatalError(string errorMessage);

        void OnWarning(string warningMessage);
    }
}
