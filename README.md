# Our.Umbraco.HoneyPot

A basic Honeypot to trap bots from posting your forms, it can be used with Umbraco Forms or on custom forms.
The idea is simple, add a random hidden field, if the field is populated and posted = it's probably a bot.

The logic is based on: https://github.com/usercode/AspNetCore.Honeypot

> In computer terminology, a honeypot is a computer security mechanism set to detect, deflect, or, in some manner, counteract attempts at unauthorized use of information systems. Generally, a honeypot consists of data (for example, in a network site) that appears to be a legitimate part of the site which contains information or resources of value to attackers. It is actually isolated, monitored, and capable of blocking or analyzing the attackers. This is similar to police sting operations, colloquially known as "baiting" a suspect.[1]




### Install with Umbraco Forms installed (V8, V9, V10)
This will install the Core version and a new field in Umbraco Forms.

```
V9+: Install-Package Out.Umbraco.Honeypot
V8: Install-Package Our.Umbraco.HoneyPot.V8

OR

V9+: dotnet add package Out.Umbraco.Honeypot
```


### Install without Umbraco Forms (V8, V9, V10)


```
V8+: Install-Package Out.Umbraco.Honeypot.Core

OR

dotnet add package Out.Umbraco.Honeypot.Core
```


#### Usage
In your Umbraco Forms form, you will add the new field "Honeypot Field", the rest is automatic.

Or

Custom usage (.net 5+):

```
@addTagHelper *, Our.Umbraco.Honeypot.Core


<honeypot-field />
<honeypot-time />

```

Valdation:

```
[HttpPost]
public async Task<IActionResult> Form(FormModel model)
{
    if (HttpContext.IsHoneypotTrapped())
    {
        //Log error
        return View("Form", model);
    }
}
```


### Global settings

```
  "Our.Umbraco.Honeypot": {
    "HoneypotEnableFieldCheck": true, 
    "HoneypotEnableTimeCheck": true,
    "HoneypotPrefixFieldName": "hp_",
    "HoneypotSuffixFieldName": "",
    "HoneypotTimeFieldName": "__time",
    "HoneypotMinTimeDuration": "00:02:00,
    "HoneypotFieldStyles": "display: none !important; position: absolute !important; left: -9000px !important;",
    "HoneypotFieldClass": "hp-field",
    "HoneypotFieldNames": [ "Name", "Phone", "Comment", "Message", "Email", "Website" ],
    "HoneypotMessage": "Something went wrong (HP)"
  }
```
