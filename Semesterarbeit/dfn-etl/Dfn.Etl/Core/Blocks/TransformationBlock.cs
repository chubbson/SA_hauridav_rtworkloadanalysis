using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Core.Blocks
{
    public class TransformationBlock<TBlock, TIn, TInMsg, TOut, TOutMsg> : BaseInputOutputBlockWithTarget<TBlock, TIn, TInMsg, TOut, TOutMsg> 
        where TInMsg : IDataflowMessage<TIn>
        where TOutMsg : IDataflowMessage<TOut>
    {
        private IDataflowNetworkTask<TIn, TInMsg, TOut, TOutMsg> m_Transformation;
        public virtual IDataflowNetworkTask<TIn, TInMsg, TOut, TOutMsg> Transformation
        {
            get { return m_Transformation; }
            protected set { m_Transformation = value; }
        }

        #region Ctor
        public TransformationBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TIn, TInMsg, TOut, TOutMsg> transformation, ExecutionDataflowBlockOptions blockOptions)
            : base(network, null, blockOptions)
        {
            m_Transformation = transformation;
        }
        #endregion

        protected override IEnumerable<TOutMsg> ProcessInputMessages(IEnumerable<TInMsg> msgs)
        {
            return Transformation.Process(msgs);
        }

        protected override void OnSourceCompletion()
        {
            base.OnSourceCompletion();

            //Finish the task
            Transformation.EndProcessingTask();
        }

        public override string ToString()
        {
            return Transformation.ToString();
        }

        public override void Dispose()
        {
            base.Dispose();

            if(Transformation != null)
            {
                Transformation.Dispose();
            }
        }
    }

    public class TransformationBlock<TIn, TInMsg, TOut, TOutMsg> : TransformationBlock<TransformationBlock<TIn, TInMsg, TOut, TOutMsg>, TIn, TInMsg, TOut, TOutMsg>
        where TInMsg : IDataflowMessage<TIn>
        where TOutMsg : IDataflowMessage<TOut>
    {
        public TransformationBlock(IIdasDataflowNetwork network, IDataflowNetworkTask<TIn, TInMsg, TOut, TOutMsg> transformation, ExecutionDataflowBlockOptions blockOptions)
            : base(network, transformation, blockOptions)
        {
        }
    }
}
