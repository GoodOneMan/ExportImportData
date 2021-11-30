#pragma once

#include <Unknwn.h>
#include <vector>
class geometry_handler
{
public:
	static void _run(IUnknown* iunk_state);
	static void _get_coordinate(VARIANT varValue, double *arrValue);
	static void _get_matrix(VARIANT varValue, double *arrValue);
	static void _lsc_to_wsc(double *matrix, double *coordinate, double *arrValue);
	static bool _comparing_arrays(double* arr_one, double* arr_two);
};