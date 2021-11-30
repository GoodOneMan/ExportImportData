using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Navisworks.Api;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;

namespace ExportGeometry.UnitsApp.Source
{
    class ModelGeometryCall_Three
    {
        DS_Three.Model_3D model;

        StreamWriter sw;

        public ModelGeometryCall_Three()
        {
            sw = new StreamWriter(@"D:\matrix.txt");

            Run();
            new Tests.WriteToObjFile_Three(model);

            sw.Close();
        }

        private void Run()
        {
            model = new DS_Three.Model_3D();
            model.name = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems.First.DisplayName;
            model.items = new List<DS_Three.Item>();

            string internel_name = "LcOpGeometryProperty";
            string user_name = "Геометрия";

            ModelItemCollection collection = new FI.FinderItem().SearchByCategory(internel_name, user_name);

            var enum_collection = collection.GetEnumerator();
            while (enum_collection.MoveNext())
            {
                ModelItem model_item = enum_collection.Current;

                DS_Three.Item item = new DS_Three.Item();
                item.name = model_item.DisplayName;

                ModelGeometry geometry = model_item.Geometry;
                item.frag_count = geometry.FragmentCount;
                item.prim_count = geometry.PrimitiveCount;

                item.fragments = new List<DS_Three.Fragment>();
                item.properties = new List<DS_Three.Property>();

                // Geometry
                RobGeometry(ref item.fragments, model_item);

                // Property


                model.items.Add(item);
            }
        }

        #region Geometry
        private void RobGeometry(ref List<DS_Three.Fragment> list, ModelItem geometry)
        {
            ComApi.InwOaPath oaPath = ComApiBridge.ToInwOaPath(geometry);
            CallbackGeometry_Three callbkListener = new CallbackGeometry_Three();

            foreach (ComApi.InwOaFragment3 fragment in oaPath.Fragments())
            {
                DS_Three.Fragment fragment_s = new DS_Three.Fragment();

                var user_offset = fragment.UserOffset;

                // Color
                var appearance = (Array)(object)fragment.Appearance;
                fragment_s.ambient = new float[] { (float)appearance.GetValue(1), (float)appearance.GetValue(2), (float)appearance.GetValue(3) };
                fragment_s.diffuse = new float[] { (float)appearance.GetValue(4), (float)appearance.GetValue(5), (float)appearance.GetValue(6) };
                fragment_s.specular = new float[] { (float)appearance.GetValue(7), (float)appearance.GetValue(8), (float)appearance.GetValue(9) };
                fragment_s.emissive = new float[] { (float)appearance.GetValue(10), (float)appearance.GetValue(11), (float)appearance.GetValue(12) };
                fragment_s.shininess = (float)appearance.GetValue(13);
                fragment_s.transparency = (float)appearance.GetValue(14);

                // Geometry
                ComApi.InwLTransform3f3 trans = fragment.GetLocalToWorldMatrix() as ComApi.InwLTransform3f3;

                if (trans.IsAffine)
                {
                    sw.WriteLine("IsAffine");
                }
                if (trans.IsIdentity)
                {
                    sw.WriteLine("IsIdentity");
                }
                if (trans.IsLinear)
                {
                    sw.WriteLine("IsLinear");
                }
                if (trans.IsRotation)
                {
                    sw.WriteLine("IsRotation");
                }
                if (trans.IsTranslation)
                {
                    sw.WriteLine("IsTranslation");
                }
                if (trans.IsUniformScale)
                {
                    sw.WriteLine("IsUniformScale");
                }
                if (trans.IsVecTransformed)
                {
                    sw.WriteLine("IsVecTransformed");
                }

                // COM
                fragment_s.matrix = present_matrix((Array)(object)fragment.GetLocalToWorldMatrix().Matrix);
                sw.WriteLine("COM");
                for (int i = 0; i < fragment_s.matrix.Length; i += 4)
                {
                    sw.WriteLine(fragment_s.matrix[i] + "  " + fragment_s.matrix[i + 1] + "  " + fragment_s.matrix[i + 2] + "  " + fragment_s.matrix[i + 3]);
                }

                // TRANSFORM
                //fragment_s.matrix = present_matrix_active(geometry.Geometry.ActiveTransform.Linear, geometry.Geometry.ActiveTransform.Translation);
                //sw.WriteLine("Active Transform");
                //for (int i = 0; i < fragment_s.matrix.Length; i += 4)
                //{
                //    sw.WriteLine(fragment_s.matrix[i] + "  " + fragment_s.matrix[i + 1] + "  " + fragment_s.matrix[i + 2] + "  " + fragment_s.matrix[i + 3]);
                //}
                

                fragment_s.triangls = new DS_Three.Triangle();
                fragment_s.triangls.vertexs = InitNullVertex();

                fragment_s.lines = new DS_Three.Line();
                fragment_s.lines.vertexs = InitNullVertex();

                fragment_s.points = new DS_Three.Point();
                fragment_s.points.vertexs = InitNullVertex();

                fragment_s.snappoints = new DS_Three.SnapPoint();
                fragment_s.snappoints.vertexs = InitNullVertex();

                callbkListener.triangle = fragment_s.triangls;
                callbkListener.line = fragment_s.lines;
                callbkListener.point = fragment_s.points;
                callbkListener.snap_point = fragment_s.snappoints;

                fragment.GenerateSimplePrimitives(ComApi.nwEVertexProperty.eNORMAL, callbkListener);

                list.Add(fragment_s);
            }
        }

