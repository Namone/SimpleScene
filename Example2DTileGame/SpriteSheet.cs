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
        float tileWidth, tileHeight;

        public SpriteSheet(int numPixelsWide,int numPixelsHigh) {
            this.tileWidth = numPixelsWide;
            this.tileHeight = numPixelsHigh;
        }

        public RectangleF getTileBounds(int tileID) {

            // My UV coordinates
            float GLX = tileID / tileWidth; // Percentage of the width
            float GLY = tileID / tileHeight; // Percentage of the height

            float boundX = GLX * 2; // Move 'box' over to other tiles based on ID...
            float boundY = GLY * 2;
            float boxW = 1f / tileWidth; // 100% / width
            float boxH = 1f / tileHeight; // 100% / height

            PointF xyLocation = new PointF (boundX, boundY);
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

    }
}
