using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gato.core
{
    public class RemoteEvent<TEventArgs> : MarshalByRefObject where TEventArgs : EventArgs
    {
        public event EventHandler<TEventArgs> eventToHandle;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Notify(object sender, TEventArgs args)
        {
            if (eventToHandle != null)
                eventToHandle(this, args);
        }
    }
}
