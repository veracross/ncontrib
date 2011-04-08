NContrib
========

An attempt to extend the .NET framework with extension methods, helper classes, and new objects
to allow developers to write less code, more readable code, and use well-tested encapsulations of common functionality

Examples
--------

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
        .AddParameters(new { Currency = currency, Amount = amount })
        .ExecuteReader()
        .Select(dr => new {
            CurrencyCode = dr.GetValue<string>("name"),
            Rate = dr.GetValue<decimal>("rate"),
        })
        .ToDictionary(x => x.CurrencyCode, x => x.Rate);

Better? Nearly.

Since there's an extension method for executing a stored procedure and returning a dictionary, we could do this instead:

    var currencies = SqlUri.OpenConnection("mssql://localhost/resources")
        .CreateCommand("Get_Currency_Conversions", CommandType.StoredProcedure)
        .AddParameters(new { Currency = currency, Amount = amount })
        .ExecuteDictionary<string, decimal>("name", "rate");