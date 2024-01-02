using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILRuntime
{
	public class ILBaseBehaviourAdapter : ILMonoBehaviourAdapter
	{
		public override Type BaseCLRType
		{
			get
			{
				return typeof(BaseBehaviour);
			}
		}
	}
}
