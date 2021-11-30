using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using System.Security.Cryptography;
using System.IO;

namespace ExportToExcel.App
{
    class ViewPointFacade
    {
        //Structure.Model _model = null;
        //Structure.Statement _statement = null;
        //Document _doc = null;
        
        //// Bitmap property
        //string _folder = "";
        //string _img_folder = "ImageCollision";
        //int width = 1024;
        //int height = 768;

        //public ViewPointFacade(string folder, Structure.Statement statement)
        //{
        //    _statement = statement;
        //    _folder = folder;

        //    _doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            
        //    _model = new Structure.Model();
        //    _model.NameProject = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentFileName;
        //    _model.CurrentDate = DateTime.Now;
        //    _model.Folders = new List<Structure.RootFolder>();

        //    Run();
        //}

        //// Generation model
        //#region Generation model
        //private void Run()
        //{
        //    Autodesk.Navisworks.Api.DocumentParts.DocumentSavedViewpoints _view_points = _doc.SavedViewpoints;

        //    foreach(var item in _view_points.ToSavedItemCollection())
        //    {
        //        if(_statement.RootFolder == "full" || item.DisplayName == _statement.RootFolder)
        //        {
        //            Structure.RootFolder root_folder = new Structure.RootFolder();

        //            root_folder.Folders = new List<Structure.TitleFolder>();
        //            root_folder.NameFolder = item.DisplayName;
        //            root_folder.Comments = GetComments(item.Comments);

        //            if (item.IsGroup)
        //                SetTitleInRoot(item, root_folder.Folders);

        //            _model.Folders.Add(root_folder);
        //        }
        //    }
        //}

        //// Set title struct
        //private void SetTitleInRoot(SavedItem folder, List<Structure.TitleFolder> list)
        //{
        //    foreach (SavedItem item in ((GroupItem)folder).Children)
        //    {
        //        Structure.TitleFolder _title_folder = new Structure.TitleFolder();

        //        _title_folder.ViewPoints = new List<Structure.ViewPoint>();
        //        _title_folder.NameFolder = item.DisplayName;
        //        _title_folder.Comments = GetComments(item.Comments);

        //        if (item.IsGroup)
        //            SetViewPointsInSubRoot(item, _title_folder.ViewPoints);

        //        list.Add(_title_folder);
        //    }
        //}

        //// Set viewpoint structure
        //private void SetViewPointsInSubRoot(SavedItem folder, List<Structure.ViewPoint> list)
        //{
        //    foreach (SavedItem item in ((GroupItem)folder).Children)
        //    {
        //        if(item is SavedViewpoint)
        //        {
        //            Structure.ViewPoint viewpoint = new Structure.ViewPoint();
        //            viewpoint.Comments = GetComments(item.Comments);

        //            bool flag = true;

        //            // Chacked
        //            //if(viewpoint.Comments.Count == 0 || _statement.Status == "full" || viewpoint.Comments[0].Status == _statement.Status)
        //            //{
        //            //    string[] arr = item.DisplayName.Split('_');
        //            //    SetDataViewPoint(viewpoint, arr);
        //            //    viewpoint.Image = GetPathBitmapFile(item);
        //            //    list.Add(viewpoint);
        //            //}
                    

        //            if(viewpoint.Comments.Count == 0 || _statement.MultiStatus.Count == 0)
        //            {
        //                string[] arr = item.DisplayName.Split('_');
        //                SetDataViewPoint(viewpoint, arr);

        //                viewpoint.Image = GetPathBitmapFile(item);

        //                list.Add(viewpoint);

        //                flag = false;
        //            }

        //            if (flag)
        //            {
        //                foreach(string status in _statement.MultiStatus)
        //                {
        //                    if(viewpoint.Comments[0].Status == status)
        //                    {
        //                        string[] arr = item.DisplayName.Split('_');
        //                        SetDataViewPoint(viewpoint, arr);
        //                        viewpoint.Image = GetPathBitmapFile(item);
        //                        list.Add(viewpoint);

        //                        flag = false;
        //                    }
        //                }
        //            }

        //        }
        //    }
        //}

        //// Helpers functions
        //#region Helpers function
        //// Get comments
        //private List<Structure.Comment> GetComments(CommentCollection comments)
        //{
        //    List<Structure.Comment> list = new List<Structure.Comment>();

        //    foreach (var item in comments)
        //    {
        //        Structure.Comment comment = new Structure.Comment();
        //        comment.Author = item.Author;
        //        comment.Date = item.CreationDate;
        //        comment.Body = item.Body;
        //        comment.Status = GetCommentStatus(item.Status);

        //        list.Add(comment);
        //    }

        //    return list;
        //}
        //// Return status to string
        //private string GetCommentStatus(CommentStatus status)
        //{
        //    string result = "";

        //    switch (status)
        //    {
        //        case CommentStatus.Active:
        //            result = "Активный"; // 41
        //            break;

        //        case CommentStatus.Approved:
        //            result = "Утверждено"; // 27
        //            break;

        //        case CommentStatus.New:
        //            result = "Новый"; // 41
        //            break;

        //        case CommentStatus.Resolved:
        //            result = "Исправлено"; // 35
        //            break;
        //    }

        //    return result;
        //}
        //// Set Data in ViewPoint
        //private void SetDataViewPoint(Structure.ViewPoint viewpoint, string[] arr)
        //{
        //    if (arr.Length == 3)
        //    {
        //        viewpoint.NumberCollision = arr[0];
        //        viewpoint.Discipline = arr[1];
        //        viewpoint.Description = arr[2];
        //    }
        //    if (arr.Length == 2)
        //    {
        //        viewpoint.NumberCollision = arr[0];
        //        viewpoint.Description = arr[1];
        //    }
        //    if (arr.Length == 1)
        //    {
        //        viewpoint.Description = arr[0];
        //    }
        //}
        //// Generate bitmap file
        //private string GetPathBitmapFile(SavedItem item)
        //{
        //    Viewpoint viewpoint = (item as SavedViewpoint).Viewpoint.CreateCopy();

        //    // Style img
        //    //viewpoint.RenderStyle = ViewpointRenderStyle.Preview;
        //    //viewpoint.Lighting = ViewpointLighting.None;

        //    View view = Autodesk.Navisworks.Api.Application.ActiveDocument.ActiveView;
        //    view.CopyViewpointFrom(viewpoint, ViewChange.JumpCut);
            
        //    string path = _folder + Path.DirectorySeparatorChar + _img_folder + Path.DirectorySeparatorChar + GetBitmapName(item.DisplayName) + ".bmp";

        //    // Save bitmap
        //    //SaveBitmap(view, path);

        //    return path;
        //}
        //// Return Bitmap name
        //private string GetBitmapName(string str)
        //{
        //    MD5 md5Hasher = MD5.Create();
        //    byte[] hashenc = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str + DateTime.Now));
        //    string result = "";
        //    foreach (var b in hashenc)
        //    {
        //        result += b.ToString("x2");
        //    }

        //    return result;
        //}
        //// Save bitmap
        //private void SaveBitmap(View view, string file)
        //{
        //    // ??? optimization
        //    System.Drawing.Bitmap bitmap = view.GenerateThumbnail(width, height);

        //    string directory = Path.GetDirectoryName(file);
        //    try
        //    {
        //        if (!Directory.Exists(directory))
        //            Directory.CreateDirectory(directory);

        //        bitmap.Save(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.Message);
        //    }
        //}
        //#endregion

        //#endregion

        //// Get data
        //#region Get other data
        //public Structure.Model GetModel()
        //{
        //    return _model;
        //}
        //#endregion
    }
    
}
