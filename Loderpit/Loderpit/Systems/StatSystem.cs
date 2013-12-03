using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.SpellEffects;

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
            List<SpellEffect> spellEffects = SystemManager.spellEffectSystem.getSpellEffectsAffecting(entityId);
            string attackDie = "d20";

            // Accumulate spell effect modifiers
            foreach (SpellEffect spellEffect in spellEffects)
            {
                if (spellEffect.type == SpellEffectType.StatBuff)
                {
                    attackDieModifier += (spellEffect as StatBuffSpellEffect).attackRollModifier;
                }
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
