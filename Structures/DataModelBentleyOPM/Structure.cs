using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModelBentleyOPM
{
    [Serializable]
    public class Model
    {
        public string Name { get; set; }
        public List<ModelRef> ModelRef { get; set; }
        public List<Line> LineNumbers { get; set; }
    }
    [Serializable]
    public class ModelRef
    {
        public string Name { get; set; }
        public List<Level> Level { get; set; }
    }
    [Serializable]
    public class Level
    {
        public string Name { get; set; }
        public List<Line> Lines { get; set; }
        public List<Element> Elements { get; set; }
    }
    [Serializable]
    public class Line
    {
        public string Name { get; set; }
        public string ElementID { get; set; }
        public List<Property> Properties { get; set; }
    }
    [Serializable]
    public class Element
    {
        public string Name { get; set; }
        public string ElementID { get; set; }
        public List<Property> Properties { get; set; }
    }
    [Serializable]
    public class Property
    {
        public string Name { get; set; }
        public string Translation { get; set; }
        public string Value { get; set; }
    }
}
