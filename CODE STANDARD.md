![HR.MobyPark - Banner](https://github.com/ItsADanny/HR.MobyPark/blob/template/HR__Banner.png?raw=true)

# HR.Mobypark - Code standard rules

This markdown file contains the code standard rules for working in/on the new implementation of the MobyPark API. All code changes/creations must follow the rules set out in this document.

If you do not follow the rules in this document then all merge request will be denied until complaince is achieved
-ItsDanny


## Rules

### Models

When creating or changing a model you must implement everything according to the iToTransform interface (This interface can be found in: *MobyPark/src/interfaces/iToTransform.cs*)

the required methods include:
| Method | Return value     | Description                |
| :-------- | :------- | :------------------------- |
| `ToJSON()` | `JObject (Newtonsoft)` | Returns a JSON Object (Newtonsoft) based on the Model |
| `ToSQLInString()` | `string` | Returns a string that can be used for **inserting** into the database |
| `ToSQLInString()` | `string` | Returns a string that can be used for **updating** the object in the database |

## Document Authors

[ItsADanny](https://github.com/ItsADanny)
- [@ItsADanny - GitHub](https://github.com/ItsADanny)
- [Danny - Personal website](https://ddesnoo.nl)
- [Danny - Linkedin](https://www.linkedin.com/in/ddesnoo/)


## Fine print

1. Rules can be updated anytime and can only be updated by ItsDanny.
2. For any requested update to the rules please contact ItsDanny with your preposed rule (change).
