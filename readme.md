# OFXSharp

[![Build+Test](https://github.com/jcoliz/OFXSharp/actions/workflows/build+test.yml/badge.svg)](https://github.com/jcoliz/OFXSharp/actions/workflows/build+test.yml)

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

## Maintainer History

This code has quite a history of forks and multiple mainainters releasing it over time!

* [James Hollingworth](https://github.com/jhollingworth) is the originator. https://github.com/jhollingworth/OFXSharp
* [Antonio Milesi Bastos](https://github.com/milesibastos) released the most popular package on NuGet in 2014. https://www.nuget.org/packages/OFXSharp/
* [Keven Carneiro](https://github.com/kevencarneiro) brought it over to NET Standard, making it look closer to the current form, in 2017. He released the most recent NuGet in 2018 https://www.nuget.org/packages/OfxSharp.NetStandard/
* [Dai Rees](https://github.com/Jehoel) made a lot of fundamental improvements and cleanups. Unfortunately does not seem to have released to NuGet.
* [James Coliz](https://github.com/jcoliz/) is now cleaning up and bringing current Dai's work, with intent to release the latest code to NuGet.