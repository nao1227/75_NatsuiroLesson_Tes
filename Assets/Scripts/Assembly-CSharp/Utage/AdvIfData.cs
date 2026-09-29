namespace Utage
{
	internal class AdvIfData
	{
		private bool isSkpping;

		public AdvIfData Parent { get; private set; }

		public bool IsSkpping
		{
			get
			{
				return isSkpping;
			}
			set
			{
				isSkpping = value;
			}
		}

		public bool IsParantSkipping
		{
			get
			{
				if (Parent == null)
				{
					return false;
				}
				if (!Parent.IsSkpping)
				{
					return Parent.IsParantSkipping;
				}
				return true;
			}
		}

		public bool IsIf { get; set; }

		internal AdvIfData(AdvIfData parent)
		{
			Parent = parent;
		}

		internal void BeginIf(AdvParamManager param, ExpressionParser exp)
		{
			IsIf = param.CalcExpressionBoolean(exp);
			isSkpping = !IsIf;
		}

		internal void ElseIf(AdvParamManager param, ExpressionParser exp)
		{
			if (!IsIf)
			{
				IsIf = param.CalcExpressionBoolean(exp);
				isSkpping = !IsIf;
			}
			else
			{
				isSkpping = true;
			}
		}

		internal void Else()
		{
			if (!IsIf)
			{
				IsIf = true;
				isSkpping = false;
			}
			else
			{
				isSkpping = true;
			}
		}

		internal void EndIf()
		{
			isSkpping = false;
		}
	}
}
