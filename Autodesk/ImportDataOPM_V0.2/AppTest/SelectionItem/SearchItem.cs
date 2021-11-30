using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;


namespace ImportDataOPM.AppTest.SelectionItem
{
    public class SearchItem
    {
        //
        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        SetPropertyForm form = null;
        
        //
        public SearchItem()
        {
            form = new SetPropertyForm(this);
            form.Show();
        }

        #region # RunAttribSearch
        // RunAttribSearch
        public void RunAttribSearch(string userCategory, string internalCategory, string userProperty, string intrenalProperty, string propertyValue)
        {
            var collection = GetSelectModel();

            ModelItemCollection selectCollection = SearchModelItems(collection, userCategory, internalCategory, userProperty, intrenalProperty, propertyValue);

            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Clear();
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.CopyFrom(selectCollection);

            form.Close();
        }

        //
        private ModelItemCollection GetSelectModel()
        {
            ModelItemCollection collection = doc.CurrentSelection.SelectedItems;

            if (collection.Count == 0)
            {
                doc.CurrentSelection.SelectAll();
                collection = doc.CurrentSelection.SelectedItems;
            }

            return collection;
        }

        // RunAttribSearch
        //
        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string userCategory, string internalCategory, string userProperty, string intrenalProperty, string propertyValue)
        {
            dynamic value = null;

            try
            {
                value = Convert.ToInt32(propertyValue);
            }
            catch
            {
                value = propertyValue;
            }

            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.SetPropertyNames(intrenalProperty, userProperty);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = value;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }
        #endregion

        #region # RunClassNameSearch
        // RunClassNameSearch
        public void RunClassNameSearch(string[] words, bool wholeWord)
        {
            ModelItemCollection selectCollection = new ModelItemCollection();

            foreach (string word in words)
            {
                selectCollection.AddRange(SearchModelItems(word.Trim(), wholeWord));
            }

            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Clear();
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.CopyFrom(selectCollection);

            form.Close();
        }


        // RunClassNameSearch
        private ModelItemCollection SearchModelItems(string classDisplayName, bool wholeWord)
        {
            ModelItemCollection coll = new ModelItemCollection();

            foreach(ModelItem modelItem in doc.CurrentSelection.SelectedItems.DescendantsAndSelf)
            {
                if(wholeWord)
                {
                    if (modelItem.DisplayName == classDisplayName)
                    {
                        coll.Add(modelItem);
                    }
                }
                else
                {
                    if (modelItem.DisplayName.Contains(classDisplayName))
                    {
                        coll.Add(modelItem);
                    }
                }
            }
            
            return coll;
        }
        #endregion
    }
}
