using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExportGeometry.UnitsApp.Tests
{
    class WriteVertexForOpenTK
    {
        DS.Model_3D model;
        StreamWriter sw;

        int faces_c = 0;
        int point_c = 0;



        public WriteVertexForOpenTK(DS.Model_3D _model)
        {
            sw = new StreamWriter(@"D:\vertex_for_openTK.obj");

            model = _model;

            Run();

            sw.Close();
        }

        #region Method Class
        private void Run()
        {
            DS.Item[] items = model.items.ToArray();
            int items_count = items.Length;

            for (int i = 0; i < items_count; i++)
            {
                w_fragments(items[i].fragments.ToArray());
            }
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

            for (int i = 0; i < count_fragments; i++)
            {
                w_points(fragments[i].points.ToArray(), fragments[i].matrix);

                w_faces(fragments[i].faces.ToArray());

                faces_c += point_c;

                if (i == 0)
                    break;
            }
        }

        private void w_points(DS.Point[] points, double[] matrix)
        {
            int count_points = points.Length;

            for (int i = 0; i < count_points; i++)
            {
                double[] coord = transform_lcs_to_gcs(matrix, points[i].coordinate);
                sw.WriteLine(coord[0] + ", " + coord[1] + ", " + coord[2] + ", ");
            }

            point_c += count_points;
        }

        private void w_faces(int[] faces)
        {
            int count_faces = faces.Length;

            for (int i = 0; i < count_faces; i += 3)
            {
                sw.WriteLine((faces[i] + faces_c) + ", " + (faces[i + 1] + faces_c) + ", " + (faces[i + 2] + faces_c) + ", ");
            }
        }
        #endregion
    }
}
