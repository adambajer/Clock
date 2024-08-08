using System;

namespace Clock
{
    internal class InPlaceHostingManager
    {
        private Uri deploymentUri;
        private bool v;

        public InPlaceHostingManager(Uri deploymentUri, bool v)
        {
            this.deploymentUri = deploymentUri;
            this.v = v;
        }

    }
}