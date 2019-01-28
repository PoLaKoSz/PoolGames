using System;

namespace CSharpSnookerCore.Models
{
    public class Pocket
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int HotSpotX { get; set; }
        public int HotSpotY { get; set; }



        public Pocket(int id, int x, int y, int hotSpotX, int hotSpotY)
        {
            Id = id;
            X = (int)(x + Ball.Radius + 4);
            Y = (int)(y + Ball.Radius + 4);
            HotSpotX = hotSpotX;
            HotSpotY = hotSpotY;
        }
    }
}
