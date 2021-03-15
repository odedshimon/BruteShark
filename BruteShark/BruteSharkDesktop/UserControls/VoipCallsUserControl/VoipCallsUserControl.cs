using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BruteSharkDesktop
{
    public partial class VoipCallsUserControl : UserControl
    {
        private GenericTableUserControl _voipCallsTableUserControl;

        public VoipCallsUserControl()
        {
            InitializeComponent();

            this._voipCallsTableUserControl = new GenericTableUserControl();
            _voipCallsTableUserControl.Dock = DockStyle.Fill;
        }

    }
}
