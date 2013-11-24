using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using Loderpit.Managers;
using Loderpit.Components;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class RenderSystem : ISystem
    {
        private const int MAX_HP_BARS = 1000;
        private const float HP_BAR_WIDTH = 30f;
        private const float HP_BAR_HEIGHT = 4f;
        private DebugView _debugView;
        private CircleShape _throwRopeShape;
        private RectangleShape _buildBridgeShape;
        private RectangleShape _reticleShape;
        private Font _font;
        private Text _actionLabel;
        private RectangleShape[] _hpBarBackgrounds;
        private RectangleShape[] _hpBarForegrounds;
        private int _usedHpBarCount;

        public SystemType systemType { get { return SystemType.Render; } }

        public RenderSystem()
        {
            _debugView = new DebugView();

            _font = ResourceManager.getResource<Font>("font");

            _actionLabel = new Text("", _font, 14);
            _actionLabel.Color = Color.White;
            
            _throwRopeShape = new CircleShape(1f);
            _throwRopeShape.Origin = new Vector2f(1f, 1f);
            _throwRopeShape.FillColor = new Color(0, 255, 0, 128);

            _buildBridgeShape = new RectangleShape(new Vector2f(0.2f, 0f));
            _buildBridgeShape.FillColor = new Color(255, 255, 0, 128);
            _buildBridgeShape.Origin = new Vector2f(0.1f, 0f);

            _hpBarBackgrounds = new RectangleShape[MAX_HP_BARS];
            _hpBarForegrounds = new RectangleShape[MAX_HP_BARS];

            for (int i = 0; i < MAX_HP_BARS; i++)
            {
                RectangleShape background = new RectangleShape();
                RectangleShape foreground = new RectangleShape();

                background.Size = new Vector2f(HP_BAR_WIDTH + 2f, HP_BAR_HEIGHT + 2f);
                background.Origin = background.Size * 0.5f;
                background.FillColor = Color.Black;

                foreground.Size = new Vector2f(HP_BAR_WIDTH, HP_BAR_HEIGHT);
                foreground.Origin = foreground.Size * 0.5f;
                foreground.FillColor = Color.Green;

                _hpBarBackgrounds[i] = background;
                _hpBarForegrounds[i] = foreground;
            }

            _reticleShape = new RectangleShape();
            _reticleShape.Texture = ResourceManager.getResource<Texture>("reticle");
            _reticleShape.Size = new Vector2f(_reticleShape.Texture.Size.X, _reticleShape.Texture.Size.Y) / 35f;
            _reticleShape.Origin = _reticleShape.Size * 0.5f;
        }

        // Prepare hp bars
        private void prepareHpBars(List<int> entities)
        {
            _usedHpBarCount = 0;

            foreach (int entityId in entities)
            {
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
                float percentHp = (float)statsComponent.currentHp / (float)SystemManager.statSystem.getMaxHp(entityId);
                Vector2f worldPosition = new Vector2f(positionComponent.position.X, positionComponent.position.Y);
                Vector2i screenPosition = Game.window.MapCoordsToPixel(worldPosition, SystemManager.cameraSystem.worldView);
                Vector2f screenPositionF = new Vector2f(screenPosition.X, screenPosition.Y) + new Vector2f(0, 28f);

                _hpBarBackgrounds[_usedHpBarCount].Position = screenPositionF;
                _hpBarForegrounds[_usedHpBarCount].Position = screenPositionF;
                _hpBarForegrounds[_usedHpBarCount++].Size = new Vector2f(HP_BAR_WIDTH * percentHp, HP_BAR_HEIGHT);
            }
        }

        // Draw actions currently being performed
        private void drawCurrentActions()
        {
            TeamSystem teamSystem = SystemManager.teamSystem;

            // Nothing to draw
            if (teamSystem.initializingSkill == null)
            {
                return;
            }

            _actionLabel.Position = Game.screenMouse + new Vector2f(16f, 0);

            if (teamSystem.initializingSkill.type == SkillType.ThrowRope)
            {
                _throwRopeShape.Position = new Vector2f(teamSystem.createRopeAnchor.X, teamSystem.createRopeAnchor.Y);
                _actionLabel.Position = Game.screenMouse + new Vector2f(16f, 0);
                _actionLabel.DisplayedString = "Throw Rope";
                Game.window.Draw(_throwRopeShape);
            }
            else if (teamSystem.initializingSkill.type == SkillType.BuildBridge)
            {
                Vector2 pointA = teamSystem.createBridgeAnchorA;
                Vector2 pointB = teamSystem.createBridgeAnchorB;
                Vector2 relative = pointB - pointA;
                float length = relative.Length();
                float angle = Helpers.radToDeg((float)Math.Atan2(relative.Y, relative.X));

                _buildBridgeShape.Position = new Vector2f(pointA.X, pointA.Y);
                _buildBridgeShape.Rotation = angle;
                _buildBridgeShape.Size = new Vector2f(0.2f, length);
                _actionLabel.DisplayedString = "Build Bridge";
                Game.window.Draw(_buildBridgeShape);
            }
            else if (teamSystem.initializingSkill.type == SkillType.MeleeAttack || teamSystem.initializingSkill.type == SkillType.RangedAttack)
            {
                _actionLabel.DisplayedString = "Attack";
            }
            else if (teamSystem.initializingSkill.type == SkillType.PowerShot)
            {
                _reticleShape.Position = Game.sfmlWorldMouse;
                _actionLabel.DisplayedString = "Power Shot";
                Game.window.Draw(_reticleShape);
            }
        }

        public void update()
        {
            List<int> renderHealthEntities = EntityManager.getEntitiesPossessing(ComponentType.RenderHealth);

            // Prepare to draw hp bars
            prepareHpBars(renderHealthEntities);
        }

        public void draw()
        {
            // Draw physical world (debug view)
            _debugView.draw();

            // Draw actions currently being performed
            drawCurrentActions();
        }

        // A seperate draw method for after the window has been switched back to the default screen view (not world coordinates)
        public void drawUsingScreenCoords()
        {
            // Draw hp bars
            for (int i = 0; i < _usedHpBarCount; i++)
            {
                Game.window.Draw(_hpBarBackgrounds[i]);
                Game.window.Draw(_hpBarForegrounds[i]);
            }

            // Draw action label
            if (SystemManager.teamSystem.initializingSkill != null)
            {
                Game.window.Draw(_actionLabel);
            }
        }
    }
}
