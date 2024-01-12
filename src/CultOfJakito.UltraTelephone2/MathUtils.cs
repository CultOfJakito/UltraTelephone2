using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2
{
    public static class MathUtils
    {
        public static float InverseLerpVector3(Vector3 min, Vector3 max, Vector3 point)
        {
            float x = Mathf.InverseLerp(min.x, max.x, point.x);
            float y = Mathf.InverseLerp(min.y, max.y, point.y);
            float z = Mathf.InverseLerp(min.z, max.z, point.z);
            return (x + y + z) / 3f;
        }
    }
}
