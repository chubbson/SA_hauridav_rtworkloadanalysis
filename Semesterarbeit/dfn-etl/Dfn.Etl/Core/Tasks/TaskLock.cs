using System;

namespace Dfn.Etl.Core.Tasks
{
    public class TaskLock : IDisposable
    {
        private ILockableTask m_LockableTask;
        private object m_SyncObj;

        public TaskLock(ILockableTask lockableTask)
        {
            m_LockableTask = lockableTask;

            AquireLock();
        }

        protected virtual void AquireLock()
        {
            m_SyncObj = m_LockableTask.SyncObj;
            //Do the lock
            m_LockableTask.LockedBy = this;
        }

        protected virtual void ReleaseLock()
        {
            //Make sure we have locked
            if (m_SyncObj == null)
                return;

            //Release the lock
            m_LockableTask.LockedBy = null;
            m_SyncObj = null;
        }

        public void Dispose()
        {
            ReleaseLock();
        }
    }

    public interface ILockableTask
    {
        TaskLock LockedBy { get; set; }
        object SyncObj { get; }
    }
}