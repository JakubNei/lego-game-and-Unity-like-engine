using System;
using System.Collections.Generic;
using System.Text;

using MyEngine;
using OpenTK;

namespace MyGame
{
    class LightMovement : MonoBehaviour
    {

        public Vector3 offset = new Vector3(0, 10, 0);
        public Vector3 dir = new Vector3(0, 0, 1);
        public float start = 0f;
        public float end = 20.0f;
        public float current = 0.0f;
        public bool goingToEnd = true;
        public float speed = 10f;


        public override void Update(double deltaTime)
        {

            var p=this.gameObject.transform.position;

            p = offset + dir * current;


            var s = -speed;
            if(goingToEnd) s=speed;

            current += (float)deltaTime * s;

            if (current > end) goingToEnd = false;
            if (current < start) goingToEnd = true;
                        

            this.gameObject.transform.position = p;
        }
    }
}
