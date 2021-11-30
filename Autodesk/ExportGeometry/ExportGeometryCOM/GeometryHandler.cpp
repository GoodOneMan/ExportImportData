
#include "GeometryHandler.h"
#include "DateClass.h"
#include <vector>
#include <atlbase.h>
#include <atlcom.h>
#include <fstream>;
#import "C:\Program Files\Autodesk\Navisworks Simulate 2017\lcodieD.dll"  rename_namespace("raw")

using namespace System;

CComModule _Module;
std::ofstream fout;

// callback class
class CallbackGeomClass :public ATL::CComObjectRoot, public IDispatchImpl<raw::InwSimplePrimitivesCB>
{
public:

	double *matrix;

	BEGIN_COM_MAP(CallbackGeomClass)
		COM_INTERFACE_ENTRY(raw::InwSimplePrimitivesCB)
	END_COM_MAP()

	STDMETHOD(raw_Triangle)(/*[in]*/ struct raw::InwSimpleVertex* v1, /*[in]*/ struct raw::InwSimpleVertex* v2, /*[in]*/ struct raw::InwSimpleVertex* v3)
	{

		VARIANT varV1;
		v1->get_coord(&varV1);
		double vertex1[3];
		geometry_handler::_get_coordinate(varV1, vertex1);
		double wsc_vertex1[3];
		geometry_handler::_lsc_to_wsc(matrix, vertex1, wsc_vertex1);
		//dateClass::outfile->WriteLine("v " + wsc_vertex1[0] + " " + wsc_vertex1[1] + " " + wsc_vertex1[2]);
		//dateClass::WriteVertexToFile(wsc_vertex1, 3);



		VARIANT varV2;
		v1->get_coord(&varV2);
		double vertex2[3];
		geometry_handler::_get_coordinate(varV2, vertex2);
		double wsc_vertex2[3];
		geometry_handler::_lsc_to_wsc(matrix, vertex2, wsc_vertex2);
		//dateClass::outfile->WriteLine("v " + wsc_vertex2[0] + " " + wsc_vertex2[1] + " " + wsc_vertex2[2]);
		//dateClass::WriteVertexToFile(wsc_vertex2, 3);

		
		VARIANT varV3;
		v1->get_coord(&varV3);
		double vertex3[3];
		geometry_handler::_get_coordinate(varV3, vertex3);
		double wsc_vertex3[3];
		geometry_handler::_lsc_to_wsc(matrix, vertex3, wsc_vertex3);
		//dateClass::outfile->WriteLine("v " + wsc_vertex3[0] + " " + wsc_vertex3[1] + " " + wsc_vertex3[2]);
		//dateClass::WriteVertexToFile(wsc_vertex3, 3);

		fout.write((char*)&wsc_vertex1, sizeof(wsc_vertex1));
		fout.write((char*)&wsc_vertex2, sizeof(wsc_vertex2));
		fout.write((char*)&wsc_vertex3, sizeof(wsc_vertex3));

		return S_OK;
	}

	STDMETHOD(raw_Line)(/*[in]*/ struct raw::InwSimpleVertex* v1,/*[in]*/ struct raw::InwSimpleVertex* v2)
	{
		return S_OK;
	}

	STDMETHOD(raw_Point)(/*[in]*/ struct raw::InwSimpleVertex* v1)
	{
		return S_OK;
	}

	STDMETHOD(raw_SnapPoint)(/*[in]*/ struct raw::InwSimpleVertex* v1)
	{
		return S_OK;
	}

	CallbackGeomClass()
	{

	}
};

void
geometry_handler::_run(IUnknown* iunk_state)
{
	fout.open("D:\\test.txt");

	raw::InwOpSelectionPtr pSelection(iunk_state);
	raw::InwSelectionPathsCollPtr pPathsColl = pSelection->Paths();

	long count_path = pPathsColl->Count;
	for (long index_path = 1; index_path <= count_path; index_path++)
	{
		raw::InwOaPathPtr path = pPathsColl->GetItem(_variant_t(index_path));
		long count_frag = path->Fragments()->Count;

		for (long index_frag = 1; index_frag <= count_frag; index_frag++)
		{
			CComObject<CallbackGeomClass> *callbkListener;
			HRESULT HR = CComObject<CallbackGeomClass>::CreateInstance(&callbkListener);

			raw::InwOaFragment3Ptr pFrag = path->Fragments()->GetItem(_variant_t(index_frag));

			if (SUCCEEDED(HR))
			{
				VARIANT varMatrix;
				HRESULT hrmatrix = pFrag->GetLocalToWorldMatrix()->get_Matrix(&varMatrix);
				if (SUCCEEDED(hrmatrix)) 
				{
					double arrMatrix[16];
					geometry_handler::_get_matrix(varMatrix, arrMatrix);

					callbkListener->matrix = arrMatrix; 
					pFrag->GenerateSimplePrimitives(raw::nwEVertexProperty::eNORMAL, callbkListener);

				}
			}
		}
	}

	fout.close();

}

// Coordinate
void
geometry_handler::_get_coordinate(VARIANT varValue, double *arrValue)
{
	SAFEARRAY *pArrV1 = varValue.parray;
	float *pVals;
	//double arrValue[3];
	HRESULT hr = SafeArrayAccessData(pArrV1, (void**)&pVals);
	if (SUCCEEDED(hr))
	{
		long lowerBound, upperBound;
		SafeArrayGetLBound(pArrV1, 1, &lowerBound);
		SafeArrayGetUBound(pArrV1, 1, &upperBound);

		long cnt_elements = upperBound - lowerBound + 1;
		for (int i = 0; i < cnt_elements; ++i)
		{
			arrValue[i] = static_cast<double>(pVals[i]);
		}
		SafeArrayUnaccessData(pArrV1);
	}
	SafeArrayDestroy(pArrV1);
}

// Matrix
void
geometry_handler::_get_matrix(VARIANT varValue, double *arrValue)
{
	SAFEARRAY *pArrV1 = varValue.parray;
	double *pVals;
	//double arrValue[16];
	HRESULT hr = SafeArrayAccessData(pArrV1, (void**)&pVals);
	if (SUCCEEDED(hr))
	{
		long lowerBound, upperBound;
		SafeArrayGetLBound(pArrV1, 1, &lowerBound);
		SafeArrayGetUBound(pArrV1, 1, &upperBound);

		long cnt_elements = upperBound - lowerBound + 1;
		for (int i = 0; i < cnt_elements; ++i)
		{
			arrValue[i] = pVals[i];
		}
		SafeArrayUnaccessData(pArrV1);
	}
	SafeArrayDestroy(pArrV1);
}

// LSC to WSC
void
geometry_handler::_lsc_to_wsc(double *matrix, double *coordinate, double *arrValue)
{
	arrValue[0] = (matrix[0] * coordinate[0] + matrix[4] * coordinate[1]) + matrix[12];
	arrValue[1] = (matrix[1] * coordinate[0] + matrix[5] * coordinate[1]) + matrix[13];
	arrValue[2] = (matrix[10] * coordinate[2]) + matrix[14];
}

// Сравнение массивов ???
bool
geometry_handler::_comparing_arrays(double* arr_one, double* arr_two) 
{
	bool value = true;
	size_t size_one = sizeof(arr_one);
	size_t size_two = sizeof(arr_two);

	if (size_one != size_two)
	{
		return false;
	}

	for (size_t i = 0; i < 3; i++)
	{
		if (arr_one[i] != arr_two[i])
		{
			return false;
		}
	}

	return value;
}