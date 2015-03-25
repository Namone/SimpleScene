﻿using System;
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
        float gridWidth, gridHeight; // 12 x 12 - size of sheet in grid units
        float tilePixelW, tilePixelH; // 48 x 48 - size of each tile in grid (by pixel)
        float sheetPixelW, sheetPixelH; // 48 * 12 - total pixel size of the sheet
        int tileID;

        /// <summary>
        /// For determing tileID based off of tile pixel height and width - works with getTileById(id)
        /// </summary>
        /// <param name="tilePixelWidth"></param>
        /// <param name="tilePixelHeight"></param>
        public SpriteSheet(int tilePixelWidth, int tilePixelHeight, int gridW, int gridH) {
            this.tilePixelW = tilePixelWidth; 
            this.tilePixelH = tilePixelHeight;
            this.gridWidth = gridW;
            this.gridHeight = gridH;

            // Get total pixels (ignoring borders, for now)
            sheetPixelW = tilePixelW * gridWidth;
            sheetPixelH = tilePixelH * gridHeight;
        }

        /// <summary>
        /// Get tile by ID (0 - ?)
        /// </summary>
        /// <param name="textureID"></param>
        /// <returns></returns>
        public RectangleF getTileByPixel(int textureID) {

            tileID = textureID;
            int columnNumber = (int)(textureID % gridWidth); // Get column number texture is in
            int rowNumber = (int)(textureID / gridHeight); // Get row number texture is in

            int boundX = 5 + (int)(columnNumber * (tilePixelW + 3)); // UV coordinates
            int boundY = 5 + (int)(rowNumber * (tilePixelH + 3));
            float boxW = (tilePixelW - 2); // Width of 'box' which contains our texture segment
            float boxH = (tilePixelH - 2);

            PointF xyLocation = new PointF(boundX, boundY);
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

        /// <summary>
        /// Calculate, based on input xID and yID, what texture to use
        /// </summary>
        /// <returns></returns>
        public RectangleF getTileByGrid(int textureID) {

            int columnNumber = (int)(textureID % gridWidth); // Get column number texture is in
            int rowNumber = (int)(textureID / gridHeight); // Get row number texture is in

            if (false) {

                float boxX = (columnNumber / gridWidth); // Using the column number, calculate UV coordinate for X axis
                float boxY = (rowNumber / gridHeight); // Using the row number, calculate the UV coordinate for the Y axis
                float boxW = (1f / gridWidth); // 100% / width
                float boxH = (1f / gridHeight); // 100% / height

                PointF xyLocation = new PointF(boxX, boxY); // These end up being my UV coordinates
                SizeF boxSize = new SizeF(boxW, boxH);

                RectangleF bounds = new RectangleF(xyLocation, boxSize);

                return bounds;

            } else {
                RectangleF bounds = getTileByPixel(textureID);

                // Convert to OpenGL coordinate space

                PointF xyLocation = new PointF(bounds.X / sheetPixelW, bounds.Y / sheetPixelH); // These end up being my UV coordinates
                SizeF boxSize = new SizeF(bounds.Width / sheetPixelW, bounds.Height / sheetPixelH);

                RectangleF boundsFloat = new RectangleF(xyLocation, boxSize);;
                Console.WriteLine("TileGrid {0} ({1},{2},w{3} h{4})", textureID, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                Console.WriteLine("TileFloat {0} ({1},{2},w{3} h{4})", textureID, boundsFloat.X, boundsFloat.Y, boundsFloat.Width, boundsFloat.Height);
                return boundsFloat;
            }

            // TODO - Save texture ID's assigned so they can be loaded from a file
        }

        public int getTileID() {
            return tileID;
        }

    }
}
