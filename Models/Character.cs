using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameTEST.Models
{
    public class Character
    {
        public int Health { get; set; }
        public string Type { get; set; }
        public int Shield { get; set; }

        public int MaxHealth { get; set; }
        public int MaxShield { get; set; }

        public float HealthBarSize { get; set; } = 1;
        public float ShieldBarSize { get; set; } = 1;

        public void TakeDamage(int damageAmount) 
        {
            for (int i = 0; i < damageAmount; i++) 
            {
                if (Shield > 0)
                    Shield -= 1;
                else
                    Health -= 1;
            }
        }


    }
}
