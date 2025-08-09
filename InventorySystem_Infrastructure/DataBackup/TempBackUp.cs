using Npgsql;
using System;
using System.Data;
using System.Text;

public class TempBackUp
{
    public void Temp(string connectionString)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            StringBuilder scriptBuilder = new();

            GenerateTableCreationScripts(connection, scriptBuilder, connectionString);

            Console.WriteLine(scriptBuilder.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private static void GenerateTableCreationScripts(NpgsqlConnection connection, StringBuilder scriptBuilder, string connString)
    {
        var tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

        using var cmd = new NpgsqlCommand(tablesQuery, connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var tableName = reader.GetString(0);
            Console.WriteLine($"Generating creation script for table: {tableName}");

            // Get Primary Key Column
            string? primaryKeyColumn = GetPrimaryKeyColumn(connString, tableName);

            // Get the columns for the current table
            var createTableQuery = $@"
            SELECT column_name, data_type, character_maximum_length, column_default, is_nullable
            FROM information_schema.columns 
            WHERE table_name = '{tableName}' ORDER BY ordinal_position ASC";

            using var newConn = new NpgsqlConnection(connString);
            newConn.Open();
            using var createCmd = new NpgsqlCommand(createTableQuery, newConn);
            using var createReader = createCmd.ExecuteReader();
            scriptBuilder.AppendLine($"DROP TABLE IF EXISTS {tableName} CASCADE; CREATE TABLE {tableName} (");

            bool firstColumn = true;

            // Loop through the columns and generate the appropriate scripts
            while (createReader.Read())
            {
                if (!firstColumn)
                {
                    scriptBuilder.AppendLine(",");
                }
                firstColumn = false;

                var columnName = createReader.GetString(0);
                var dataType = createReader.GetString(1);
                var maxLength = createReader.IsDBNull(2) ? string.Empty : $"({createReader.GetInt32(2)})";
                var columnDefault = createReader.IsDBNull(3) ? string.Empty : createReader.GetString(3);
                var isNullable = createReader.GetString(4) == "YES" ? "NULL" : "NOT NULL";

                // Add primary key first if the column is part of the primary key
                if (primaryKeyColumn == columnName)
                {
                    scriptBuilder.Append($"    {columnName} INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY");
                }
                else
                {
                    scriptBuilder.Append($"    {columnName} {dataType}{maxLength} {isNullable}");
                    if (!string.IsNullOrEmpty(columnDefault))
                    {
                        scriptBuilder.Append($" DEFAULT {columnDefault}");
                    }
                }
            }

            // Add Primary Key constraint if necessary
            AddPrimaryKeyConstraint(scriptBuilder, tableName, primaryKeyColumn);

            // Add Foreign Keys
            AddForeignKeys(connString, scriptBuilder, tableName);

            // Add Unique Constraints
            AddUniqueConstraints(connString, scriptBuilder, tableName);

            // Add Check Constraints
            AddCheckConstraints(connString, scriptBuilder, tableName);

            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine(");");
            scriptBuilder.AppendLine();
        }
    }


    //private static void GenerateTableCreationScripts(NpgsqlConnection connection, StringBuilder scriptBuilder, string connString)
    //{
    //    var tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

    //    using var cmd = new NpgsqlCommand(tablesQuery, connection);
    //    using var reader = cmd.ExecuteReader();
    //    while (reader.Read())
    //    {
    //        var tableName = reader.GetString(0);
    //        Console.WriteLine($"Generating creation script for table: {tableName}");

    //        // Get the columns for the current table
    //        var createTableQuery = $@"
    //                SELECT column_name, data_type, character_maximum_length, column_default, is_nullable
    //                FROM information_schema.columns 
    //                WHERE table_name = '{tableName}' ORDER BY ordinal_position ASC";

    //        using var createCmd = new NpgsqlCommand(createTableQuery, connection);
    //        using var createReader = createCmd.ExecuteReader();
    //        scriptBuilder.AppendLine($"DROP TABLE IF EXISTS {tableName} CASCADE; CREATE TABLE {tableName} (");

    //        bool firstColumn = true;
    //        string? primaryKeyColumn = GetPrimaryKeyColumn(connString, tableName);

    //        // Loop through the columns and generate the appropriate scripts
    //        while (createReader.Read())
    //        {
    //            if (!firstColumn)
    //            {
    //                scriptBuilder.AppendLine(",");
    //            }
    //            firstColumn = false;

    //            var columnName = createReader.GetString(0);
    //            var dataType = createReader.GetString(1);
    //            var maxLength = createReader.IsDBNull(2) ? string.Empty : $"({createReader.GetInt32(2)})";
    //            var columnDefault = createReader.IsDBNull(3) ? string.Empty : createReader.GetString(3);
    //            var isNullable = createReader.GetString(4) == "YES" ? "NULL" : "NOT NULL";

    //            if (primaryKeyColumn == columnName)
    //            {
    //                scriptBuilder.Append($"    {columnName} INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY");
    //            }
    //            else
    //            {
    //                scriptBuilder.Append($"    {columnName} {dataType}{maxLength} {isNullable}");
    //                if (!string.IsNullOrEmpty(columnDefault))
    //                {
    //                    scriptBuilder.Append($" DEFAULT {columnDefault}");
    //                }
    //            }
    //        }

    //        // Add Primary Key constraint if necessary
    //        AddPrimaryKeyConstraint(scriptBuilder, tableName, primaryKeyColumn);

    //        // Add Foreign Keys
    //        AddForeignKeys(connString, scriptBuilder, tableName);

    //        // Add Unique Constraints
    //        AddUniqueConstraints(connString, scriptBuilder, tableName);

    //        // Add Check Constraints
    //        AddCheckConstraints(connString, scriptBuilder, tableName);

    //        scriptBuilder.AppendLine();
    //        scriptBuilder.AppendLine(");");
    //        scriptBuilder.AppendLine();
    //    }
    //}

    private static string? GetPrimaryKeyColumn(string connectionString, string tableName)
    {
        string pkQuery = $@"
            SELECT column_name
            FROM information_schema.key_column_usage
            WHERE table_name = @tableName AND constraint_name = 'PRIMARY'";

        NpgsqlConnection npgsqlConnection = new(connectionString);
        npgsqlConnection.Open();

        using var pkCmd = new NpgsqlCommand(pkQuery, npgsqlConnection);
        pkCmd.Parameters.AddWithValue("tableName", tableName);
        using var pkReader = pkCmd.ExecuteReader();
        if (pkReader.Read())
        {
            return pkReader.GetString(0);
        }

        return null;
    }

    private static void AddPrimaryKeyConstraint(StringBuilder scriptBuilder, string tableName, string? primaryKeyColumn)
    {
        if (primaryKeyColumn != null)
        {
            scriptBuilder.AppendLine($",    CONSTRAINT {tableName}_pkey PRIMARY KEY ({primaryKeyColumn})");
        }
    }

    private static void AddForeignKeys(string connectionString, StringBuilder scriptBuilder, string tableName)
    {
        string fkQuery = $@"
            SELECT conname, pg_catalog.pg_get_constraintdef(r.oid)
            FROM pg_catalog.pg_constraint r
            INNER JOIN pg_catalog.pg_namespace n ON n.oid = r.connamespace
            WHERE conrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname = @tableName)
            AND n.nspname = 'public' AND r.contype = 'f'";

        NpgsqlConnection npgsqlConnection = new(connectionString);
        npgsqlConnection.Open();

        using var fkCmd = new NpgsqlCommand(fkQuery, npgsqlConnection);
        fkCmd.Parameters.AddWithValue("tableName", tableName);
        using var fkReader = fkCmd.ExecuteReader();
        while (fkReader.Read())
        {
            var constraintName = fkReader.GetString(0);
            var constraintDef = fkReader.GetString(1);
            scriptBuilder.AppendLine($",    CONSTRAINT {constraintName} {constraintDef}");
        }
    }

