using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportToExcel.GUInterface
{
    public class FilterCheckBoxFormModel
    {
        App.Structure.Model Model = null;
        FilterCheckBoxForm InstanceForm = null;
        FilterCheckBoxFormModel InstanceModel = null;

        public FilterCheckBoxFormModel(FilterCheckBoxForm InstanceForm, App.Structure.Model Model)
        {
            InstanceModel = this;
            this.InstanceForm = InstanceForm;
            this.Model = Model;
        }

        public void GenerationReports(App.Structure.Statement Statement)
        {
            var FilterModel = App.Viewpoint.ModelViewpoints.Filter(Statement, Model);

        }
    }
}
