![HR.MobyPark - Banner](https://github.com/ItsADanny/HR.MobyPark/blob/template/HR__Banner.png?raw=true)

# HR.Mobypark - Code standard rules

[![Hogeschool Rotterdam Project](https://img.shields.io/badge/Hogeschool_Rotterdam-Software_Construction:_Process_and_Tools-cc0133.svg?logo=googlescholar&logoColor=cc0133)]()
[![.NET](https://img.shields.io/badge/Microsoft_.NET_-9.0-3776AB.svg?logo=dotnet)]()

**![DOCVERSION](https://img.shields.io/badge/-000000.svg?logo=readthedocs) DOC-VERSION: 1.0 | 16/10/2025**

This markdown file contains the code standard rules for working in/on the new implementation of the MobyPark API. All code changes/creations must follow the rules set out in this document.

If you do not follow the rules in this document then all merge request will be denied until complaince is achieved
-ItsDanny


## Rules

### Models

Models are meant for the storage of data/values. ***They should not contain any or as little as possible methods***. Some models do contain methods because they work with data that should ***never*** be accessed outside of the object such as a **password**

When creating or changing a model you must implement everything according to the **iToTransform** interface (This interface can be found in: *MobyPark/src/interfaces/iToTransform.cs*)

the ***required*** methods include:
| Method | Return value     | Description                |
| :-------- | :------- | :------------------------- |
| `ToJSON()` | `JObject (Newtonsoft)` | Returns a JSON Object (Newtonsoft) based on the Model |
| `ToSQLInString()` | `string` | Returns a string that can be used for **inserting** into the database |
| `ToSQLInString()` | `string` | Returns a string that can be used for **updating** the object in the database |

When updating a private value in a model you must write a unsaved change log. This is so that in the event something goes wrong in a update we can easily research where the error occured.

---

Example:
```Example
    # This is an example from userrole.cs
    # File can be found in: MobyPark/src/models/userrole.cs

    public void UpdateRead(bool to)
    {
        Log.UnsavedChange("Userrole", ID, "READ Permission", ReadPermission, to);
        _writePermission = General.BoolToInt(to);
    }
```

## Document Authors

[ItsADanny](https://github.com/ItsADanny)
- [@ItsADanny - GitHub](https://github.com/ItsADanny)
- [Danny - Personal website](https://ddesnoo.nl)
- [Danny - Linkedin](https://www.linkedin.com/in/ddesnoo/)


## Fine print

1. Rules can be updated anytime and can only be updated by ItsDanny.
2. For any requested update to the rules please contact ItsDanny with your preposed rule (change).
