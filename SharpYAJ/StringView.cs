using System;
using System.Collections.Generic;
using System.Text;

namespace SharpYAJ
{
#if SHARE_INTERNAL_METHODS
		public
#endif
	class StringView
	{
		public string realString;
		public int offset;

		public int Length => Math.Max(this.realString.Length - offset, 0);
		public char this[int idx] => this.realString[this.offset + idx];

		public StringView(string realString, int offset = 0)
		{
			this.realString = realString;
			this.offset = offset;
		}

		public void TrimStart()
		{
			while(this.Length != 0 && char.IsWhiteSpace(this[0]))
				this.Move(1);
		}

		public void Move(int offset)
		{
			this.offset += offset;
		}

		public StringView SubString(int offset)
		{
			return new StringView(this.realString, this.offset + offset);
		}
		public string SubString(int offset, int length)
		{
			return this.realString.Substring(this.offset + offset, length);
		}

		public override string ToString()
		{
			return this.realString.Substring(this.offset);
		}
	}
}
