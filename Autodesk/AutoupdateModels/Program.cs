using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoupdateModels
{
    class Program
    {
        static Source.Config _config = Source.Config.Instance;

        static void Main(string[] args)
        {
            InitConsole();

            // test display
            #region Test display show
            //Source.Display.Show(_config.LoopPause.ToString(), "primary", 1, "[ "," ]");
            //Source.Display.Show(_config.CopyFilePause.ToString(), "secondary", 1, "[ ", " ]");
            //Source.Display.Show(_config.LoopPause.ToString(), "success", 1, "[ ", " ]");
            //Source.Display.Show(_config.CopyFilePause.ToString(), "danger", 1, "[ ", " ]");
            //Source.Display.Show(_config.LoopPause.ToString(), "warning", 1, "[ ", " ]");
            //Source.Display.Show(_config.CopyFilePause.ToString(), "info", 1, "[ ", " ]");
            //Source.Display.Show(_config.LoopPause.ToString(), "light", 1, "[ ", " ]");
            //Source.Display.Show(_config.CopyFilePause.ToString(), "pink", 1, "[ ", " ]");
            //Source.Display.Show(_config.CopyFilePause.ToString(), "", 1, "[ ", " ]");
            #endregion

            if(_config.InitFlag)
            {
                #region STEP 1 - Out display message of GraderFiles
                App.GraderFiles grader = new App.GraderFiles();
                List<string> message = grader.GetMessage();
                if(message.Count > 0)
                {
                    foreach (string msg in message)
                    {
                        Source.Display.Show(msg, Source.DisplayColor.danger, 2, "[ ", " ]");
                    }

                    // answer ???
                    //Source.Display.Show("continue or cancel ? : (yes/no) ", "warning", 0, "... ");
                    //string answer = Console.ReadLine();
                    //if (answer != "yes")
                    //    return;
                }
                else
                {
                    Source.Display.Show(_config.InitMessage, Source.DisplayColor.success, 2, "... ", " !");
                }

                Source.Display.Show("", Source.DisplayColor.primary, 1);
                Source.Display.Show(new string('#', Console.WindowWidth), Source.DisplayColor.primary, 2);
                #endregion

                #region STEP 2 - Run autoupdate model
                App.RunnerUpdate runner = new App.RunnerUpdate();
                runner.Run();
                #endregion
            }
            else
            {
                Source.Display.Show(_config.InitMessage, Source.DisplayColor.danger, 1, "... ", " !");
            }

            Console.Read();
        }


        // Init console
        private static void InitConsole()
        {
            Console.Title = "Updating Navisworks models";
            Console.BufferHeight = 8192;
            Console.BufferWidth = 94;
            Console.WindowWidth = 94;
            Console.ForegroundColor = ConsoleColor.White;

            Source.Display.Show("", Source.DisplayColor.primary, 1);
            Source.Display.Show("start initialization", Source.DisplayColor.success, 2, " ... ", " ... ");
            Source.Display.Show(new string('#', Console.WindowWidth), Source.DisplayColor.primary, 2);
        }
    }
}