        // struct vertex
        private DS_Three.Vertex InitNullVertex()
        {
            DS_Three.Vertex vertex = new DS_Three.Vertex();

            vertex.coordinates = new List<float[]>();
            vertex.index_coordinate = new List<int>();

            vertex.textures = new List<float[]>();
            vertex.index_texture = new List<int>();

            vertex.normals = new List<float[]>();
            vertex.index_normal = new List<int>();

            return vertex;
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
        //
        double[] present_matrix_active(Matrix3 in_matrix, Vector3D vector)
        {
            double[] out_matrix = new double[16];

            out_matrix[0] = in_matrix.Get(0, 0);
            out_matrix[1] = in_matrix.Get(0, 1);
            out_matrix[2] = in_matrix.Get(0, 2);
            out_matrix[3] = 0.0;

            out_matrix[4] = in_matrix.Get(1, 0);
            out_matrix[5] = in_matrix.Get(1, 1);
            out_matrix[6] = in_matrix.Get(1, 2);
            out_matrix[7] = 0.0;

            out_matrix[8] = in_matrix.Get(2, 0);
            out_matrix[9] = in_matrix.Get(2, 1);
            out_matrix[10] = in_matrix.Get(2, 2);
            out_matrix[11] = 0.0;

            out_matrix[12] = vector.X;
            out_matrix[13] = vector.Y;
            out_matrix[14] = vector.Z;
            out_matrix[15] = 1.0;

            return out_matrix;
        }
        #endregion
    }

    #region InwSimplePrimitivesCB Class
    class CallbackGeometry_Three : ComApi.InwSimplePrimitivesCB
    {
        public DS_Three.Triangle triangle;
        public DS_Three.Line line;
        public DS_Three.Point point;
        public DS_Three.SnapPoint snap_point;

        public void Line(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2)
        {
            set_vertex(line.vertexs, v1);
            set_vertex(line.vertexs, v2);
        }

        public void Point(ComApi.InwSimpleVertex v1)
        {
            set_vertex(point.vertexs, v1);
        }

        public void SnapPoint(ComApi.InwSimpleVertex v1)
        {
            set_vertex(snap_point.vertexs, v1);
        }

        public void Triangle(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2, ComApi.InwSimpleVertex v3)
        {
            set_vertex(triangle.vertexs, v1);
            set_vertex(triangle.vertexs, v2);
            set_vertex(triangle.vertexs, v3);
        }

        void set_vertex(DS_Three.Vertex vertexs, ComApi.InwSimpleVertex vertex)
        {
            // coordinate vertex
            float[] coord = convert_to_float((Array)(object)vertex.coord);
            int coord_index = get_index(vertexs.coordinates, coord);
            if (coord_index == -1)
            {
                vertexs.coordinates.Add(coord);

                // normal
                float[] normals = convert_to_float((Array)(object)vertex.normal);
                int normal_index = get_index(vertexs.normals, normals);
                if (normal_index == -1)
                {
                    vertexs.normals.Add(normals);
                    vertexs.index_normal.Add(vertexs.normals.Count);
                }
                else
                {
                    vertexs.index_normal.Add(normal_index + 1);
                }

                // texture
                float[] textures = convert_to_float((Array)(object)vertex.tex_coord);
                int texture_index = get_index(vertexs.textures, textures);
                if(texture_index == -1)
                {
                    vertexs.textures.Add(textures);
                    vertexs.index_texture.Add(vertexs.textures.Count);
                }
                else
                {
                    vertexs.index_texture.Add(texture_index + 1);
                }

                vertexs.index_coordinate.Add(vertexs.coordinates.Count);
            }
            else
            {
                vertexs.index_coordinate.Add(coord_index + 1);
                vertexs.index_texture.Add(vertexs.index_texture[coord_index]);
                vertexs.index_normal.Add(vertexs.index_normal[coord_index]);
            }
        }

        int get_index(List<float[]> value, float[] arr)
        {
            int count = value.Count;
            for (int i = 0; i < count; i++)
            {
                if (equel_vert(value[i], arr))
                    return i;
            }
            return -1;
        }

        bool equel_vert(float[] arr_one, float[] arr_two)
        {
            int count = arr_one.Length;
            for(int i = 0; i < count; i++)
            {
                if(arr_one[i] != arr_two[i])
                    return false;
            }
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
