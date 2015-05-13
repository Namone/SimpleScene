// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using SimpleScene;
using System.Collections;

namespace Example2DTileGame
{
	partial class Example2DTileGame : OpenTK.GameWindow
	{

		SSObjectGDISurface_Text fpsDisplay;
        public static int mapWidth = 1; // for now
        public static int mapHeight = 0;
        public static int mapDepth = 1; // for now

        
        SSObject player;
		SSMap mapObject;

        SSRenderConfig renderConfig;

        // Positions for the camera
        float CameraX = 0.0f,
              CameraY = 50.0f,
              CameraZ = 0.0f;

        public static SSObject[,] map = new SSObject[mapWidth, mapDepth];
        
        /// <summary>
        /// Setup the scene (to be rendered)
        /// </summary>
		public void setupScene() {
            renderConfig = new SSRenderConfig();

            scene.BaseShader = shaderPgm;
            shaderPgm.Activate();
           
            // make and position the camera
            camera = new SSCameraThirdPerson();
            camera.basePos = new Vector3(CameraX, CameraY, CameraZ);   // the ground plane is in (X,Z) so Y+30 is units "above" the ground                           
                   
            scene.AddObject(camera);
            scene.ActiveCamera = camera;

            var lightPos = new Vector3(50.0f, 50.0f, 10.0f);
            var light = new SSLight();

            light.Pos = lightPos;
            light.Direction = new Vector3(50.0f, 10.0f, 0.0f);
            light.Ambient = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
            light.Specular = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);

            var triangle = new SSObjectTriangle();
            triangle.Pos = light.Pos;

            scene.AddLight(light);
            scene.AddObject(triangle);

            setupMap();
            //setupPlayer();
		}

        /// <summary>
        /// Load map
        /// </summary>
        public void setupMap()
        {

            mapObject = new SSMap();

            mapObject.Pos = new Vector3(0, 0, 0); // places the origin of the map mesh into the world at (0,0,0)
            mapObject.renderState.lighted = false;
            mapObject.ambientMatColor = new Color4(8.0f, 8.0f, 8.0f, 0.0f);
            mapObject.emissionMatColor = new Color4(8.0f, 8.0f, 8.0f, 0.0f);
            mapObject.diffuseMatColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            mapObject.specularMatColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            scene.AddObject(mapObject);
        }

        public void newPlayer(float x, float y)
        {
            player = new SSPlayer(x, y);
            scene.AddObject(player);      
        } 

		public void setupHUD() {
			hudScene.ProjectionMatrix = Matrix4.Identity;

			// HUD Triangle...
			//SSObject triObj = new SSObjectTriangle ();
			//hudScene.addObject (triObj);
			//triObj.Pos = new Vector3 (50, 50, 0);
			//triObj.Scale = new Vector3 (50.0f);

			// HUD text....
			fpsDisplay = new SSObjectGDISurface_Text ();
			fpsDisplay.Label = "FPS: ...";
            // Add the object to screen
			hudScene.AddObject (fpsDisplay);
            // Location of FPS HUD
			fpsDisplay.Pos = new Vector3 (10f, 10f, 0f);
            // Scale of text... (size)
			fpsDisplay.Scale = new Vector3 (1.0f);
			
		}

        /// <summary>
        /// Get player
        /// </summary>
        /// <returns></returns>
        public SSObject getPlayer()
        {
            return player;
        }

	}
}