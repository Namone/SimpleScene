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

            float boundW = GLX * 2;
            float boundH = GLY * 2;

            RectangleF bounds = new RectangleF(GLX, GLY, boundW, boundH);

            return bounds;
        }

    }
}
