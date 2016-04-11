namespace Dfn.Etl.Core.Tasks
{
    public abstract class FinalizationTarget<TTarget, TInput, TInputMsg> : TargetPerSingle<TTarget, TInput, TInputMsg> 
        where TInputMsg : IDataflowMessage<TInput>
    {
        public override void ExecuteForMessage(TInputMsg msg, Context context)
        {
            FinalizeMessage(msg);
        }

        protected virtual void FinalizeMessage(TInputMsg msg)
        {
            //Do nothing
        }
    }

    public class FinalizationTarget<TInput> : FinalizationTarget<FinalizationTarget<TInput>, TInput, IDataflowMessage<TInput>>
    {
        public override IDataflowMessage<TInput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TInput>();
        }
    }
}
