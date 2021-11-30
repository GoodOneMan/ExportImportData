using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.Navisworks.Api.Interop;


namespace ExportGeometry.UnitsApp.Tests
{
    class Reflectiontest
    {
        public Reflectiontest()
        {
            Type myType = typeof(LcVwISimplePrimitives);

           
            //Type myType = Type.GetType("Autodesk.Navisworks.Api.Interop.LcVwISimplePrimitives", false, true);
            foreach (MethodInfo method in myType.GetMethods())
            {
                string modificator = "";
                if (method.IsStatic)
                    modificator += "static ";
                if (method.IsVirtual)
                    modificator += "virtual ";

                string str = modificator + method.ReturnType.Name + " " + method.Name + " (";

                ParameterInfo[] parameters = method.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    str += parameters[i].ParameterType.Name + " " + parameters[i].Name;
                    if (i + 1 < parameters.Length) str += ", ";
                }
                str += ")";
            }
        }
    }
}
