﻿using System;
using System.IO;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


using MyEngine;

namespace MyGame
{
    class MoveWithArrowKeys : MonoBehaviour
    {

        public float velocityChangeSpeed = 10.0f;

        private Vector3 up = Vector3.UnitY;
        private float pitch = 0.0f;
        private float facing = 0.0f;

        public bool disabledInput = false;

        float speedModifier = 10.0f;
        Point lastMousePos;
        int scrollWheelValue;
        Vector3 currentVelocity;

        public override void Update(double deltaTime)
        {


            float d = speedModifier * (float)deltaTime;
            if (speedModifier < 10) speedModifier = 10;


            var r = Vector3.Zero;

            var targetVelocity = Vector3.Zero;
            if (Input.GetKey(Key.ControlRight))
            {
                if (Input.GetKey(Key.Down)) r.Z -= d;
                if (Input.GetKey(Key.Up)) r.Z += d;
                if (Input.GetKey(Key.Left)) r.X -= d;
                if (Input.GetKey(Key.Right)) r.X += d;
                if (Input.GetKey(Key.PageDown)) r.Y -= d;
                if (Input.GetKey(Key.PageUp)) r.Y += d;

                if(r.Length>0)
                {
                    this.gameObject.transform.rotation *=(
                        Matrix4.CreateRotationX(r.X) *
                        Matrix4.CreateRotationY(r.Y) *
                        Matrix4.CreateRotationZ(r.Z)
                       ).ExtractRotation();
                }
            }
            else
            {
                if (Input.GetKey(Key.Down)) targetVelocity.Z -= d;
                if (Input.GetKey(Key.Up)) targetVelocity.Z += d;
                if (Input.GetKey(Key.Left)) targetVelocity.X -= d;
                if (Input.GetKey(Key.Right)) targetVelocity.X += d;
                if (Input.GetKey(Key.PageDown)) targetVelocity.Y -= d;
                if (Input.GetKey(Key.PageUp)) targetVelocity.Y += d;
            }

            var pos = Matrix4.CreateTranslation(targetVelocity);

            float c = 100;
            if (targetVelocity.Length > 0.1) speedModifier += (float)deltaTime*c;
            else speedModifier -= (float)deltaTime*c;

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, velocityChangeSpeed * (float)deltaTime);

            gameObject.transform.position += currentVelocity;
        }
    }
}
