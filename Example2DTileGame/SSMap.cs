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
        public float MAX_HEIGHT = 60.0f;
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
            public Vector3 averageNormal;

            public VertexData(Vector3 pos, Color4 color, Vector3 normal)
            {
                this.Pos = pos;
                this.Color = color;
                this.triangleFaceNormal = normal;
                this.averageNormal = new Vector3(0, 0, 0);
            }

            public VertexData(Vector3 pos, Color4 color, Vector3 normal, Vector3 avgNormal)
            {
                this.Pos = pos;
                this.Color = color;
                this.triangleFaceNormal = normal;
                this.averageNormal = avgNormal;
            }
        }
        //-------------------------------------------------------------------------------------------

		// constructor
        public SSMap() {
			this.ambientMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.emissionMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.diffuseMatColor = new Color4(0.0f,0.0f,0.0f,0.0f);

			constructMap(); // Construct the map (set points)
		}

		private void constructMap() {
            Random rand = new Random();
            float avgHeight;
            float totalHeight;


            // Store heights
            for(int i = 0; i < mapHeight.GetLength(0) - 1; i++)
            {
                for(int j = 0; j < mapHeight.GetLength(1) - 1; j++)
                {
                    mapHeight[i, j] = ((float)rand.NextDouble() * MAX_HEIGHT) - MAX_HEIGHT/2.0f; // Store random heights that are less than max height
                }
            }

            // Relax the data 
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

            // construct the 3d mesh data
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
            

            // step 1. add lines to groundMesh_Lines

			// ...box around the tile
            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y)));  
            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y)));
            //groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y)));
            //groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y)));

            // ...lines to the middle
            //groundMesh_Lines.Add(new VertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p1, colorForHeight(p1.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));
            //groundMesh_Lines.Add(new VertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new VertexData(middle, colorForHeight(middle.Y)));


            //----------------------------------------------------

            // step 2. add Triangles to groundMesh_Tri
             
            // bottom-left : middle : top-left
			addGroundTriangle(p0,middle,p1);

			// top-left : middle : top-right
			addGroundTriangle(p1,middle,p3);

			// top-right : middle : bottom-right
			addGroundTriangle(p3,middle,p2);

            // bottom-right: middle : bottom-left
			addGroundTriangle(p2,middle,p0);
        }

		
		private void addGroundTriangle(Vector3 tp0, Vector3 tp1, Vector3 tp2) {
			// compute the triangle normal
			var triNormal = calcNormal(tp0,tp1,tp2);

			// add the triangle to the ground mesh
	        groundMesh_Tri.Add(new VertexData(tp0, colorForHeight(tp0.Y), triNormal));
	        groundMesh_Tri.Add(new VertexData(tp1, colorForHeight(tp1.Y), triNormal));
	        groundMesh_Tri.Add(new VertexData(tp2, colorForHeight(tp2.Y), triNormal)); 
			
			// accumulate the triangle normal for all three points.
			storeNormal(tp0,triNormal);
			storeNormal(tp1,triNormal);
			storeNormal(tp2,triNormal);
		}

        /// <summary>
        /// Calculate normals/store point values associated with each other
        /// </summary>
        /// <param name="p0">Point 1</param>
        /// <param name="p1">Point 2</param>
        /// <param name="p2">Middle</param>
        /// <returns></returns>
        private Vector3 calcNormal(Vector3 p0, Vector3 p1, Vector3 p2) 
		{
            Vector3 normal = Vector3.Cross(p0 - p1, p0 - p2);
            return normal;
		}


		/// <summary>
		/// store the triangle normal in the accumulation dictionary, indexed by position
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="triNormal">Tri normal.</param>
        private void storeNormal(Vector3 position, Vector3 triNormal)
        {
			// step 1. find out if there is an entry positionToNormalList[position] 

            if (positionToNormalList.ContainsKey(position))
            {
                positionToNormalList[position].Add(triNormal); // Add it into the list
            }

            // step 2...if not, create an empty list and put it in positionToNormalList[position]
            else
            {
                positionToNormalList.Add(position, new List<Vector3>());
            }


            // step 3. add triNormal to the list

        }


        /// <summary>
        /// Render
        /// </summary>
        /// <param name="renderConfig"></param>
        public override void Render(ref SSRenderConfig renderConfig)
        {
            base.Render(ref renderConfig);

            // step 1. Set-up render
            SSShaderProgram.DeactivateAll(); // Disable GLSL
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Lighting);
            // End of set-up

            // step 2. Draw the wireframe 'outline' of the map
            GL.Begin(PrimitiveType.Lines);
            {
                foreach (VertexData v in groundMesh_Lines) {
                    GL.Color4(v.Color);
                    GL.Vertex3(v.Pos); // Draw
                }
            }
            GL.End();

            // step 3. Draw the triangle 'ground' mesh
            GL.Begin(PrimitiveType.Triangles);
            {
                foreach (VertexData v in groundMesh_Tri) {
                    GL.Color4(v.Color);
					GL.Vertex3(v.Pos);
                }
            }
            GL.End();

            // step 4. Draw the per-triangle triFaceNormal
            GL.Begin(PrimitiveType.Lines);
            {
                foreach (VertexData v in groundMesh_Tri) {
					GL.Color4(Color4.Red);
					GL.Vertex3(v.Pos);
					GL.Vertex3(v.Pos + v.triangleFaceNormal.Normalized()); // Draw normalized vector
                }
            }
            GL.End();

            // step 5. Draw all average normals from the dictionary positionToNormalList
            GL.Begin(PrimitiveType.Lines);
            {
                foreach (var kvp in positionToNormalList) {
					Vector3 position = kvp.Key;
					List<Vector3> normalList = kvp.Value;
					Vector3 avgNormal = new Vector3(0,0,0);
					foreach (var normal in normalList) {
						avgNormal += normal;
					}
					avgNormal /= normalList.Count;
					avgNormal = avgNormal.Normalized();

					GL.Color4(Color4.Blue);
					GL.Vertex3(position);
					GL.Vertex3(position + (avgNormal * 1.5f)); // Draw normalized vector
                }
            }
            GL.End();

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
