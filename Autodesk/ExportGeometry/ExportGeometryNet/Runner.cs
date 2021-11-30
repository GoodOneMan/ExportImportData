using Autodesk.Navisworks.Api.Plugins;
using System.Windows.Forms;

namespace ExportGeometry
{
    [Plugin("TZ_ExportGeometry", "TZ", DisplayName = "ExportGeometry")]

    [Strings("template.name")]
    [RibbonLayout("template.xaml")]

    [RibbonTab("ID_Template_Tab")]
    [Command("ID_Select_Button", LargeIcon = @"Images\SelectButtonС.ico", ToolTip = "Выбор действия")]
    [Command("ID_Export_Geometry", LargeIcon = @"Images\GetListPath.ico", ToolTip = "экспорт геометрии")]
    [Command("ID_Transparent_Insulation", LargeIcon = @"Images\GetListPath.ico", ToolTip = "прозрачная изоляция")]
    [Command("ID_Update_File", LargeIcon = @"Images\GetListPath.ico", ToolTip = "обновить модел")]
    [Command("ID_Test", LargeIcon = @"Images\GetListPath.ico", ToolTip = "тест")]
    public class Runner : CommandHandlerPlugin
    {
        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            switch (commandId)
            {
                case "ID_Export_Geometry":
                    Export_Geometry_Call();
                    break;

                case "ID_Transparent_Insulation":
                    Transparent_Insulation_Call();
                    break;

                case "ID_Update_File":
                    Update_File_Call();
                    break;

                case "ID_Test":
                    Test_Call();
                    break;
            }

            return 0;
        }
        
        // ID_Export_Geometry
        private void Export_Geometry_Call()
        {
            //new UnitsApp.Source.ModelGeometryCall();
            //new UnitsApp.Source.ModelGeometryCall_Two();
            new UnitsApp.Source.ModelGeometryCall_Three();
        }

        // ID_Transparent_Insulation
        private void Transparent_Insulation_Call()
        {
            new UnitsApp.Source.TransparentCall();
        }

        // ID_Update_File
        private void Update_File_Call()
        {
            new UnitsApp.Source.UpdataFileCall();
        }

        // ID_Test
        private void Test_Call()
        {
            //new UnitsApp.Tests.Test();
            //new UnitsApp.Source.UpdateFile.EventSkin();
            //new UnitsApp.Tests.GeometryReadTest();
            //new UnitsApp.Tests.ExportFbx();
            //new UnitsApp.Tests.Reflectiontest();
            new UnitsApp.Tests.OpenTKTest.StartGame();
        }
    }
}
