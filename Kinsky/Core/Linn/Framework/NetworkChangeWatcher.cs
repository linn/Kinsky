using System;

namespace Linn
{
    public  class NetworkChangeWatcher : IDisposable
    {
        public event EventHandler<EventArgs> EventNetworkChanged;

        public NetworkChangeWatcher()
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += NetworkChanged;
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChanged;
        }

        private void NetworkChanged(object sender, EventArgs e)
        {
            OnEventNetworkChanged();
        }

        private void OnEventNetworkChanged()
        {
            EventHandler<EventArgs> del = EventNetworkChanged;
            if (del != null)
            {
                del(null, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= NetworkChanged;
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged -= NetworkChanged;
        }
    }
}
