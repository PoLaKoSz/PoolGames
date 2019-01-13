using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace CSharpSnookerCore.Model
{
    [Serializable]
    public class Player
    {
        #region attributes
        int id = 0;
        Ball ballOn = null;
        string name = "";
        int points = 0;
        List<int> foulList = new List<int>();
        int strength = 50;
        bool justSwapped = true;
        int shotCount = 0;
        Vector2D testPosition = new Vector2D(0, 0);
        int testStrenght = 0;
        #endregion attributes

        #region constructor
        public Player() { }

        public Player(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        #endregion constructor

        #region properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [XmlIgnore]
        public Ball BallOn
        {
            get { return ballOn; }
            set { ballOn = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        [XmlIgnore]
        public List<int> FoulList
        {
            get { return foulList; }
            set { foulList = value; }
        }

        public int Strength
        {
            get { return strength; }
            set { strength = value; }
        }

        public bool JustSwapped
        {
            get { return justSwapped; }
            set { justSwapped = value; }
        }

        public int ShotCount
        {
            get { return shotCount; }
            set { shotCount = value; }
        }

        public Vector2D TestPosition
        {
            get { return testPosition; }
            set { testPosition = value; }
        }

        public int TestStrength
        {
            get { return testStrenght; }
            set { testStrenght = value; }
        }
        #endregion properties
    }
}
