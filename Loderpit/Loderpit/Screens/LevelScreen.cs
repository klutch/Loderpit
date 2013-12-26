using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Formations;
using Loderpit.Managers;
using Loderpit.Systems;

namespace Loderpit.Screens
{
    public class LevelScreen : Screen
    {
        private Texture _teammateSelectionIndicator;
        private Font _font;

        public LevelScreen()
            : base(ScreenType.Level)
        {
        }

        public override void loadContent()
        {
            _teammateSelectionIndicator = new Texture("resources/ui/teammate_selection_indicator.png");
            _font = ResourceManager.getResource<Font>("gooddog_font");
        }

        public override void initialize()
        {
            addScreenComponent(new SelectedCharacterComponent(this, _teammateSelectionIndicator));
            for (int i = 0; i < SystemManager.teamSystem.playerGroup.entities.Count; i++)
            {
                addScreenComponent(new CharacterPaneComponent(this, _font, i));
                addScreenComponent(new CharacterStatusComponent(this, _font, SystemManager.teamSystem.getTeammateEntityId(i), new Vector2f(0, 16)));
            }
        }

        public void addTemporaryWorldText(string value, Vector2 position)
        {
            addScreenComponent(new TemporaryWorldTextComponent(this, _font, value, position));
        }

        public override void update()
        {
            foreach (ScreenComponent component in screenComponents)
            {
                if (component is TemporaryWorldTextComponent)
                {
                    TemporaryWorldTextComponent temporaryWorldTextComponent = (TemporaryWorldTextComponent)component;

                    if (temporaryWorldTextComponent.delay <= 0)
                    {
                        removeScreenComponent(temporaryWorldTextComponent);
                    }
                }
            }

            base.update();
        }
    }
}
