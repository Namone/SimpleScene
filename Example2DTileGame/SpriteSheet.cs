using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenTK;

namespace Example2DTileGame
{
    public class SpriteSheet
    {
        float gridWidth, gridHeight;
        float GLX;
        float GLY;

        public SpriteSheet(int numOfTilesX,int numOfTilesY) {
            this.gridWidth = numOfTilesX;
            this.gridHeight = numOfTilesY;
        }

        public SpriteSheet(float tileWidth, float tileHeight) {

        }

        public RectangleF getTileBounds(int xID, int yID) {

            // My UV coordinates
            GLX = xID / gridWidth; // Percentage of the width
            GLY = yID / gridHeight; // Percentage of the height


            float boundX = GLX * 2; // Move 'box' over to other tiles based on ID...
            float boundY = GLY * 2;
            float boxW = 1f / gridWidth; // 100% / width
            float boxH = 1f / gridHeight; // 100% / height

            PointF xyLocation = new PointF (boundX, boundY);
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

        public Vector2 getUVCoordinates() {
            return new Vector2(GLX, GLY);
        }

    }
}
