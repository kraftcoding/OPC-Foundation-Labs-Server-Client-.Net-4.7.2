using System;

namespace TaskLib.Helpers
{
    internal static class ProcessHelper
    {
        internal static Guid GetProcessGuid() {  return Guid.NewGuid(); }
    }
}
