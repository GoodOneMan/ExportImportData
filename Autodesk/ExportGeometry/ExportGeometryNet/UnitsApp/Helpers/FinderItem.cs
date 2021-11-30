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

namespace ExportGeometry.UnitsApp.Helpers
{
    class FinderItem
    {
        int param;
        ComApi.InwOpState3 opState = ComApiBridge.State;


        public FinderItem(int param)
        {
            this.param = param;
        }

        // SearchByCategory
        public ModelItemCollection SearchByCategory(string internal_name = "", string user_name = "")
        {
            ComApi.InwOpSelectionSet2 oNewSelSet = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            oNewSelSet.name = "ByCategorySelection";
            
            ComApi.InwOpFindSpec nwFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition nwFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            nwFindCondition.SetAttributeNames(internal_name, user_name);
            nwFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;
            
            nwFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            nwFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            nwFindSpec.Conditions().Add(nwFindCondition);
            
            oNewSelSet.ImplicitFindSpec = nwFindSpec;
            
            return ComApiBridge.ToModelItemCollection(oNewSelSet.selection);
        }

        // SearchByCategoryAndProperty
        public ModelItemCollection SearchByCategoryAndProperty(string cat_internal_name = "", string cat_user_name = "", string prop_internal_name = "", string prop_user_name = "", string value = "")
        {
            ComApi.InwOpSelectionSet2 oNewSelSet = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            oNewSelSet.name = "ByCategorySelection";

            ComApi.InwOpFindSpec nwFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition nwFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            nwFindCondition.SetAttributeNames(cat_internal_name, cat_user_name);
            nwFindCondition.SetPropertyNames(prop_internal_name, prop_user_name);
            nwFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            nwFindCondition.value = value;

            nwFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            nwFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            nwFindSpec.Conditions().Add(nwFindCondition);

            oNewSelSet.ImplicitFindSpec = nwFindSpec;

            return ComApiBridge.ToModelItemCollection(oNewSelSet.selection);
        }

        // SearchByProperty
        public ModelItemCollection SearchByProperty(string prop_internal_name = "", string prop_user_name = "", string value = "")
        {
            ComApi.InwOpSelectionSet2 oNewSelSet = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            oNewSelSet.name = "ByCategorySelection";

            ComApi.InwOpFindSpec nwFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition nwFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            nwFindCondition.SetPropertyNames(prop_internal_name, prop_user_name);
            nwFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            nwFindCondition.value = value;

            nwFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            nwFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            nwFindSpec.Conditions().Add(nwFindCondition);

            oNewSelSet.ImplicitFindSpec = nwFindSpec;

            return ComApiBridge.ToModelItemCollection(oNewSelSet.selection);
        }


        // New Method

        public ModelItemCollection _SearchByCategoryAndProperty(string cat_internal_name = "", string cat_user_name = "", string prop_internal_name = "", string prop_user_name = "", string value = "")
        {
            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(cat_internal_name, cat_user_name);
            opFindCondition.SetPropertyNames(prop_internal_name, prop_user_name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = value;

            opFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);
            opFindSpec.ResultDisjoint = false;
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_ALL_PATHS;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComApiBridge.ToModelItemCollection(opSelSet2.selection);
        }

        public ModelItemCollection _SearchByCategory(string internal_name = "", string user_name = "")
        {
            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internal_name, user_name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_ALL_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComApiBridge.ToModelItemCollection(opSelSet2.selection);
        }
    }
}

namespace ExportGeometry.UnitsApp.FI
{
    class FinderItem
    {
        public ModelItemCollection SearchByCategory(string internal_name = "", string user_name = "")
        {
            ComApi.InwOpState10 opState = ComApiBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internal_name, user_name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComApiBridge.ToModelItemCollection(opSelSet2.selection);
        }

        public ComApi.InwOpSelection _SearchByCategory(string internal_name = "", string user_name = "")
        {
            ComApi.InwOpState10 opState = ComApiBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internal_name, user_name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = opState.CurrentSelection;
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return opSelSet2.selection;
        }
    }
}
