namespace Dfn.Etl.Core.Tasks
{
    /*
    public abstract class ContentTaskBase<TTask>
    {
        #region Static helper methods
        /// <summary>
        /// Gets the name of the task by the typename of the task.
        /// </summary>
        /// <returns></returns>
        protected static string GetTaskNameByTaskType()
        {
            return typeof(TTask).Name;
        }
        #endregion

        /// <summary>
        /// Gets the this task instance cast to the specialized task type. Just a helper property for convenience...
        /// </summary>
        /// <value>
        /// The current task instance cast to the specialized task type.
        /// </value>
        protected virtual TTask _This
        {
            get { return (TTask)(object)this; }
        }

        private string m_Title;
        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public virtual string Title { get { return m_Title; } protected set { m_Title = value; } }

        protected ContentTaskBase()
            : this(GetTaskNameByTaskType())
        {
        }

        protected ContentTaskBase(string title)
        {
            m_Title = title;
        }
    }

    public abstract class ContentTaskBase<TTask, TInput, TOutput> : ContentTaskBase<TTask>
    {
        #region Helper classes
        protected class Context
        {
            private ContentTaskBase<TTask, TInput, TOutput> m_Task;

            private HashSet<IContent<TOutput>> m_GeneratedOutputContents;
            public virtual IEnumerable<IContent<TOutput>> GeneratedContents { get { return m_GeneratedOutputContents; } protected set { m_GeneratedOutputContents = new HashSet<IContent<TOutput>>(value); } }

            private IEnumerable<IContent<TInput>> m_InputContents;
            /// <summary>
            /// The current input contents
            /// </summary>
            public virtual IEnumerable<IContent<TInput>> InputContents
            {
                get
                {
                    //VerifyIsProcessing("Cannot access InputContents when task isn't processing.");
                    return m_InputContents;
                }
                set
                {
                    //VerifyIsProcessing("Cannot set InputContents when task isn't processing.");
                    m_InputContents = value;
                }
            }

            private IEnumerable<IContent<TOutput>> m_OutputContents;
            /// <summary>
            /// The current output contents - accessing this may trigger generating output
            /// </summary>
            public virtual IEnumerable<IContent<TOutput>> OutputContents
            {
                get
                {
                    //VerifyIsProcessing("Cannot access OutputContents when task isn't processing.");
                    return m_OutputContents;
                }
                set
                {
                    //VerifyIsProcessing("Cannot set OutputContents when task isn't processing.");
                    m_OutputContents = value;
                }
            }

            private bool m_CanContinue;
            public virtual bool CanContinue { get { return m_CanContinue; } protected set { m_CanContinue = value; } }
            
            public Context(ContentTaskBase<TTask, TInput, TOutput> task)
            {
                m_Task = task;
                m_GeneratedOutputContents = new HashSet<IContent<TOutput>>();
                m_CanContinue = true;
            }

            #region Error handling helpers

            public virtual void Abort(string reason)
            {
                CanContinue = false;
            }

            protected virtual void AddError(Exception exception, string failedToProcessWithTask)
            {
            }
            #endregion

            public void SetInputContents(IEnumerable<IContent<TInput>> inputContents)
            {
                //Just set the input contents
                m_InputContents = inputContents;
            }

            public virtual TContent AddOutputContent<TContent>(TContent outputContent)
                where TContent : IContent<TOutput>
            {
                if(!m_GeneratedOutputContents.Contains(outputContent))
                {
                    m_GeneratedOutputContents.Add(outputContent);
                }
                m_Task.AddGeneratedOutputContent(outputContent);

                return outputContent;
            }

            public virtual Content<TOutput> AddOutputData(TOutput outputData)
            {
                var outputContent = CreateOutputContent(outputData);
                return AddOutputContent(outputContent);
            }

            protected virtual Content<TOutput> CreateOutputContent(TOutput outputData)
            {
                return m_Task.CreateOutputContent(outputData);
            }
        }
        
        #endregion

        private HashSet<IContent<TOutput>> m_AllGeneratedContents;
        public virtual IEnumerable<IContent<TOutput>> AllGeneratedContents { get { return m_AllGeneratedContents; } protected set { m_AllGeneratedContents = new HashSet<IContent<TOutput>>(value); } }

        protected ContentTaskBase()
        {
            m_AllGeneratedContents = new HashSet<IContent<TOutput>>();
        }

        #region Process
        /// <summary>
        /// Main entry point for the task. Processes the specified input, possibly generating some output, it's up to the task implementation.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public IContent<TOutput> Process(IContent<TInput> input)
        {
            Context context = null;
            try
            {
                context = BeginProcess(input);
                if(!context.CanContinue)
                {
                    //No content has yet been processed - so return null content result
                    return null;
                }

                //Now start processing the contents
                ProcessContents(context);
                if(!context.CanContinue)
                {
                    //Even if contents have been processed - we don't want to output those, since we are not allowed to proceed - so return null content result
                    return null;
                }
                
                //Prepare the output content to be returned
                var outputContent = PrepareOutputContents(context);
                return outputContent;
            }
            catch(Exception e)
            {
                throw new Exception("Failed to process with task.", e);
            }
            finally
            {
                EndProcess();
            }

            //Just return an empty content
            return null;
        }

        protected Context BeginProcess(IContent<TInput> inputContent)
        {
            var context = CreateContext();

            //Prepare the input contents to be processed
            PrepareInputContents(context, inputContent);
            if(!context.CanContinue)
            {
                //Just return
                return context;
            }
            
            //Now say that we are starting
            Start(context);
            if (!context.CanContinue)
            {
                //Just return
                return context;
            }

            //In case you want to add futher initialization... add it here...

            return context;
        }

        protected virtual Context CreateContext()
        {
            return new Context(this);
        }

        protected virtual void Start(Context c)
        {
        }

        protected void EndProcess()
        {
            End();
        }

        protected virtual void End()
        {
        }

        protected virtual void ProcessContents(Context c)
        {
        }
        #endregion

        #region Verification helpers
        protected void VerifyIsProcessing(string message = null)
        {
        }
        #endregion

        #region Input / Output content helpers

        #region Input content helpers
        /// <summary>
        /// Prepares the input contents for processing, making sure to expand all contents if multiple are available.
        /// </summary>
        /// <param name="inputContent">Content of the input.</param>
        protected virtual void PrepareInputContents(Context c, IContent<TInput> inputContent)
        {
            IEnumerable<IContent<TInput>> allInputContents = null;
            if(inputContent == null)
            {
                //Just set the input as an empty list...
                c.SetInputContents(new List<IContent<TInput>>());
                return;
            }

            if(!inputContent.HasMany)
            {
                //We have a single content that doesn't have any children or anything... but we still need to represent it as an enumerable...
                allInputContents = SingleContentEnumerable(inputContent);
            }
            else
            {
                //If we have a content that has many internal contents, we want to expand it... and then in turn expand the children etc...
                allInputContents = ExpandContentEnumerable(inputContent);
            }

            //Now just set the Input contents
            c.SetInputContents(allInputContents);
        }

        #endregion

        #region Output content helpers

        protected virtual IContent<TOutput> PrepareOutputContents(Context c)
        {
            //We have generated many output contents or a single... doesn't matter... 
            // - but want to output it as a single content 
            // - and making sure it is "lazy evaluation"
            //Thus using the "MultiContent" implementation, which keeps multiple internal contents as enumerable that is lazy in evaluation
            return Content.CreateMulti(c.OutputContents);
        }

        private void AddGeneratedOutputContent(IContent<TOutput> outputContent)
        {
            if(!m_AllGeneratedContents.Contains(outputContent))
            {
                m_AllGeneratedContents.Add(outputContent);
            }
        }

        protected virtual Content<TOutput> CreateOutputContent(TOutput outputData)
        {
            return Content.Create(outputData);
        }

        protected virtual void FinalizeOutputContent(IContent<TOutput> outputContent)
        {
        }

        #endregion

        #endregion

        #region Enumerable helper methods

        protected IEnumerable<IContent<TData>> ExpandContentEnumerable<TData>(IContent<TData> inputContent)
        {
            if(inputContent == null)
                yield break;

            if(!inputContent.HasMany)
            {
                yield return inputContent;
                yield break;
            }

            //Iterate all the contents
            foreach(var content in inputContent.GetAllContent())
            {
                //Now expand the content if possible

                foreach(var c in ExpandContentEnumerable(content))
                {
                    yield return c;
                }
            }
        }

        protected IEnumerable<IContent<TData>> SingleContentEnumerable<TData>(IContent<TData> content)
        {
            yield return content;
        }

        protected IEnumerable<IContent<TData>> MergedContentsEnumerable<TData>(params IEnumerable<IContent<TData>>[] contentsList)
        {
            foreach(var contents in contentsList)
            {
                foreach(var content in contents)
                {
                    yield return content;
                }
            }
        }

        protected IEnumerable<TData> ContentToDataEnumerable<TData>(Context c, IEnumerable<IContent<TData>> contents)
        {
            if (contents == null)
                yield break;

            foreach (var inputContent in contents)
            {
                yield return inputContent.Data;
            }
        }

        protected IEnumerable<IContent<TOutput>> DataToContentEnumerable(Context c, IEnumerable<TOutput> transformedData)
        {
            if (transformedData == null)
                yield break;

            foreach (var data in transformedData)
            {
                //Now return a content for the current data
                yield return c.AddOutputData(data);
            }
        }

        #endregion
    }*/
}