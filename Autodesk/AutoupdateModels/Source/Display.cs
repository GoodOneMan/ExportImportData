using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoupdateModels.Source
{
    public enum DisplayColor : int
    {
        primary,
        secondary,
        success,
        danger,
        warning,
        info,
        light,
        pink
    }

    class Display
    {
        //public static void Show(string message, string flag, int line = 0, string left = "", string right = "")
        public static void Show(string message, DisplayColor flag, int line = 0, string left = "", string right = "")
        {
            string new_line = "";
            switch (line)
            {
                case 1:
                    new_line = Environment.NewLine;
                    break;
                case 2:
                    new_line = Environment.NewLine + Environment.NewLine;
                    break;
                case 3:
                    new_line = Environment.NewLine + Environment.NewLine + Environment.NewLine;
                    break;
                default:
                    break;
            }

            switch (flag)
            {
                case DisplayColor.primary:
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.secondary:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.success:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.danger:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.info:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.light:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(left + message + right + new_line);
                    break;
                case DisplayColor.pink:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(left + message + right + new_line);
                    break;
                default:
                    Console.Write(left + message + right + new_line);
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
