# OFXSharp
[![NuGet](https://buildstats.info/nuget/OfxSharp.NetStandard)](http://www.nuget.org/packages/OfxSharp.NetStandard)

This library is a port from the original OFXParser to .NET Standard.

OFXParser is a library to parse OFX Files into plain C# objects, with some small changed done to handle OFX Files used by Brazilian banks, that don't strict follow the OFX pattern.

If you have any request, fell free to open a issue.

## How to use

```C#
var parser = new OFXDocumentParser();
var ofxDocument = parser.Import(new FileStream(@"c:\ofxdoc.ofx", FileMode.Open));
```

This will give you an object of type `OFXDocument`, with the following properties:

```C#
public class OFXDocument
{
    public DateTime? StatementStart { get; set; }

    public DateTime? StatementEnd { get; set; }

    public AccountType AccType { get; set; }

    public string Currency { get; set; }

    public SignOn SignOn { get; set; }

    public Account Account { get; set; }

    public Balance Balance { get; set; }

    public List<Transaction> Transactions { get; set; }
}
```
