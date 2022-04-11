﻿using robotManager.Helpful;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace WholesomeToolbox
{
    /// <summary>
    /// Pathfinder related methods
    /// </summary>
    public class WTPathFinder
    {
        /// <summary>
        /// Returns the index of the closest path node from the player
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Node index, or 0 if not path</returns>
        internal static int GetIndexOfClosestPoint(List<Vector3> path)
        {
            if (path == null || path.Count <= 0) return 0;
            Vector3 myPos = ObjectManager.Me.PositionWithoutType;

            var curIndex = 0;
            var curDistance = float.MaxValue;

            for (var i = 0; i < path.Count; i++)
            {
                float distance = myPos.DistanceTo(path[i]);
                if (distance < curDistance)
                {
                    curDistance = distance;
                    curIndex = i;
                }
            }

            return curIndex;
        }

        /// <summary>
        /// Returns the distance in yards of a location from a line, can be used to check if enemies are along the current path
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns>Point distance</returns>
        public static float PointDistanceToLine(Vector3 start, Vector3 end, Vector3 point)
        {
            float vLenSquared = (start.X - end.X) * (start.X - end.X) +
                                (start.Y - end.Y) * (start.Y - end.Y) +
                                (start.Z - end.Z) * (start.Z - end.Z);
            if (vLenSquared == 0f) return point.DistanceTo(start);

            Vector3 ref1 = point - start;
            Vector3 ref2 = end - start;
            float clippedSegment = System.Math.Max(0, System.Math.Min(1, Vector3.Dot(ref ref1, ref ref2) / vLenSquared));

            Vector3 projection = start + (end - start) * clippedSegment;
            return point.DistanceTo(projection);
        }

        /// <summary>
        /// Returns the total distance of the current active path
        /// </summary>
        /// <returns>Path distance in yards</returns>
        public static float GetCurrentPathTotalDistance()
        {
            float result = 0;
            List<Vector3> currentPath = MovementManager.CurrentPath;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                result += currentPath[i].DistanceTo(currentPath[i + 1]);
            }
            return result;
        }

        /// <summary>
        /// Returns the remaining distance of the current active path
        /// </summary>
        /// <returns>Remaining path distance in yards</returns>
        public static float GetCurrentPathRemainingDistance()
        {
            float result = 0;
            List<Vector3> currentPath = MovementManager.CurrentPath;
            Vector3 nextNode = MovementManager.CurrentMoveTo;
            result += ObjectManager.Me.Position.DistanceTo(nextNode);
            bool nextNodeFound = false;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                if (!nextNodeFound && currentPath[i] == nextNode)
                {
                    nextNodeFound = true;
                }
                if (nextNodeFound)
                {
                    result += currentPath[i].DistanceTo(currentPath[i + 1]);
                }
            }
            return result;
        }
    }
}