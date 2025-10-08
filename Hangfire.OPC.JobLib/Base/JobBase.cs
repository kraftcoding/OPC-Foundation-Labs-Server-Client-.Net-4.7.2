using System;

namespace Hangfire.OPC.JobLib.Base
{
    public class JobBase
    {       
        public Guid JobId { get; internal set; }        

        public JobBase() 
        {
            JobId = Guid.NewGuid();            
        }
    }
}
