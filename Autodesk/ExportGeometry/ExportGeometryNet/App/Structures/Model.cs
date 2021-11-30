using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportGeometry.App.Structures
{
    struct Model
    {
        public string name { get; set; }
        public List<Item> items { get; set; }
    }

    struct Item
    {
        public string name { get; set; }
        public int count_fragments;
        public int count_primitives;
        public float[] color;
        public float transparency;
        public List<Property> properties;
        public List<Fragment> fragments;
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
        public Vertex triangls;
        public Vertex lines;
        public Vertex points;
        public Vertex snappoints;
    }

    struct Vertex
    {
        public List<float[]> coordinates;
        public List<int> index_coordinate;
        public List<float[]> normals;
        public List<int> index_normal;
        public List<float[]> textures;
        public List<int> index_texture;
    }

    struct Property
    {
        public string name;
        public string value; 
    }
}
