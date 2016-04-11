namespace Dfn.Etl.Core.Blocks
{
    public interface IBlock
    {
        void Dispose();
    }

    public interface ISourceBlock : IBlock
    {
        void Start();
        void Complete();
    }
}