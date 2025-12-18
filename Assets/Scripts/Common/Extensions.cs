using UnityEngine;

namespace Common
{
    public static class Extensions
    {
        private static readonly Quaternion ForwardRotation =  Quaternion.Euler(0f, 0f, 0f);
        private static readonly Quaternion BackwardRotation =  Quaternion.Euler(0f, 180f, 0f);

        public static void FlipX(this Transform transform, float velocityX)
        {
            transform.rotation = velocityX switch
            {
                > 0 => ForwardRotation,
                < 0 => BackwardRotation,
                
                _ => transform.rotation
            };
        }
    }
}