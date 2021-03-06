# SharpYAJ - YetAnotherJSON Reader/Writer for C#

SharpYAJ (pronounced as 'Sharp-Jay') is a .NET Standard 2.0 compatible library to serialize and deserialize JSON strings.

There's the class `JavaScriptSerializer` in .NET which does exactly this task, but unfortunately it's not yet included in .NET Standard any therefore only available when using .NET Framework. There would be Newtonsofts JSON implementation, but that library is way too big if you just want to read or write small JSON files, or if you just want simple .NET types instead of wrappers for each JSON type.

So, until the `JavaScriptSerializer` is defined in .NET Standard, this library can be used alternatively.

## Reading
SharpYAJ deserializes objects the same way, `JavaScriptSerializer` does:
- Objects are represented as `Dictionary<string, object>`
- Arrays are represented as `IEnumerable<object>`
- Primitives are represented as their corresponding .NET type (int, double, string, bool, null (object))

```csharp
using SharpYAJ;

var myJSONString = "[1, 3, 3, 7, \"is\", true]";

object deserialized = YAJReader.ReadJSON(myJSONString);
//deserialized: IEnumerable<object> { 1, 3, 3, 7, "is", true }
```

### Numbers
In JSON, numbers can be infinitely high. In SharpYAJ they are read as follows:

|Range|Read as|
|---|---|
| -2^31 to (2^31 - 1) | int32 |
| -2^63 to (2^63 - 1) | int64/long |
| 2^63 to (2^64 - 1) | uint64/ulong |
| Everything else | double |

### Trailing commas
JSON doesn't allow trailing commas in arrays or objects like JavaScript does (e. g. `[1, 2, 3,]`), therefore SharpYAJ also doesn't allow it by default.
However if you want to parse such "invalid" JSON, you can enable that feature by compiling with flags `ALLOW_TRAILING_ARRAY_COMMAS` and `ALLOW_TRAILING_OBJECT_COMMAS`.

A list of all supported flags is at the end of this README.

### Comments
JSON doesn't allow comments like programming languages do (e. g. `[1, 2, /* 3, */ 4]`), therefore SharpYAJ also doesn't allow it by default.
However if you want to parse such "invalid" JSON, you can enable that feature by compiling with flags `ALLOW_LINE_COMMENTS` for `//`-comments and `ALLOW_BLOCK_COMMENTS` for `/* */`-comments.

A list of all supported flags is at the end of this README.

### Using internal methods
Beside `ReadJSON`, YAJReader contains methods like `ReadArray`, `ReadInt`, `ReadBool` and so on. These methods are used internally. If you want to use these methods for whatever reason, you have to tell SharpYAJ to perform additional checks in these methods, as by default they omit checks done by the SharpYAJ-caller method. To annouce the usage of these methods, compile the library with the `SHARE_INTERNAL_METHODS` flag.

To be more performant, the reading is done by simply moving a cursor over the string, so that a new substring does not have to be created for each element. This string-shifting is done by the internal class `StringView`. This class is also only shown if you activate compiliation flag `SHARE_INTERNAL_METHODS`. So for using the internal methods, you have to create a `StringView` instance.

## Writing
For writing objects SharpYAJ expects basically the same object types like it produced when reading a string:
- Objects have to be `IDictionary<string, object>`
- Arrays have to be `IEnumerable`
- Allowed primitive types are
  - `short`
  - `int`
  - `long`
  - `float`
  - `double`
  - `string`
  - `bool`
  - `null`

```csharp
using SharpYAJ;

var serialized = YAJWriter.WriteJSON(deserialized);
//serialized: "[1,3,3,7,\"is\",true]"
```

### Indention / PrettyPrint
By default, YAJWriter doesn't print any spaces or line-breaks to separate the elements.
If you want a pretty-printed JSON string, pass the `indention` flag to `WriteJSON`:

```csharp
using SharpYAJ;

var serialized = YAJWriter.WriteJSON(deserialized, true);
/*serialized:
"[
	1,
	3,
	3,
	7,
	\"is\",
	true
]"
*/
```

To specify the separation and line-break char, they can also be passed to `WriteJSON`:
```csharp
using SharpYAJ;

var serialized = YAJWriter.WriteJSON(deserialized, "~~~~", "\r\n");
/*serialized:
"[
~~~~1,
~~~~3,
~~~~3,
~~~~7,
~~~~\"is\",
~~~~true
]"
*/
```

#### Own pretty print implementation
Pretty printing is done using the internal class `IndentWriter`. If you want to implement pretty print yourself, create a subclass overriding the methods of `IndentWriter` and pass the instance to the `WriteJSON` method:
```csharp
using SharpYAJ;

class MyIndentWriter : IndentWriter
{
	public override void Write(StringBuilder sb)
	{
		/* ... */
	}
}

var myIndentWriter = new MyIndentWriter();

var serialized = YAJWriter.WriteJSON(deserialized, myIndentWriter);
```

## Compiler flags
Following compiler flags are supported by SharpYAJ:

|Flag|Description|
|---|---|
|USE_INTERNAL_METHODS|Makes some internal methods public and adds extra checks to them so they can be used safely|
|ALLOW_TRAILING_ARRAY_COMMAS|Allow a trailing comma after the last element of an array|
|ALLOW_TRAILING_OBJECT_COMMAS|Allow a trailing comma after the last entry of an object|
|ALLOW_LINE_COMMENTS|Allow line comments `//` to mark the rest of the current line (until the next `\n`) as comment|
|ALLOW_BLOCK_COMMENTS|Allow block comments `/* */` to mark a certain area as comment|

## License
SharpYAJ is licensed under the MIT License
