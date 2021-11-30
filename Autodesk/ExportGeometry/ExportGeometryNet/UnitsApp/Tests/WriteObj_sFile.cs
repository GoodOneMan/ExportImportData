using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExportGeometry.UnitsApp.Tests
{
    class WriteObj_sFile
    {
        #region Property Class
        DS_Two.Model_3D model;

        StreamWriter sw_obj;
        StreamWriter sw_mtl;

        int faces_c;
        int point_c;
        List<int> full_faces;

        #endregion

        public WriteObj_sFile(DS_Two.Model_3D _model)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            faces_c = 0;
            point_c = 0;

            model = _model;

            sw_obj = new StreamWriter(@"D:\Models_Obj\" + model.name + ".obj");
            sw_mtl = new StreamWriter(@"D:\Models_Obj\" + model.name + ".mtl");
            w_header();
            sw_obj.WriteLine("mtllib " + model.name + ".mtl");

            Run();

            sw_obj.Close();
            sw_mtl.Close();
        }

        #region Method Class
        private void Run()
        {
            DS_Two.Item[] items = model.items.ToArray();
            int items_count = items.Length;
            for (int i = 0; i < items_count; i++)
            {
                //full_faces = new List<int>();

                string name = items[i].name + "_count_" + i;

                name = name.Replace(' ', '_');

                sw_obj.WriteLine("o " + name);

                w_fragments(items[i].fragments.ToArray(), name);

                //w_material(items[i], name);

                //w_full_faces(full_faces.ToArray());
            }
        }

        private void w_header()
        {
            sw_obj.WriteLine("# Wavefront OBJ");
            sw_mtl.WriteLine("# Wavefront MTL");
        }

        //private void w_material(DS_Two.Item item, string name)
        //{
        //    sw_mtl.WriteLine("newmtl " + name + "_mat");
            
        //    sw_mtl.WriteLine("Ka ");
        //    sw_mtl.WriteLine("Kd ");
        //    sw_mtl.WriteLine("Ks ");
        //    sw_mtl.WriteLine("Tf ");
        //    sw_mtl.WriteLine("Ns ");
        //    sw_mtl.WriteLine("d ");
        //    //sw_mtl.WriteLine("illum 2");
        //}

        private void w_material_frag(DS_Two.Fragment frag, string name)
        {
            sw_mtl.WriteLine("newmtl " + name);

            sw_mtl.WriteLine("Ka " + frag.ambient[0] + " " + frag.ambient[1] + " " + frag.ambient[2]);
            sw_mtl.WriteLine("Kd " + frag.diffuse[0] + " " + frag.diffuse[1] + " " + frag.diffuse[2]);
            sw_mtl.WriteLine("Ks " + frag.specular[0] + " " + frag.specular[1] + " " + frag.specular[2]);
            sw_mtl.WriteLine("Tf " + frag.emissive[0] + " " + frag.emissive[1] + " " + frag.emissive[2]);
            sw_mtl.WriteLine("Ns " + frag.shininess);
            sw_mtl.WriteLine("d " + frag.transparency);
            //sw_mtl.WriteLine("illum 2");
        }

        private double[] transform_lcs_to_gcs(double[] matrix, float[] coordinate)
        {
            double[] GCS_Coordinate = new double[3];
            GCS_Coordinate[0] = ((matrix[0] * coordinate[0] + matrix[4] * coordinate[1]) + matrix[12]);
            GCS_Coordinate[1] = ((matrix[1] * coordinate[0] + matrix[5] * coordinate[1]) + matrix[13]);
            GCS_Coordinate[2] = ((matrix[10] * coordinate[2]) + matrix[14]);

            return GCS_Coordinate;
        }

        private void w_fragments(DS_Two.Fragment[] fragments, string name)
        {
            int count_fragments = fragments.Length;
            for (int i = 0; i < count_fragments; i++)
            {
                w_points(fragments[i].points.ToArray(), fragments[i].matrix);

                sw_obj.WriteLine("usemtl " + name + "_mat_" + i );
                sw_obj.WriteLine("s off");

                w_faces(fragments[i].faces.ToArray());

                w_material_frag(fragments[i], name + "_mat_" + i);

                faces_c = point_c;
            }
        }

        private void w_points(DS_Two.Point[] points, double[] matrix)
        {
            int count_points = points.Length;
            for (int i = 0; i < count_points; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, points[i].coordinate);

                double x = Math.Round((coord[0] - 2110),8);
                double y = Math.Round((coord[1] + 3200),8);
                double z = Math.Round(coord[2], 8);

                sw_obj.WriteLine("v " + x + " " + y + " " + z);
            }

            int count_normal = points.Length;
            for (int i = 0; i < count_points; i++)
            {
                sw_obj.WriteLine("vn " + points[i].normal[0] + " " + points[i].normal[1] + " " + points[i].normal[2]);
            }

            point_c += count_points;
        }

        private void w_faces(int[] faces)
        {
            int count_faces = faces.Length;

            for (int i = 0; i < count_faces; i += 3)
            {
                int one = faces[i] + faces_c;
                int two = faces[i + 1] + faces_c;
                int three = faces[i + 2] + faces_c;

                sw_obj.WriteLine("f " + one + "//" + one + " " + two + "//" + two + " " + three + "//" + three);
            }
        }

        //private void w_faces(int[] faces)
        //{
        //    int count_faces = faces.Length;

        //    for (int i = 0; i < count_faces; ++i)
        //    {
        //        full_faces.Add(faces[i] + faces_c);
        //    }
        //}

        //private void w_full_faces(int[] faces)
        //{
        //    int count_faces = faces.Length;
        //    for (int i = 0; i < count_faces; i += 3)
        //    {
        //        sw_obj.WriteLine("f " + (faces[i])+"//"+ (faces[i]) + " " + (faces[i + 1]) + "//" + (faces[i + 1]) + " " + (faces[i + 2]) + "//" + (faces[i + 2]));
        //    }
        //}
        #endregion
    }
}
