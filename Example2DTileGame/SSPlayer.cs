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
        SSObject playerObj;
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
            if (numOfPlayers < MAX_PLAYERS) {
                // Create player mesh and add it to scene
                var playerMesh = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./pigcharacter/", "pig.obj"); // Player model
                SSObject playerObject = new SSObjectMesh(playerMesh);
                playerObj = playerObject;
                scene.AddObject(playerObject); // add to scene
                playerObject.Pos = playerPosition;
                Console.WriteLine("New Player!");
                numOfPlayers++;
                ///////////////////////////////////////////////////////////
                Console.WriteLine(pos);
            }

        }

        // Getters and setters

        public SSObject getPlayerObject() { return playerObj; }

        public void setPlayerPos(Vector3 newPos) { pos = newPos; }

    }
}
