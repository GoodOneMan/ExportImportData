using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Autodesk.Navisworks.Api;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using System.Numerics;

namespace ExportGeometry.UnitsApp.Helpers
{
    class ConvertLCSToWCS
    {
        public static Vector3 getCoordinate(Array matrix, Array point)
        {
            double x = (Convert.ToDouble(matrix.GetValue(1)) * Convert.ToDouble(point.GetValue(1)) + Convert.ToDouble(matrix.GetValue(5)) * Convert.ToDouble(point.GetValue(2))) + Convert.ToDouble(matrix.GetValue(13));
            double y = (Convert.ToDouble(matrix.GetValue(2)) * Convert.ToDouble(point.GetValue(1)) + Convert.ToDouble(matrix.GetValue(6)) * Convert.ToDouble(point.GetValue(2))) + Convert.ToDouble(matrix.GetValue(14));
            double z = (Convert.ToDouble(matrix.GetValue(11)) * Convert.ToDouble(point.GetValue(3))) + Convert.ToDouble(matrix.GetValue(15));

            Vector3 coordinate = new Vector3((float)x, (float)y, (float)z);

            return coordinate;
        }

        public static void setCoordinate(double[] list, Array matrix, Array point)
        {
            list[0] = (( Convert.ToDouble(matrix.GetValue(1)) * Convert.ToDouble(point.GetValue(1)) + Convert.ToDouble(matrix.GetValue(5)) * Convert.ToDouble(point.GetValue(2)) ) + Convert.ToDouble(matrix.GetValue(13)) );
            list[1] = (( Convert.ToDouble(matrix.GetValue(2)) * Convert.ToDouble(point.GetValue(1)) + Convert.ToDouble(matrix.GetValue(6)) * Convert.ToDouble(point.GetValue(2)) ) + Convert.ToDouble(matrix.GetValue(14)) );
            list[2] = (( Convert.ToDouble(matrix.GetValue(11)) * Convert.ToDouble(point.GetValue(3)) ) + Convert.ToDouble(matrix.GetValue(15)) );
        }


        public static double[] setCoordinate(double[] matrix, float[] coordinate)
        {
            double[] GCS_Coordinate = new double[3];
            GCS_Coordinate[0] = ((matrix[0] * coordinate[0] + matrix[4] * coordinate[1]) + matrix[12]);
            GCS_Coordinate[1] = ((matrix[1] * coordinate[0] + matrix[5] * coordinate[1]) + matrix[13]);
            GCS_Coordinate[2] = ((matrix[10] * coordinate[2]) + matrix[14]);
            return GCS_Coordinate;
        }
    }
}
