using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Navisworks.Api;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;


namespace ExportGeometry.UnitsApp.Tests
{
    class GeometryReadTest
    {
        DS.Model_3D model;
        DS.Item ds_item;
        DS.Fragment fragment_s;
        StreamWriter sw;

        public GeometryReadTest()
        {
            sw = new StreamWriter(@"D:\line_point.txt");
            model = new DS.Model_3D();
            model.items = new List<DS.Item>();

            Run();

            Tests.WriteToObj wtof = new Tests.WriteToObj(model);
            sw.Close();
        }

        private void Run()
        {
            SelectItems();
        }

        private void SelectItems()
        {
            ComApi.InwOpState10 opState = ComApiBridge.State;

            string internal_name = "LcOpGeometryProperty"; 
            string user_name = "Геометрия";

            //string internal_name = "LcOaPropOverrideCat"; 
            //string user_name = "Данные MicroStation";

            //string internal_name = "LcRevitData_Element"; 
            //string user_name = "Объект";

            ComApi.InwOpSelection selection = new FI.FinderItem()._SearchByCategory(internal_name, user_name);

            ComApi.InwSelectionPathsColl paths = selection.Paths();

            int count = paths.Count;

            if(count > 0)
            {
                var enum_paths = paths.GetEnumerator();

                while (enum_paths.MoveNext())
                {
                    ComApi.InwOaPath3 path = enum_paths.Current as ComApi.InwOaPath3;
                    ds_item = new DS.Item();
                    
                    GeometryHandler(path);

                    model.items.Add(ds_item);
                }
            }
        }

        private void GeometryHandler(ComApi.InwOaPath3 path)
        {
            ComApi.InwNodeFragsColl frags = path.Fragments();

            ds_item.fragments = new List<DS.Fragment>();

            int count = frags.Count;

            if(count > 0)
            {
                var enum_frag = frags.GetEnumerator();

                while(enum_frag.MoveNext())
                {

                    ComApi.InwOaFragment3 frag = enum_frag.Current as ComApi.InwOaFragment3;

                    FragmentsHandler(frag);
                }
            }
        }

        private void FragmentsHandler(ComApi.InwOaFragment3 fragment)
        {

            fragment_s = new DS.Fragment();
            CallbackGeomListenerFive callbkListener = new CallbackGeomListenerFive();

            fragment_s.matrix = present_matrix((Array)(object)fragment.GetLocalToWorldMatrix().Matrix);
            fragment_s.points = new List<DS.Point>();
            fragment_s.faces = new List<int>();

            callbkListener.points = fragment_s.points;
            callbkListener.faces = fragment_s.faces;
            callbkListener.sw = sw;
            fragment.GenerateSimplePrimitives(ComApi.nwEVertexProperty.eNORMAL, callbkListener);

            ds_item.fragments.Add(fragment_s);
        }

        double[] present_matrix(Array matx)
        {
            double[] matrix = new double[16];

            for (int i = 0; i < 16; i++)
            {
                matrix[i] = (double)matx.GetValue(i + 1);
            }

            return matrix;
        }
    }

    #region InwSimplePrimitivesCB Class
    class CallbackGeomListenerFive : ComApi.InwSimplePrimitivesCB
    {
        public List<DS.Point> points;
        public List<int> faces;
        public StreamWriter sw;

        public void Line(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2)
        {
            var v_1 = (Array)(object)v1.coord;
            var v_2 = (Array)(object)v2.coord;
            sw.WriteLine("Line p1 " + v_1.GetValue(1) + " " + v_1.GetValue(2) + " " + v_1.GetValue(3));
            sw.WriteLine("Line p2 " + v_2.GetValue(1) + " " + v_2.GetValue(2) + " " + v_2.GetValue(3));

            set_ds_point(v1);
            set_ds_point(v2);
        }

        public void Point(ComApi.InwSimpleVertex v1)
        {
            var v_1 = (Array)(object)v1.coord;
            sw.WriteLine("Point " + v_1.GetValue(1) + " " + v_1.GetValue(2) + " " + v_1.GetValue(3));

            set_ds_point(v1);
        }

        public void SnapPoint(ComApi.InwSimpleVertex v1)
        {
            var v_1 = (Array)(object)v1.coord;
            sw.WriteLine("SnapPoint " + v_1.GetValue(1) + " " + v_1.GetValue(2) + " " + v_1.GetValue(3));

            set_ds_point(v1);
        }

        public void Triangle(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2, ComApi.InwSimpleVertex v3)
        {
            set_ds_point(v1);
            set_ds_point(v2);
            set_ds_point(v3);
        }

        void set_ds_point(ComApi.InwSimpleVertex vertex)
        {
            // coordinate vertex
            float[] coord = convert_to_float((Array)(object)vertex.coord);
            int index = get_index(points, coord);

            if (index == -1)
            {
                DS.Point point = new DS.Point();

                point.coordinate = coord;
                //point.color = convert_to_float((Array)(object)vertex.color);
                point.normal = convert_to_float((Array)(object)vertex.normal);
                //point.texture = convert_to_float((Array)(object)vertex.tex_coord);

                points.Add(point);
                faces.Add(points.Count);
            }
            else
            {
                faces.Add(index + 1);
            }
        }

        int get_index(List<DS.Point> points, float[] coord)
        {
            int count = points.Count;

            for (int i = 0; i < count; i++)
            {
                if (equel_coordinate(points[i].coordinate, coord))
                    return i;
            }

            return -1;
        }

        bool equel_coordinate(float[] arr_one, float[] arr_two)
        {
            if (arr_one[0] != arr_two[0])
                return false;
            if (arr_one[1] != arr_two[1])
                return false;
            if (arr_one[2] != arr_two[2])
                return false;

            return true;
        }

        float[] convert_to_float(Array data)
        {
            int count = data.Length;
            float[] result = new float[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = (float)(object)data.GetValue(i + 1);
            }

            return result;
        }
    }
    #endregion
}
