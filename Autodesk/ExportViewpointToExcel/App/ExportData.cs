using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportToExcel.App
{
    public class ExportData
    {
        public ExportData()
        {
            #region Access 
            //App.Access.SocketClient sc = new Access.SocketClient();
            //var data = sc.GetResult();
            //if (data != "Access complete")
            //{
            //    System.Windows.Forms.MessageBox.Show("HRESULT 0xc8000247", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            #endregion

            #region Init
            Viewpoint.ModelViewpoints ModelViewpoints = new Viewpoint.ModelViewpoints();
            var Model = ModelViewpoints.GetModel();

            GUInterface.FilterCheckBoxForm filter_form = new GUInterface.FilterCheckBoxForm(Model);
            filter_form.Show();
            #endregion
        }
       
    }
}
