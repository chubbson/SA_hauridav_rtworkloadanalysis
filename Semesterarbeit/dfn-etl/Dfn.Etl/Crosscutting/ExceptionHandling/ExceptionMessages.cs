namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    /// <summary>
    /// Represents exception messages that are used multiple times.
    /// </summary>
    public static class ExceptionMessages
    {
        public static string CreateFatalNetworkError(string blockType, string blockName)
        {
            return string.Format("[{0}-{1}] FATAL NETWORK ERROR", blockType, blockName);
        }

        public static string CreateUnknownError(string blockType, string blockName)
        {
            return string.Format("[{0}-{1}] UNKNOWN ERROR", blockType, blockName);
        }

        public static string CreateDataflowMessageBroke(string blockType, string blockName, string messageTitle, string breakReason)
        {
            return string.Format("[{0}-{1}] item: {2} broke. Reason: {3}", blockType, blockName, messageTitle, breakReason);
        }
    }
}
