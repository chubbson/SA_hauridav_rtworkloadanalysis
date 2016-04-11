namespace Dfn.Etl.Core.Tasks
{
    #region Target
    public interface ITargetTask<TInput, TInputMsg> : IDataflowNetworkTask<TInput, TInputMsg, TInput, TInputMsg>
        where TInputMsg : IDataflowMessage<TInput>
    {
    }

    public interface ITargetTask<TInput> : ITargetTask<TInput, IDataflowMessage<TInput>>
    {
    }
    #endregion

    #region TargetPerSingle
    public abstract class Target<TTarget, TInput, TInputMsg> : DataflowNetworkTaskBase<TTarget, TInput, TInputMsg, TInput, TInputMsg>, ITargetTask<TInput, TInputMsg>
        where TInputMsg : IDataflowMessage<TInput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.Target; }
        }

        protected Target()
        {
        }
    }

    public abstract class Target<TSource, TInput> : Target<TSource, TInput, IDataflowMessage<TInput>>, ITargetTask<TInput>
    {
        public override IDataflowMessage<TInput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TInput>();
        }
    }
    #endregion
}
