namespace Dfn.Etl.Core.Tasks
{
    #region Transform
    public interface ITransformationTask<in TInput, in TInputMsg, out TOutput, out TOutputMsg> : IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
    }

    public interface ITransformationTask<in TInput, out TOutput> : 
        ITransformationTask<TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>,
        IDataflowNetworkTask<TInput, TOutput>
    {
    }

    public abstract class Transform<TTransform, TInput, TInputMsg, TOutput, TOutputMsg> : DataflowNetworkTaskBase<TTransform, TInput, TInputMsg, TOutput, TOutputMsg>, ITransformationTask<TInput, TInputMsg, TOutput, TOutputMsg> 
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.Transformation; }
        }

        protected Transform()
        {
        }
    }

    public abstract class Transform<TTransform, TInput, TOutput> : Transform<TTransform, TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>, ITransformationTask<TInput, TOutput> 
    {
        public override IDataflowMessage<TOutput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TOutput>();
        }
    }
    #endregion
}