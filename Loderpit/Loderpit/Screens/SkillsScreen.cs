using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Loderpit.Screens
{
    public class SkillsScreen : Screen
    {
        private InterLevelScreen _interLevelScreen;
        private RectangleShape _background;

        public SkillsScreen(InterLevelScreen interLevelScreen)
            : base(ScreenType.Skills)
        {
            _interLevelScreen = interLevelScreen;
        }

        public override void initialize()
        {
        }

        public override void loadContent()
        {
            _background = new RectangleShape();
            _background.FillColor = new Color(0, 0, 0, 128);
            _background.Size = new Vector2f(Game.window.GetView().Size.X, Game.window.GetView().Size.Y);
        }

        public override void update()
        {
            base.update();

            if (Game.newKeyState.isPressed(Keyboard.Key.Escape) && Game.oldKeyState.isReleased(Keyboard.Key.Escape))
            {
                _interLevelScreen.closeSkillsMenu();
            }
        }

        public override void draw()
        {
            Game.window.Draw(_background);

            base.draw();
        }
    }
}
