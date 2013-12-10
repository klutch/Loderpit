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

        // Strength
        public int getStrength(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntities = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int strength = statsComponent.baseStrength;

            // Accumulate strength modifiers from spells
            foreach (int spellId in affectedBySpellEntities.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    strength += statModifierComponent.strengthMod;
                }
            }

            return strength;
        }

        // Dexterity
        public int getDexterity(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntities = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int dexterity = statsComponent.baseDexterity;

            // Accumulate dexterity modifiers from spells
            foreach (int spellId in affectedBySpellEntities.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    dexterity += statModifierComponent.dexterityMod;
                }
            }

            return dexterity;
        }

        // Intelligence
        public int getIntelligence(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntities = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int intelligence = statsComponent.baseIntelligence;

            // Accumulate dexterity modifiers from spells
            foreach (int spellId in affectedBySpellEntities.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    intelligence += statModifierComponent.intelligenceMod;
                }
            }

            return intelligence;
        }

        // Max hp
        public int getMaxHp(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntities = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int maxHp = statsComponent.baseHp;

            // Accumulate spell max hp modifiers
            foreach (int spellId in affectedBySpellEntities.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    maxHp += statModifierComponent.maxHpMod;
                }
            }

            return maxHp;
        }

        // Ability modifier
        public int getStatModifier(int value)
        {
            return (int)Math.Floor((float)(value - 10) / 2f);
        }

        // Armor class: 10 + armor bonus + dexterity modifier
        public int getArmorClass(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int armorClass = 10 + getStatModifier(statsComponent.baseDexterity);

            // TODO: Calculate armor bonus from items

            // Accumulate armor class modifier from spells
            foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    armorClass += statModifierComponent.armorClassMod;
                }
            }

            return armorClass;
        }

        // Get attack delay
        public int getAttackDelay(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int totalAttackDelay = statsComponent.baseAttackDelay;

            // Accumulate changes from spells
            foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    totalAttackDelay += statModifierComponent.attackDelayMod;
                }
            }

            // TODO: Accumulate changes from items
            // ...

            return Math.Max(0, totalAttackDelay);
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

        // Damage die
        public string getDamageDie(int entityId)
        {
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            string damageDie = statsComponent.unarmedHitDie; // TODO: Get hit die from weapon
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
            int damageDieModifier = 0;

            foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
            {
                StatModifierComponent statModifierComponent = EntityManager.getStatModifierComponent(spellId);

                if (statModifierComponent != null)
                {
                    damageDieModifier += statModifierComponent.damageDieMod;
                }
            }

            // Modify damage die
            if (damageDieModifier != 0)
            {
                damageDie += (damageDieModifier > 0 ? "+" : "-") + damageDieModifier.ToString();
            }

            return damageDie;
        }

        public void update()
        {
        }
    }
}