    private static void AddUniqueConstraints(string connectionString, StringBuilder scriptBuilder, string tableName)
    {
        string uniqueQuery = $@"
            SELECT conname, pg_catalog.pg_get_constraintdef(r.oid)
            FROM pg_catalog.pg_constraint r
            INNER JOIN pg_catalog.pg_namespace n ON n.oid = r.connamespace
            WHERE contype = 'u' AND conrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname = @tableName)
            AND n.nspname = 'public'";

        NpgsqlConnection npgsqlConnection = new(connectionString);
        npgsqlConnection.Open();

        using var uniqueCmd = new NpgsqlCommand(uniqueQuery, npgsqlConnection);
        uniqueCmd.Parameters.AddWithValue("tableName", tableName);
        using var uniqueReader = uniqueCmd.ExecuteReader();
        while (uniqueReader.Read())
        {
            var constraintName = uniqueReader.GetString(0);
            var constraintDef = uniqueReader.GetString(1);
            scriptBuilder.AppendLine($",    CONSTRAINT {constraintName} {constraintDef}");
        }
    }

    private static void AddCheckConstraints(string connectionString, StringBuilder scriptBuilder, string tableName)
    {
        string checkQuery = $@"
            SELECT conname, pg_catalog.pg_get_constraintdef(r.oid)
            FROM pg_catalog.pg_constraint r
            INNER JOIN pg_catalog.pg_namespace n ON n.oid = r.connamespace
            WHERE contype = 'c' AND conrelid = (SELECT oid FROM pg_catalog.pg_class WHERE relname = @tableName)
            AND n.nspname = 'public'";

        NpgsqlConnection npgsqlConnection = new(connectionString);
        npgsqlConnection.Open();

        using var checkCmd = new NpgsqlCommand(checkQuery, npgsqlConnection);
        checkCmd.Parameters.AddWithValue("tableName", tableName);
        using var checkReader = checkCmd.ExecuteReader();
        while (checkReader.Read())
        {
            var constraintName = checkReader.GetString(0);
            var constraintDef = checkReader.GetString(1);
            scriptBuilder.AppendLine($",    CONSTRAINT {constraintName} {constraintDef}");
        }
    }
}
