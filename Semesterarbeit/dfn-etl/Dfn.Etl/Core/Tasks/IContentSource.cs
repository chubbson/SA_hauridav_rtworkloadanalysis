namespace Dfn.Etl.Core.Tasks
{
    public interface IContentSource<TOutput, TMsg> : IDataflowNetworkTask<TOutput, TMsg, TOutput, TMsg> 
        where TMsg : IDataflowMessage<TOutput>
    {
    }
}