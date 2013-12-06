using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Components.SpellEffects;
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

        // Attack die
        public string getAttackDie(int entityId)
        {
            int attackDieModifier = 0;
            string attackDie = "d20";
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);

            // Accumulate modifiers from spells
            foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
            {
                StatModifierComponent statModiferComponent = EntityManager.getStatModifierComponent(spellId);

                // Skip spells that don't modify stats
                if (statModiferComponent == null)
                {
                    continue;
                }

                attackDieModifier += statModiferComponent.attackDieMod;
            }

            // TODO: accumulate modifiers from equipment
            // ...

            // Modify attack die
            if (attackDieModifier != 0)
            {
                attackDie += (attackDieModifier > 0 ? "+" : "-") + attackDieModifier.ToString();
            }

            return attackDie;
        }

        // Hit die
        public string getHitDie(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);

            // TODO: Get hit die from weapons
            return statsComponent.unarmedHitDie;
        }

        public void update()
        {
        }
    }
}
