using Opc.Ua;
using System;

namespace OPCFoundation.TaskLib.Base
{
    public class TaskBase
    {
        public Guid TaskId;
        public TaskBase() 
        {
            TaskId = Guid.NewGuid();
        }

        //public void Initialize()
        //{
        //    Utils.Trace("Process ID is {0}", TaskId);
        //}
    }
}
