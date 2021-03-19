using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace BruteSharkDesktop
{
    public partial class VoipCallsUserControl : UserControl
    {
        private readonly GenericTableUserControl _voipCallsTableUserControl;

        public int VoipCallsCount => _voipCallsTableUserControl.ItemsCount;

        public VoipCallsUserControl()
        {
            InitializeComponent();

            _voipCallsTableUserControl = new GenericTableUserControl();
            _voipCallsTableUserControl.Dock = DockStyle.Fill;
            _voipCallsTableUserControl.SetTableDataType(typeof(CommonUi.VoipCall));
            this.Controls.Clear();
            this.Controls.Add(_voipCallsTableUserControl);
        }

        public void AddVoipCall(CommonUi.VoipCall voipCall)
        {
            _voipCallsTableUserControl.AddDataToTable(voipCall);
        }

        public void UpdateVoipCall(CommonUi.VoipCall voipCall, PropertyInfo propertyInfo, object newValue)
        {
            var existingVoipCall = _voipCallsTableUserControl.ItemsHashSet.Where(c => c.Equals(voipCall)).First();

            if (existingVoipCall != null)
            {
                typeof(CommonUi.VoipCall).GetProperty(propertyInfo.Name).SetValue(existingVoipCall, newValue);
            }
        }

    }
}
