namespace RTDS
{
    public abstract class AbstractErrorHandler
    {
        public abstract void OnFatalError(string errorMessage);

        public abstract void OnWarning(string warningMessage);
    }
}
