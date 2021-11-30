using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExportGeometry.UnitsApp.Tests
{
    class WriteToFileData
    {

        #region Sneep
        //struct Point
        //{
        //    public Array coordinate;
        //    public Array color;
        //    public Array normal;
        //    public Array texture;
        //}

        //struct Fragment
        //{
        //    public double[] matrix;
        //    public List<Point> points;
        //    public List<int> faces;
        //}

        //struct Property
        //{
        //    public string name;
        //    public string value;
        //}

        //struct Item
        //{
        //    public string name;
        //    public List<Property> properties;
        //    public List<Fragment> fragments;
        //}

        //struct Model_3D
        //{
        //    public string name;
        //    public List<Item> items;
        //}


        #endregion

        #region Property Class
        DS.Model_3D model;
        StreamWriter sw;

        #endregion

        public WriteToFileData(DS.Model_3D _model)
        {
            sw = new StreamWriter(@"D:\vertex_for_unity.txt");

            model = _model;

            Run();

            sw.Close();
        }
        
        #region Method Class
        private void Run()
        {
            DS.Item[] items = model.items.ToArray();

            int items_count = items.Length;

            for(int i = 0; i < items_count; i++)
            {
                sw.WriteLine("#item_name");
                sw.WriteLine(items[i].name);

                w_property(items[i].properties.ToArray());
                w_fragment(items[i].fragments.ToArray());
            }
        }
        
        // Write property
        private void w_property(DS.Property[] properties)
        {
            int count_properties = properties.Length;

            sw.WriteLine("#properties");

            for (int i = 0; i < count_properties; i++)
            {
                sw.WriteLine(properties[i].name + ")=(" + properties[i].value);
            }
        }
        
        // Write fragment
        private void w_fragment(DS.Fragment[] fragments)
        {
            int count_fragments = fragments.Length;

            sw.WriteLine("#fragments");

            for(int i = 0; i < count_fragments; i++)
            {
                sw.WriteLine("#matrix");
                //double[] matrix = fragments[i].matrix;
                sw.WriteLine(_get_matrix(fragments[i].matrix));

                sw.WriteLine("#points");
                w_points(fragments[i].points.ToArray());

                sw.WriteLine("#fices");
                w_faces(fragments[i].faces.ToArray());
            }
        }

        // write matrix
        private string _get_matrix(double[] matrix)
        {
            string s_matrix = "";

            for(int i = 0; i < matrix.Length; i++)
            {
                s_matrix += matrix[i] + " ";
            }

            return s_matrix;
        }
        
        // write points
        private void w_points(DS.Point[] points)
        {
            int count_points = points.Length;

            for(int i = 0; i < count_points; i++)
            {
                sw.WriteLine("#coordinate");
                var coord = points[i].coordinate;
                sw.WriteLine(coord.GetValue(1) + " " + coord.GetValue(2) + " " + coord.GetValue(3));
                sw.WriteLine("#color");
                var color = points[i].color;
                sw.WriteLine(color.GetValue(1) + " " + color.GetValue(2) + " " + color.GetValue(3) + " " + color.GetValue(4));
                sw.WriteLine("#normal");
                var normal = points[i].normal;
                sw.WriteLine(normal.GetValue(1) + " " + normal.GetValue(2) + " " + normal.GetValue(3));
                sw.WriteLine("#texture");
                var texture = points[i].texture;
                sw.WriteLine(texture.GetValue(1) + " " + texture.GetValue(2));
            }
        }

        // write faces
        private void w_faces(int[] faces)
        {
            int count_faces = faces.Length;
            for(int i = 0; i < count_faces; i++)
            {
                sw.Write(faces[i] + " ");
            }
            sw.WriteLine();
        }

        #endregion
    }


    // 
    class WriteToObj
    {
        DS.Model_3D model;
        StreamWriter sw;

        int total_points;
        int total_faces;
        List<int> faces_present;

        public WriteToObj(DS.Model_3D _model)
        {
            sw = new StreamWriter(@"D:\vertex_wto.obj");

            model = _model;

            Run();

            sw.Close();
        }

        // Start
        private void Run()
        {
            w_header();
            w_items();

            sw.WriteLine("s off");

            w_faces(faces_present.ToArray());
        }

        private void w_header()
        {
            sw.WriteLine("# Obj_Navisworks");
            sw.WriteLine("o Model");
        }

        // Items
        private void w_items()
        {
            DS.Item[] items = model.items.ToArray();

            // Init total
            total_points = 0;
            total_faces = 0;
            faces_present = new List<int>();

            int count = items.Length;
            for (int i = 0; i < count; i++)
            {
                DS.Fragment[] fragments = items[i].fragments.ToArray();
                //DS.Property[] properties = items[i].properties.ToArray();
                string name = items[i].name;

                //w_properties(properties);
                w_fragments(fragments);
            }
        }

        // Properties
        private void w_properties(DS.Property[] properties)
        {
            int count = properties.Length;
            for (int i = 0; i < count; i++)
            {
                string name = properties[i].name;
                string value = properties[i].value;
            }
        }

        // Fragments
        private void w_fragments(DS.Fragment[] fragments)
        {
            int count = fragments.Length;
            for(int i = 0; i < count; i++)
            {
                int[] faces = fragments[i].faces.ToArray();
                DS.Point[] points = fragments[i].points.ToArray();

                w_points(points, fragments[i].matrix);
                //w_faces(faces);
                present_fase(faces);

                total_faces = total_points;
            }
        }


        // Points
        private void w_points(DS.Point[] points, double[] matrix)
        {
            int count = points.Length;

            // Coordinate
            for(int i = 0; i < count; i++)
            {
                double[] coordinate = Helpers.ConvertLCSToWCS.setCoordinate(matrix, points[i].coordinate);

                sw.WriteLine("v " + (coordinate[0] - 2100) + " " + (coordinate[2] - 100) + " " + (coordinate[1] + 3200));

                total_points++;
            }

            // Color
            //for (int i = 0; i < count; i++)
            //{
            //    float[] color = points[i].color;
            //    sw.WriteLine("v " + color[0] + " " + color[1] + " " + color[2] + " " + color[3]);
            //}

            // Normal
            for (int i = 0; i < count; i++)
            {
                float[] normal = points[i].normal;
                sw.WriteLine("vn " + normal[0] + " " + normal[1] + " " + normal[2]);
            }

            // Texture
            for (int i = 0; i < count; i++)
            {
                float[] texture = points[i].texture;
                sw.WriteLine("vt " + texture[0] + " " + texture[1]);
            }
        }

        // Faces
        private void w_faces(int[] faces)
        {
            int count = faces.Length;

            for (int i = 0; i < count; i++)
            {
                //sw.WriteLine("f " + (faces[i] + total_faces) + " " + (faces[++i] + total_faces) + " " + (faces[++i] + total_faces));
                sw.WriteLine("f " + (faces[i]) + " " + (faces[++i]) + " " + (faces[++i]));
            }
        }

        private void present_fase(int[] faces)
        {
            int count = faces.Length;

            for (int i = 0; i < count; i++)
            {
                //sw.WriteLine("f " + (faces[i] + total_faces) + " " + (faces[++i] + total_faces) + " " + (faces[++i] + total_faces));
                faces_present.Add(faces[i] + total_faces);
            }
        }
    }


    // 
    class WritToUnity
    {
        DS.Model_3D model;
        StreamWriter sw;

        int total_points;
        int total_faces;
        List<int> faces_present;

        public WritToUnity(DS.Model_3D _model)
        {
            sw = new StreamWriter(@"D:\vertex_wtu.txt");

            model = _model;
            Run();

            sw.Close();
        }

        // Start
        private void Run()
        {
            w_items();
        }

        // Items
        private void w_items()
        {
            DS.Item[] items = model.items.ToArray();

            // Init total
            total_points = 0;
            total_faces = 0;
            faces_present = new List<int>();

            int count = items.Length;
            for (int i = 0; i < count; i++)
            {
                DS.Fragment[] fragments = items[i].fragments.ToArray();
                DS.Property[] properties = items[i].properties.ToArray();
                string name = items[i].name;

                w_properties(properties);
                w_fragments(fragments);
            }
        }

        // Properties
        private void w_properties(DS.Property[] properties)
        {
            int count = properties.Length;
            for (int i = 0; i < count; i++)
            {
                string name = properties[i].name;
                string value = properties[i].value;
            }
        }

        // Fragments
        private void w_fragments(DS.Fragment[] fragments)
        {
            int count = fragments.Length;
            for (int i = 0; i < count; i++)
            {
                sw.WriteLine("#start_fragment");

                int[] faces = fragments[i].faces.ToArray();
                DS.Point[] points = fragments[i].points.ToArray();

                //w_points_gsc(points, fragments[i].matrix);
                w_points_lsc(points);
                w_faces(faces);

                sw.WriteLine("#end_fragment");
            }
        }


        // Points
        private void w_points_gsc(DS.Point[] points, double[] matrix)
        {
            int count = points.Length;

            // Coordinate
            for (int i = 0; i < count; i++)
            {
                double[] coordinate = Helpers.ConvertLCSToWCS.setCoordinate(matrix, points[i].coordinate);

                //sw.WriteLine("v " + (coordinate[0] - 2100) + " " + (coordinate[2] - 100) + " " + (coordinate[1] + 3200));

            }

            // Color
            //for (int i = 0; i < count; i++)
            //{
            //    float[] color = points[i].color;
            //    sw.WriteLine("v " + color[0] + " " + color[1] + " " + color[2] + " " + color[3]);
            //}

            // Normal
            for (int i = 0; i < count; i++)
            {
                float[] normal = points[i].normal;
                //sw.WriteLine("vn " + normal[0] + " " + normal[1] + " " + normal[2]);
            }

            // Texture
            //for (int i = 0; i < count; i++)
            //{
            //    float[] texture = points[i].texture;
            //    sw.WriteLine("vt " + texture[0] + " " + texture[1]);
            //}
        }

        private void w_points_lsc(DS.Point[] points)
        {
            int count = points.Length;

            // Coordinate
            sw.WriteLine("#start_coordinate");
            for (int i = 0; i < count; i++)
            {
                float[] coordinate = points[i].coordinate;
                sw.WriteLine(coordinate[0]);
                sw.WriteLine(coordinate[1]);
                sw.WriteLine(coordinate[2]);
            }
            sw.WriteLine("#end_coordinate");

            // Color
            //for (int i = 0; i < count; i++)
            //{
            //    float[] color = points[i].color;
            //}

            // Normal
            //for (int i = 0; i < count; i++)
            //{
            //    float[] normal = points[i].normal;
            //}

            // Texture
            //for (int i = 0; i < count; i++)
            //{
            //    float[] texture = points[i].texture;
            //}
        }

        // Faces
        private void w_faces(int[] faces)
        {
            int count = faces.Length;

            sw.WriteLine("#start_face");
            for (int i = 0; i < count; i++)
            {
                sw.WriteLine(faces[i]);
            }
            sw.WriteLine("#end_face");
        }
    }
}
