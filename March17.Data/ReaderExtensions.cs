using System;
using System.Data.SqlClient;

public static class ReaderExtensions
{
    public static T Get<T>(this SqlDataReader reader, string name)
    {
        object value = reader[name];
        if (value == DBNull.Value)
        {
            return default(T);
        }

        return (T)value;
    }
}