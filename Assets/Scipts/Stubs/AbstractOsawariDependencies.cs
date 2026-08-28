using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Stubs
{
    public enum ParameterName
    {
        HeadX, HeadY, RightHandOnHead, LeftHandOnHead
    }

    public enum HandType
    {
        Right, Left
    }

    public class Hand
    {
        public HandType HandType;
    }

  

    public class ParameterDictionary
    {
        private Dictionary<ParameterName, int> _table = new Dictionary<ParameterName, int>
        {
            { ParameterName.HeadX, 0 },
            { ParameterName.HeadY, 1 },
            { ParameterName.RightHandOnHead, 2 },
            { ParameterName.LeftHandOnHead, 3 }
        };

        public Dictionary<ParameterName, int> GetTable() { return _table; }
    }

    public class HandParamValue
    {
        public bool Locked;
        public ParameterValue Get(HandType type) { return new ParameterValue(null); }
        public void SetValue(HandType type, float value) { }
        public void Appear(List<Hand> hands) { }
        public void Disappear() { }
    }

    public class SpeedRange
    {
        public float Min;
        public float Max;
        public bool IsInRange(float speed) { return speed >= Min && speed <= Max; }
    }

    public class OsawariBlocker
    {
        public bool IsBlocked() { return false; }
    }

    public class OsawariConditions
    {
        public static OsawariConditions Empty => new OsawariConditions();
        public bool IsPistonMoving;
        public Vector3 Move;
        public float Speed;
        public float MovedDistance;
        public void UpdateTime() { }
        public void ResetTime() { }
    }

    public class FPSChecker
    {
        public float GetFps() { return 60f; }
    }

    public static class Easing
    {
        public enum Ease { OutSine }
        public static System.Func<float, float> GetEasingMethod(Ease ease)
        {
            return (x) => x;
        }
    }

    public class PhysicsCalculater : MonoBehaviour
    {
        public void InvalidCalc(int id, bool Switch) { }
    }

    public class FaceController : MonoBehaviour
    {
        public bool AllowBlink;
    }
}