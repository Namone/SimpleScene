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

        // Points to draw the player (cube)
        Vector3 p0;
        Vector3 p1;
        Vector3 p2;

        float m1, m2;

        private float PLAYER_WIDTH = 0f, PLAYER_HEIGHT = 0f; 
        Random rand = new Random();
        /// <summary>
        /// Constructs the player at specific location (location of mouse)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SSPlayer()
        {
            float random = (float)rand.Next(50);
            p0 = new Vector3(0f * PLAYER_WIDTH, 0f, 0f);
            p1 = new Vector3(0f, 1f * PLAYER_HEIGHT, 0f);
            p2 = new Vector3(0f, 0f, 1f * PLAYER_WIDTH);

        }

        /// <summary>
        /// For setting position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SSPlayer(float x, float y)
        {
            m1 = x;
            m2 = y;
            x /= 10;
            y /= 10;
            p0 = new Vector3(x + PLAYER_WIDTH, 0f, 0f);
            p1 = new Vector3(0f, 0, 0f);
            p2 = new Vector3(0f, 0f, x + PLAYER_WIDTH);
            Console.WriteLine("New Player!");
        }

        /// <summary>
        /// Render the player
        /// </summary>
        /// <param name="renderConfig"></param>
        public override void Render(ref SSRenderConfig renderConfig)
        {
            base.Render(ref renderConfig);
            SSShaderProgram.DeactivateAll(); // Disable GLSL
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            // Draw the 'player' (which, for now, is just a 2D triangle)
            GL.Begin(PrimitiveType.Triangles);
            {
                GL.Color4(Color4.Snow);
                GL.Vertex3(p0); // Point 1
                GL.Vertex3(p1); // Point 2
                GL.Vertex3(p2); // Point 3
            }
            GL.End();

            Console.WriteLine(m1 + " - " + m2);

        }

    }
}
