using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api;
using System.Windows.Forms;

namespace ImportDataOPM
{
    [Plugin("ImportDataOPM", "TZ", DisplayName = "AddinsCXPP")]
    [Strings("template.name")]
    [RibbonLayout("template.xaml")]
    [RibbonTab("ID_Template_Tab")]
    [Command("ID_Select_Button", LargeIcon = @"Images\SelectButtonС.ico", ToolTip = "Выбор действия")]
    [Command("ID_Import_Data", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Импорт данных из OPM")]
    [Command("ID_Import_Data_Select_Model", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Импорт данных в выбраную модель из OPM")]
    public class Runner : CommandHandlerPlugin
    {
        // Load Assembly
        AppUnits.LoadingResources loadResoutce = new AppUnits.LoadingResources();

        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            switch (commandId)
            {
                case "ID_Import_Data":
                    //MessageBox.Show("ID_Import_Data");
                    TestRun();
                    break;

                case "ID_Import_Data_Select_Model":
                    MessageBox.Show("ID_Import_Data_Select_Model");
                    break;
            }

            return 0;
        }

        //
        private void TestRun()
        {
            //var modelHandler = new AppUnits.ModelHandler();
            //var collection = modelHandler.SearchCategoty("LcRevitData_Element", "Объект");
            //var collection = modelHandler.SearchCategoryAndProperty("LcOaNode", "Элемент", "LcOaSceneBaseClassName", "Внутренний тип", "LcDgnReference");

            // ImportData
            AppTest.ModelTest test = new AppTest.ModelTest();
            var m = test.GetModel();
        }
    }
}
