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

            // My UV coordinates
            float GLX = tileID / gridWidth; 
            float GLY = tileID / gridHeight;

            float boundX = GLX * 2;
            float boundY = GLY * 2;

            RectangleF bounds = new RectangleF(GLX, GLY, boundX, boundY);

            return bounds;
        }

    }
}
