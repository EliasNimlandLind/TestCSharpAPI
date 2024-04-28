namespace Backend;

public static class Result
{
    private static Obj RowModifier(dynamic row)
    {
        // If null then change the result to error Not Found
        row = row != null ? row :
            new DynObject(new { error = "Not found." });
        // Delete the password
        row.Delete("password");
        // JSON.parse all fields called "data"
        if (row.HasKey("data"))
        {
            row.data = JSON.Parse(row.data);
        }
        return row;
    }

    public static IResult encode(dynamic result)
    {
        int statusCode = 200;

        if (result is Arr arr)
        {
            result = arr.Map(x => RowModifier(x));
        }
        else
        {
            dynamic r = result == null ? null! : Obj(result);
            // 500 = Internal Server Error
            // 404 = Not found
            // 200 = OK
            statusCode =
                result == null ? 404 :
                r.HasKey("error") ? 500 :
                r.HasKey("rowsAffected") && r.GetInt("rowsAffected") == 0 ? 404 :
                200;

            result = RowModifier(r);
        }

        return Results.Text(
          JSON.Stringify(result),
          "application/json; charset=utf-8",
          null,
          statusCode
        );
    }
}