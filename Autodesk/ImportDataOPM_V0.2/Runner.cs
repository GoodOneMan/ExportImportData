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
    [Command("ID_Import_Data", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Импорт данных из OPM по File -> Level -> ElementID")]
    [Command("ID_Import_Data_ElementID", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Импорт данных из OPM по ElementID")]
    [Command("ID_Select_Items_On_Criteria", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Выбрать элементы по критериям")]
    [Command("ID_Other_Test", LargeIcon = @"Images\GetListPath.ico", ToolTip = "Выбрать элементы по критериям")]
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
                    AppUnits.ModelHandler modelHandler = new AppUnits.ModelHandler();
                    break;

                case "ID_Import_Data_ElementID":
                    //MessageBox.Show("ID_Import_Data");
                    AppUnits.ModelHandlerElementId modelHandlerElementId = new AppUnits.ModelHandlerElementId();
                    break;

                case "ID_Select_Items_On_Criteria":
                    //MessageBox.Show("ID_Import_Data_Select_Model");
                    AppTest.SelectionItem.SearchItem searchItem = new AppTest.SelectionItem.SearchItem();
                    break;

                case "ID_Other_Test":

                    AppTest.QueryElement.SelectItemsForm selectionForm = new AppTest.QueryElement.SelectItemsForm();
                    selectionForm.Show();

                    //new AppTest.QueryElement.RvtQueryElement();
                    //MessageBox.Show("Ok");
                    break;
            }

            return 0;
        }
    }
}
