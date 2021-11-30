using Autodesk.Navisworks.Api.Plugins;

namespace ExportToExcel
{
    [Plugin("TZ_ExportDataInNavisworks", "TZ", DisplayName = "ExportToExcel")]

    [Strings("template.name")]
    [RibbonLayout("template.xaml")]

    [RibbonTab("ID_Template_Tab")]
    [Command("ID_Select_Button", LargeIcon = @"Images\SelectButtonС.ico", ToolTip = "Выбор действия")]
    [Command("ID_Viewpoint_Report", LargeIcon = @"Images\ExportToExcel.ico", ToolTip = "отчет по точкам обзора")]
    public class Runner : CommandHandlerPlugin
    {
        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            switch (commandId)
            {
                case "ID_Viewpoint_Report":
                    Viewpoint_Report_Call();
                    break;
            }

            return 0;
        }

        // ID_Viewpoint_Report
        private void Viewpoint_Report_Call()
        {
            new App.ExportData();
        }
    }
}
