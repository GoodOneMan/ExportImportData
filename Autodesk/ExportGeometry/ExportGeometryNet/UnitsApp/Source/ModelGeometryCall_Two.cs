using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Navisworks.Api;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;

namespace ExportGeometry.UnitsApp.Source
{
    class ModelGeometryCall_Two
    {
        DS_Two.Model_3D model;

        public ModelGeometryCall_Two()
        {
            Run();

            new Tests.WriteObj_sFile(model);
        }

        private void Run()
        {
            model = new DS_Two.Model_3D();
            model.name = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems.First.DisplayName;
            model.items = new List<DS_Two.Item>();
            
            string internel_name = "LcOpGeometryProperty";
            string user_name = "Геометрия";

            ModelItemCollection collection = new FI.FinderItem().SearchByCategory(internel_name, user_name);

            var enum_collection = collection.GetEnumerator();
            while(enum_collection.MoveNext())
            {
                ModelItem model_item = enum_collection.Current;

                if (model_item.Geometry.PrimitiveTypes != PrimitiveTypes.Triangles)
                    continue;

                DS_Two.Item item = new DS_Two.Item();
                item.name = model_item.DisplayName;
                ModelGeometry geometry = model_item.Geometry;
                // Color color = geometry.ActiveColor;
                item.frag_count = geometry.FragmentCount;
                item.prim_count = geometry.PrimitiveCount;
                // item.color = new double[] { color.R, color.G, color.B };
                item.fragments = new List<DS_Two.Fragment>();
                item.properties = new List<DS_Two.Property>();

                // Geometry
                RobGeometry(ref item.fragments, model_item);

                // Property


                model.items.Add(item);
            }
        }

        #region Geometry
        private void RobGeometry(ref List<DS_Two.Fragment> list, ModelItem geometry)
        {
            ComApi.InwOaPath oaPath = ComApiBridge.ToInwOaPath(geometry);
            CallbackGeometry callbkListener = new CallbackGeometry();
            
            foreach (ComApi.InwOaFragment3 fragment in oaPath.Fragments())
            {
                DS_Two.Fragment fragment_s = new DS_Two.Fragment();
                fragment_s.matrix = present_matrix((Array)(object)fragment.GetLocalToWorldMatrix().Matrix);

                var appearance = (Array)(object)fragment.Appearance;
                fragment_s.ambient = new float[] { (float)appearance.GetValue(1), (float)appearance.GetValue(2), (float)appearance.GetValue(3) };
                fragment_s.diffuse = new float[] { (float)appearance.GetValue(4), (float)appearance.GetValue(5), (float)appearance.GetValue(6) };
                fragment_s.specular = new float[] { (float)appearance.GetValue(7), (float)appearance.GetValue(8), (float)appearance.GetValue(9) };
                fragment_s.emissive = new float[] { (float)appearance.GetValue(10), (float)appearance.GetValue(11), (float)appearance.GetValue(12) };
                fragment_s.shininess = (float)appearance.GetValue(13);
                fragment_s.transparency = (float)appearance.GetValue(14);

                fragment_s.points = new List<DS_Two.Point>();
                fragment_s.faces = new List<int>();

                callbkListener.points = fragment_s.points;
                callbkListener.faces = fragment_s.faces;

                fragment.GenerateSimplePrimitives(ComApi.nwEVertexProperty.eNORMAL, callbkListener);

                list.Add(fragment_s);
            }
        }

        // matrix transformation
        double[] present_matrix(Array matx)
        {
            double[] matrix = new double[16];

            for (int i = 0; i < 16; i++)
            {
                matrix[i] = (double)matx.GetValue(i + 1);
            }

            return matrix;
        }
        #endregion
    }

    #region InwSimplePrimitivesCB Class
    class CallbackGeometry : ComApi.InwSimplePrimitivesCB
    {
        public List<DS_Two.Point> points;
        public List<int> faces;

        public void Line(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2)
        {
            var v_1 = v1.coord;
            var v_2 = v2.coord;
        }

        public void Point(ComApi.InwSimpleVertex v1)
        {
            var v_1 = v1.coord;
        }

        public void SnapPoint(ComApi.InwSimpleVertex v1)
        {
            var v_1 = v1.coord;
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
                DS_Two.Point point = new DS_Two.Point();

                point.coordinate = coord;
                point.normal = convert_to_float((Array)(object)vertex.normal);
                point.texture = convert_to_float((Array)(object)vertex.tex_coord);

                points.Add(point);
                faces.Add(points.Count);
            }
            else
            {
                faces.Add(index + 1);
            }
        }

        int get_index(List<DS_Two.Point> points, float[] coord)
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
