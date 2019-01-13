using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CSharpSnookerCore.Model
{
    [Serializable]
    //[XmlRoot("BallPosition")]
    public class BallPosition
    {
        #region attributes
        int snapShot = 0;
        int ballIndex = 0;
        int x = 0;
        int y = 0;
        #endregion attributes

        #region constructor
        public BallPosition() { }

        public BallPosition ( int snapShot, int ballIndex, int x, int y )
        {
            this.snapShot = snapShot;
            this.ballIndex = ballIndex;
            this.x = x;
            this.y = y;
        }
        #endregion constructor

        #region functions

        //[XmlAttribute("SnapShot")]
        public int SnapShot { get { return snapShot; } set { snapShot = value; } }
        //[XmlAttribute("BallIndex")]
        public int BallIndex { get { return ballIndex; } set { ballIndex = value; } }
        //[XmlAttribute("X")]
        public int X { get { return x; } set { x = value; } }
        //[XmlAttribute("Y")]
        public int Y { get { return y; } set { y = value; } }

        #endregion functions
    }
}
