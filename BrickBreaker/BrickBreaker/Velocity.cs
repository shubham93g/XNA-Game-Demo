using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BrickBreaker
{
    /// <summary>
    /// A simple velocity class with X & Y speed components stored as a Vector2
    /// </summary>
    class Velocity
    {
        Vector2 speed;

        public Velocity(Vector2 speed)
        {
            this.speed = speed;
        }

        public Vector2 get()
        {
            return speed;
        }

        public void reverseX()
        {
            speed.X = speed.X * -1;
        }

        public void reverseY()
        {
            speed.Y = speed.Y * -1;
        }

    }
}
