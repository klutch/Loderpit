using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    using Key = Keyboard.Key;

    public class InterLevelScreen : Screen
    {
        private int _playerUid;
        private Texture _bigButtonTexture;

        public int playerUid { get { return _playerUid; } set { _playerUid = value; } }

        public InterLevelScreen()
            : base(ScreenType.InterLevel)
        {
        }

        public override void initialize()
        {
        }

        public override void loadContent()
        {
            _bigButtonTexture = new Texture("resources/ui/inter_level_screen/button.png");

            addScreenComponent(
                new BigLabeledButtonComponent(
                    this,
                    _bigButtonTexture,
                    new Vector2f(32, 32),
                    "Skills",
                    new Color(10, 180, 10, 255),
                    () => { Console.WriteLine("open skills menu"); }));

            addScreenComponent(
                new BigLabeledButtonComponent(
                    this,
                    _bigButtonTexture,
                    new Vector2f(32, 164),
                    "Group",
                    new Color(10, 10, 180, 255),
                    () => { Console.WriteLine("open group menu"); }));

            addScreenComponent(
                new BigLabeledButtonComponent(
                    this,
                    _bigButtonTexture,
                    new Vector2f(32, 296),
                    "Continue",
                    new Color(180, 10, 10, 255),
                    () => { continueGame(); }));
        }

        // Continue game
        private void continueGame()
        {
            Game.endInterLevelState();
            Game.startLevelState(PlayerDataManager.lastLoadedLevelUid);
        }

        // Update
        public override void update()
        {
            if (Game.inFocus)
            {
                // TEMPORARY: Check for enter key to start a new level
                if (Game.newKeyState.isPressed(Key.Return) && Game.oldKeyState.isReleased(Key.Return))
                {
                    continueGame();
                }

                // Test mouse input
                foreach (ScreenComponent screenComponent in _screenComponents)
                {
                    BigLabeledButtonComponent labelComponent = screenComponent as BigLabeledButtonComponent;

                    if (labelComponent.testPoint(new Vector2f(Game.newMouseState.position.X, Game.newMouseState.position.Y)))
                    {
                        labelComponent.selected = true;

                        if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
                        {
                            labelComponent.onClick();
                        }
                    }
                    else
                    {
                        labelComponent.selected = false;
                    }
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
