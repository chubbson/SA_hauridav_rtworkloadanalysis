using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Core.Blocks
{
    public class TargetBlock<TBlock, TIn, TInMsg> : BaseInputOutputBlockWithTarget<TBlock, TIn, TInMsg, TIn, TInMsg>
        where TInMsg : IDataflowMessage<TIn>
    {
        private IDataflowNetworkTask<TIn, TInMsg, TIn, TInMsg> m_Action;
        public virtual IDataflowNetworkTask<TIn, TInMsg, TIn, TInMsg> Action
        {
            get { return m_Action; }
            protected set { m_Action = value; }
        }

        #region Ctor
        public TargetBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TIn, TInMsg, TIn, TInMsg> action, ExecutionDataflowBlockOptions blockOptions)
            : base(network, null, blockOptions)
        {
            m_Action = action;
        }
        #endregion

        public override TBlock LinkTo(ITargetBlock<TInMsg> target, IdasDataflowLinkOptions linkOptions = null)
        {
            if(linkOptions == null)
                linkOptions = new IdasDataflowLinkOptions();

            if(!linkOptions.WaitForTargetCompletion.HasValue)
                linkOptions.WaitForTargetCompletion = false;

            return base.LinkTo(target, linkOptions);
        }

        protected override IEnumerable<TInMsg> ProcessInputMessages(IEnumerable<TInMsg> msgs)
        {
            Action.Process(msgs);
            yield break;
        }

        public override string ToString()
        {
            return Action.ToString();
        }
    }

    public class TargetBlock<TIn, TInMsg> : TargetBlock<TargetBlock<TIn, TInMsg>, TIn, TInMsg>
        where TInMsg : IDataflowMessage<TIn>
    {
        public TargetBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TIn, TInMsg, TIn, TInMsg> action, ExecutionDataflowBlockOptions blockOptions)
            : base(network, action, blockOptions)
        {
        }
    }


    public interface IFinalizerBlock : IBlock
    {
    }
    public class FinalizerTargetBlock<TIn, TInMsg> : TargetBlock<TargetBlock<TIn, TInMsg>, TIn, TInMsg>, IFinalizerBlock
        where TInMsg : IDataflowMessage<TIn>
    {
        public FinalizerTargetBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TIn, TInMsg, TIn, TInMsg> action, ExecutionDataflowBlockOptions blockOptions)
            : base(network, action, blockOptions)
        {
        }
    }
}
