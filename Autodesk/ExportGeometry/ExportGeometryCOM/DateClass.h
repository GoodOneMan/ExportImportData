#pragma once

ref class dateClass {
public:
	static System::DateTime stTime;
	static System::IO::StreamWriter^ outfile;

	//Test metod
	static void WriteVertexToFile(double* arr, int size_arr) 
	{
		for (int i = 0; i < size_arr; i++)
		{
			outfile->WriteLine(arr[i]);
		}
	}
};