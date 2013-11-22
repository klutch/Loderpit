using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Loderpit.Managers;
using Loderpit.Systems;

namespace Loderpit
{
    using FCircleShape = FarseerPhysics.Collision.Shapes.CircleShape;
    using FPolygonShape = FarseerPhysics.Collision.Shapes.PolygonShape;
    using FShape = FarseerPhysics.Collision.Shapes.Shape;
    using ShapeType = FarseerPhysics.Collision.Shapes.ShapeType;
    using FTransform = FarseerPhysics.Common.Transform;

    public class DebugView
    {
        private const int MAX_SHAPES = 5000;
        private Color _staticColor;
        private Color _dynamicColor;
        private Color _staticOutlineColor;
        private Color _dynamicOutlineColor;
        private ConvexShape[] _polygons;
        private CircleShape[] _circles;
        private RectangleShape[] _lines;
        private int _numPolygons;
        private int _numCircles;
        private int _numLines;

        public DebugView()
        {
            _staticColor = new Color(100, 150, 100, 128);
            _dynamicColor = new Color(100, 100, 150, 128);
            _staticOutlineColor = new Color(_staticColor.R, _staticColor.G, _staticColor.B, 255);
            _dynamicOutlineColor = new Color(_dynamicColor.R, _dynamicColor.G, _dynamicColor.B, 255);
            _circles = new CircleShape[MAX_SHAPES];
            _polygons = new ConvexShape[MAX_SHAPES];
            _lines = new RectangleShape[MAX_SHAPES];

            for (int i = 0; i < MAX_SHAPES; i++)
            {
                _circles[i] = new CircleShape();
                _circles[i].OutlineThickness = -0.05f;
                _polygons[i] = new ConvexShape();
                _polygons[i].OutlineThickness = -0.05f;
                _lines[i] = new RectangleShape();
            }
        }

        public void DrawCircle(Vector2 center, float radius, float rotationInDegrees, ref Color fillColor, ref Color outlineColor)
        {
            CircleShape circle = _circles[_numCircles];
            RectangleShape line = _lines[_numLines];
            Vector2f position = new Vector2f(center.X, center.Y);

            circle.Radius = radius;
            circle.Position = position;
            circle.Origin = new Vector2f(radius, radius);
            circle.FillColor = fillColor;
            circle.OutlineColor = outlineColor;
            line.Size = new Vector2f(radius, 0.05f);
            line.FillColor = outlineColor;
            line.Position = position;
            line.Rotation = rotationInDegrees;

            _numCircles++;
            _numLines++;
        }

        public void DrawPolygon(Vector2[] vertices, int count, ref Color fillColor, ref Color outlineColor, bool closed = true)
        {
            ConvexShape polygon = _polygons[_numPolygons];

            polygon.SetPointCount((uint)count);
            polygon.FillColor = fillColor;
            polygon.OutlineColor = outlineColor;
            for (int i = 0; i < count; i++)
            {
                polygon.SetPoint((uint)i, new Vector2f(vertices[i].X, vertices[i].Y));
            }

            _numPolygons++;
        }

        public void draw()
        {
            foreach (Body body in SystemManager.physicsSystem.world.BodyList)
            {
                foreach (Fixture fixture in body.FixtureList)
                {
                    FShape shape = fixture.Shape;

                    if (shape.ShapeType == ShapeType.Circle)
                    {
                        FCircleShape circleShape = shape as FCircleShape;
                        //Vector2 position = body.GetWorldVector(circleShape.Position);

                        if (body.BodyType == BodyType.Static)
                        {
                            DrawCircle(body.Position, circleShape.Radius, Helpers.radToDeg(body.Rotation), ref _staticColor, ref _staticOutlineColor);
                        }
                        else
                        {
                            DrawCircle(body.Position, circleShape.Radius, Helpers.radToDeg(body.Rotation), ref _dynamicColor, ref _dynamicOutlineColor);
                        }
                    }
                    else if (shape.ShapeType == ShapeType.Polygon)
                    {
                        FPolygonShape polygonShape = shape as FPolygonShape;
                        int count = polygonShape.Vertices.Count;
                        Vector2[] vertices = new Vector2[count];
                        FTransform xf;

                        body.GetTransform(out xf);
                        for (int i = 0; i < count; i++)
                        {
                            vertices[i] = MathUtils.Mul(ref xf, polygonShape.Vertices[i]);
                        }

                        if (body.BodyType == BodyType.Static)
                        {
                            DrawPolygon(vertices, count, ref _staticColor, ref _staticOutlineColor);
                        }
                        else
                        {
                            DrawPolygon(vertices, count, ref _dynamicColor, ref _dynamicOutlineColor);
                        }
                    }
                }
            }

            for (int i = 0; i < _numCircles; i++)
            {
                Game.window.Draw(_circles[i]);
            }

            for (int i = 0; i < _numLines; i++)
            {
                Game.window.Draw(_lines[i]);
            }

            for (int i = 0; i < _numPolygons; i++)
            {
                Game.window.Draw(_polygons[i]);
            }

            reset();
        }

        public void reset()
        {
            _numCircles = 0;
            _numPolygons = 0;
            _numLines = 0;
        }
    }
}
