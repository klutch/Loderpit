using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Systems;
using Loderpit.Managers;

namespace Loderpit.Backgrounds
{
    public enum BackgroundType
    {
        Cave
    }

    public class Background
    {
        private const int MAX_LAYERS = 10;
        private BackgroundType _type;
        private RectangleShape[] _shapes;
        private Texture[] _textures;
        private Vector2f[] _movementFactors;
        private Vector2f _basePosition;
        private int _numLayers;

        public BackgroundType type { get { return _type; } }
        public Vector2f basePosition { get { return _basePosition; } set { _basePosition = value; } }
        public int numLayers { get { return _numLayers; } }
        public RectangleShape[] shapes { get { return _shapes; } }

        public Background(BackgroundType type)
        {
            _type = type;
            _textures = new Texture[MAX_LAYERS];
            _movementFactors = new Vector2f[MAX_LAYERS];
            _shapes = new RectangleShape[MAX_LAYERS];
            for (int i = 0; i < MAX_LAYERS; i++)
            {
                _shapes[i] = new RectangleShape();
            }
        }

        public void addLayer(Texture texture, Vector2f movementFactor)
        {
            _textures[_numLayers] = texture;
            _movementFactors[_numLayers] = movementFactor;
            _shapes[_numLayers].Texture = texture;
            _shapes[_numLayers].Size = new Vector2f(texture.Size.X, texture.Size.Y) / CameraSystem.ORIGINAL_SCALE;
            _shapes[_numLayers].Origin = _shapes[_numLayers].Size * 0.5f;
            _numLayers++;
        }

        public void update()
        {
            Vector2f center = SystemManager.cameraSystem.worldView.Center;
            Vector2f relative = _basePosition - center;

            for (int i = 0; i < _numLayers; i++)
            {
                Vector2f movementFactor = _movementFactors[i];

                _shapes[i].Position = center + new Vector2f(relative.X * movementFactor.X, relative.Y * movementFactor.Y);
            }
        }
    }
}
