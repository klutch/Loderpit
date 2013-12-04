using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.SpellEffects
{
    public interface IDoTSpellEffect
    {
        string damageDie { get; }
        int baseDelay { get; }
        int currentDelay { get; set; }
        void onTick(int ownerId, int receiverId);
    }
}
