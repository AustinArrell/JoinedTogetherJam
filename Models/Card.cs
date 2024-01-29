using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace monogameTEST.Models
{
    public class Card
    {
        public Rectangle cardArea { get; set; } = new Rectangle();
        public Point Location { get; set; } = new Point();
        public Point Size { get; set; } = new Point(187, 262);

        public string Type { get; set; }
        public string Owner { get; set; }
        public bool isHovered { get; set; } = false;
        public bool isHeld { get; set; } = false;
    }
}
