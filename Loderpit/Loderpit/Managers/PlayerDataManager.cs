﻿using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Managers
{
    public class PlayerDataManager
    {
        private static int _lastLoadedLevelUid;
        private static string _storageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Loderpit");

        public static int lastLoadedLevelUid { get { return _lastLoadedLevelUid; } }

        // Get unused player uid
        public static int getUnusedPlayerUid()
        {

            // Return 0 if storage directory doesn't exist yet
            if (!Directory.Exists(_storageDirectory))
            {
                return 0;
            }

            // Otherwise, check for existing files
            for (int i = 0; i < int.MaxValue; i++)
            {
                string fileName = "player_" + i.ToString() + ".xml";
                string fullPath = Path.Combine(_storageDirectory, fileName);

                if (!File.Exists(fullPath))
                {
                    return i;
                }
            }

            Debug.Assert(false, "Exceeded maximum amount of save slots.");
            return -1;
        }

        // Save player data
        public static void savePlayerData(int uid)
        {
            XDocument document = new XDocument();
            XElement playerData = new XElement("Player",
                new XAttribute("uid", uid),
                new XAttribute("xp", SystemManager.teamSystem.currentXp),
                new XAttribute("skill_orbs", SystemManager.teamSystem.skillOrbs));
            GroupComponent groupComponent = SystemManager.teamSystem.playerGroup;

            // Characters
            for (int i = 0; i < groupComponent.entities.Count; i++)
            {
                int entityId = groupComponent.entities[i];
                XElement characterData = new XElement("Character", new XAttribute("group_slot", i));
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                // Character class data
                characterData.Add(new XAttribute("class", characterComponent.characterClass));

                // Skill data
                foreach (Skill skill in skillsComponent.skills)
                {
                    XElement skillData = new XElement("Skill",
                        new XAttribute("type", skill.type),
                        new XAttribute("level", skill.level));

                    characterData.Add(skillData);
                }

                playerData.Add(characterData);
            }

            // TODO: Level
            // ...

            document.Add(playerData);
            saveDocument(uid, document);
        }

        // Save xml document to the correct location, depending on platform
        // TODO: Make this method support other platforms
        private static void saveDocument(int uid, XDocument document)
        {
            string fullPath = Path.Combine(_storageDirectory, string.Format("player_{0}.xml", uid));

            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            document.Save(fullPath);
        }

        // Load and create entities from saved data.
        public static void loadPlayerData(int playerUid)
        {
            string fullPath = Path.Combine(_storageDirectory, string.Format("player_{0}.xml", playerUid));
            XDocument document = XDocument.Load(fullPath);
            XElement playerData = document.Element("Player");
            int playerGroupId = EntityFactory.createPlayerGroup(document);

            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(playerGroupId);
            SystemManager.teamSystem.currentXp = int.Parse(playerData.Attribute("xp").Value);
            SystemManager.teamSystem.skillOrbs = int.Parse(playerData.Attribute("skill_orbs").Value);
        }

        // Get a list of all saved player data
        public static List<XElement> getAllPlayerData()
        {
            string[] files = Directory.GetFiles(_storageDirectory, "*.xml");
            List<XElement> results = new List<XElement>();

            foreach (string filePath in files)
            {
                XDocument document = XDocument.Load(filePath);

                results.Add(document.Element("Player"));
            }

            return results;
        }
    }
}
