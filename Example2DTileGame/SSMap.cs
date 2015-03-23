using System;
using System.IO;
using System.Xml;
using System.Drawing;
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
        static int arrayW = 30;
        static int arrayH = 30;
        MapTile[,] mapHeight = new MapTile[arrayW, arrayH];
        public float MAX_HEIGHT = 60.0f;
        float squareWidth = 4;
        
        SSTexture myTex;        

        // 1st value = Key
        // 2nd value = Value
        Dictionary<Vector3, List<Vector3>> positionToNormalList = new Dictionary<Vector3, List<Vector3>>();

        // TODO: change these to arrays, then change them to VBOs
        List<LineVertexData> groundMesh_Lines = new List<LineVertexData>(); // List to hold the line-segment verticies
        List<VertexData> groundMesh_Tri = new List<VertexData>(); // List to hold triangle verticies'
        List<int> textureIDs = new List<int>();

        public struct MapTile
        {
            public float height;
            public int tileType;
        }

        struct LineVertexData {
            public Vector3 Pos;
            public Color4 Color;

            public LineVertexData(Vector3 pos, Color4 color)               
            {
                this.Pos = pos;
                this.Color = color;
            }
        }

        struct VertexData
        {
            public Vector3 Pos;
            public Color4 Color;
            public Vector3 triangleFaceNormal;
            public Vector3 averageNormal;
            public Vector2 uvCoord; // 2D texture coordinate
            public int textureID;

            public VertexData(
                Vector3 pos, 
                Color4 color, 
                Vector3 normal,                 
                Vector2 uv,
                int id)
            {
                this.Pos = pos;
                this.Color = color;
                this.triangleFaceNormal = normal;                
                this.uvCoord = uv;
                this.textureID = id;

                this.averageNormal = new Vector3(0, 0, 0); // this is never known at construct time
            }
        }
        //-------------------------------------------------------------------------------------------

		// constructor
        public SSMap() {
			this.ambientMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.emissionMatColor = new Color4(1.0f,1.0f,1.0f,0.0f);
			this.diffuseMatColor = new Color4(0.0f,0.0f,0.0f,0.0f);

			constructMap(); // Construct the map (set points)

			setupMesh();
            loadTexture("RPG_Tiles_01.png"); 
		}

		private void setupMesh() {
            // compute and setup bounding sphere for map mesh
            float radius = 0f;
            foreach(var vertex in groundMesh_Tri) {
				radius = Math.Max(radius,vertex.Pos.Length);
            }
			this.boundingSphere = new SSObjectSphere(radius);
			this.OnChanged += (sender) => { 
					this.boundingSphere.Pos = this.Pos;
					this.boundingSphere.Scale = this.Scale;
			};
		}

		private void constructMap() {

            // If we have a height map save file...
            if (File.Exists(@"..\mapSave.xml")) {
                Console.WriteLine("Info: Map-save found!");
               
                // Returned array of saved heights
                float[,] savedHeights = loadHeightMap();
                textureIDs = loadTextureData(); // Load the texture IDs
                //Console.WriteLine(textureIDs[0]); testing...
                for (int i = 0; i < mapHeight.GetLength(0); i++) {
                    for (int j = 0; j < mapHeight.GetLength(1); j++) {

                        if (savedHeights[i, j] > 0 || savedHeights[i, j] < 0) {
                            //Console.WriteLine(savedHeights[i, j]);
                        }
                        
                        mapHeight[i, j].height = savedHeights[i, j];

                    }
                }
            }

            // If we don't already have a saved height map -> make a new map
            else if (!File.Exists(@"..\mapSave.xml")) {
                Random rand = new Random();
                float avgHeight;
                float totalHeight;


                // Store heights
                for (int i = 0; i < mapHeight.GetLength(0) - 1; i++) {
                    for (int j = 0; j < mapHeight.GetLength(1) - 1; j++) {
                        mapHeight[i, j].height = ((float)rand.NextDouble() * MAX_HEIGHT) - MAX_HEIGHT / 2.0f; // Store random heights that are less than max height
                    }
                }

                // Relax the data 
                {
                    MapTile[,] tHeights = new MapTile[arrayW, arrayH];

                    for (int num = 0; num < 4; num++) {
                        for (int i = 1; i < mapHeight.GetLength(0) - 1; i++) {
                            for (int j = 1; j < mapHeight.GetLength(1) - 1; j++) {
                                float h1 = mapHeight[i + 0, j - 1].height;
                                float h2 = mapHeight[i - 1, j + 0].height;
                                float h3 = mapHeight[i + 1, j + 0].height;
                                float h4 = mapHeight[i + 0, j + 1].height;

                                totalHeight = h1 + h2 + h3 + h4;
                                avgHeight = totalHeight / 4;

                                tHeights[i, j].height = avgHeight;
                            }
                        }

                        mapHeight = tHeights; // Set array used for drawing to the new values in the temp array
                    }
                }
            }

            // generate the mesh from heightmap
            generateMeshFromHeightmap();
                
        }

        private void generateMeshFromHeightmap() {

            // clear existing map data
            positionToNormalList = new Dictionary<Vector3, List<Vector3>>();
            groundMesh_Lines = new List<LineVertexData>(); // List to hold the vectors
            groundMesh_Tri = new List<VertexData>(); // List to hold vectors of triangles

            // construct the 3d mesh data
            for (int i = 1; i < mapHeight.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < mapHeight.GetLength(1) - 1; j++)
                {
                    float middleWidthOffset = squareWidth / 2;
                    float squareCX = i * squareWidth;
                    float squareCY = j * squareWidth;

                    // each corner height is the average of the 2x2 grid around it
                    var p0 = new Vector3(squareCX, average2x2(i, j), squareCY);
                    var p1 = new Vector3(squareCX + squareWidth, average2x2(i + 1, j), squareCY);
                    var p2 = new Vector3(squareCX, average2x2(i, j + 1), squareCY + squareWidth);
                    var p3 = new Vector3(squareCX + squareWidth, average2x2(i + 1, j + 1), squareCY + squareWidth);

                    // the middleHeight should be the average of the corners of the tile..
                    var middleHeight = (p0.Y + p1.Y + p2.Y + p3.Y) / 4.0f;
                    var middle = new Vector3(squareCX + middleWidthOffset, middleHeight, squareCY + middleWidthOffset);

                    addToMapArray(p0, p1, p2, p3, middle);

                }
            }

            // sweep over the positionToNormalList, compute and populate the average normal for each vertex

            for (int i = 0; i < groundMesh_Tri.Count; i++) {
                // retrieve the list of normals at this point
                var vertexData = groundMesh_Tri[i];
                var normalList = positionToNormalList[vertexData.Pos];

                // compute the average normal
                var avgNormal = new Vector3(0,0,0);
                foreach (var normal in positionToNormalList[vertexData.Pos]) {
                    avgNormal += normal;
                }
                avgNormal /= normalList.Count;

                // store the final average normal to the tri-mesh
                vertexData.averageNormal = avgNormal;
                groundMesh_Tri[i] = vertexData;
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
            groundMesh_Lines.Add(new LineVertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new LineVertexData(p1, colorForHeight(p1.Y)));
            groundMesh_Lines.Add(new LineVertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new LineVertexData(p2, colorForHeight(p2.Y)));
            groundMesh_Lines.Add(new LineVertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new LineVertexData(p3, colorForHeight(p3.Y)));
            groundMesh_Lines.Add(new LineVertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new LineVertexData(p1, colorForHeight(p1.Y)));

            // ...lines to the middle
            groundMesh_Lines.Add(new LineVertexData(p0, colorForHeight(p0.Y))); groundMesh_Lines.Add(new LineVertexData(middle, colorForHeight(middle.Y)));
            groundMesh_Lines.Add(new LineVertexData(p1, colorForHeight(p1.Y))); groundMesh_Lines.Add(new LineVertexData(middle, colorForHeight(middle.Y)));
            groundMesh_Lines.Add(new LineVertexData(p2, colorForHeight(p2.Y))); groundMesh_Lines.Add(new LineVertexData(middle, colorForHeight(middle.Y)));
            groundMesh_Lines.Add(new LineVertexData(p3, colorForHeight(p3.Y))); groundMesh_Lines.Add(new LineVertexData(middle, colorForHeight(middle.Y)));


            //----------------------------------------------------
            bool isUsingSpriteSheet = true; // For testing purposes
            // Add in SpriteSheet
            RectangleF bounds = new RectangleF();
            SpriteSheet spriteSheet = new SpriteSheet(48f, 48f, 12f, 12f);
            Random rand = new Random();
            int textureID = 0; // Default

            if (File.Exists(@"..\mapSave.xml")) {
                foreach (int i in textureIDs) {
                    textureID = i;
                }
            } else {
                textureID = rand.Next(12);
            }

            bounds = spriteSheet.getTileByID(textureID);

            // step 2. add Triangles to groundMesh_Tri

            // UV Coordinates for texture mapping
            var uv0 = new Vector2(0, 0); // bottom-left
            var uv1 = new Vector2(0, 1); // top-left
            var uv2 = new Vector2(1, 0); // bottom-right
            var uv3 = new Vector2(1, 1); // top-right
            var uvMiddle = new Vector2(0.5f, 0.5f); // middle

            if (isUsingSpriteSheet) {
                // UV Coordinates for spriteSheet bounds
                uv0 = new Vector2(bounds.X, bounds.Y); // bottom-left
                uv1 = new Vector2(bounds.X, bounds.Y + bounds.Height); // top-left
                uv2 = new Vector2(bounds.X + bounds.Width, bounds.Y); // bottom-right
                uv3 = new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height); // top-right
                uvMiddle = new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2); // middle
            }

            // bottom-left : middle : top-left
			storeTriangle(p0, middle, p1, uv1, uvMiddle, uv0, textureID);

			// top-left : middle : top-right
            storeTriangle(p1, middle, p3, uv0, uvMiddle, uv2, textureID);

			// top-right : middle : bottom-right
            storeTriangle(p3, middle, p2, uv2, uvMiddle, uv3, textureID);

            // bottom-right: middle : bottom-left
            storeTriangle(p2, middle, p0, uv3, uvMiddle, uv1, textureID);
        }

		/// <summary>
		/// Calculate normal for particular triangle-face
		/// </summary>
		/// <param name="tp0">Tp0.</param>
		/// <param name="tp1">Tp1.</param>
		/// <param name="tp2">Tp2.</param>
		private void storeTriangle(Vector3 tp0, Vector3 tp1, Vector3 tp2, 
            Vector2 uv0, Vector2 uvMiddle, Vector2 uv1, int textureID) 
		{

			// compute the triangle normal
			Vector3 triNormal = calcNormal (tp0, tp1, tp2);
            
            // Add the triangle to the ground mesh
            groundMesh_Tri.Add(new VertexData(tp0, colorForHeight(tp0.Y), triNormal, uv0, textureID)); // Add point 1
            groundMesh_Tri.Add(new VertexData(tp1, colorForHeight(tp1.Y), triNormal, uvMiddle, textureID)); // Add point 2 (middle/height)
            groundMesh_Tri.Add(new VertexData(tp2, colorForHeight(tp2.Y), triNormal, uv1, textureID)); // Add point 3

			// accumulate the triangle normal for all three points. (later we will compute average normal)
			accumulateTriNormal(tp0, triNormal);
            accumulateTriNormal(tp1, triNormal);
            accumulateTriNormal(tp2, triNormal);
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
		/// store the triangle in the accumulation dictionary, indexed by position
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="triNormal">Tri normal.</param>
        private void accumulateTriNormal(Vector3 position, Vector3 triNormal)
        {
			// step 1. find out if there is an entry positionToNormalList[position] 

			if (positionToNormalList.ContainsKey (position)) {
				positionToNormalList [position].Add (triNormal); // Add it into the list
			}
            // step 2...if not, create an empty list and put it in positionToNormalList[position]
            else {
				positionToNormalList.Add (position, new List<Vector3> { triNormal });            
			}
        }

		/// <summary>
		/// Adds points, height colors, and average normal into VertexData (moved from addGroundNormals)
		/// </summary>
		/// <param name="tp0">Tp0.</param>
		/// <param name="tp1">Tp1.</param>
		/// <param name="tp2">Tp2.</param>
		/// <param name="triNormal">Tri normal.</param>
		public void addPointsToList(Vector3 position, Vector3 tp0, Vector3 tp1, Vector3 tp2, Vector3 triNormal)
		{
			Vector3 avgNormal = new Vector3 (0, 0, 0);

			foreach (var lists in positionToNormalList) 
			{
				List<Vector3> normalList = lists.Value; // Duplicate lists

				foreach (Vector3 v in normalList) 
				{
					avgNormal += v;
				}

				avgNormal /= positionToNormalList.Count;

			}
			
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
            GL.Disable(EnableCap.Lighting);

            // End of set-up

            // step 2. Draw the wireframe 'outline' of the map
            GL.Begin(PrimitiveType.Lines);
            {
                foreach (LineVertexData v in groundMesh_Lines) {
                    GL.Color4(Color.Green);
                    // GL.Color4(v.Color);
                    GL.Vertex3(v.Pos); // Draw
                }
            }
            GL.End();

            // step 3. Draw the triangle 'ground' mesh
            // setup texture
            GL.Disable(EnableCap.Lighting);
            
            GL.ActiveTexture(TextureUnit.Texture0); // select texture unit 0
            GL.Enable(EnableCap.Texture2D);         // enable the texture unit   
         
            // use the texture we loaded the "easy way"
            GL.BindTexture(TextureTarget.Texture2D, myTex.TextureID);

            GL.Begin(PrimitiveType.Triangles);
            {                
                foreach (VertexData v in groundMesh_Tri) {
                    // first, attributes..
					GL.Normal3 (v.averageNormal);
                    GL.Color4(Color.White);
                    GL.TexCoord2(v.uvCoord);

                    // last, vertex...
                    GL.Vertex3(v.Pos);                    
				}
            }
            GL.End();
			/*
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
			*/

			// boilerplate to render bounding sphere
			// if (this.boundingSphere != null) {
            //    this.boundingSphere.Render(ref renderConfig);
			// }
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
            return (mapHeight[x - 1, y - 1].height
                + mapHeight[x - 1, y].height
                + mapHeight[x, y - 1].height
                + mapHeight[x, y].height) / 4.0f;
        }

        Color4 colorForHeight(float height)
        {
            return new Color4((height / MAX_HEIGHT) + 0.2f, height / MAX_HEIGHT * 5, height / MAX_HEIGHT * 5, 0);
        }

		/// <summary>
		/// 'Terraforming' where mouse clicks - refreshes map after each click
		/// </summary>
		public void terraRaiseLandAt(Vector3 worldSpacePoint, float raiseAmount) {
            // first convert the world-space point into a 2d Map tile

            float tileSpace_x = worldSpacePoint.X;            
            float tileSpace_y = worldSpacePoint.Z;

            int tileGridSpace_x = (int) (tileSpace_x / squareWidth);
            int tileGridSpace_y = (int) (tileSpace_y / squareWidth);
          
            
            Console.WriteLine("tileSpaceXY ({0},{1})  tileGridSpaceXY ({2},{3})", tileSpace_x,tileSpace_y,tileGridSpace_x,tileGridSpace_y);

			float brushRadius = 10f; // ideally, this should be evaluated in world space coordinates			
            
			// Loop over a bounding box around our brush
            // ....we'll use the whole map to start (eventually make this tighter around brush for performance)
			for (int x = 0; x < mapHeight.GetLength(0); x ++) {
				for (int y = 0; y < mapHeight.GetLength(1); y ++) {
                    bool isCenter = false; // Default
                    // evaluate the brush.

                    // calcualte the worldspace distance to the click point
                    float distanceFromCenter = (new Vector2(x * squareWidth, y * squareWidth) - 
                                                new Vector2(tileSpace_x, tileSpace_y)).Length;

                    if (x == tileGridSpace_x && y == tileGridSpace_y) {
                        isCenter = true;
                    }

                    // if it is inside our brush, then modify the map here..
                    if (distanceFromCenter < brushRadius || isCenter) {
                        mapHeight [x, y].height += raiseAmount;
                    }               
				}
			}

            generateMeshFromHeightmap();
        }

		/// <summary>
		/// Returns map location point
		/// </summary>
		/// <returns>The space point to map location.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Vector3 worldSpacePointToMapLocation(float x, float z)
		{
			x = (int)x / (int)squareWidth;
			z = (int)z / (int)squareWidth;
			Vector3 returnVector = new Vector3(x, 0, z); 
			Console.WriteLine (returnVector);
			// Return vector2 with new worldLocation (mostly used to story x & z values)
			return returnVector;
		}

		public override bool PreciseIntersect(ref SSRay worldSpaceRay, ref float distanceAlongRay) {
			// test to see if ray intersects any of the triangles in our mesh	
			float localNearestContact = float.MaxValue;;
			bool hitMesh = false; // By default, it is not hitting the mesh	

			// step 1. convert the ray from world space into object-local space
			SSRay localRay = worldSpaceRay.Transformed (this.worldMat.Inverted ());
            Vector3 hitPoint = new Vector3(0, 0, 0);

			// step 2. iterate through our triangle mesh, testing each triangle
			for (int n = 0; n < groundMesh_Tri.Count; n += 3) {
				// grab three points of a triangle
				var V1 = groundMesh_Tri[n].Pos; // Point 1 
				var V2 = groundMesh_Tri[n+1].Pos; // Point 2
				var V3 = groundMesh_Tri[n+2].Pos; // Point 3

				// run ray-to-triangle intersection test
				float contact;
				if (OpenTKHelper.TriangleRayIntersectionTest (V1, V2, V3, localRay.pos, localRay.dir, out contact)) {
					// we hit, so return distance
					if (!hitMesh) { // first hit
						localNearestContact = contact;
						hitMesh = true;						
					} else { // next hit
						localNearestContact = Math.Min (localNearestContact, contact);	
					}
				}
			}

			if (hitMesh) {
				// transform local-object-space hit distance, into world space
		
				// TODO: why is this inverted? it seems wrong.
				float worldSpaceContactDistance = -localNearestContact;
				distanceAlongRay = worldSpaceContactDistance;

			}
			return hitMesh;
		}
        /// <summary>
        /// Save the map in an XML file
        /// </summary>
        public void saveMap() {
            // Formatting
            XmlWriterSettings xmlSettings = new XmlWriterSettings { Indent = true };
            using (XmlWriter xmlWriter = XmlWriter.Create(@"../mapSave.xml", xmlSettings)) {

                xmlWriter.WriteStartDocument(); // Start writing  
                xmlWriter.WriteStartElement("MapTiles");

                // Move through mapHeights array (containing MapTile structs) and save the values 
                for (int i = 0; i < mapHeight.GetLength(0); i++) {
                    for (int j = 0; j < mapHeight.GetLength(1); j++) {
                        xmlWriter.WriteStartElement("MapTile");
                        xmlWriter.WriteAttributeString("x", i.ToString());
                        xmlWriter.WriteAttributeString("y", j.ToString());
                        xmlWriter.WriteAttributeString("type", mapHeight[i, j].tileType.ToString());
                        xmlWriter.WriteAttributeString("height", mapHeight[i, j].height.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                
                Console.WriteLine("Info: Map Saved!");

                foreach (VertexData v in groundMesh_Tri) {
                    xmlWriter.WriteStartElement("TextureID");
                    xmlWriter.WriteAttributeString("id", v.textureID.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        /// <summary>
        /// Load map-height data
        /// </summary>
        public float[,] loadHeightMap() {

            float[,] heightStorage = new float [arrayW, arrayH];
            List<int> textIDStorage = new List<int>();
            // Read the file - assigning values as we go
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader xmlReader = XmlReader.Create(@"../mapSave.xml", settings)) {

                        while (xmlReader.Read()) {
                            switch (xmlReader.Name) {
                                    // Read maptile data (heights, tyle type, etc.)
                                case "MapTile":
                                    int x = int.Parse(xmlReader.GetAttribute(0)); // get x coord
                                    int y = int.Parse(xmlReader.GetAttribute(1)); // get y coord
                                    float height = float.Parse(xmlReader.GetAttribute(3)); // get height  
                              
                                    heightStorage[x, y] = height;

                                    break;                                

                    }
                }

                return heightStorage;

             }  
           

        }

        /// <summary>
        /// Load texture ID data
        /// </summary>
        /// <returns></returns>
        public List<int> loadTextureData() {

            List<int> textIDStorage = new List<int>();
            // Read the file - assigning values as we go
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader xmlReader = XmlReader.Create(@"../mapSave.xml", settings)) {

                while (xmlReader.Read()) {
                    switch (xmlReader.Name) {
                        // Read maptile data (heights, tyle type, etc.)
                        case "TextureID":
                            int id = int.Parse(xmlReader.GetAttribute(0)); // get texture id of tile
                            textIDStorage.Add(id);
                            break;
                    }
                }

                return textIDStorage;

            }

        }

        /// <summary>
        /// Delete map-save file and generates new map
        /// </summary>
        public void deleteMapSave() {
            File.Delete(@"../mapSave.xml");
            Console.WriteLine("Info: Map-save deleted!");
            constructMap();
        }

        ////////////////////
        //Texture Loading//
        //////////////////

        public void loadTexture(string fileName) {

            // this is the SimpleScene way...
            var ctx = new SSAssetManager.Context("maptextures");
            myTex = SSAssetManager.GetInstance<SSTexture>(ctx, fileName);


        }
    }
}
