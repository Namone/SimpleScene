using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Example2DTileGame
{
    public class SpriteSheet
    {
        int gridWidth,gridHeight;

        public SpriteSheet(int numTilesWide,int numTilesHigh) {
            this.gridWidth = numTilesWide;
            this.gridHeight = numTilesHigh;
        }

        public RectangleF getTileBounds(int tileID) {

            // TODO: implement this
            float xGLCoordinate = tileID / 12;
            float yGLCoordinate = 0; // I'm not too worried about the Y direction right now..

            float tileWidth = 1 / gridWidth;
            float tileHeight = 1 / gridHeight;
            // given the tileID, calculate the opengl 0.0-1.0 coordinate space location of the bounds
            // of that tile inside the sprite sheet

            RectangleF bounds = new RectangleF(xGLCoordinate, yGLCoordinate, tileWidth, tileHeight);

            return bounds;
        }

    }
}
