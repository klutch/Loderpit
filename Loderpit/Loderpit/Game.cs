﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Loderpit.Formations;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Systems;
using Loderpit.Screens;

namespace Loderpit
{
    using Key = Keyboard.Key;

    public class Game : IDisposable
    {
        public const float DT = 1f / 60f;
        private static RenderWindow _window;
        public static bool inFocus = true;
        public static KeyboardState newKeyState;
        public static KeyboardState oldKeyState;
        public static MouseState newMouseState;
        public static MouseState oldMouseState;
        public static Vector2 worldMouse;
        private Stopwatch _stopwatch;
        private int _fps;
        private Text _fpsText;
        private FixedMouseJoint _mouseJoint;

        public static RenderWindow window { get { return _window; } }

        public Game()
        {
            _window = new RenderWindow(new VideoMode(1280, 720), "Loderpit");
            _stopwatch = new Stopwatch();
            _window.Closed += new EventHandler(_window_Closed);
            _window.GainedFocus += new EventHandler(_window_GainedFocus);
            _window.LostFocus += new EventHandler(_window_LostFocus);
            _window.SetVerticalSyncEnabled(true);

            // Create systems
            SystemManager.physicsSystem = new PhysicsSystem();
            SystemManager.statSystem = new StatSystem();
            SystemManager.levelSystem = new LevelSystem();
            SystemManager.teamSystem = new TeamSystem();
            SystemManager.cameraSystem = new CameraSystem();
            SystemManager.characterSystem = new CharacterSystem();
            SystemManager.renderSystem = new RenderSystem();
            SystemManager.obstacleSystem = new ObstacleSystem();
            SystemManager.groupSystem = new GroupSystem();
            SystemManager.skillSystem = new SkillSystem();
            SystemManager.combatSystem = new CombatSystem();
            SystemManager.enemyAISystem = new EnemyAISystem();

            loadContent();

            // Create player group
            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(EntityFactory.createPlayerGroup(new Vector2(0f, 0f)));

            // Create screens
            ScreenManager.addScreen(new LevelScreen());
        }

        private void _window_LostFocus(object sender, EventArgs e)
        {
            inFocus = false;
        }

        private void _window_GainedFocus(object sender, EventArgs e)
        {
            inFocus = true;
        }

        private void _window_Closed(object sender, EventArgs e)
        {
            _window.Close();
        }

        public void Dispose()
        {
        }

        public void loadContent()
        {
            // Permanent/global
            ResourceManager.addResource("font", new Font("resources/courbd.ttf"));
            ResourceManager.addResource("sword_icon", new Texture("resources/ui/action_icons/sword.png"));
            ResourceManager.addResource("bow_icon", new Texture("resources/ui/action_icons/bow.png"));
            ResourceManager.addResource("wand_icon", new Texture("resources/ui/action_icons/wand.png"));
            ResourceManager.addResource("rope_icon", new Texture("resources/ui/action_icons/rope.png"));
            ResourceManager.addResource("bridge_icon", new Texture("resources/ui/action_icons/bridge.png"));

            // Debug
            _fpsText = new Text("FPS:", ResourceManager.getResource<Font>("font"), 14);
            _fpsText.Color = Color.Red;
            _fpsText.Position = new Vector2f(16, 16);
        }

        public void run()
        {
            float frameTime = 0f;
            int frameCount = 0;

            _stopwatch.Start();
            while (_window.IsOpen())
            {
                float currentTime = (float)_stopwatch.Elapsed.TotalSeconds;

                frameTime += currentTime;
                _stopwatch.Restart();

                update();
                draw();
                frameCount++;

                if (frameTime >= 1)
                {
                    _fps = frameCount;
                    frameCount = 0;
                    frameTime = 0;
                }
            }
        }

        public void update()
        {
            Vector2f sfmlWorldMouse = _window.MapPixelToCoords(_window.InternalGetMousePosition(), SystemManager.cameraSystem.worldView);
            List<int> entities = SystemManager.teamSystem.getTeamEntities();

            _window.DispatchEvents();

            oldKeyState = newKeyState;
            newKeyState = KeyboardState.get();
            oldMouseState = newMouseState;
            newMouseState = MouseState.get();
            worldMouse = new Vector2(sfmlWorldMouse.X, sfmlWorldMouse.Y);

            if (inFocus)
            {
                if (newMouseState.isLeftButtonPressed && !oldMouseState.isLeftButtonPressed)
                {
                    Fixture fixture = SystemManager.physicsSystem.world.TestPoint(worldMouse);

                    if (fixture != null)
                    {
                        _mouseJoint = JointFactory.CreateFixedMouseJoint(SystemManager.physicsSystem.world, fixture.Body, worldMouse);
                    }
                }
                else if (newMouseState.isLeftButtonPressed && oldMouseState.isLeftButtonPressed)
                {
                    if (_mouseJoint != null)
                    {
                        _mouseJoint.WorldAnchorB = worldMouse;
                    }
                }
                else if (!newMouseState.isLeftButtonPressed && oldMouseState.isLeftButtonPressed)
                {
                    if (_mouseJoint != null)
                    {
                        SystemManager.physicsSystem.world.RemoveJoint(_mouseJoint);
                        _mouseJoint = null;
                    }
                }
            }

            SystemManager.physicsSystem.update();
            SystemManager.levelSystem.update();
            SystemManager.teamSystem.update();
            SystemManager.groupSystem.update();
            SystemManager.obstacleSystem.update();
            SystemManager.cameraSystem.update();
            SystemManager.enemyAISystem.update();
            SystemManager.characterSystem.update();
            SystemManager.statSystem.update();
            SystemManager.renderSystem.update();
            SystemManager.skillSystem.update();
            SystemManager.combatSystem.update();
            ScreenManager.update();

            // FPS
            _fpsText.DisplayedString = _fps.ToString();
        }

        public void draw()
        {
            _window.Clear();

            // Switch to world view
            _window.SetView(SystemManager.cameraSystem.worldView);

            // Draw render system
            SystemManager.renderSystem.draw();

            // Restore screen view
            _window.SetView(_window.DefaultView);

            // Screens
            ScreenManager.draw();

            // FPS
            _window.Draw(_fpsText);

            _window.Display();
        }
    }
}