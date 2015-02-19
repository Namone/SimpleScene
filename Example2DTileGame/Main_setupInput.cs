// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;

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
						// we hit the map mesh! place an object there
					
						Vector3 hitPoint = ray.pos - (ray.dir.Normalized() * (distance - 0.01f));
						var obj = new SSObjectCube();
						obj.Pos = hitPoint;
						scene.AddObject(obj);
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

			this.KeyPress += (object sender, KeyPressEventArgs e) => {
				switch (e.KeyChar) {
				}
			};
		}

	}
}

