using System;
using System.Collections.Generic;
using System.Text;

namespace SharpYAJ
{
	public class IndentWriter
	{
		public const string DEFAULT_INDENT_STR = "\t";
		public const string DEFAULT_SEPARATOR_STR = " ";
		public const string DEFAULT_NEWLINE_STR = "\n";

		public int depth;
		public string indentStr;
		public string separatorStr;
		public string newLineStr;

		public IndentWriter(string indentString = null, string separatorStr = null, string newLineStr = null)
		{
			this.indentStr = indentString ?? DEFAULT_INDENT_STR;
			this.separatorStr = separatorStr ?? DEFAULT_SEPARATOR_STR;
			this.newLineStr = newLineStr ?? DEFAULT_NEWLINE_STR;
		}
		public IndentWriter(IndentWriter writer, int depthDelta)
		{
			this.depth = Math.Max(writer.depth + depthDelta, 0);
			this.indentStr = writer.indentStr;
			this.separatorStr = writer.separatorStr;
			this.newLineStr = writer.newLineStr;
		}


		public virtual void Write(StringBuilder sb)
		{
			var depth = this.depth;
			var indentStr = this.indentStr;

			for(int i = 0; i < depth; ++i)
				sb.Append(indentStr);
		}
		public virtual void WriteSeparator(StringBuilder sb)
		{
			sb.Append(this.separatorStr);
		}
		public virtual void WriteLineBreak(StringBuilder sb)
		{
			sb.Append(this.newLineStr);
		}

		public virtual IndentWriter IncreaseDepth(int delta = 1)
		{
			return new IndentWriter(this, delta);
		}
		public virtual IndentWriter DecreaseDepth(int delta = 1)
		{
			return new IndentWriter(this, -delta);
		}

		public static IndentWriter operator +(IndentWriter writer, int delta)
		{
			return writer.IncreaseDepth(delta);
		}
		public static IndentWriter operator -(IndentWriter writer, int delta)
		{
			return writer.DecreaseDepth(delta);
		}
	}
	public class NoIndentWriter : IndentWriter
	{
		public override void Write(StringBuilder sb)
		{ }
		public override void WriteSeparator(StringBuilder sb)
		{ }
		public override void WriteLineBreak(StringBuilder sb)
		{ }
		public override IndentWriter IncreaseDepth(int delta = 1)
		{
			return this;
		}
		public override IndentWriter DecreaseDepth(int delta = 1)
		{
			return this;
		}

		public static NoIndentWriter operator +(NoIndentWriter writer, int delta)
		{
			return writer;
		}
		public static NoIndentWriter operator -(NoIndentWriter writer, int delta)
		{
			return writer;
		}
	}
}
