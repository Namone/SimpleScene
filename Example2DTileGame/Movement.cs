using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using SimpleScene;

namespace Example2DTileGame
{
    partial class Example2DTileGame : OpenTK.GameWindow
    {

        bool canMove = true; // Help detect collision
        /// <summary>
        /// Move the camera - needs work
        /// </summary>
        public void moveCamera()
        {

            // Follow player
           // camera.basePos.Xz = player.Pos.Xz;         

        }
        
        /// <summary>
        /// Move the player
        /// </summary>
        public void movePlayer()
        {

        }

        /// <summary>
        /// Detect if there is collision
        /// </summary>
        public void collide()
        {
            if (player.Pos.X > map.GetLength(0) * 2)
            {
                canMove = false;
                Console.WriteLine("Collide");
            }
        }
    }
}
