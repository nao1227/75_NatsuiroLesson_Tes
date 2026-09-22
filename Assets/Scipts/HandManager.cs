using System;
using System.Collections.Generic;
using Live2D.Cubism.Framework;
using UnityEngine;
using Stubs;   // ← これを追加

namespace Paidia.satsuki1
{
	public class HandManager
	{
		public Hand RightHand;

		public Hand LeftHand;

		private ValuePreserver _preserver;

		public bool GrabLeft => LeftHand.IsGrabbing;

		public bool GrabRight => RightHand.IsGrabbing;

		public bool IsAnyHandEmpty
		{
			get
			{
				if (GrabLeft)
				{
					return !GrabRight;
				}
				return true;
			}
		}

		public bool IsGrabbingAny
		{
			get
			{
				if (!GrabLeft)
				{
					return GrabRight;
				}
				return true;
			}
		}

		public HandManager()
		{
			RightHand = new Hand(HandType.Right);
			LeftHand = new Hand(HandType.Left);
			_preserver = UnityEngine.Object.FindObjectOfType<ValuePreserver>();
		}

		public void Grab(HandType handType, AbstractOsawari target)
		{
			if (handType == HandType.Left)
			{
				LeftHand.Grab(target);
			}
			else
			{
				RightHand.Grab(target);
			}
		}

		public Hand GetHandToUse()
		{
			if (LeftHand.IsGrabbing && !RightHand.IsGrabbing)
			{
				return RightHand;
			}
			return LeftHand;
		}

		public void Release(HandType handType)
		{
			if (handType == HandType.Left)
			{
				LeftHand.Release();
			}
			else
			{
				RightHand.Release();
			}
		}

		public void MoveHand(HandType handType, ParameterValue paramValue, int modelIndex = 0)
		{
			Hand hand = handType switch
			{
				HandType.Left => LeftHand, 
				HandType.Right => RightHand, 
				_ => throw new Exception(), 
			};
			if (!(hand.Parameter == null))
			{
				// _preserver.SetValue(hand.Parameter.UnmanagedIndex, paramValue.Value, CubismParameterBlendMode.Override, modelIndex);
			}
		}

		public bool IsGrabbing(AbstractOsawari target)
		{
			if (target == null)
			{
				return false;
			}
			if (RightHand.GrabbingTarget == target || LeftHand.GrabbingTarget == target)
			{
				return true;
			}
			return false;
		}

		public List<Hand> GetHandGrabbing(AbstractOsawari target)
		{
			List<Hand> list = new List<Hand>();
			if (RightHand.GrabbingTarget == target)
			{
				list.Add(RightHand);
			}
			if (LeftHand.GrabbingTarget == target)
			{
				list.Add(LeftHand);
			}
			return list;
		}

		private void Update()
		{
			if (RightHand.IsGrabbing)
			{
				RightHand.GrabbingTarget.ToString();
			}
			if (LeftHand.IsGrabbing)
			{
				LeftHand.GrabbingTarget.ToString();
			}
		}
	}
}
