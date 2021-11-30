using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExportGeometry.UnitsApp.Tests
{
    class WriteToObjFile
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

        int faces_c = 0;
        int point_c = 0;

        #endregion

        public WriteToObjFile(DS.Model_3D _model)
        {
            sw = new StreamWriter(@"D:\vertex_for_obj.obj");

            model = _model;

            Run();

            sw.Close();
        }

        #region Method Class
        private void Run()
        {
            w_header();

            DS.Item[] items = model.items.ToArray();
            int items_count = items.Length;

            for(int i = 0; i < items_count; i++)
            {
                w_fragments(items[i].fragments.ToArray());
            }
        }

        private void w_header()
        {
            sw.WriteLine("# Ware");
            sw.WriteLine("o Model");
        }

        private double[] transform_lcs_to_gcs(double[] matrix, float[] coordinate)
        {
            double[] GCS_Coordinate = new double[3];
            GCS_Coordinate[0] = ((matrix[0] * coordinate[0] + matrix[4] * coordinate[1]) + matrix[12]);
            GCS_Coordinate[1] = ((matrix[1] * coordinate[0] + matrix[5] * coordinate[1]) + matrix[13]);
            GCS_Coordinate[2] = ((matrix[10] * coordinate[2]) + matrix[14]);

            return GCS_Coordinate;
        }
        
        private void w_fragments(DS.Fragment[] fragments)
        {
            int count_fragments = fragments.Length;

            for(int i = 0; i < count_fragments; i++)
            {
                w_points(fragments[i].points.ToArray(), fragments[i].matrix);

                w_faces(fragments[i].faces.ToArray());

                faces_c += point_c;
            }
        }

        private void w_points(DS.Point[] points, double[] matrix)
        {
            int count_points = points.Length;

            for(int i = 0; i < count_points; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, points[i].coordinate);
                sw.WriteLine("v " + coord[0] + " " + coord[1] + " " + coord[2]);
            }

            point_c += count_points;
        }

        private void w_faces(int[] faces)
        {
            int count_faces = faces.Length;

            for(int i = 0; i < count_faces; i+=3)
            {
                sw.WriteLine("f " + (faces[i] + faces_c) + " " + (faces[i + 1] + faces_c)+ " " + (faces[i + 2] + faces_c));
            }
        }
        #endregion
    }

    class WriteToObjFile_Three
    {
        #region Property Class

        DS_Three.Model_3D model;
        List<DS_Three.Fragment> repeating_fragments;
        
        StreamWriter sw_obj;
        StreamWriter sw_mtl;
        
        int triangle_coord_vertex = 0;
        int line_coord_vertex = 0;
        int point_coord_vertex = 0;
        int snappoint_coord_vertex = 0;

        int triangle_normal_vertex = 0;
        int line_normal_vertex = 0;
        int point_normal_vertex = 0;
        int snappoint_normal_vertex = 0;

        int triangle_texture_vertex = 0;
        int line_texture_vertex = 0;
        int point_texture_vertex = 0;
        int snappoint_texture_vertex = 0;

        int full_vertex = 0;
        int full_texture = 0;
        int full_normal = 0;

        int offset_x = 2104;
        int offset_y = -3290;
        int offset_z = 106;

        int[] faces_count = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        string object_name;
        string material_name;

        StringBuilder triangle_v_str;
        StringBuilder triangle_vt_str;
        StringBuilder triangle_vn_str;
        StringBuilder triangle_faces_str;
        
        StringBuilder line_v_str;
        StringBuilder line_vt_str;
        StringBuilder line_vn_str;
        StringBuilder line_faces_str;

        StringBuilder point_v_str;
        StringBuilder point_vt_str;
        StringBuilder point_vn_str;
        StringBuilder point_faces_str;

        StringBuilder snappoint_v_str;
        StringBuilder snappoint_vt_str;
        StringBuilder snappoint_vn_str;
        StringBuilder snappoint_faces_str;
        #endregion

        public WriteToObjFile_Three(DS_Three.Model_3D _model)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            repeating_fragments = new List<DS_Three.Fragment>();
            model = _model;
            
            sw_obj = new StreamWriter(@"D:\Model_Obj_1\" + model.name + ".obj");
            sw_mtl = new StreamWriter(@"D:\Model_Obj_1\" + model.name + ".mtl");
            write_header();

            Run();

            sw_obj.Close();
            sw_mtl.Close();
        }

        #region Method Class

        // Header
        private void write_header()
        {
            sw_obj.WriteLine("# Model navisworks");
            sw_obj.WriteLine("# Obj file");
            sw_obj.WriteLine("mtllib " + model.name + ".mtl");

            sw_mtl.WriteLine("# Model navisworks");
            sw_mtl.WriteLine("# Mtl file");
        }

        // Run
        private void Run()
        {
            //sw_obj.WriteLine("o " + "Model");
            DS_Three.Item[] items = model.items.ToArray();
            int items_count = items.Length;
            for (int i = 0; i < items_count; i++)
            {
                object_name = items[i].name.Replace(' ', '_').Trim() + i;
                sw_obj.WriteLine("o " + object_name);

                read_fragments(items[i]);
            }
        }
        
        // Fragment
        private void read_fragments(DS_Three.Item item)
        {
            int count_fragment = item.fragments.Count;
            if(count_fragment == item.frag_count)
            {
                for (int i = 0; i < count_fragment; i++)
                {
                    write_fragment(item.fragments[i], "mat_" + object_name + "_" + i);
                }
            }
            else
            {
                for (int i = 0; i < count_fragment; i++)
                {
                    bool trigger = false;

                    DS_Three.Fragment fragment = item.fragments[i];
                    foreach (var rep_frag in repeating_fragments)
                    {
                        if (compare_fragment(fragment, rep_frag))
                        {
                            trigger = true;
                        }
                    }

                    if (!trigger)
                    {
                        repeating_fragments.Add(fragment);
                        write_fragment(item.fragments[i], "mat_" + object_name + "_" + i);
                    }
                }
            }
        }

        // Compare fragment
        private bool compare_fragment(DS_Three.Fragment one_fragment, DS_Three.Fragment two_fragment)
        {
            bool trigger = false;

            if(equel_matrix(one_fragment.matrix, two_fragment.matrix))
            {
                DS_Three.Vertex one = one_fragment.triangls.vertexs;
                DS_Three.Vertex two = two_fragment.triangls.vertexs;

                if(one.coordinates.Count == two.coordinates.Count)
                {
                    for(int i = 0; i < one.coordinates.Count; i++)
                    {
                        trigger = true;

                        try
                        {
                            if (!equel_vert(one.coordinates[i], two.coordinates[i]))
                            {
                                trigger = false;
                                break;
                            }
                        }
                        catch(Exception ex)
                        {
                            trigger = false;
                            break;
                        }
                    }
                }
            }

            return trigger;
        }

        // Write fragment
        private void write_fragment(DS_Three.Fragment fragment, string material)
        {
            init_string_builder();

            //repeating_fragments.Add(fragment);

            write_triangle(fragment.triangls, fragment.matrix);
            //write_line(fragment.lines, fragment.matrix);
            //write_point(fragment.points, fragment.matrix);
            //write_snappoint(fragment.snappoints, fragment.matrix);

            // marerial name
            material_name = material;

            // write fragment 
            // newmtl material
            write_date_in_file();

            //init_faces_counter();
            write_material(fragment);
        }

        // Triangle
        private void write_triangle(DS_Three.Triangle triangles, double[] matrix)
        {
            DS_Three.Vertex vertexs = triangles.vertexs;
            int i;

            // Coordinate
            List<float[]> coordinates = vertexs.coordinates;
            for (i = 0; i < coordinates.Count; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, coordinates[i]);

                double x = Math.Round((coord[0] - offset_x), 8);
                double y = Math.Round((coord[1] - offset_y), 8);
                double z = Math.Round((coord[2] - offset_z), 8);

                triangle_v_str.Append("v " + x + " " + z + " " + y + Environment.NewLine);
                triangle_coord_vertex++;
            }

            // Texture
            //List<float[]> textures = vertexs.textures;
            //for (i = 0; i < textures.Count; i++)
            //{
            //    float u = textures[i][0];
            //    float w = textures[i][1];

            //    triangle_vt_str.Append("vt " + u + " " + w + Environment.NewLine);
            //    triangle_texture_vertex++;
            //}

            // Normal
            List<float[]> normales = vertexs.normals;
            for (i = 0; i < normales.Count; i++)
            {
                float x = normales[i][0];
                float y = normales[i][1];
                float z = normales[i][2];

                triangle_vn_str.Append("vn " + x + " " + y + " " + z + Environment.NewLine);
                triangle_normal_vertex++;
            }

            //List<int> index_coord = vertexs.index_coordinate;
            //List<int> index_texture = vertexs.index_texture;
            //List<int> index_normal = vertexs.index_normal;
            //for (i = 0; i < index_coord.Count; i += 3)
            //{
            //    int v1 = index_coord[i] + faces_count[0];
            //    int v2 = index_coord[i + 1] + faces_count[0];
            //    int v3 = index_coord[i + 2] + faces_count[0];

            //    int vt1 = index_texture[i] + faces_count[4];
            //    int vt2 = index_texture[i + 1] + faces_count[4];
            //    int vt3 = index_texture[i + 2] + faces_count[4];

            //    int vn1 = index_normal[i] + faces_count[8];
            //    int vn2 = index_normal[i + 1] + faces_count[8];
            //    int vn3 = index_normal[i + 2] + faces_count[8];

            //    triangle_faces_str.Append("f " + v1 + "/" + vt1 + "/" + vn1 + " " + v2 + "/" + vt2 + "/" + vn2 + " " + v3 + "/" + vt3 + "/" + vn3 + Environment.NewLine);
            //}

            List<int> index_coord = vertexs.index_coordinate;
            List<int> index_texture = vertexs.index_texture;
            List<int> index_normal = vertexs.index_normal;
            for (i = 0; i < index_coord.Count; i += 3)
            {
                int v1 = index_coord[i] + full_vertex;
                int v2 = index_coord[i + 1] + full_vertex;
                int v3 = index_coord[i + 2] + full_vertex;

                int vt1 = index_texture[i] + full_texture;
                int vt2 = index_texture[i + 1] + full_texture;
                int vt3 = index_texture[i + 2] + full_texture;

                int vn1 = index_normal[i] + full_normal;
                int vn2 = index_normal[i + 1] + full_normal;
                int vn3 = index_normal[i + 2] + full_normal;

                //triangle_faces_str.Append("f " + v1 + "/" + vt1 + "/" + vn1 + " " + v2 + "/" + vt2 + "/" + vn2 + " " + v3 + "/" + vt3 + "/" + vn3 + Environment.NewLine);
                triangle_faces_str.Append("f " + v1 + "//" + vn1 + " " + v2 + "//" + vn2 + " " + v3 + "//" + vn3 + Environment.NewLine);
            }

            init_faces_counter();
        }

        // Line
        private void write_line(DS_Three.Line lines, double[] matrix)
        {
            DS_Three.Vertex vertexs = lines.vertexs;
            int i;

            List<float[]> coordinates = vertexs.coordinates;
            for (i = 0; i < coordinates.Count; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, coordinates[i]);

                double x = Math.Round((coord[0] - offset_x), 8);
                double y = Math.Round((coord[1] - offset_y), 8);
                double z = Math.Round((coord[2] - offset_z), 8);

                line_v_str.Append("v " + x + " " + y + " " + z + Environment.NewLine);
                line_coord_vertex++;
            }

            List<float[]> textures = vertexs.textures;
            for (i = 0; i < textures.Count; i++)
            {
                float u = textures[i][0];
                float w = textures[i][1];

                line_vt_str.Append("vt " + u + " " + w + Environment.NewLine);
                line_texture_vertex++;
            }

            List<float[]> normales = vertexs.normals;
            for (i = 0; i < normales.Count; i++)
            {
                float x = normales[i][0];
                float y = normales[i][1];
                float z = normales[i][2];

                line_vn_str.Append("vn " + x + " " + y + " " + z + Environment.NewLine);
                line_normal_vertex++;
            }

            //List<int> index_coord = vertexs.index_coordinate;
            //List<int> index_texture = vertexs.index_texture;
            //List<int> index_normal = vertexs.index_normal;
            //for (i = 0; i < index_coord.Count; i += 2)
            //{
            //    int v1 = index_coord[i] + faces_count[1];
            //    int v2 = index_coord[i + 1] + faces_count[1];

            //    int vt1 = index_texture[i] + faces_count[5];
            //    int vt2 = index_texture[i + 1] + faces_count[5];

            //    int vn1 = index_normal[i] + faces_count[9];
            //    int vn2 = index_normal[i + 1] + faces_count[9];

            //    line_faces_str.Append("l " + v1 + "/" + vt1 + "/" + vn1 + " " + v2 + "/" + vt2 + "/" + vn2 + Environment.NewLine);
            //}

            List<int> index_coord = vertexs.index_coordinate;
            List<int> index_texture = vertexs.index_texture;
            List<int> index_normal = vertexs.index_normal;
            for (i = 0; i < index_coord.Count; i += 2)
            {
                if(i == 0)
                {
                    line_faces_str.Append("l ");
                }

                int v1 = index_coord[i] + full_vertex;
                int v2 = index_coord[i + 1] + full_vertex;

                int vt1 = index_texture[i] + full_texture;
                int vt2 = index_texture[i + 1] + full_texture;

                int vn1 = index_normal[i] + full_normal;
                int vn2 = index_normal[i + 1] + full_normal;

                line_faces_str.Append(v1 + " " + v2 + " ");

                if(i == index_coord.Count - 1)
                {
                    line_faces_str.Append(Environment.NewLine);
                }
            }

            init_faces_counter();
        }

        // Point
        private void write_point(DS_Three.Point points, double[] matrix)
        {
            DS_Three.Vertex vertexs = points.vertexs;
            int i;

            List<float[]> coordinates = vertexs.coordinates;
            for (i = 0; i < coordinates.Count; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, coordinates[i]);

                double x = Math.Round((coord[0] - offset_x), 8);
                double y = Math.Round((coord[1] - offset_y), 8);
                double z = Math.Round((coord[2] - offset_z), 8);

                point_v_str.Append("v " + x + " " + y + " " + z + Environment.NewLine);
                point_coord_vertex++;
            }

            List<float[]> textures = vertexs.textures;
            for (i = 0; i < textures.Count; i++)
            {
                float u = textures[i][0];
                float w = textures[i][1];

                point_vt_str.Append("vt " + u + " " + w + Environment.NewLine);
                point_texture_vertex++;
            }

            List<float[]> normales = vertexs.normals;
            for (i = 0; i < normales.Count; i++)
            {
                float x = normales[i][0];
                float y = normales[i][1];
                float z = normales[i][2];

                point_vn_str.Append("vn " + x + " " + y + " " + z + Environment.NewLine);
                point_normal_vertex++;
            }

            List<int> index_coord = vertexs.index_coordinate;
            List<int> index_texture = vertexs.index_texture;
            List<int> index_normal = vertexs.index_normal;
            for (i = 0; i < index_coord.Count; i++)
            {
                int v1 = index_coord[i] + faces_count[2];

                int vt1 = index_texture[i] + faces_count[6];

                int vn1 = index_normal[i] + faces_count[10];

                point_faces_str.Append("f " + v1 + "/" + vt1 + "/" + vn1 + Environment.NewLine);
            }
        }

        // SnapPoint
        private void write_snappoint(DS_Three.SnapPoint snappoints, double[] matrix)
        {
            DS_Three.Vertex vertexs = snappoints.vertexs;
            int i;

            List<float[]> coordinates = vertexs.coordinates;
            for (i = 0; i < coordinates.Count; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, coordinates[i]);

                double x = Math.Round((coord[0] - offset_x), 8);
                double y = Math.Round((coord[1] - offset_y), 8);
                double z = Math.Round((coord[2] - offset_z), 8);

                snappoint_v_str.Append("v " + x + " " + y + " " + z + Environment.NewLine);
                snappoint_coord_vertex++;
            }

            List<float[]> textures = vertexs.textures;
            for (i = 0; i < textures.Count; i++)
            {
                float u = textures[i][0];
                float w = textures[i][1];

                snappoint_vt_str.Append("vt " + u + " " + w + Environment.NewLine);
                snappoint_texture_vertex++;
            }

            List<float[]> normales = vertexs.normals;
            for (i = 0; i < normales.Count; i++)
            {
                float x = normales[i][0];
                float y = normales[i][1];
                float z = normales[i][2];

                snappoint_vn_str.Append("vn " + x + " " + y + " " + z + Environment.NewLine);
                snappoint_normal_vertex++;
            }

            List<int> index_coord = vertexs.index_coordinate;
            List<int> index_texture = vertexs.index_texture;
            List<int> index_normal = vertexs.index_normal;
            for (i = 0; i < index_coord.Count; i++)
            {
                int v1 = index_coord[i] + faces_count[3];

                int vt1 = index_texture[i] + faces_count[7];

                int vn1 = index_normal[i] + faces_count[11];

                snappoint_faces_str.Append("f " + v1 + "/" + vt1 + "/" + vn1 + Environment.NewLine);
            }
        }

        // Transformation ???
        private double[] transform_lcs_to_gcs(double[] matrix, float[] coordinate)
        {
            double[] GCS_Coordinate = new double[3];
            GCS_Coordinate[0] = ((matrix[0] * coordinate[0] + matrix[4] * coordinate[1] + matrix[8] * coordinate[2]) + matrix[12]);
            GCS_Coordinate[1] = ((matrix[1] * coordinate[0] + matrix[5] * coordinate[1] + matrix[9] * coordinate[2]) + matrix[13]);
            GCS_Coordinate[2] = ((matrix[2] * coordinate[0] + matrix[6] * coordinate[1] + matrix[10] * coordinate[2]) + matrix[14]);

            return GCS_Coordinate;
        }

        // Write data in file
        private void write_date_in_file()
        {
            // v
            #region  Vertex
            if (triangle_v_str.Length != 0)
            {
                sw_obj.WriteLine("# triangle_v_str");
                sw_obj.WriteLine(triangle_v_str);
            }

            if (line_v_str.Length != 0)
            {
                sw_obj.WriteLine("# line_v_str");
                sw_obj.WriteLine(line_v_str);
            }

            if (point_v_str.Length != 0)
            {
                sw_obj.WriteLine("# point_v_str");
                sw_obj.WriteLine(point_v_str);
            }

            if (snappoint_v_str.Length != 0)
            {
                sw_obj.WriteLine("# snappoint_v_str");
                sw_obj.WriteLine(snappoint_v_str);
            }
            #endregion
            
            // vt
            #region  Texture
            if (triangle_vt_str.Length != 0)
            {
                sw_obj.WriteLine("# triangle_vt_str");
                sw_obj.WriteLine(triangle_vt_str);
            }

            if (line_vt_str.Length != 0)
            {
                sw_obj.WriteLine("# line_vt_str");
                sw_obj.WriteLine(line_vt_str);
            }

            if (point_vt_str.Length != 0)
            {
                sw_obj.WriteLine("# point_vt_str");
                sw_obj.WriteLine(point_vt_str);
            }

            if (snappoint_vt_str.Length != 0)
            {
                sw_obj.WriteLine("# snappoint_vt_str");
                sw_obj.WriteLine(snappoint_vt_str);
            }
            #endregion
            
            // vn
            #region  Normale
            if (triangle_vn_str.Length != 0)
            {
                sw_obj.WriteLine("# triangle_vn_str");
                sw_obj.WriteLine(triangle_vn_str);
            }

            if (line_vn_str.Length != 0)
            {
                sw_obj.WriteLine("# line_vn_str");
                sw_obj.WriteLine(line_vn_str);
            }

            if (point_vn_str.Length != 0)
            {
                sw_obj.WriteLine("# point_vn_str");
                sw_obj.WriteLine(point_vn_str);
            }

            if (snappoint_vn_str.Length != 0)
            {
                sw_obj.WriteLine("# snappoint_vn_str");
                sw_obj.WriteLine(snappoint_vn_str);
            }
            #endregion
            
            // material
            sw_obj.WriteLine("usemtl " + material_name);
            sw_obj.WriteLine("s 20");

            // f
            #region  Face
            if (triangle_faces_str.Length != 0)
            {
                sw_obj.WriteLine("# triangle_faces_str");
                sw_obj.WriteLine(triangle_faces_str);
            }

            if (line_faces_str.Length != 0)
            {
                sw_obj.WriteLine("# line_faces_str");
                sw_obj.WriteLine(line_faces_str);
            }

            if (point_faces_str.Length != 0)
            {
                sw_obj.WriteLine("# point_faces_str");
                sw_obj.WriteLine(point_faces_str);
            }

            if (snappoint_faces_str.Length != 0)
            {
                sw_obj.WriteLine("# snappoint_faces_str");
                sw_obj.WriteLine(snappoint_faces_str);
            }
            #endregion
        }

        // Write Material
        private void write_material(DS_Three.Fragment fragment)
        {
            //sw_mtl.WriteLine("newmtl " + this.material_name);
            //sw_mtl.WriteLine("Ka " + fragment.ambient[0] + " " + fragment.ambient[1] + " " + fragment.ambient[2]);
            //sw_mtl.WriteLine("Kd " + fragment.diffuse[0] + " " + fragment.diffuse[1] + " " + fragment.diffuse[2]);
            //sw_mtl.WriteLine("Ks " + fragment.specular[0] + " " + fragment.specular[1] + " " + fragment.specular[2]);
            //sw_mtl.WriteLine("Tf " + fragment.emissive[0] + " " + fragment.emissive[1] + " " + fragment.emissive[2]);
            //sw_mtl.WriteLine("Ns " + fragment.shininess);
            //sw_mtl.WriteLine("d " + fragment.transparency);

            sw_mtl.WriteLine("newmtl " + this.material_name);
            sw_mtl.WriteLine("Ns 498.039216");
            sw_mtl.WriteLine("Ka 1.000000 1.000000 1.000000");
            sw_mtl.WriteLine("Kd 0.398431 0.398431 0.398431");
            sw_mtl.WriteLine("Ks 0.000000 0.000000 0.000000");
            sw_mtl.WriteLine("Ke 0.000000 0.000000 0.000000");
            sw_mtl.WriteLine("Ni 1.000000");
            sw_mtl.WriteLine("d 1.000000");
            sw_mtl.WriteLine("illum 2");
        }

        // Init StringBuilder
        private void init_string_builder()
        {
            triangle_v_str = new StringBuilder();
            triangle_vt_str = new StringBuilder();
            triangle_vn_str = new StringBuilder();
            triangle_faces_str = new StringBuilder();

            line_v_str = new StringBuilder();
            line_vt_str = new StringBuilder();
            line_vn_str = new StringBuilder();
            line_faces_str = new StringBuilder();

            point_v_str = new StringBuilder();
            point_vt_str = new StringBuilder();
            point_vn_str = new StringBuilder();
            point_faces_str = new StringBuilder();

            snappoint_v_str = new StringBuilder();
            snappoint_vt_str = new StringBuilder();
            snappoint_vn_str = new StringBuilder();
            snappoint_faces_str = new StringBuilder();
        }

        // Init faces counter
        private void init_faces_counter()
        {
            faces_count[0] = triangle_coord_vertex;
            faces_count[1] = line_coord_vertex;
            faces_count[2] = point_coord_vertex;
            faces_count[3] = snappoint_coord_vertex;

            faces_count[4] = triangle_texture_vertex;
            faces_count[5] = line_texture_vertex;
            faces_count[6] = point_texture_vertex;
            faces_count[7] = snappoint_texture_vertex;

            faces_count[8] = triangle_normal_vertex;
            faces_count[9] = line_normal_vertex;
            faces_count[10] = point_normal_vertex;
            faces_count[11] = snappoint_normal_vertex;


            full_vertex = triangle_coord_vertex + line_coord_vertex + point_coord_vertex + snappoint_coord_vertex;
            full_texture = triangle_texture_vertex + line_texture_vertex + point_texture_vertex + snappoint_texture_vertex;
            full_normal = triangle_normal_vertex + line_normal_vertex + point_normal_vertex + snappoint_normal_vertex;
        }

        //
        bool equel_matrix(double[] arr_one, double[] arr_two)
        {
            int count = arr_one.Length;
            for (int i = 0; i < count; i++)
            {
                if (arr_one[i] != arr_two[i])
                    return false;
            }
            return true;
        }

        //
        bool equel_vert(float[] arr_one, float[] arr_two)
        {
            int count = arr_one.Length;
            for(int i = 0; i<count; i++)
            {
                if(arr_one[i] != arr_two[i])
                    return false;
            }
            return true;
        }
        #endregion
    }
}
