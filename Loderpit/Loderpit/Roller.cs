using System;
using System.Collections.Generic;
using DiceNotation;

namespace Loderpit
{
    public class Roller
    {
        public static int roll(string expression)
        {
            return Dice.Parse(expression).Roll().Value;
        }
    }
}
