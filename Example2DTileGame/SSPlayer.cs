using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using SimpleScene;

namespace Example2DTileGame
{
    class SSPlayer : SSObject
    {
        static int numOfPlayers = 0; // By default
        static readonly int MAX_PLAYERS = 1;
        public Vector3 pos;
        /// <summary>
        /// For setting position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SSPlayer(Vector3 playerPosition, SSScene scene)
        {
            pos = playerPosition;
            if (numOfPlayers < MAX_PLAYERS) {
                // Create player mesh and add it to scene
                var playerMesh = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./pigcharacter/", "pig.obj"); // Player model
                SSObject playerObject = new SSObjectMesh(playerMesh);
                scene.AddObject(playerObject); // add to scene
                playerObject.Pos = pos;
                Console.WriteLine("New Player!");
                numOfPlayers++;
                ///////////////////////////////////////////////////////////
                Console.WriteLine(pos);
            }

        }

        // Getters and setters

        public Vector3 getPlayerPos() { return pos; }

        public void setPlayerPos(Vector3 newPos) { pos = newPos; }

    }
}
