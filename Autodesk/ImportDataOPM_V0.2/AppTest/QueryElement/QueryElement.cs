using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using System.IO;
using System.Runtime.InteropServices;

namespace ImportDataOPM.AppTest.QueryElement
{
    class QueryElement
    {
        List<string> listEcClass = new List<string>();
        List<string> listFamily = new List<string>();
        List<string> listCategory = new List<string>();

        public QueryElement()
        {
            ModelItemCollection collection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            if(collection.Count == 0)
            {
                Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectAll();
                collection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            }

            // DGN
            // string internalCategory = "LcOaPropOverrideCat";
            // string userCategory = "MicroStation - Данные на элементе"; 
            // string intrenalProperty = "EC_CLASS_NAME";
            // string userProperty = "Имя класса EC";

            // RVT
            // string internalCategory = "LcRevitData_Element"
            // string userCategory = "Объект"

            // string intrenalProperty = "LcRevitPropertyElementFamily";
            // string userProperty = "Семейство";

            // string intrenalProperty = "LcRevitPropertyElementCategory";
            // string userProperty = "Категория";

            #region Init
            InitDgn(collection);
            InitRvt(collection);
            #endregion
        }

        #region Get data
        public List<string> GetEcClass()
        {
            return listEcClass;
        }

        public List<string> GetFamily()
        {
            return listFamily;
        }

        public List<string> GetCategory()
        {
            return listCategory;
        }
        #endregion

        #region DGN Search
        private void InitDgn(ModelItemCollection collection)
        {
            string internalCategory = "LcOaPropOverrideCat";
            string userCategory = "MicroStation - Данные на элементе";
            string intrenalProperty = "EC_CLASS_NAME";
            string userProperty = "Имя класса EC";

            ModelItemCollection searchCollection = SearchModelItems(collection, internalCategory, userCategory);

            foreach(ModelItem item in searchCollection)
            {
                DataProperty property = item.PropertyCategories.FindPropertyByCombinedName(new NamedConstant(internalCategory, userCategory), new NamedConstant(intrenalProperty, userProperty));

                if(property != null)
                {
                    string value = property.Value.ToDisplayString();
                    if (!listEcClass.Contains(value))
                        listEcClass.Add(value);
                }
            }

        }
        #endregion

        #region RVT Search
        private void InitRvt(ModelItemCollection collection)
        {
            string internalCategory = "LcRevitData_Element";
            string userCategory = "Объект";

            string intrenalPropertyF = "LcRevitPropertyElementFamily";
            string userPropertyF = "Семейство";

            string intrenalPropertyC = "LcRevitPropertyElementCategory";
            string userPropertyC = "Категория";

            ModelItemCollection searchCollection = SearchModelItems(collection, internalCategory, userCategory);

            foreach (ModelItem item in searchCollection)
            {
                DataProperty propertyF = item.PropertyCategories.FindPropertyByCombinedName(new NamedConstant(internalCategory, userCategory), new NamedConstant(intrenalPropertyF, userPropertyF));
                DataProperty propertyC = item.PropertyCategories.FindPropertyByCombinedName(new NamedConstant(internalCategory, userCategory), new NamedConstant(intrenalPropertyC, userPropertyC));

                if (propertyF != null)
                {
                    string value = propertyF.Value.ToDisplayString();
                    if (!listFamily.Contains(value))
                        listFamily.Add(value);
                }

                if (propertyC != null)
                {
                    string value = propertyC.Value.ToDisplayString();
                    if (!listCategory.Contains(value))
                        listCategory.Add(value);
                }
            }
        }

        #endregion

        #region Search cretaries
        public void SearchCriteries(List<string> ecClass, List<string> family, List<string> category)
        {
            ModelItemCollection collection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            if (collection.Count == 0)
            {
                Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectAll();
                collection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            }

            ComApi.InwOpState10 opState = ComBridge.State;

            opState.HiddenItemsResetAll();

            ModelItemCollection selectCollection = new ModelItemCollection();

            string internalCategoryDGN = "LcOaPropOverrideCat";
            string userCategoryDGN = "MicroStation - Данные на элементе";
            string intrenalPropertyDGN = "EC_CLASS_NAME";
            string userPropertyDGN = "Имя класса EC";

            string internalCategoryRVT = "LcRevitData_Element";
            string userCategoryRVT = "Объект";

            string intrenalPropertyRVT_F = "LcRevitPropertyElementFamily";
            string userPropertyRVT_F = "Семейство";

            string intrenalPropertyRVT_C = "LcRevitPropertyElementCategory";
            string userPropertyRVT_C = "Категория";

            foreach (string ec_class_name in ecClass)
            {
                ModelItemCollection selection = SearchModelItems(collection, internalCategoryDGN, userCategoryDGN, intrenalPropertyDGN, userPropertyDGN, ec_class_name);
                selectCollection.AddRange(selection);
            }

            foreach(string family_name in family)
            {
                ModelItemCollection selection = SearchModelItems(collection, internalCategoryRVT, userCategoryRVT, intrenalPropertyRVT_F, userPropertyRVT_F, family_name);
                selectCollection.AddRange(selection);
            }

            foreach(string category_name in category)
            {
                ModelItemCollection selection = SearchModelItems(collection, internalCategoryRVT, userCategoryRVT, intrenalPropertyRVT_C, userPropertyRVT_C, category_name);
                selectCollection.AddRange(selection);
            }

            ComApi.InwOpSelection comSelection = ComBridge.ToInwOpSelection(selectCollection);
            comSelection.Invert();
            opState.SelectionHidden[comSelection] = true;
        }
        #endregion

        #region Search item
        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string internalCategory, string userCategory)
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string internalCategory, string userCategory, string intrenalProperty, string userProperty, string propertyValue)
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
        
    }
}
