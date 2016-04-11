namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a part of a dataflow-network.
    /// Its purpose is to tell other objects such as the log what part of the dataflow-network
    /// the object was called from.
    /// </summary>
    public enum DataflowNetworkConstituent
    {
        /// <summary>
        /// Represents the entire network.
        /// </summary>
        Network,

        /// <summary>
        /// Represents an entire source.
        /// </summary>
        Source,

        /// <summary>
        /// Represents one item produced by a source.
        /// </summary>
        SourceItem,

        /// <summary>
        /// Represents a transformation.
        /// </summary>
        Transformation,

        /// <summary>
        /// Represents a one-to-many transformation
        /// </summary>
        TransformMany,

        /// <summary>
        /// Represents a many-to-one transformation
        /// </summary>
        TransformationBatched,

        /// <summary>
        /// Represents a target.
        /// </summary>
        Target,

        /// <summary>
        /// Represents a target that receives groups of items instead of one.
        /// </summary>
        TargetBatched
    }
}