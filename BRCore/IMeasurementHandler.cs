using System;
using System.Collections.Generic;
using System.Text;

namespace BRCore
{
    public interface IMeasurementHandler
    {
        public void AddTimer(string name);

        public void RemoveTimer(string name);

        public void StartTimer(string name, double intervalLength);

        public void StopTimer(string name);
    }
}
