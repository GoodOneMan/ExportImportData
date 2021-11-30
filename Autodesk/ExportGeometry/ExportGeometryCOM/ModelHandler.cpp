
#include "ModelHandler.h"
#include "DateClass.h";
#include "GeometryHandler.h";

#import "C:\Program Files\Autodesk\Navisworks Simulate 2017\lcodieD.dll"  rename_namespace("raw")

namespace Api = Autodesk::Navisworks::Api;
namespace ComApiAccess = Autodesk::Navisworks::Api::ComApi;
namespace InteropComApi = Autodesk::Navisworks::Api::Interop::ComApi;

void
model_handler::run() 
{
	Api::Document^ doc = Api::Application::ActiveDocument;
	Api::ModelItemCollection^ collection = doc->CurrentSelection->SelectedItems;

	char* internal_name = "LcOpGeometryProperty";
	char* user_name = "Геометрия";

	InteropComApi::InwOpSelection^ selection = model_handler::finder(collection, internal_name, user_name);

	dateClass::outfile = gcnew System::IO::StreamWriter("D:\\vertex.txt");
	/*dateClass::outfile = gcnew System::IO::StreamWriter("D:\\vertex_test.obj");
	dateClass::outfile->WriteLine("# 3ds Max Wavefront OBJ Exporter v0.97b - (c)2007 guruware");
	dateClass::outfile->WriteLine("o Model_float");*/

	// Call GeometryHandler
	IUnknown* iuSelection = static_cast<IUnknown*>(System::Runtime::InteropServices::Marshal::GetIUnknownForObject(selection).ToPointer());
	geometry_handler::_run(iuSelection);

	dateClass::outfile->Close();
}

InteropComApi::InwOpSelection^
model_handler::finder(Api::ModelItemCollection^ collection, char* internal_name_ch, char* user_name_ch)
{
	//State10
	InteropComApi::InwOpState10^ opState10 = ComApiAccess::ComApiBridge::State;
	//System::IntPtr state10Ptr = Marshal::GetIUnknownForObject(opState10);
	IUnknown* iuState10 = static_cast<IUnknown*>(System::Runtime::InteropServices::Marshal::GetIUnknownForObject(opState10).ToPointer());
	raw::InwOpState10Ptr pState10(iuState10);

	//SelectionSet2
	InteropComApi::InwOpSelectionSet2^ opSelSet2 = nullptr;
	opSelSet2 = static_cast<InteropComApi::InwOpSelectionSet2^>(opState10->ObjectFactory(InteropComApi::nwEObjectType::eObjectType_nwOpSelectionSet, NULL, NULL));
	//System::IntPtr selSet2Ptr = Marshal::GetIUnknownForObject(opSelSet2);
	IUnknown* iuSelSet2 = static_cast<IUnknown*>(System::Runtime::InteropServices::Marshal::GetIUnknownForObject(opSelSet2).ToPointer());
	raw::InwOpSelectionSet2Ptr pSelSet2(iuSelSet2);

	pSelSet2->name = SysAllocString(L"ByCategorySelection");

	//FindSpecPtr FindCondition
	raw::InwOpFindSpecPtr nwFindSpec = pState10->ObjectFactory(raw::eObjectType_nwOpFindSpec);
	raw::InwOpFindConditionPtr nwFindCondition = pState10->ObjectFactory(raw::eObjectType_nwOpFindCondition);

	BSTR internal_name = _bstr_t(internal_name_ch);

	nwFindCondition->SetAttributeNames(internal_name, user_name_ch);
	nwFindCondition->Condition = raw::eFind_HAS_ATTRIB;

	//Selection2
	InteropComApi::InwOpSelection2^ opSelection2 = static_cast<InteropComApi::InwOpSelection2^>(ComApiAccess::ComApiBridge::ToInwOpSelection(collection));
	IUnknown* iuSelection2 = static_cast<IUnknown*>(System::Runtime::InteropServices::Marshal::GetIUnknownForObject(opSelection2).ToPointer());
	raw::InwOpSelection2Ptr pSelect2(iuSelection2);

	nwFindSpec->selection = pSelect2;
	nwFindSpec->SearchMode = raw::eSearchMode_BELOW_SELECTED_PATHS;
	nwFindSpec->ResultDisjoint = false;
	nwFindSpec->Conditions()->Add(nwFindCondition);

	pSelSet2->ImplicitFindSpec = nwFindSpec;

	return opSelSet2->selection;
}