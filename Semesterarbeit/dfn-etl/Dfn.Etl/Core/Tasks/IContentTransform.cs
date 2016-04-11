namespace Dfn.Etl.Core.Tasks
{
    public interface IContentTransform<in TInput, in TInputMsg, out TOutput, out TOutputMsg> : IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
    }
}