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

### Using internal methods
Beside `ReadJSON`, YAJReader contains methods like `ReadArray`, `ReadInt`, `ReadBool` and so on. These methods are used internally. If you want to use these methods for whatever reason, you have to tell SharpYAJ to perform additional checks in these methods, as by default they omit checks done by the SharpYAJ-caller method. To annouce the usage of these methods, compile the library with the `SHARE_INTERNAL_METHODS` flag.

To be more performant, the reading is done by simply moving a cursor over the string, so that a new substring does not have to be created for each element. This string-shifting is done by the internal class `StringView`. This class is also only shown if you activate compiliation flag `SHARE_INTERNAL_METHODS`. So for using the internal methods, you have to create a `StringView` instance.

## Writing
For writing objects SharpYAJ expects the same object types like it produced when reading a string:
- Objects have to be `Dictionary<string, object>`
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

## License
SharpYAJ is licensed under the MIT License
