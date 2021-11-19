using UnityEngine;

namespace Pixelplacement
{
    public static class MathUtilities
    {
        //http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
        public static bool AreLinesIntersecting(Vector3 l1_p1, Vector3 l1_p2, Vector3 l2_p1, Vector3 l2_p2, bool shouldIncludeEndPoints = false)
        {
            bool isIntersecting = false;

            float denominator = (l2_p2.z - l2_p1.z) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.z - l1_p1.z);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.z - l2_p1.z) - (l2_p2.z - l2_p1.z) * (l1_p1.x - l2_p1.x)) / denominator;
                float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.z - l2_p1.z) - (l1_p2.z - l1_p1.z) * (l1_p1.x - l2_p1.x)) / denominator;

                //Are the line segments intersecting if the end points are the same
                if (shouldIncludeEndPoints)
                {
                    //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (u_a >= 0f && u_a <= 1f && u_b >= 0f && u_b <= 1f)
                    {
                        isIntersecting = true;
                    }
                }
                else
                {
                    //Is intersecting if u_a and u_b are between 0 and 1
                    if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
                    {
                        isIntersecting = true;
                    }
                }
		
            }

            return isIntersecting;
        }
    }
}