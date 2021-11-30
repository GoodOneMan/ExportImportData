using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportDataToModel.AppUnits.ModelHandlers
{
    class ModelStructure
    {
        public string Name { get; set; }
        public List<Attachment> Models { get; set; }
    }

    class Attachment
    {
        public string Name { get; set; }
        public List<Level> Levels { get; set; }
    }

    class Level
    {
        public string Name { get; set; }
        public List<Element> Elements { get; set; }
    }

    class Element
    {
        public string ID_Element { get; set; }
        public List<Property> Properties { get; set; }
    }

    class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}


namespace ExportDataToModel.StructuresFalse
{
    public class Model
    {
        public string Name { get; set; }
        public List<ModelRef> ModelRef { get; set; }
    }

    public class ModelRef
    {
        public string Name { get; set; }
        public List<Level> Level { get; set; }
    }

    public class Level
    {
        public string Name { get; set; }
        public List<Line> Lines { get; set; }
        public List<Element> Elements { get; set; }
    }

    public class Line
    {
        public string Name { get; set; }
        public string ElementID { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class Element
    {
        public string Name { get; set; }
        public string ElementID { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Translation { get; set; }
        public string Value { get; set; }
    }
}