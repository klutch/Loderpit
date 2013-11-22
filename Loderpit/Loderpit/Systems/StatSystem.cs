using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class StatSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Stat; } }

        public StatSystem()
        {
        }

        public int getMaxHp(int entityId) { return getMaxHp(EntityManager.getStatsComponent(entityId)); }
        public int getMaxHp(StatsComponent vitalsComponent)
        {
            return vitalsComponent.baseHp + getStatModifier(vitalsComponent.dexterity);
        }

        public int getStatModifier(int value)
        {
            return (int)Math.Floor((float)(value - 10) / 2f);
        }

        // Armor class: 10 + armor bonus + dexterity modifier
        public int getArmorClass(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);

            // TODO: Calculate armor bonus from items
            return 10 + getStatModifier(statsComponent.dexterity);
        }

        public void update()
        {
        }
    }
}
