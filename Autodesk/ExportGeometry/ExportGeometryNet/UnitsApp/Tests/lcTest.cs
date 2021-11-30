using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Api.Interop;

namespace ExportGeometry.UnitsApp.Tests
{
    class lcTest
    {

        public lcTest()
        {
            Run();
        }


        private void Run()
        {
            LcOpKernelEx ke = LcOpKernelEx.GetActiveInstance() as LcOpKernelEx;

            if(ke != null)
            {
                LcVwDocument doc = new LcVwDocument(ke);

                //LcOwDocument l_doc = doc as LcOwDocument;
            }

            new RobModlTest().Run();
        }
    }

    // 

    class CallbakPrimetives : LcVwISimplePrimitives
    {
        public override void BeginFragment()
        {

        }

        public override void BeginIndexed(Autodesk.Navisworks.Api.Interop.LcVwIArrayContext array_context, int start, int end)
        {

        }

        public override void EndFragment()
        {

        }

        public override void EndIndexed()
        {

        }

        public override void IndexedLine(int vertex1, int vertex2)
        {

        }

        public override void IndexedPoint(int vertex1)
        {

        }

        public override void IndexedTriangle(int vertex1, int vertex2, int vertex3)
        {

        }

        

        public CallbakPrimetives() : base()
        {
            
        }

        public override void Line(Autodesk.Navisworks.Api.Interop.LcVwData3f vertex1, Autodesk.Navisworks.Api.Interop.LcVwData3f vertex2)
        {

        }

        public override void Point(Autodesk.Navisworks.Api.Interop.LcVwData3f vertex1)
        {

        }

        public override void Triangle(Autodesk.Navisworks.Api.Interop.LcVwData3f vertex1, Autodesk.Navisworks.Api.Interop.LcVwData3f vertex2, Autodesk.Navisworks.Api.Interop.LcVwData3f vertex3)
        {
            double x = vertex1.GetX();
            double y = vertex1.GetY();
            double z = vertex1.GetZ();
        }
        
    }


    #region Rob model
    class RobModlTest
    {

        // запуск из вне 
        // возврат списка
        public void Run()
        {
            SelectedModel();
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
            ModelItemCollection collection = new FI.FinderItem().SearchByCategory("LcOpGeometryProperty", "Геометрия");

            int count = collection.Count;

            for (int i = 0; i < count; i++)
            {
                ModelItem item = collection[i];


                FillFragments(item);

            }
        }

        #region Geometry
        //
        // на входе Item ModelItem
        //
        //
        //
        private void FillFragments(ModelItem item)
        {
            foreach (ModelItem item_geometry in item.DescendantsAndSelf)
            {
                if (item_geometry.HasGeometry)
                {
                   
                    ModelGeometry geometry = item_geometry.Geometry;

                    //if (geometry.IsSolid)
                    //    continue;

                    //if (geometry.PrimitiveTypes != PrimitiveTypes.Triangles)
                    //    continue;

                    HandlerModelGeometry(item_geometry.Geometry);
                }
            }
        }

        private void HandlerModelGeometry(ModelGeometry modelGeometry)
        {
            ComApi.InwOaPath oaPath = ComApiBridge.ToInwOaPath(modelGeometry.Item);

            ComApi.InwCollBase cool = oaPath.Fragments() as ComApi.InwCollBase;

            int a = cool.Count;

            var ienum = cool.GetEnumerator();

            while(ienum.MoveNext())
            {
                ComApi.InwOaFragment frag = ienum.Current as ComApi.InwOaFragment;

            }


            var col_pos = cool as ComApi.InwLPos3fColl;
            int aa = col_pos.Count;

            //CallbakPrimetives call = new CallbakPrimetives();
            //var callbkListener = call as ComApi.InwSimplePrimitivesCB;

            //foreach (ComApi.InwOaFragment3 fragment in oaPath.Fragments())
            //{
            //    fragment.GenerateSimplePrimitives(fragment.VertexProps, callbkListener);

            //}
        }

        #endregion
    }
    #endregion
}
