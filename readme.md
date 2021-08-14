# OFXSharp
[![NuGet](https://buildstats.info/nuget/OfxSharp.NetStandard)](http://www.nuget.org/packages/OfxSharp.NetStandard)

This library is a port from the original OFXParser to .NET Standard.

OFXParser is a library to parse OFX Files into plain C# objects, with some small changed done to handle OFX Files used by Brazilian banks, that don't strict follow the OFX pattern.

If you have any request, fell free to open a issue.

## How to use

```C#
OfxDocument ofx = OfxDocumentReader.FromSgmlFile( filePath: @"c:\ofxdoc.ofx" );
OfxStatementResponse statement = ofx.Statements.First();
```

This will give you an object of type `OfxStatementResponse`, with the following properties:

```C#
public class OfxStatementResponse
{
    public OfxStatus ResponseStatus { get; }

    public String DefaultCurrency { get; }

    public Account AccountFrom { get; }

    public DateTimeOffset TransactionsStart { get; }

    public DateTimeOffset TransactionsEnd   { get; }

    public List<Transaction> Transactions { get; }

    public Balance LedgerBalance { get; }

    public Balance AvailableBalance { get; }
}
```
