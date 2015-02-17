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
    class SSMap : SSObject
    {
        // Variables
        //-------------------------------------------------------------------------------------------
        static int arrayW = 50;
        static int arrayH = 50;
        float[,] mapHeight = new float[arrayW, arrayH];
        public float MAX_HEIGHT = 50.0f;
        float squareWidth = 4;

        // 1st value = Key
        // 2nd value = Value
        Dictionary<Vector3, List<Vector3>> positionToNormalList = new Dictionary<Vector3, List<Vector3>>();

        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        Vector3 middle;

        List<VertexData> groundMesh_Lines = new List<VertexData>(); // List to hold the vectors
        List<VertexData> groundMesh_Tri = new List<VertexData>(); // List to hold vectors of triangles

        struct VertexData
        {
            public Vector3 Pos;
            public Color4 Color;
            public Vector3 triangleFaceNormal;

            public VertexData(Vector3 pos, Color4 color, Vector3 normal)
            {
                this.Pos = pos;
                this.Color = color;
                this.triangleFaceNormal = normal;
            }
        }
        //-------------------------------------------------------------------------------------------

		// constructor
        public SSMap() {
			this.ambientMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.emissionMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.diffuseMatColor = new Color4(0.0f,0.0f,0.0f,0.0f);

			constructMap();
		}

		private void constructMap() {
            Random rand = new Random();
            float avgHeight;
            float totalHeight;


            // Store heights \\
            for(int i = 0; i < mapHeight.GetLength(0) - 1; i++)
            {
                for(int j = 0; j < mapHeight.GetLength(1) - 1; j++)
                {
                    mapHeight[i, j] = ((float)rand.NextDouble() * MAX_HEIGHT) - MAX_HEIGHT/2.0f; // Store random heights that are less than max height
                }
            }

            // Relax the data \\
            {
                float[,] tHeights = new float[arrayW, arrayH];
                float h1;
                float h2;
                float h3;
                float h4;

                for(int num = 0; num < 4; num++)
                {
                    for(int i = 1; i < mapHeight.GetLength(0) - 1; i++)
                    {
                        for(int j = 1; j < mapHeight.GetLength(1) - 1; j++)
                        {
                            h1 = mapHeight[i + 0, j - 1];
                            h2 = mapHeight[i - 1, j + 0];
                            h3 = mapHeight[i + 1, j + 0];
                            h4 = mapHeight[i + 1, j + 0];

                            totalHeight = h1 + h2 + h3 + h4;
                            avgHeight = totalHeight / 4;

                            tHeights[i, j] = avgHeight;
                        }
                    }

                    mapHeight = tHeights; // Set array used for drawing to the new values in the temp array
                }


            }

            // Draw the mesh \\
            for (int i = 1; i < mapHeight.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < mapHeight.GetLength(1) - 1; j++)
                {
                    float Middle = squareWidth / 2;
                    float squareCX = i * squareWidth;
                    float squareCY = j * squareWidth;

                    p0 = new Vector3(squareCX, average2x2(i, j), squareCY);
                    p1 = new Vector3(squareCX + squareWidth, average2x2(i + 1, j), squareCY);
                    p2 = new Vector3(squareCX, average2x2(i, j + 1), squareCY + squareWidth);
                    p3 = new Vector3(squareCX + squareWidth, average2x2(i + 1, j + 1), squareCY + squareWidth);

                    middle = new Vector3(squareCX + Middle, mapHeight[i, j], squareCY + Middle);

                    addToMapArray(p0, p1, p2, p3, middle);
                   
                }
            }
        }

        /// <summary>
        /// Draw map
        /// </summary>
        public void drawMap()
        {

            // Draw the 'outline' of the map
            GL.Begin(PrimitiveType.Lines);
            {

                // Drawing code
                foreach (VertexData v in groundMesh_Lines)
                {
                    GL.Color4(v.Color);
                    GL.Vertex3(v.Pos); // Draw
                }

            }
            GL.End();

            // Draw the triangle 'ground'
            GL.Begin(PrimitiveType.Triangles);
            {
                foreach (VertexData v in groundMesh_Tri)
                {
                    GL.Color4(v.Color);
					GL.Vertex3(v.Pos);
                }
            }
            GL.End();

            // Draw the triangle 'ground'
            GL.Begin(PrimitiveType.Lines);
            {
                foreach (VertexData v in groundMesh_Tri)
                {
					GL.Color4(Color4.Red);
					GL.Vertex3(v.Pos);
					GL.Vertex3(v.Pos + v.triangleFaceNormal.Normalized()); // Draw normalized vector
                }
            }
            GL.End();


        }

        /// <summary>
        /// Add the points to the array list
        /// </summary>
        /// <param name="p0">Bottom-left corner</param>
        /// <param name="p1">Top-left corner</param>
        /// <param name="p2">Bottom-right corner</param>
        /// <param name="p3">Top-right corner</param>
        /// <param name="middle">Middle of square drawn</param>
        public void addToMapArray(Vector3 p0, Vector3 p1, 
            Vector3 p2, Vector3 p3, Vector3 middle)
        {
            // TEMPORARLIY COMMENTED OUT
            // Base
            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y)));  

            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y)));

            //groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y)));

            //groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y)));

            //// Middle
            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));


            //----------------------------------------------------

            //Triangles

            Vector3 tri1 = calcNormals(p0, p1, middle);
            Vector3 tri2 = calcNormals(p1, p3, middle);
            Vector3 tri3 = calcNormals(p3, p2, middle);
            Vector3 tri4 = calcNormals(p2, p0, middle);

            Vector3 totalNorm = tri1 + tri2 + tri3 + tri4;

            Vector3 avgNorm = totalNorm / 4;
             
            // bottom-left : middle : top-left
            groundMesh_Tri.Add(new VertexData(p0, colorForHeight(p0.Y), avgNorm));
            groundMesh_Tri.Add(new VertexData(middle, colorForHeight(middle.Y), avgNorm));
            groundMesh_Tri.Add(new VertexData(p1, colorForHeight(p1.Y), avgNorm)); 


			// top-left : middle : top-right
            groundMesh_Tri.Add(new VertexData(p1, colorForHeight(p1.Y), avgNorm)); // 1
            groundMesh_Tri.Add(new VertexData(middle, colorForHeight(middle.Y), avgNorm)); // 2
            groundMesh_Tri.Add(new VertexData(p3, colorForHeight(p3.Y), avgNorm)); // 3

			// top-right : middle : bottom-right
            groundMesh_Tri.Add(new VertexData(p3, colorForHeight(p3.Y), avgNorm)); // 1
            groundMesh_Tri.Add(new VertexData(middle, colorForHeight(middle.Y), avgNorm)); // 2
            groundMesh_Tri.Add(new VertexData(p2, colorForHeight(p2.Y), avgNorm)); // 3

            // bottom-right: middle : bottom-left
            groundMesh_Tri.Add(new VertexData(p2, colorForHeight(p2.Y), avgNorm)); // 1
            groundMesh_Tri.Add(new VertexData(middle, colorForHeight(middle.Y), avgNorm)); // 2
            groundMesh_Tri.Add(new VertexData(p0, colorForHeight(p0.Y), avgNorm)); // 3

        }

        /// <summary>
        /// Calculate the normals for every vertex
        /// </summary>
        /// <param name="p0">Point 1</param>
        /// <param name="p1">Point 2</param>
        /// <param name="p2">Middle</param>
        /// <returns></returns>
        public Vector3 calcNormals(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 normal = Vector3.Cross(p0 - p2, p0 - p1);
            return normal;
        }


        /// <summary>
        /// Render
        /// </summary>
        /// <param name="renderConfig"></param>
        public override void Render(ref SSRenderConfig renderConfig)
        {
            base.Render(ref renderConfig);

            // Set-up render
            SSShaderProgram.DeactivateAll(); // Disable GLSL
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            // End of set-up

            drawMap(); // Draw it

        }
        

        /// <summary>
        /// Average out the values of the heights by 
        /// adding x & y together and dividing by 4
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float average2x2(int x, int y)
        {
            return (mapHeight[x - 1, y - 1]
                + mapHeight[x - 1, y]
                + mapHeight[x, y - 1]
                + mapHeight[x, y]) / 4.0f;
        }

        Color4 colorForHeight(float height)
        {
            return new Color4((height / MAX_HEIGHT) + 0.2f, height / MAX_HEIGHT * 5, height / MAX_HEIGHT * 5, 0);
        }



    }
}
