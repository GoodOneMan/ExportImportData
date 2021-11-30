// ManagedPluginCpp.h

#pragma once

using namespace System;
using namespace Autodesk::Navisworks::Api;
using namespace Autodesk::Navisworks::Api::Plugins;


namespace ManagedPluginCpp 
{
   [PluginAttribute("RawCOMTest", "ADSK", DisplayName = "COM_Geometry")]
   [AddInPluginAttribute(AddInLocation::AddIn)]
   public ref class MainPlugin : AddInPlugin
   {
   public:
      virtual int Execute(...cli::array<System::String^,1>^ parameters)override;
    
   };

}
