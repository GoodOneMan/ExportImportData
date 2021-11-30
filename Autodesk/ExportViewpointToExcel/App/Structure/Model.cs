using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExportToExcel.App.Structure
{
    public class Model
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<RootItem> RootItem { get; set; }
    }

    public class RootItem
    {
        public string Name { get; set; }
        public Autodesk.Navisworks.Api.CommentCollection Comments { get; set; }
        public List<SubRootItem> SubRootItem { get; set; }
    }

    public class SubRootItem
    {
        public string Name { get; set; }
        public Autodesk.Navisworks.Api.CommentCollection Comments { get; set; }
        public List<Viewpoint> Viewpoints { get; set; }
    }

    public class Viewpoint
    {
        public string Name { get; set; }
        public string NumberCollision { get; set; }
        public string Discipline { get; set; }
        public string Description { get; set; }
        public Autodesk.Navisworks.Api.CommentCollection Comments { get; set; }
        public Autodesk.Navisworks.Api.Viewpoint View { get; set; }
    }

    public class Statement
    {
        public string FolderSave { get; set; }
        public string Date { get; set; }
        public List<Autodesk.Navisworks.Api.CommentStatus> MultiStatus { get; set; }

        public Statement()
        {
            Date = "full";
            MultiStatus = new List<Autodesk.Navisworks.Api.CommentStatus>();
        }
    }
}
