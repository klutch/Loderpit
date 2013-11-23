using System;
using System.Collections.Generic;
using SFML.Window;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    using Key = Keyboard.Key;

    public class InterLevelScreen : Screen
    {
        public InterLevelScreen()
            : base(ScreenType.InterLevel)
        {
        }

        public override void loadContent()
        {
        }

        public override void initialize()
        {
        }

        public override void update()
        {
            if (Game.inFocus)
            {
                if (Game.newKeyState.isPressed(Key.Return) && Game.oldKeyState.isReleased(Key.Return))
                {
                    List<CharacterClass> characterClasses = new List<CharacterClass>();
                    GroupComponent groupComponent = SystemManager.teamSystem.playerGroup;

                    foreach (int entityId in groupComponent.entities)
                    {
                        characterClasses.Add(EntityManager.getCharacterComponent(entityId).characterClass);
                    }

                    Game.endInterLevelState();
                    Game.startLevelState(characterClasses);
                }
            }

            base.update();
        }

        public override void draw()
        {
            base.draw();
        }
    }
}
