using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpYAJ
{
	public static class YAJReader
	{
		public static object ReadJSON(string input)
		{
			return ReadJSON(new StringView(input));
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static object ReadJSON(StringView input)
		{
			object result = ReadElement(input);
			input.TrimStart();
			if(input.Length != 0)
				throw new Exception($"Forbidden second root element at offset {input.offset}");

			return result;
		}

#if SHARE_INTERNAL_METHODS
		public
#endif
		static object ReadElement(StringView input)
		{
			input.TrimStart();
			if(input.Length == 0)
				throw new Exception("Unexpected EOF");

			char indicator = input[0];
			//ASCII 0 - 9
			if(indicator >= 0x30 && indicator <= 0x39)
				return ReadNumber(input);

			switch(input[0])
			{
				case '{':
					return ReadObject(input);
				case '[':
					return ReadArray(input);
				case '"':
					return ReadString(input);
				case '-':
					return ReadNumber(input);
				case 't':
				case 'f':
					return ReadBool(input);
				case 'n':
					return ReadNull(input);
			}

			throw new Exception("Unknown value at pos " + input.offset);
		}

#if SHARE_INTERNAL_METHODS
		public
#endif
		static IEnumerable<object> ReadArray(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimStart();
			if(input.Length < 1 || input[0] != '[')
				throw new Exception($"No Array at offset {input.offset}");
#endif

			List<object> objects = new List<object>();
			int startOffset = input.offset;

			char separator;
			while(input.Length != 0 && (separator = input[0]) != ']')
			{
				if(separator == '[' || separator == ',')
					input.Move(1);
				else
					throw new Exception($"Unexpected char at offset {input.offset}. Expected array separator ','");

				input.TrimStart();

				object child = ReadElement(input);
				objects.Add(child);

				input.TrimStart();
			}

			if(input.Length == 0)
				throw new Exception($"Array at offset {startOffset} isn't closed");

			input.Move(1);
			return objects;
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static Dictionary<string, object> ReadObject(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimStart();
			if(input.Length < 1 || input[0] != '{')
				throw new Exception($"No Object at offset {input.offset}");
#endif

			Dictionary<string, object> childs = new Dictionary<string, object>();
			int startOffset = input.offset;

			char separator;
			while(input.Length != 0 && (separator = input[0]) != '}')
			{
				if(separator == '{' || separator == ',')
					input.Move(1);
				else
					throw new Exception($"Unexpected char at offset {input.offset}. Expected object separator ','");


				input.TrimStart();

				var entry = ReadObjectEntry(input);
				if(childs.ContainsKey(entry.Item1))
					throw new Exception($"Key '{entry.Item1}' already existing in object at offset {startOffset}");

				childs[entry.Item1] = entry.Item2;

				input.TrimStart();
			}

			if(input.Length == 0)
				throw new Exception($"Object at offset {startOffset} isn't closed");

			input.Move(1);
			return childs;
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static Tuple<string, object> ReadObjectEntry(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimStart();
#endif
			string key = ReadString(input);
			input.TrimStart();

			if(input.Length == 0 || input[0] != ':')
				throw new Exception($"Unexpected value at offset {input.offset}. Expected object key-value separator ':'");

			input.Move(1);
			object value = ReadElement(input);

			return new Tuple<string, object>(key, value);
		}


#if SHARE_INTERNAL_METHODS
		public
#endif
		static string ReadString(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimStart();
			if(input.Length < 1 || input[0] != '"')
				throw new Exception($"No string at offset {input.offset}");
#endif

			if(input.Length < 2)
				throw new Exception($"Invalid string at offset {input.offset}");

			input.Move(1);

			var builder = new StringBuilder();
			bool escape = false;
			bool stringClosed = false;

			while(input.Length != 0)
			{
				char c = input[0];
				input.Move(1);

				if(escape)
				{
					switch(c)
					{
						case '\\':
							builder.Append('\\');
							break;
						case 'b':
							builder.Append('\b');
							break;
						case 'f':
							builder.Append('\f');
							break;
						case 'r':
							builder.Append('\r');
							break;
						case 'n':
							builder.Append('\n');
							break;
						case 't':
							builder.Append('\t');
							break;
						case '"':
							builder.Append('"');
							break;
						case 'u':
							builder.Append(ReadEscapedUnicodeChar());
							break;
						default:
							throw new Exception($"Invalid string escape sequence at offset {input.offset - 2}");
					}

					escape = false;
					continue;
				}

				if(c == '\\')
				{
					escape = true;
					continue;
				}
				if(c == '"')
				{
					stringClosed = true;
					break;
				}

				builder.Append(c);
			}

			if(!stringClosed)
				throw new Exception($"String not closed at offset {input.offset}");

			return builder.ToString();


			char ReadEscapedUnicodeChar()
			{
				if(input.Length < 4)
					throw new Exception($"Invalid Unicode escape sequence at offset {input.offset}");

				string hexChars = input.SubString(0, 4);
				foreach(var c in hexChars)
				{
					if((c >= 0x30 && c <= 0x39) || (c >= 0x41 && c <= 0x46) || (c >= 0x61 && c <= 0x66))
						continue;

					throw new Exception($"Invalid Unicode escape sequence at offset {input.offset}");
				}

				input.Move(4);

				int unicodePos = int.Parse(hexChars, NumberStyles.HexNumber);
				return (char)unicodePos;
			}
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static object ReadNumber(StringView input)
		{
#if SHARE_INTERNAL_METHODS
			input.TrimStart();
#endif

			int offset = -1;
			while(++offset < input.Length)
			{
				char c = input[offset];

				if(c == '.')
					return ReadDouble(input);
				if(c == '-' && offset == 0)
					continue;
				//ASCII 0 - 9
				if(c >= 0x30 && c <= 0x39)
					continue;

				break;
			}

			return ReadInt(input);
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static int ReadInt(StringView input)
		{
			int offset = -1;
			while(++offset < input.Length)
			{
				char c = input[offset];

				if(c == '-' && offset == 0)
					continue;
				//ASCII 0 - 9
				if(c >= 0x30 && c <= 0x39)
					continue;

				break;
			}

			var intString = input.SubString(0, offset);
			input.Move(offset);

			return Convert.ToInt32(intString);
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static double ReadDouble(StringView input)
		{
			bool negative = false;
			bool doubleDot = false;
			int doubleDotOffset = -1;

			int offset = -1;
			while(++offset < input.Length)
			{
				char c = input[offset];

				if(c == '-' && offset == 0)
				{
					negative = true;
					continue;
				}
				if(c == '.' && !doubleDot)
				{
					if(offset == 0 || (negative && offset == 1))
						throw new Exception($"Invalid float value at offset {input.offset}. Floats mustn't start with a dot");

					doubleDotOffset = offset;
					doubleDot = true;
					continue;
				}
				//ASCII 0 - 9
				if(c >= 0x30 && c <= 0x39)
					continue;

				break;
			}

			if(doubleDotOffset == offset - 1)
				throw new Exception($"Invalid float value at offset {input.offset}. Floats mustn't end with a dot");

			var doubleString = input.SubString(0, offset);
			input.Move(offset);

			return Convert.ToDouble(doubleString, CultureInfo.InvariantCulture);
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static bool ReadBool(StringView input)
		{
			if(input.Length < 4)
				throw new Exception($"Unexpected value at offset {input.offset}");

			int offset = -1;
			while(++offset < input.Length)
			{
				char c = input[offset];
				//ASCII a - z
				if(c >= 0x61 && c <= 0x7a)
					continue;

				break;
			}

			if(offset > 5)
				throw new Exception($"Unexpected value at offset {input.offset}");

			string boolStr = input.SubString(0, offset);
			input.Move(boolStr.Length);

			if(boolStr == "true")
				return true;
			if(boolStr == "false")
				return false;

			throw new Exception($"Unexpected value at offset {input.offset}");
		}
#if SHARE_INTERNAL_METHODS
		public
#endif
		static object ReadNull(StringView input)
		{
			if(input.Length < 4)
				throw new Exception($"Unexpected value at offset {input.offset}");

			string nullStr = input.SubString(0, 4);
			input.Move(4);

			if(nullStr == "null")
				return null;

			throw new Exception($"Unexpected value at offset {input.offset}");
		}
	}
}
