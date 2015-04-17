# JSON Library Usage Notes

The JSON library implements the standard JSON encoding / decoding protocol.  The Library has three main APIs:

- **static JObject JSON.GetRootObject(string JSONencodedObject):**
	- This method decodes the string and returns the root JSON Object.
	
- **JValue JObject.GetValueFor(string accessorString):**
	- Returns the JSON value retrieve from the accessor string.  JNull is return if the value cannot be determined.
	- The accessor string format is similar to the standard language structures.  A period (.) is used to access an attribute of an object and the subscribe operator ([]) is used for arrays.  So to get the 3rd element from the attribute "name" of object "person" in the root object would have an accessor of:
		- "person.name[3]"

- **string JObject.Encode():**

	- Encodes the JObject and returns a JSON formatted string representing that JObject.