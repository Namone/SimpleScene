// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using SimpleScene;

namespace Example2DTileGame
{
	partial class Example2DTileGame : OpenTK.GameWindow {

		SSObject selectedObject = null;
        MouseAction currentMode = MouseAction.RAISE_LAND;

        protected enum MouseAction
        {
            RAISE_LAND,
            LOWER_LAND,
            DRAW_GRASS,
            DRAW_GRAVEL,
            ADD_HOUSE,
            ADD_STONE0,
            ADD_STONE1, // if I want other types of stones
            ADD_TREE0,
        }
		Vector3 hitPoint;

        private void adjustMouseCursor() {
			// this doesn't seem to work on MacOS...
			if (this.mouseRightButtonDown) {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
			} else if (this.mouseLeftButtonDown) { 
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.VSplit;
			} else {
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
			}
		}

		public void setupInput() {
            List<SSObject> listOfHouses = new List<SSObject>(); // List to contain house objects
            List<SSObject> listOfStones = new List<SSObject>(); // List to contain stone objects
            // Create variables to hold mesh data... could probably put this somewhere else in program
            var houseMesh = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./houseModel/", "actualhouse.obj");
            var stoneMesh0 = SSAssetManager.GetInstance<SSMesh_wfOBJ>("./stoneModel/", "stone.obj");
            /////////////////////////////////////////////////////////////////////////////////////////////


			// hook mouse drag input...
			this.MouseDown += (object sender, MouseButtonEventArgs e) => {
				this.mouseLeftButtonDown = e.Mouse.IsButtonDown(MouseButton.Left);
				this.mouseRightButtonDown = e.Mouse.IsButtonDown(MouseButton.Right);
				this.adjustMouseCursor();

				// cast ray for mouse click
				var clientRect = new System.Drawing.Size(ClientRectangle.Width, ClientRectangle.Height);
				Vector2 mouseLoc = new Vector2(e.X,e.Y);

				SSRay ray = OpenTKHelper.MouseToWorldRay(
					this.scene.ProjectionMatrix,this.scene.InvCameraViewMatrix, clientRect, mouseLoc);

				// -- this will add a visual "ray" rendering to the scene...
				// Console.WriteLine("mouse ({0},{1}) unproject to ray ({2})",e.X,e.Y,ray);
				// scene.AddObject(new SSObjectRay(ray));

				// -- this will test for collision with scene objects
				// selectedObject = scene.Intersect(ref ray);

				// -- this collides the ray with the map mesh
				if (mapObject != null) {
					float distance = 0.0f;
					if ( mapObject.PreciseIntersect(ref ray, ref distance) ) {
						// we hit the map mesh! calculate where...
						hitPoint = ray.pos - (ray.dir.Normalized() * (distance - 0.01f));

                        // terra forming
                        switch (currentMode) {
                            case MouseAction.RAISE_LAND:
                                mapObject.terraRaiseLandAt(hitPoint, 2.5f); // Terraforming purposes
                                break;
                            case MouseAction.LOWER_LAND:
                                mapObject.terraRaiseLandAt(hitPoint, -2.5f); // Terraforming purposes
                                break;
                            case MouseAction.DRAW_GRASS:
                                mapObject.terraChangeTextureId(hitPoint, 22);
                                break;
                            case MouseAction.DRAW_GRAVEL:
                                mapObject.terraChangeTextureId(hitPoint, 28);
                                break;
                            case MouseAction.ADD_HOUSE:
                                var house = new SSObjectMesh(houseMesh);
                                listOfHouses.Add(house); // add it to the list
                                house.Pos = hitPoint;
                                scene.AddObject(house);
                                mapObject.storeObjectData("house.obj", house.Pos.X, house.Pos.Y, house.Pos.Z);
                                break;
                            case MouseAction.ADD_STONE0:
                                var stone = new SSObjectMesh(stoneMesh0);
                                listOfStones.Add(stone); // add it to the list
                                stone.Pos = hitPoint;
                                scene.AddObject(stone);
                                mapObject.storeObjectData("stone.obj", stone.Pos.X, stone.Pos.Y, stone.Pos.Z);
                                break;

                        }

	
					}
				}

			};

			this.MouseUp += (object sender, MouseButtonEventArgs e) => { 
				this.mouseLeftButtonDown = e.Mouse.IsButtonDown(MouseButton.Left);
				this.mouseRightButtonDown = e.Mouse.IsButtonDown(MouseButton.Right);
				this.adjustMouseCursor();
			};
			this.MouseMove += (object sender, MouseMoveEventArgs e) => {
				if (this.mouseRightButtonDown) { 
					// move the mouse in projected X,Z, to keep the map a constant distance away
					this.scene.ActiveCamera.MousePan(Vector3.UnitY, e.XDelta,e.YDelta);
				} else if (this.mouseLeftButtonDown) {

					// Console.WriteLine("mouse dragged: {0},{1}",e.XDelta,e.YDelta);
					this.scene.ActiveCamera.MouseDeltaOrient(e.XDelta,e.YDelta);
					// this.activeModel.MouseDeltaOrient(e.XDelta,e.YDelta);
				}
			};
			this.MouseWheel += (object sender, MouseWheelEventArgs e) => { 
				// Console.WriteLine("mousewheel {0} {1}",e.Delta,e.DeltaPrecise);
				SSCameraThirdPerson ctp = scene.ActiveCamera as SSCameraThirdPerson;
				if (ctp != null) {
					ctp.followDistance += -e.DeltaPrecise;
				} 
			};

            int arrayPos = 0; // Position in array
			this.KeyPress += (object sender, KeyPressEventArgs e) => {
				switch (e.KeyChar) {
                    case '1' :
                        currentMode = MouseAction.RAISE_LAND;
                        break;
                    case '2':
                        currentMode = MouseAction.LOWER_LAND;
                        break;
                    case '3':
                        currentMode = MouseAction.DRAW_GRASS;
                        break;
                    case '4':
                        currentMode = MouseAction.DRAW_GRAVEL;
                        break;
                    case '5':
                        currentMode = MouseAction.ADD_HOUSE;
                        break;
                    case '6':
                        currentMode = MouseAction.ADD_STONE0;
                        break;
                    case 'p': // save
                        mapObject.saveMap();
                        break;
                    case 'o': // delete save
                        mapObject.deleteMapSave();
                        break;
                    case 'w': // move mesh up and down (currently the first house is default)
                        var y = listOfHouses[arrayPos].Pos.Y;
                        y += 1;
                        Console.WriteLine("First value: {0}", listOfHouses[arrayPos].Pos.Y);
                        Vector3 newPosUp = new Vector3(listOfHouses[arrayPos].Pos.X, y, listOfHouses[arrayPos].Pos.Z);
                        listOfHouses[arrayPos].Pos = newPosUp;
                        Console.WriteLine("Second value: {0}", listOfHouses[arrayPos].Pos.Y);
                        break;
                    case 's': // move mesh up and down (currently the first house is default)
                        var y2 = listOfHouses[arrayPos].Pos.Y;
                        y2 -= 1;
                        Console.WriteLine("First value: {0}", listOfHouses[arrayPos].Pos.Y);
                        Vector3 newPosDown = new Vector3(listOfHouses[arrayPos].Pos.X, y2, listOfHouses[arrayPos].Pos.Z);
                        listOfHouses[arrayPos].Pos = newPosDown;
                        Console.WriteLine("Second value: {0}", listOfHouses[arrayPos].Pos.Y);
                        break;  
                    case 'e':
                        arrayPos += 1;
                        break;
                    case 'q':
                        arrayPos -= 1;
                        break;

				}
			};
		}

		public Vector3 getHitPoint()
		{
			return hitPoint;
		}

	}
}

