using CSharpSnooker.WinForms.Components;
using System.Collections.Generic;

namespace CSharpSnookerCore.Models
{
    public class Player
    {
        public int Id { get; set; }
        public Ball BallOn { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public List<int> FoulList { get; set; }
        public int Strength { get; set; }
        public int ShotCount { get; set; }
        public Vector2D TestPosition { get; set; }
        public int TestStrength { get; set; }
        public bool IsComputer { get; }



        public Player(int id, string name, bool isComputer = false)
        {
            Id = id;
            Name = name;
            FoulList = new List<int>();
            Strength = 50;
            TestPosition = new Vector2D(0, 0);
            IsComputer = isComputer;
        }



        public override string ToString()
        {
            return Name;
        }
    }
}
