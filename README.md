<style type="text/css">
    pre > code {
        display: block;
        padding: .5em;
        background-color: #eee;
        border: solid 1px #bbb;
    }
</style>

NContrib
========

An attempt to extend the .NET framework with extension methods, helper classes, and new objects
to allow developers to write less code, more readable code, and use well-tested encapsulations of common functionality.
I want to write less code, declare fewer variables, and generally have a more fluid feel to the code. Workin' on that.

Background
----------

Nearly all functionality in NContrib was born out of my own personal need/desire whilst developing various applications,
so you may find unexpected gaps in functionality. That's just because I haven't had the need yet. I have tried to anticipate
some obvious needs, but there's no use going off on a tangent coding without purpose. I like all the additions to be considered,
necessary, and adding value.

Structure
---------

The solution is broken up into several projects

* NContrib - Targets .NET 3.5 and is the core projects
* NContrib.Drawing - Targets .NET 3.5 and provides helpers and extensions related to System.Drawing functionality
* NContrib.International - Targets .NET 3.5 and provides functionality for projects with multi-national reach
* NContrib4 - Targets .NET 4 and contains code that can only run on .NET 4, such as dynamics

And two projects for examples and unit testing

* NContrib.Examples
* NContrib.Tests

FAQ
---

**Q. Why are you using Q&R brace-styles?**

Because it is the right way of doing it ;) But in all seriousness, if it becomes a problem for would-be developers I'd be willing to use the .NET default

**Q. Can you add some functionality for me?**

This is an open-source project so you could fork it and issue a pull request.
But I understand not being comfortable with adding code. If you would like something added, sure, ask and we'll talk.

Examples
--------

(Documentation is far from complete)

### String extensions

**CamelCase**

    "transaction-id".CamelCase()
    => transactionId

**ContainsOnly**
    
    "XL".ContainsOnly('S', 'M', 'L', 'X')
    => true

**DigitAt**
    
    "070-605 91 23".DigitAt(1)
    => 7

**FromIndexOf**

    "S-21144".FromIndexOf("-")
    => 21144

**Join**
    
    new[] {"Sweden", "Denmark", "Norway",}.Join(", ")
    => Sweden, Denmark, Norway

**Inject**

    "Hello, {Name}. Today is {Today:dd-MM-yyyy}".Inject(new { Name = "Mike", Today = DateTime.Now })
    => Hello, Mike. Today is 21-04-2011
    
**IsBlank**

    " ".IsBlank()
    => true

    ((string)null).IsBlank()
    => true
    
    " ".IsBlank()
    => true

**IsEmpty**
    
    "".IsEmpty()
    => true
    
    " ".IsEmpty()
    => false

**IsDigits**

    "12345".IsDigits()
    => true

**IsLetters**
    
    "asd".IsLetters()
    => true

**IsLettersOrDigits**
    
    "S99".IsLettersOrDigits()
    => true

**IsNotBlank**
    
    "hello".IsNotBlank()
    => true

**IsNotEmpty**
    
    "haj".IsNotEmpty()
    => true
    
**Left**

    "hello".Left(2)
    => he

**Repeat**
    
    @"/!\".Repeat(5)
    => /!\/!\/!\/!\/!\

**Right**
    
    "there".Right(2)
    => re

**SnakeCase**
    
    "transactionId".SnakeCase()
    => transaction_id

**UntilIndexOf**
    
    "Boston, MA".UntilIndexOf(",")
    => Boston

**W**
    
    "id first_name last_name".W().Describe()
    => [id, first_name, last_name]

**Words**
    
    "Öresundståg-mot-Kastrup-Köpenhamn-Helsingør-84".Words().Describe()
    => [Öresundståg, mot, Kastrup, Köpenhamn, Helsingør, 84]
    
    
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean lectus sem, facilisis convallis vestibulum vel, tincidunt nec dolor.".Wrap(40)
    =>
    Lorem ipsum dolor sit amet,
    consectetur adipiscing elit. Aenean
    lectus sem, facilisis convallis
    vestibulum vel, tincidunt nec dolor.
    
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean lectus sem, facilisis convallis vestibulum vel, tincidunt nec dolor.".Wrap(40, TextWrapMethod.HardBreakAlways)
    =>
    Lorem ipsum dolor sit amet, consectetur
    adipiscing elit. Aenean lectus sem, faci
    lisis convallis vestibulum vel, tincidun
    t nec dolor.

### DateTime Extensions

**ToTimeZone**

    var date = DateTime.UtcNow
    => 2011-04-08 20:11:37
    
    date.ToTimeZone("Tokyo Standard Time")
    => 2011-04-09 05:11:37

### Object extensions

**ConvertTo**
    
    "123.45".ConvertTo<decimal>(CultureInfo.InvariantCulture)
    => 123.45
    
    "False".ConvertTo<bool>()
    => False

**In**

    4.In(1, 2, 3, 4, 5)
    => true
    
    "Dave".In("Carter", "Dave", "Steffan")
    => true


### Working with a data result

Without NContrib, this is how you might connect to a database, execute a procedure, and read its results into a dictionary

    var currencies = new Dictionary<string, decimal>();
    
    using (var cn = new SqlConnection("server=myserver;database=resources;uid=services;pwd=passw0rd;encrypt=yes"))
    using (var cmd = new SqlCommand("Get_Currency_Conversions", cn)) {
        cn.Open();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("currency", "EUR");
        cmd.Parameters.AddWithValue("amount", 123.45m);
        using (var dr = cmd.ExecuteReader()) {        
            while (dr.Read()) {
                currencies.Add(dr.GetString(dr.GetOrdinal("name")),
                                dr.GetDecimal(dr.GetOrdinal("rate")));
            }
        }
    }
    
Using some helper objects and extension methods in NContrib, you can do the same thing like this:

    var currencies = SqlUri.OpenConnection("mssqls://services:passw0rd@localhost/resources")
        .CreateCommand("Get_Currency_Conversions", CommandType.StoredProcedure)
        .AddParameters(new { Currency = "EUR", Amount = 123.45m })
        .ExecuteReader()
        .Transform(dr => new {
            CurrencyCode = dr.GetValue<string>("name"),
            Rate = dr.GetValue<decimal>("rate"),
        })
        .ToDictionary(x => x.CurrencyCode, x => x.Rate);

Better? Yes, we can do better.

Since there's an extension method for executing a stored procedure and returning a dictionary, we could do this instead:

    var currencies = SqlUri.OpenConnection("mssqls://services:passw0rd@localhost/resources")
        .CreateCommand("Get_Currency_Conversions", CommandType.StoredProcedure)
        .AddParameters(new { Currency = "EUR", Amount = 123.45m })
        .ExecuteDictionary<string, decimal>("name", "rate");

### MIME

    MimeHelper.GetMimeFromFileName("data.docx")
    => application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
    
    MimeHelper.GetMimeFromBytes(@"c:\temp\a-jpg-without-a-file-ext");
    => image/jpeg

### Luhn checksum and generation
    
    Luhn.IsValid("354957031609855")
    => true
    
    Luhn.Generate(16, "4581")
    => 4581555790158762

### Range
    
    var r = new Range<int>(10, 20)
    r.Includes(5)
    => true
    
    r.Includes(20)
    => true
    
    var r = new Range<int>(10, 20, minInclusive: true, maxInclusive: false)
    r.Includes(10)
    => true
    
    r.Includes(19)
    => true
    
    r.Includes(20)
    => false