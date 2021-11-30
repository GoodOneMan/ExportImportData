#pragma once

class model_handler
{
public:
	// run
	static void run();
	// finder
	static Autodesk::Navisworks::Api::Interop::ComApi::InwOpSelection^ finder(
		Autodesk::Navisworks::Api::ModelItemCollection^ collection,
		char* internel_name,
		char* user_name);
};