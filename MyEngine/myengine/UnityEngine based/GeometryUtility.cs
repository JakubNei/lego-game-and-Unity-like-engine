using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace MyEngine
{
    public static class GeometryUtility
    {
        public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
        {
            for (uint i = 0; i < 6; i++)
            {
                if (planes[i].GetDistanceToPoint(bounds.center) < -bounds.extents.Length * 2)
                {
                    return false;
                }
            }
            return true;        
        }
        public static Plane[] CalculateFrustumPlanes(Camera cam)
        {

            var p = new Plane[6];
            var m = cam.GetViewMat()*cam.GetProjectionMat();


            const int FRUSTUM_RIGHT = 0;
            const int FRUSTUM_LEFT = 1;
            const int FRUSTUM_DOWN = 2;
            const int FRUSTUM_UP = 3;
            const int FRUSTUM_FAR = 4;
            const int FRUSTUM_NEAR = 5;

            p[FRUSTUM_RIGHT].normal.X = m[0, 3] - m[0, 0];
            p[FRUSTUM_RIGHT].normal.Y = m[1, 3] - m[1, 0];
            p[FRUSTUM_RIGHT].normal.Z = m[2, 3] - m[2, 0];
            p[FRUSTUM_RIGHT].distance = m[3, 3] - m[3, 0];

            p[FRUSTUM_LEFT].normal.X = m[0, 3] + m[0, 0];
            p[FRUSTUM_LEFT].normal.Y = m[1, 3] + m[1, 0];
            p[FRUSTUM_LEFT].normal.Z = m[2, 3] + m[2, 0];
            p[FRUSTUM_LEFT].distance = m[3, 3] + m[3, 0];

            p[FRUSTUM_DOWN].normal.X = m[0, 3] + m[0, 1];
            p[FRUSTUM_DOWN].normal.Y = m[1, 3] + m[1, 1];
            p[FRUSTUM_DOWN].normal.Z = m[2, 3] + m[2, 1];
            p[FRUSTUM_DOWN].distance = m[3, 3] + m[3, 1];

            p[FRUSTUM_UP].normal.X = m[0, 3] - m[0, 1];
            p[FRUSTUM_UP].normal.Y = m[1, 3] - m[1, 1];
            p[FRUSTUM_UP].normal.Z = m[2, 3] - m[2, 1];
            p[FRUSTUM_UP].distance = m[3, 3] - m[3, 1];

            p[FRUSTUM_FAR].normal.X = m[0, 3] - m[0, 2];
            p[FRUSTUM_FAR].normal.Y = m[1, 3] - m[1, 2];
            p[FRUSTUM_FAR].normal.Z = m[2, 3] - m[2, 2];
            p[FRUSTUM_FAR].distance = m[3, 3] - m[3, 2];

            p[FRUSTUM_NEAR].normal.X = m[0, 3] + m[0, 2];
            p[FRUSTUM_NEAR].normal.Y = m[1, 3] + m[1, 2];
            p[FRUSTUM_NEAR].normal.Z = m[2, 3] + m[2, 2];
            p[FRUSTUM_NEAR].distance = m[3, 3] + m[3, 2];
            
            return p;

        }
    }
}
