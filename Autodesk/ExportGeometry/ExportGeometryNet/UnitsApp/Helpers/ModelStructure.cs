using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ExportGeometry.UnitsApp.Helpers
{
    class ModelStructure
    {

    }

    struct Model
    {
        public string name;
        public int count_vec;
        public Vector3[] vectors;
        public int[] faces;

        // new structure
        public List<Item> model_item;
    }

    struct coordinate
    {
        public float x;
        public float y;
        public float z;
    }

    struct color
    {
        public float ra;
        public float rb;
        public float rc;
        public float ro;
    }

    struct normal
    {
        public float x;
        public float y;
        public float z;
    }

    struct texture
    {
        public float u;
        public float w;
    }

    struct Point
    {
        public coordinate coord;
        public color color;
        public normal normal;
        public texture texture;
    }

    struct Fragment
    {
        public double[] matrix;
        public List<Point> points;
        public List<int> faces;
    }

    struct Property
    {
        public string name;
        public string value;
    }

    struct Item
    {
        public string name;
        public List<Property> properties;
        public List<Fragment> fragments;
    }


}

namespace ExportGeometry.UnitsApp.DS
{

    struct Point
    {
        public float[] coordinate;
        public float[] color;
        public float[] normal;
        public float[] texture;
    }

    struct Fragment
    {
        public double[] matrix;
        public List<Point> points;
        public List<int> faces;
    }

    struct Property
    {
        public string name;
        public string value;
    }

    struct Item
    {
        public string name;
        public List<Property> properties;
        public List<Fragment> fragments;
    }

    struct Model_3D
    {
        public string name;
        public List<Item> items;
    }
}

namespace ExportGeometry.UnitsApp.DS_Two
{
    struct Point
    {
        public float[] coordinate;
        public float[] normal;
        public float[] texture;
    }

    struct Fragment
    {
        public double[] matrix;
        public float[] ambient;
        public float[] diffuse;
        public float[] specular;
        public float[] emissive;
        public float shininess;
        public float transparency;
        public List<Point> points;
        public List<int> faces;
    }

    struct Property
    {
        public string name;
        public string value;
    }

    struct Item
    {
        public string name;
        public double[] color;
        public int frag_count;
        public int prim_count;
        public List<Property> properties;
        public List<Fragment> fragments;
    }

    struct Model_3D
    {
        public string name;
        public List<Item> items;
    }
}

namespace ExportGeometry.UnitsApp.DS_Three
{
    struct Vertex
    {
        public List<float[]> coordinates;
        public List<int> index_coordinate;
        public List<float[]> normals;
        public List<int> index_normal;
        public List<float[]> textures;
        public List<int> index_texture;
    }

    struct Triangle
    {
        public Vertex vertexs;
    }

    struct Line
    {
        public Vertex vertexs;
    }

    struct Point
    {
        public Vertex vertexs;
    }

    struct SnapPoint
    {
        public Vertex vertexs;
    }

    struct Fragment
    {
        // Color
        public float[] ambient;
        public float[] diffuse;
        public float[] specular;
        public float[] emissive;
        public float shininess;
        public float transparency;

        // Geometry
        public double[] matrix;
        public Triangle triangls;
        public Line lines;
        public Point points;
        public SnapPoint snappoints;
    }

    struct Property
    {
        public string name;
        public string value;
    }

    struct Item
    {
        public string name;
        public int frag_count;
        public int prim_count;
        public List<Property> properties;
        public List<Fragment> fragments;
    }

    struct Model_3D
    {
        public string name;
        public List<Item> items;
    }
}
