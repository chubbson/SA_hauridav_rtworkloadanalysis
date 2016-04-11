using System;
using System.Collections.Generic;
using Dfn.Etl.Core.Functors;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a group of functions that ease the pain of casting actual methods on objects to their Func/Action equivalents.
    /// This is required because the TPL-Dataflow-Library offers two overloads in their DataflowBlock-Constructors and without
    /// casting it does not know which one to pick.
    /// </summary>
    public static class DataflowCasting
    {
        public static Func<IEnumerable<TOut>> AsFunction<TOut>(this ISource<TOut> source)
        {
            return source.Pull;
        }

        public static Func<TIn,TOut> AsFunction<TIn, TOut>(this ITransformation<TIn, TOut> transformation)
        {
            return transformation.Transform;
        }

        public static Action<TIn> AsFunction<TIn>(this ITarget<TIn> target)
        {
            return target.Push;
        }

        public static Action<TIn[]> AsFunction<TIn>(this ITargetBatched<TIn> target)
        {
            return target.Push;
        }

        public static Func<IEnumerable<IDataflowMessage<TOut>>> AsFunction<TOut>(this ISourceFunctor<TOut> source)
        {
            return source.Pull;
        }

        public static Func<IDataflowMessage<TIn>, IDataflowMessage<TOut>> AsFunction<TIn, TOut>(this ITransformationFunctor<TIn, TOut> transformation)
        {
            return transformation.Transform;
        }

        public static Func<IDataflowMessage<TIn>[], IDataflowMessage<TOut>> AsFunction<TIn, TOut>(this ITransformationBatchedFunctor<TIn, TOut> transformation)
        {
            return transformation.Transform;
        }

        public static Func<IDataflowMessage<TIn>, IEnumerable<IDataflowMessage<TOut>>> AsFunction<TIn, TOut>(this ITransformManyFunctor<TIn, TOut> transformation)
        {
            return transformation.TransformMany;
        }

        public static Action<IDataflowMessage<TIn>> AsFunction<TIn>(this ITargetFunctor<TIn> target)
        {
            return target.Push;
        }

        public static Action<IDataflowMessage<TIn>[]> AsFunction<TIn>(this ITargetBatchedFunctor<TIn> target)
        {
            return target.Push;
        }
    }
}