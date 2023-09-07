using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Minesweeper
{
    public class Tile
    {
        bool hasMine;
        int mineColour;
        bool isUncovered;
        bool hasFlag;
        int adjacentMines;
        Rectangle position;
         

        public Tile(Rectangle position)
        {
            this.position = position;

        }


        public bool GetIfMine()
        {
            return hasMine;
        }


        public int GetMineColour()
        {
            return mineColour;
        }


        public bool GetIfUncovered()
        {
            return isUncovered;
        }


        public bool GetIfFlag()
        {
            return hasFlag;
        }


        public int GetAdjacentMines()
        {
            return adjacentMines;
        }


        public Rectangle GetPosition()
        {
            return position;
        }


        public void SetToMine(bool hasMine)
        {
            this.hasMine = hasMine;
        }


        public void SetMineColour(int mineColour)
        {
            this.mineColour = mineColour;
        }


        public void SetIfUncovered(bool isUncovered)
        {
            this.isUncovered = isUncovered;
        }


        public void SetFlag(bool hasFlag)
        {
            this.hasFlag = hasFlag;
        }


        public void SetAdjacentMines(int adjacentMines)
        {
            this.adjacentMines = adjacentMines;
        }


        public void SetPosition(Rectangle position)
        {
            this.position.Width = position.Width;
            this.position.Height = position.Height;

        }


    }
}
