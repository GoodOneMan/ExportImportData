using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportToExcel.App.Viewpoint
{
    class ModelViewpoints
    {
        Autodesk.Navisworks.Api.DocumentParts.DocumentSavedViewpoints SavedViewpoints = null;
        Autodesk.Navisworks.Api.Document Document = Autodesk.Navisworks.Api.Application.ActiveDocument;
        Structure.Model Model = new Structure.Model();

        //
        public ModelViewpoints()
        {
            Model.Name = Document.FileName;
            Model.Date = DateTime.Now;
            Model.RootItem = new List<Structure.RootItem>();

            SavedViewpoints = Document.SavedViewpoints;

            RootItemHandler(Model.RootItem);
        }

        #region Private method

        //
        private void RootItemHandler(List<Structure.RootItem> list)
        {
            foreach (SavedItem item in SavedViewpoints.ToSavedItemCollection())
            {
                Structure.RootItem RootItem = new Structure.RootItem();

                // RootItem
                RootItem.Name = item.DisplayName;
                RootItem.Comments = item.Comments;
                RootItem.SubRootItem = new List<Structure.SubRootItem>();

                if (item.IsGroup)
                {
                    // TitleItem
                    SubRootItemHandler(item, RootItem.SubRootItem);
                }

                list.Add(RootItem);
            }
        }

        //
        private void SubRootItemHandler(SavedItem SaveItem, List<Structure.SubRootItem> list)
        {
            foreach (SavedItem item in ((GroupItem)SaveItem).Children)
            {
                Structure.SubRootItem SubRootItem = new Structure.SubRootItem();

                SubRootItem.Name = item.DisplayName;
                SubRootItem.Comments = item.Comments;
                SubRootItem.Viewpoints = new List<Structure.Viewpoint>();

                if (item.IsGroup)
                {
                    ViewpointHandler(item, SubRootItem.Viewpoints);
                }

                list.Add(SubRootItem);
            }
        }

        //
        private void ViewpointHandler(SavedItem SaveItem, List<Structure.Viewpoint> list)
        {
            foreach (SavedItem item in ((GroupItem)SaveItem).Children)
            {
                Structure.Viewpoint Viewpoint = new Structure.Viewpoint();

                Viewpoint.Name = item.DisplayName;

                string[] data = item.DisplayName.Split('_');

                if (data.Length == 3)
                {
                    Viewpoint.NumberCollision = data[0];
                    Viewpoint.Discipline = data[1];
                    Viewpoint.Description = data[2];
                }

                Viewpoint.Comments = item.Comments;

                if (item is SavedViewpoint)
                {
                    Viewpoint.View = (item as SavedViewpoint).Viewpoint;
                }

                list.Add(Viewpoint);
            }
        }
        #endregion

        #region Public method
        //
        public Structure.Model GetModel()
        {
            return Model;
        }


        #region static Convert status 
        //
        public static string GetStatusToString(CommentStatus status)
        {
            string result = "";

            switch (status)
            {
                case CommentStatus.New:
                    result = "Новый";
                    break;

                case CommentStatus.Active:
                    result = "Активный";
                    break;

                case CommentStatus.Resolved:
                    result = "Исправлено";
                    break;

                case CommentStatus.Approved:
                    result = "Утверждено";
                    break;
            }

            return result;
        }

        //
        public static CommentStatus GetStringToStatus(string status)
        {
            CommentStatus result = new CommentStatus();

            switch (status)
            {
                case "Новый":
                    result = CommentStatus.New;
                    break;

                case "Активный":
                    result = CommentStatus.Active;
                    break;

                case "Исправлено":
                    result = CommentStatus.Resolved;
                    break;

                case "Утверждено":
                    result = CommentStatus.Approved;
                    break;
            }

            return result;
        }
        #endregion

        //
        public static Structure.Model Filter(Structure.Statement Statement, Structure.Model Model)
        {
            Structure.Model _Model = new Structure.Model();
            _Model.Name = Model.Name;
            _Model.Date = Model.Date;
            _Model.RootItem = new List<Structure.RootItem>();

            // Root 
            foreach (var RootItem in Model.RootItem)
            {
                if (Statement.Date == "full" || Statement.Date == RootItem.Name)
                {
                    Structure.RootItem root_item = new Structure.RootItem();
                    root_item.Name = RootItem.Name;
                    root_item.Comments = RootItem.Comments;
                    root_item.SubRootItem = new List<Structure.SubRootItem>();

                    // SubRoot
                    foreach (var SubRootItem in RootItem.SubRootItem)
                    {
                        Structure.SubRootItem subroot_item = new Structure.SubRootItem();
                        subroot_item.Name = SubRootItem.Name;
                        subroot_item.Comments = SubRootItem.Comments;
                        subroot_item.Viewpoints = new List<Structure.Viewpoint>();

                        // ViewPoint
                        foreach (var ViewPointItem in SubRootItem.Viewpoints)
                        {
                            bool flag = true;
                            if (Statement.MultiStatus.Count == 0)
                            {
                                subroot_item.Viewpoints.Add(ViewPointItem);

                                flag = false;
                            }

                            if (flag)
                            {
                                if (ViewPointItem.Comments.Count != 0)
                                {
                                    foreach (CommentStatus status in Statement.MultiStatus)
                                    {
                                        if (ViewPointItem.Comments[0].Status == status)
                                        {
                                            subroot_item.Viewpoints.Add(ViewPointItem);
                                        }
                                    }
                                }
                            }
                        }

                        if (subroot_item.Viewpoints.Count != 0)
                            root_item.SubRootItem.Add(subroot_item);
                    }

                    _Model.RootItem.Add(root_item);
                }
            }

            return _Model;
        }
        #endregion
    }
}
