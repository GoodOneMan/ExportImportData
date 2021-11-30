using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Navisworks.Api;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace ExportGeometry.UnitsApp.Source
{
    class ModelGeometryCall
    {
        DS.Model_3D model;

        public ModelGeometryCall()
        {


            model = new DS.Model_3D();
            model.items = new List<DS.Item>();

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();

            Run();

            watch.Stop();
            System.Windows.Forms.MessageBox.Show(watch.ElapsedMilliseconds.ToString());

            //Tests.WriteToFileData wtfd = new Tests.WriteToFileData(model);
            //Tests.WriteToObjFile wtof = new Tests.WriteToObjFile(model);
            //Tests.WriteToObj wtof = new Tests.WriteToObj(model);
            //Tests.WritToUnity wtof = new Tests.WritToUnity(model);
            Tests.WriteVertexForOpenTK wvfotk = new Tests.WriteVertexForOpenTK(model);

        }

        // void Run()
        private void Run()
        {

            RobModel geometry = new RobModel("LcOpGeometryProperty", "Геометрия");
            var geometry_list = geometry.Run();

            //RobModel opm = new RobModel("LcOaPropOverrideCat", "Данные MicroStation");
            //var opm_list = opm.Run();
            
            //RobModel rvt = new RobModel("LcRevitData_Element", "Объект");
            //var rvt_list = rvt.Run();

            model.items = new List<DS.Item>();

            model.items.AddRange(geometry_list);

            //model.items.AddRange(opm_list);
            //model.items.AddRange(rvt_list);

        }
    }

    #region InwSimplePrimitivesCB Class
    class CallbackGeomListenerFive : ComApi.InwSimplePrimitivesCB
    {
        public List<DS.Point> points;
        public List<int> faces;

        public void Line(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2) {
            var v_1 = v1.coord;
            var v_2 = v2.coord;
        }

        public void Point(ComApi.InwSimpleVertex v1) {
            var v_1 = v1.coord;
        }

        public void SnapPoint(ComApi.InwSimpleVertex v1) {
            var v_1 = v1.coord;
        }

        public void Triangle(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2, ComApi.InwSimpleVertex v3)
        {
            set_ds_point(v1);
            set_ds_point(v2);
            set_ds_point(v3);
        }

        void set_ds_point(ComApi.InwSimpleVertex vertex)
        {
            // coordinate vertex
            float[] coord = convert_to_float((Array)(object)vertex.coord);
            int index = get_index(points, coord);

            if(index == -1)
            {
                DS.Point point = new DS.Point();

                point.coordinate = coord;
                point.color = convert_to_float((Array)(object)vertex.color);
                point.normal = convert_to_float((Array)(object)vertex.normal);
                point.texture = convert_to_float((Array)(object)vertex.tex_coord);

                points.Add(point);
                faces.Add(points.Count);
            }
            else
            {
                faces.Add(index + 1);
            }
        }

        int get_index(List<DS.Point> points, float[] coord)
        {
            int count = points.Count;

            for (int i = 0; i < count; i++)
            {
                if (equel_coordinate(points[i].coordinate, coord))
                    return i;
            }

            return -1;
        }

        bool equel_coordinate(float[] arr_one, float[] arr_two)
        {
            if (arr_one[0] != arr_two[0])
                return false;
            if (arr_one[1] != arr_two[1])
                return false;
            if (arr_one[2] != arr_two[2])
                return false;

            return true;
        }

        float[] convert_to_float(Array data)
        {
            int count = data.Length;
            float[] result = new float[count];

            for(int i = 0; i < count; i++)
            {
                result[i] = (float)(object)data.GetValue(i + 1);
            }

            return result;
        }
    }
    #endregion

    #region Rob model
    class RobModel
    {
        // поля 
        private ComApi.InwOpState opState;
        private List<DS.Item> _list;
        private string internel_name;
        private string user_name;

        // конструктор
        public RobModel(string _internel_name, string _user_name)
        {
            opState = ComApiBridge.State;
            _list = new List<DS.Item>();
            internel_name = _internel_name;
            user_name = _user_name;
        }

        // запуск из вне 
        // возврат списка
        public List<DS.Item> Run()
        {
            SelectedModel();

            return _list;
        }

        // делае запрос в модель получает коллекцию
        // пробегает по коллекции получает элементы
        // создает DS.Item
        // добовляет имя элемента, получает категорию со свойствами
        // вызов метода заполнения свойст
        // вызов метода заполнения фрагментов
        // добавление в список _list
        private void SelectedModel()
        {

            ModelItemCollection collection = new FI.FinderItem().SearchByCategory(internel_name, user_name);

            int count = collection.Count;

            for (int i = 0; i < count; i++)
            {
                ModelItem item = collection[i];

                DS.Item ds_item = new DS.Item();
                ds_item.name = item.DisplayName;

                // category
                PropertyCategory category = item.PropertyCategories.FindCategoryByCombinedName(new NamedConstant(internel_name, user_name));

                // Property and Fragment
                FillProperties(ref ds_item, category);
                FillFragments(ref ds_item, item);

                _list.Add(ds_item);
            }
        }

        #region Property
        //
        // На вход получает созданный item и
        // category из которой нужно получить
        // свойства, создает список свойств 
        //
        private void FillProperties(ref DS.Item ds_item, PropertyCategory category)
        {
            DataPropertyCollection property_collection = category.Properties;

            ds_item.properties = new List<DS.Property>();

            foreach (DataProperty property in property_collection)
            {

                DS.Property ds_property = new DS.Property();

                ds_property.name = property.DisplayName;
                ds_property.value = get_value(property.Value);

                ds_item.properties.Add(ds_property);
            }
        }

        private string get_value(VariantData property)
        {
            string value = "";

            if (property.IsBoolean)
            {
                value = property.ToBoolean().ToString();
            }
            else if (property.IsDisplayString)
            {
                value = property.ToDisplayString();
            }
            else if (property.IsInt32)
            {
                value = property.ToInt32().ToString();
            }
            else if (property.IsNamedConstant)
            {
                value = property.ToNamedConstant().Value.ToString();
            }
            else if (property.IsAnyDouble)
            {
                value = property.ToAnyDouble().ToString();
            }
            else if (property.IsDouble)
            {
                value = property.ToDouble().ToString();
            }
            else if (property.IsDoubleAngle)
            {
                value = property.ToDoubleAngle().ToString();
            }
            else if (property.IsDoubleArea)
            {
                value = property.ToDoubleArea().ToString();
            }
            else if (property.IsDoubleLength)
            {
                value = property.ToDoubleLength().ToString();
            }
            else if (property.IsDoubleVolume)
            {
                value = property.ToDoubleVolume().ToString();
            }
            else if (property.IsDateTime)
            {
                value = property.ToDateTime().ToString();
            }

            return value;
        }
        #endregion

        #region Geometry
        //
        // на входе Item ModelItem
        //
        //
        //
        private void FillFragments(ref DS.Item ds_item, ModelItem item)
        {

            foreach (ModelItem item_geometry in item.DescendantsAndSelf)
            {
                if (item_geometry.HasGeometry)
                {
                    ds_item.fragments = new List<DS.Fragment>();
                    ModelGeometry geometry = item_geometry.Geometry;

                    //if (geometry.IsSolid)
                    //    continue;

                    if (geometry.PrimitiveTypes != PrimitiveTypes.Triangles)
                        continue;

                    HandlerModelGeometry(ds_item.fragments, item_geometry.Geometry);
                }
            }
        }

        private void HandlerModelGeometry(List<DS.Fragment> list, ModelGeometry modelGeometry)
        {

            ComApi.InwOaPath oaPath = ComApiBridge.ToInwOaPath(modelGeometry.Item);
            CallbackGeomListenerFive callbkListener = new CallbackGeomListenerFive();

            foreach (ComApi.InwOaFragment3 fragment in oaPath.Fragments())
            {
                DS.Fragment fragment_s = new DS.Fragment();

                fragment_s.matrix = present_matrix((Array)(object)fragment.GetLocalToWorldMatrix().Matrix);
                fragment_s.points = new List<DS.Point>();
                fragment_s.faces = new List<int>();

                callbkListener.points = fragment_s.points;
                callbkListener.faces = fragment_s.faces;

                fragment.GenerateSimplePrimitives(ComApi.nwEVertexProperty.eNORMAL, callbkListener);

                list.Add(fragment_s);
            }
        }

        // matrix transformation
        double[] present_matrix(Array matx)
        {
            double[] matrix = new double[16];

            for (int i = 0; i < 16; i++)
            {
                matrix[i] = (double)matx.GetValue(i + 1);
            }

            return matrix;
        }
        #endregion
    }
    #endregion

}
