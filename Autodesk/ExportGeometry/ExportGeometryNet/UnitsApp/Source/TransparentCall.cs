using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using System.Diagnostics;

namespace ExportGeometry.UnitsApp.Source
{
    class TransparentCall
    {
        ComApi.InwOpState3 opState = ComApiBridge.State;

        string cat_internal_name = "";
        string cat_user_name = "";
        string prop_internal_name = "";
        string prop_user_name = "";
        string value = "";
        string solid_value = "";
        double value_transparent = 0.7;

        public TransparentCall()
        {
            //System.Windows.Forms.MessageBox.Show("TransparentCall");
            Run();
        }

        private void Run()
        {
            ModelItemCollection select_item = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            if (select_item.Count != 0)
                HandlerSelectItems(select_item);
            else
            {
                try
                {
                    //System.Windows.Forms.MessageBox.Show("Модель выбери!");
                    Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectAll();
                    select_item = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
                    HandlerSelectItems(select_item);
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }

        private void HandlerSelectItems(ModelItemCollection select_item)
        {
            #region FinderItem
            Helpers.FinderItem finder = new Helpers.FinderItem(0);

            cat_internal_name = "LcOaNode";
            cat_user_name = "Элемент";
            prop_internal_name = "LcOaSceneBaseClassName";
            prop_user_name = "Внутренний тип";
            value = "LcDgnLevel";
            solid_value = "Изоляция";

            ModelItemCollection collection_select = finder.SearchByCategoryAndProperty(cat_internal_name, cat_user_name, prop_internal_name, prop_user_name, value);

            ModelItemCollection collection_transparent = new ModelItemCollection();
            foreach (ModelItem model_item in collection_select)
            {
                if (model_item.DisplayName.Contains(solid_value))
                {
                    collection_transparent.Add(model_item);
                }
            }

            ComApi::InwOpSelection2 selection = ComApiBridge.ToInwOpSelection(collection_transparent) as ComApi::InwOpSelection2;
            opState.OverrideTransparency(selection, value_transparent);

            // File save ???
            //Autodesk.Navisworks.Api.Application.ActiveDocument.Models.OverrideTemporaryTransparency(collection_hidden, 0.7);
            //Autodesk.Navisworks.Api.Application.ActiveDocument.SaveFile(@"D:\12.nwd");
            #endregion
        }
    }
}
