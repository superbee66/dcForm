using System;

namespace dCForm.Client.WebControls
{
    public class ApprovalEventArgs : EventArgs
    {
        public ApprovalEventArgs(LightDoc _LightDoc) { LightDoc = _LightDoc; }
        public LightDoc LightDoc { get; private set; }
    }
}