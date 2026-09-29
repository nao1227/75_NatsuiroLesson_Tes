namespace Utage
{
	public class TagData : IParsedTextData
	{
		public string TagString { get; private set; }

		public string TagName { get; private set; }

		public string TagArg { get; private set; }

		public bool IgnoreTagString { get; private set; }

		public TagData(string str, string name, string arg)
		{
			TagString = str;
			TagName = name;
			TagArg = arg;
		}

		public TagData(string str, string name, string arg, bool ignoreTagString)
			: this(str, name, arg)
		{
			IgnoreTagString = ignoreTagString;
		}
	}
}
