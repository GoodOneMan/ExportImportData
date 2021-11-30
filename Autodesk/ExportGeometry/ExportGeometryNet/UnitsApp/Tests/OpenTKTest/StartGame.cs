using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportGeometry.UnitsApp.Tests.OpenTKTest
{
    class StartGame
    {
        public StartGame()
        {
            //using (Game game = new Game(800, 600, "LearnOpenTK"))
            //{
            //    //Run takes a double, which is how many frames per second it should strive to reach.
            //    //You can leave that out and it'll just update as fast as the hardware will allow it.
            //    game.Run(60.0);
            //}

            using (Game game = new Game(800, 600, "Camera"))
            {
                //Run takes a double, which is how many frames per second it should strive to reach.
                //You can leave that out and it'll just update as fast as the hardware will allow it.
                game.Run(60.0);
            }

        }
    }
}
