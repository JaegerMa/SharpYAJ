using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SharpYAJ
{
    public static class YAJWriter
    {
		public static string WriteJSON(object element, bool indent)
		{
			var indentWriter = indent ? new IndentWriter() : null;
			return WriteJSON(element, indentWriter);
		}
		public static string WriteJSON(object element, string indentWriter, string newLine = null)
		{
			return WriteJSON(element, new IndentWriter(indentString: indentWriter, newLineStr: newLine));
		}
		public static string WriteJSON(object element, IndentWriter indentWriter = null)
		{
			var stringBuilder = WriteJSONToStringBuilder(element, indentWriter: indentWriter);
			return stringBuilder.ToString();
		}

		public static StringBuilder WriteJSONToStringBuilder(object element, StringBuilder stringBuilder = null, IndentWriter indentWriter = null)
		{
			if(indentWriter == null)
				indentWriter = new NoIndentWriter();

			stringBuilder = stringBuilder ?? new StringBuilder();
			WriteElement(element, stringBuilder, indentWriter);

			return stringBuilder;
		}

		public static void WriteElement(object input, StringBuilder sb, IndentWriter indentWriter)
		{
			if(input is string)
				WriteString((string)input, sb);
			else if(input is char)
				WriteString(input.ToString(), sb);
			else if(input is long)
				WriteLong((long)input, sb);
			else if(input is int)
				WriteInt((int)input, sb);
			else if(input is short)
				WriteShort((short)input, sb);
			else if(input is double)
				WriteDouble((double)input, sb);
			else if(input is float)
				WriteFloat((float)input, sb);
			else if(input is bool)
				WriteBool((bool)input, sb);
			else if(input is null)
				WriteNull(sb);
			else if(IsObject(input))
				WriteObject((IDictionary<string, object>)input, sb, indentWriter);
			else if(IsArray(input))
				WriteArray((IEnumerable)input, sb, indentWriter);
			else
				WriteString(input.ToString(), sb);
		}
		public static void WriteObject(IDictionary<string, object> input, StringBuilder sb, IndentWriter indentWriter)
		{
			sb.Append("{");
			var childIndentWriter = indentWriter + 1;

			bool firstElement = true;
			foreach(var child in input)
			{
				if(!firstElement)
					sb.Append(",");
				indentWriter.WriteLineBreak(sb);

				childIndentWriter.Write(sb);
				WriteString(child.Key, sb);

				sb.Append(":");
				if(IsObject(child.Value) || IsArray(child.Value))
				{
					childIndentWriter.WriteLineBreak(sb);
					childIndentWriter.Write(sb);
				}
				else
				{
					childIndentWriter.WriteSeparator(sb);
				}

				WriteElement(child.Value, sb, childIndentWriter);

				firstElement = false;
			}

			indentWriter.WriteLineBreak(sb);
			indentWriter.Write(sb);
			sb.Append("}");
		}
		public static void WriteArray(IEnumerable input, StringBuilder sb, IndentWriter indentWriter)
		{
			sb.Append("[");
			var childIndentWriter = indentWriter + 1;

			bool firstElement = true;
			foreach(var child in input)
			{
				if(!firstElement)
					sb.Append(",");
				indentWriter.WriteLineBreak(sb);

				childIndentWriter.Write(sb);
				WriteElement(child, sb, childIndentWriter);

				firstElement = false;
			}

			indentWriter.WriteLineBreak(sb);
			indentWriter.Write(sb);
			sb.Append("]");
		}
		public static void WriteString(string input, StringBuilder sb)
		{
			var escaped = input
				.Replace("\b", @"\b")
				.Replace("\f", @"\f")
				.Replace("\n", @"\n")
				.Replace("\r", @"\r")
				.Replace("\t", @"\t")
				.Replace("\"", "\\\"")
				.Replace("\\", @"\\");

			sb.Append("\"");
			sb.Append(escaped);
			sb.Append("\"");
		}
		public static void WriteShort(short input, StringBuilder sb)
		{
			sb.Append(input);
		}
		public static void WriteInt(int input, StringBuilder sb)
		{
			sb.Append(input);
		}
		public static void WriteLong(long input, StringBuilder sb)
		{
			sb.Append(input);
		}
		public static void WriteFloat(float input, StringBuilder sb)
		{
			sb.Append(input);
		}
		public static void WriteDouble(double input, StringBuilder sb)
		{
			sb.Append(input);
		}
		public static void WriteBool(bool input, StringBuilder sb)
		{
			sb.Append(input ? "true" : "false");
		}
		public static void WriteNull(StringBuilder sb)
		{
			sb.Append("null");
		}

		public static bool IsObject(object input)
		{
			return input is IDictionary<string, object> && !IsPrimitive(input);
		}
		public static bool IsArray(object input)
		{
			return input is IEnumerable && !IsPrimitive(input);
		}
		public static bool IsPrimitive(object input)
		{
			switch(input)
			{
				case int intVal:
				case string stringVal:
				case char charVal:
				case bool boolVal:
				case float floatVal:
				case long longVal:
				case short shortVal:
				case double doubleVal:
				case uint uintVal:
				case ushort ushortVal:
				case ulong ulongVal:
				case byte byteVal:
				case sbyte sbyteVal:
					return true;
				default:
					return false;
			}
		}
	}
}
