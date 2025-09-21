using Npgsql;
using System.Text;

namespace InventorySystem_Infrastructure.DataBackup
{
    public static class PostgresBackup
    {
        public static async Task<StringBuilder> GenerateBackup(
            //string backupFilePath, 
            string connectionString)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Connected to the database.");

                var scriptBuilder = new StringBuilder();

                // Step 1: Generate Table Creation Scripts
                GenerateTableCreationScripts(connection, scriptBuilder, connectionString);

                // Step 2: Generate Index Scripts
                GenerateIndexScripts(connection, scriptBuilder);

                // Step 3: Generate Foreign Key Constraints Scripts
                //GenerateForeignKeyScriptsAsync(connection, scriptBuilder, connectionString).GetAwaiter().GetResult();

                await GenerateFKAndCompositePKScriptsAsync(connection, scriptBuilder, connectionString);

                // Step 4: Generate Primary Key Constraints Scripts (after foreign key constraints)
                //GeneratePrimaryKeyScripts(connectionString, scriptBuilder);  // Ensure this is done sequentially

                // Step 5: Generate Insert Scripts for Data
                GenerateInsertScripts(connection, scriptBuilder, connectionString);

                scriptBuilder.AppendLine();
                scriptBuilder.AppendLine("-- ======================");
                scriptBuilder.AppendLine("-- Align all identity sequences");
                scriptBuilder.AppendLine("-- ======================");
                scriptBuilder.AppendLine(@"DO $$
                    DECLARE
                        rec RECORD;
                                BEGIN
                                    FOR rec IN  SELECT table_name, column_name
                                    FROM information_schema.columns WHERE is_identity = 'YES' AND table_schema = 'public'
                                LOOP
                                    EXECUTE format(
                                        'SELECT setval(pg_get_serial_sequence(''%I.%I'', ''%I''), COALESCE((SELECT MAX(%I) FROM %I.%I),1))',
                                        'public', rec.table_name, rec.column_name, rec.column_name, 'public', rec.table_name);
                                END LOOP;
                    END $$;");

                // Save to file
                //File.WriteAllText(backupFilePath, scriptBuilder.ToString());
                //Console.WriteLine($"Backup script generated: {backupFilePath}");
                return scriptBuilder;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private static void GenerateTableCreationScripts(NpgsqlConnection connection, StringBuilder scriptBuilder, string conn)
        {
            var tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

            using (var cmd = new NpgsqlCommand(tablesQuery, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var tableName = reader.GetString(0);
                    Console.WriteLine($"Generating creation script for table: {tableName}");

                    // Open a separate connection for column definitions
                    using (var newConnection = new NpgsqlConnection(conn))
                    {
                        newConnection.Open();

                        // Get the columns for the current table
                        var createTableQuery = $@"
                                SELECT column_name, data_type, character_maximum_length, column_default
                                FROM information_schema.columns 
                                WHERE table_name = '{tableName}'  ORDER  BY ORDINAL_POSITION ASC";

                        using (var createCmd = new NpgsqlCommand(createTableQuery, newConnection))
                        using (var createReader = createCmd.ExecuteReader())
                        {
                            scriptBuilder.AppendLine($"DROP TABLE IF EXISTS {tableName} CASCADE; CREATE TABLE {tableName} (");

                            bool firstColumn = true;
                            string? primaryKeyColumn = GetPrimaryKeyIdentityColumn(conn, tableName);
                            bool hasIdentityColumn = false;

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

                                Console.WriteLine($"Column: {columnName}, DataType: {dataType}, MaxLength: {maxLength}");

                                // Check if the column is intended as the primary key
                                if (primaryKeyColumn is not null && columnName == primaryKeyColumn && !hasIdentityColumn)
                                {
                                    // Get the primary key dynamically
                                    scriptBuilder.Append($"    {primaryKeyColumn} INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY");
                                    hasIdentityColumn = true;
                                }
                                else
                                {
                                    scriptBuilder.Append($"    {columnName} {dataType}{maxLength}");

                                    // Add the default value if it exists
                                    if (!string.IsNullOrEmpty(columnDefault))
                                    {
                                        scriptBuilder.Append($" DEFAULT {columnDefault}");
                                    }
                                }
                            }

                            // Add primary key constraint if no primary key column is found
                            //if (primaryKeyColumn != null)
                            //{
                            //    scriptBuilder.AppendLine($",    CONSTRAINT {tableName}_pkey PRIMARY KEY ({primaryKeyColumn})");
                            //}

                            scriptBuilder.AppendLine();
                            scriptBuilder.AppendLine(");");
                            scriptBuilder.AppendLine();
                        }
                    }
                }
            }
        }


        // Helper method to get the primary key column dynamically
        private static string? GetPrimaryKeyIdentityColumn(string connection, string tableName)
        {
            var query = $@"
                    SELECT kc.column_name
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kc
                        ON kc.constraint_name = tc.constraint_name
                        AND kc.table_name = tc.table_name
                    JOIN information_schema.columns c
                        ON c.table_name = kc.table_name
                        AND c.column_name = kc.column_name
                    WHERE tc.table_name = '{tableName}'
                      AND tc.constraint_type = 'PRIMARY KEY'
                      AND c.is_identity = 'YES'";

            using var newConnection = new NpgsqlConnection(connection);
            newConnection.Open();

            using var cmd = new NpgsqlCommand(query, newConnection);
            using var reader = cmd.ExecuteReader();
            {
                if (reader.Read())
                {
                    return reader.GetString(0);  // Return the PK column name if it's also identity
                }
            }

            return null;  // Return null if not found
        }




        //private void GenerateTableCreationScripts(NpgsqlConnection connection, StringBuilder scriptBuilder, string conn)
        //{
        //    var tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

        //    using (var cmd = new NpgsqlCommand(tablesQuery, connection))
        //    using (var reader = cmd.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            var tableName = reader.GetString(0);
        //            Console.WriteLine($"Generating creation script for table: {tableName}");

        //            // Open a separate connection for column definitions
        //            using (var newConnection = new NpgsqlConnection(conn))
        //            {
        //                newConnection.Open();

        //                // Get the columns for the current table
        //                var createTableQuery = $@"
        //                    SELECT column_name, data_type, character_maximum_length
        //                    FROM information_schema.columns 
        //                    WHERE table_name = '{tableName}'";

        //                using (var createCmd = new NpgsqlCommand(createTableQuery, newConnection))
        //                using (var createReader = createCmd.ExecuteReader())
        //                {
        //                    scriptBuilder.AppendLine($"CREATE TABLE {tableName} (");

        //                    bool firstColumn = true;
        //                    string? primaryKeyColumn = null;
        //                    while (createReader.Read())
        //                    {
        //                        if (!firstColumn)
        //                        {
        //                            scriptBuilder.AppendLine(",");
        //                        }
        //                        firstColumn = false;

        //                        var columnName = createReader.GetString(0);
        //                        var dataType = createReader.GetString(1);
        //                        var maxLength = createReader.IsDBNull(2) ? string.Empty : $"({createReader.GetInt32(2)})";

        //                        // Check if the column is an integer and should be an identity column
        //                        if (dataType == "integer" && primaryKeyColumn == null)
        //                        {
        //                            // Mark the first integer column as GENERATED ALWAYS AS IDENTITY and PRIMARY KEY
        //                            scriptBuilder.Append($"    {columnName} INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY");
        //                            primaryKeyColumn = columnName;
        //                        }
        //                        else
        //                        {
        //                            scriptBuilder.Append($"    {columnName} {dataType}{maxLength}");
        //                        }
        //                    }

        //                    // Add primary key constraint for the column if it's not already set
        //                    if (primaryKeyColumn == null)
        //                    {
        //                        AddPrimaryKeyConstraint(tableName, conn, scriptBuilder);
        //                    }

        //                    scriptBuilder.AppendLine();
        //                    scriptBuilder.AppendLine(");");
        //                    scriptBuilder.AppendLine();
        //                }
        //            }
        //        }
        //    }
        //}

        // Method to retrieve and add primary key constraint
        private static void AddPrimaryKeyConstraint(string tableName, string conn, StringBuilder scriptBuilder)
        {
            // Open a new connection for primary key constraint query
            using (var newConnection = new NpgsqlConnection(conn))
            {
                newConnection.Open();

                // Query to get primary key constraint and column numbers
                var pkQuery = @"
                    SELECT c.conname, a.attname
                    FROM pg_constraint c
                    JOIN pg_attribute a ON a.attnum = ANY(c.conkey)
                    WHERE c.conrelid = (SELECT oid FROM pg_class WHERE relname = @tableName)
                    AND c.contype = 'p'";

                using (var cmd = new NpgsqlCommand(pkQuery, newConnection))
                {
                    cmd.Parameters.AddWithValue("tableName", tableName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            scriptBuilder.AppendLine($"    CONSTRAINT pk_{tableName} PRIMARY KEY (");

                            var pkColumns = new List<string>();

                            while (reader.Read())
                            {
                                // Add the actual column names for the primary key
                                string columnName = reader.GetString(1);
                                pkColumns.Add(columnName);
                            }

                            // Only add primary key constraint if valid columns are found
                            if (pkColumns.Count > 0)
                            {
                                scriptBuilder.Append(string.Join(", ", pkColumns));
                                scriptBuilder.AppendLine(")");
                            }
                            else
                            {
                                Console.WriteLine($"Warning: No valid columns found for primary key '{tableName}'.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: No primary key found for table '{tableName}'.");
                        }
                    }
                }
            }
        }
        private static void GenerateIndexScripts(NpgsqlConnection connection, StringBuilder scriptBuilder)
        {
            var indexesQuery = @"
        SELECT indexname, indexdef 
        FROM pg_indexes
        WHERE schemaname = 'public'";

            using var cmd = new NpgsqlCommand(indexesQuery, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var indexName = reader.GetString(0);
                var indexDef = reader.GetString(1);

                Console.WriteLine($"Generating index script for index: {indexName}");

                // Skip Primary Key indexes (typically named 'table_name_pkey')
                if (indexName.EndsWith("_pkey"))
                {
                    Console.WriteLine($"Skipping primary key index: {indexName}");
                    continue;
                }

                // Ensure unique index for user_name column
                if (indexDef.Contains("user_name") && !indexDef.Contains("UNIQUE"))
                {
                    // If the index definition doesn't already have UNIQUE, make sure it's unique
                    Console.WriteLine("Ensuring unique constraint on user_name.");
                    indexDef = $"CREATE UNIQUE INDEX {indexName} ON {indexDef.Substring(indexDef.IndexOf("ON") + 3)};";
                }

                // Ensure the index definition ends with a semicolon
                if (!indexDef.EndsWith(";"))
                {
                    indexDef += ";";
                }

                scriptBuilder.AppendLine(indexDef);
            }
        }

        private static void GeneratePrimaryKeyScripts(string conn, StringBuilder scriptBuilder)
        {
            // Open a new connection specifically for primary key query
            using (var newConnection = new NpgsqlConnection(conn))
            {
                newConnection.Open();

                // Get all primary keys in the schema
                var pkQuery = @"
            SELECT t.relname as table_name, c.conname as constraint_name, c.conkey as column_numbers
            FROM pg_constraint c
            JOIN pg_class t ON t.oid = c.conrelid
            WHERE c.contype = 'p' AND t.relnamespace = 'public'::regnamespace";

                using var cmd = new NpgsqlCommand(pkQuery, newConnection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tableName = reader.GetString(0);
                    var constraintName = reader.GetString(1);
                    var columnNumbers = reader["column_numbers"] as short[];

                    // Log the retrieved values for debugging purposes
                    Console.WriteLine($"Primary Key Constraint: {constraintName} for Table: {tableName}");

                    // Check if columnNumbers is null or empty
                    if (columnNumbers == null || columnNumbers.Length == 0)
                    {
                        // If columnNumbers are missing, log and throw a detailed exception
                        Console.WriteLine($"Warning: No column numbers found for primary key '{constraintName}' on table '{tableName}'.");
                        throw new InvalidOperationException($"The column numbers for primary key '{constraintName}' could not be retrieved.");
                    }

                    // Open a separate connection specifically for column names query
                    using (var columnConnection = new NpgsqlConnection(conn))
                    {
                        columnConnection.Open();

                        // Get the column names for the primary key
                        var columnNamesQuery = @"
                    SELECT a.attname
                    FROM pg_attribute a
                    WHERE a.attnum = ANY(@columnNumbers) AND a.attrelid = (SELECT oid FROM pg_class WHERE relname = @tableName)";

                        var columnNames = new List<string>();

                        using var columnCmd = new NpgsqlCommand(columnNamesQuery, columnConnection);

                        // Bind columnNumbers as an array type
                        var columnNumbersParam = new NpgsqlParameter("columnNumbers", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Smallint)
                        {
                            Value = columnNumbers
                        };

                        columnCmd.Parameters.Add(columnNumbersParam);
                        columnCmd.Parameters.AddWithValue("tableName", tableName);

                        // Ensure the query finishes before proceeding to the next one
                        using (var columnReader = columnCmd.ExecuteReader())
                        {
                            while (columnReader.Read())
                            {
                                columnNames.Add(columnReader.GetString(0));
                            }
                        }

                        // If no columns are retrieved, log the issue
                        if (columnNames.Count == 0)
                        {
                            Console.WriteLine($"Warning: No columns found for primary key '{constraintName}' on table '{tableName}'.");
                            throw new InvalidOperationException($"No columns found for primary key '{constraintName}' on table '{tableName}'.");
                        }

                        // Format the column names correctly in the ALTER TABLE statement
                        var columnNamesString = string.Join(", ", columnNames);
                        string sql = $"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({columnNamesString});";

                        // Log the generated SQL
                        Console.WriteLine($"Generated SQL: {sql}");

                        scriptBuilder.AppendLine($"-- Primary Key {constraintName}");
                        scriptBuilder.AppendLine(sql);
                    }
                }
            }
        }
        //private async Task GenerateForeignKeyScriptsAsync(NpgsqlConnection connection, StringBuilder scriptBuilder, string conString)
        //{
        //    var fkQuery = @"
        //SELECT conname, condeferrable, condeferred, pg_get_constraintdef(oid), conrelid
        //FROM pg_constraint
        //WHERE connamespace = 'public'::regnamespace";  // Ensure it's looking in the 'public' schema

        //    using var cmd = new NpgsqlCommand(fkQuery, connection);
        //    using var reader = await cmd.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        var constraintName = reader.GetString(0);  // Name of the constraint (FK, PK, etc.)
        //        var constraintDef = reader.GetString(3);  // The full constraint definition (e.g., column(s) involved)

        //        // The table OID is fetched from the 'conrelid' column.
        //        var tableOid = reader.GetValue(4);  // This is the OID of the table involved in the constraint

        //        Console.WriteLine($"Generating script for constraint: {constraintName}");

        //        // Convert the tableOid to long (you could use int or uint depending on your DB)
        //        long tableOidLong = Convert.ToInt64(tableOid);  // Ensure you're using the correct type

        //        // Get the table name using the OID asynchronously
        //        var tableName = await GetTableNameFromOidAsync(conString, tableOidLong);

        //        // Generate the constraint script based on the definition
        //        string script = GenerateConstraintScriptFromDef(constraintDef, tableName, constraintName);

        //        // Append the script to the StringBuilder
        //        scriptBuilder.AppendLine($"-- Constraint {constraintName}");
        //        scriptBuilder.AppendLine(script);
        //    }
        //}


        private static async Task GenerateFKAndCompositePKScriptsAsync(NpgsqlConnection connection, StringBuilder scriptBuilder, string conString)
        {
            var constraintQuery = @"
                    SELECT 
                        conname, 
                        contype,
                        pg_get_constraintdef(oid) AS constraint_def,
                        conrelid
                    FROM pg_constraint
                    WHERE connamespace = 'public'::regnamespace
                      AND (
                        contype = 'f' -- Foreign Key
                        OR (contype = 'p' AND array_length(conkey, 1) > 1) -- Composite PK
                )";

            using var cmd = new NpgsqlCommand(constraintQuery, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string constraintName = reader.GetString(0);
                char constraintType = Convert.ToChar(reader.GetValue(1)); // 'f' or 'p'
                string constraintDef = reader.GetString(2);
                long tableOid = Convert.ToInt64(reader.GetValue(3));

                string tableName = await GetTableNameFromOidAsync(conString, tableOid);

                string script = GenerateConstraintScriptFromDef(constraintDef, tableName, constraintName);

                scriptBuilder.AppendLine($"-- {constraintType switch { 'f' => "Foreign Key", 'p' => "Composite Primary Key", _ => "Constraint" }}: {constraintName}");
                scriptBuilder.AppendLine(script);
            }
        }



        /// <summary>
        /// Generates the ALTER TABLE script based on the constraint definition (primary key, foreign key, etc.).
        /// </summary>
        //private string GenerateConstraintScriptFromDef(string constraintDef, string tableName, string constraintName)
        //{
        //    var builder = new StringBuilder();

        //    // If the constraint is a Primary Key
        //    if (constraintDef.StartsWith("PRIMARY KEY"))
        //    {
        //        builder.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({ExtractColumnsFromConstraintDef(constraintDef)});");
        //    }
        //    // If the constraint is a Foreign Key
        //    else
        //    if (constraintDef.StartsWith("FOREIGN KEY"))
        //    {
        //        builder.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} FOREIGN KEY ({ExtractColumnsFromConstraintDef(constraintDef)})");
        //        builder.AppendLine($"  {constraintDef.Substring(constraintDef.IndexOf("REFERENCES"))};");
        //    }
        //    else
        //    {
        //        builder.AppendLine($"-- Unsupported constraint type: {constraintDef}");
        //    }

        //    return builder.ToString();
        //}

        private static string GenerateConstraintScriptFromDef(string constraintDef, string tableName, string constraintName)
        {
            var builder = new StringBuilder();

            // If the constraint is a Primary Key
            if (constraintDef.StartsWith("PRIMARY KEY"))
            {
                var columns = ExtractColumnsFromConstraintDef(constraintDef);
                var columnList = columns.Split(',').Select(c => c.Trim()).ToList();

                // Only include composite primary keys (more than one column)
                if (columnList.Count > 1)
                {
                    builder.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({string.Join(", ", columnList)});");
                }
            }
            // If the constraint is a Foreign Key
            else if (constraintDef.StartsWith("FOREIGN KEY"))
            {
                var columns = ExtractColumnsFromConstraintDef(constraintDef);
                var referencesPart = constraintDef.Substring(constraintDef.IndexOf("REFERENCES", StringComparison.Ordinal));

                builder.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} FOREIGN KEY ({columns})");
                builder.AppendLine($"  {referencesPart};");
            }
            else
            {
                builder.AppendLine($"-- Unsupported constraint type: {constraintDef}");
            }

            return builder.ToString();
        }


        /// <summary>
        /// Asynchronously fetches the table name by OID.
        /// </summary>
        private static async Task<string> GetTableNameFromOidAsync(string conString, long tableOid)
        {
            using var connection = new NpgsqlConnection(conString);
            connection.Open();
            // Query to fetch the table name by OID
            var query = $"SELECT relname FROM pg_class WHERE oid = {tableOid}";

            using var cmd = new NpgsqlCommand(query, connection);
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() ?? "unknown_table";
        }


        /// <summary>
        /// Extracts the column names from the constraint definition (e.g., PRIMARY KEY (col1, col2)).
        /// </summary>
        private static string ExtractColumnsFromConstraintDef(string constraintDef)
        {
            // Look for the part inside parentheses after "PRIMARY KEY" or "FOREIGN KEY"
            var startIdx = constraintDef.IndexOf('(');
            var endIdx = constraintDef.IndexOf(')');

            if (startIdx == -1 || endIdx == -1 || endIdx <= startIdx)
                return string.Empty; // No columns found or invalid format

            // Extract the substring between parentheses and trim any surrounding whitespace
            var columnsPart = constraintDef.Substring(startIdx + 1, endIdx - startIdx - 1).Trim();

            return columnsPart;
        }


        private static void GenerateInsertScripts(NpgsqlConnection connection, StringBuilder scriptBuilder, string conn)
        {
            var tablesQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

            using var cmd = new NpgsqlCommand(tablesQuery, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var tableName = reader.GetString(0);
                Console.WriteLine($"Generating insert script for table: {tableName}");

                using (var newConnection = new NpgsqlConnection(conn))
                {
                    newConnection.Open();

                    // Get the primary key columns for the current table
                    var primaryKeyColumnsQuery = $@"
                SELECT column_name
                FROM information_schema.key_column_usage
                WHERE table_name = '{tableName}' AND constraint_name = (
                    SELECT constraint_name
                    FROM information_schema.table_constraints
                    WHERE table_name = '{tableName}' AND constraint_type = 'PRIMARY KEY'
                )";

                    var primaryKeyColumns = new List<string>();
                    using (var pkCmd = new NpgsqlCommand(primaryKeyColumnsQuery, newConnection))
                    using (var pkReader = pkCmd.ExecuteReader())
                    {
                        while (pkReader.Read())
                        {
                            primaryKeyColumns.Add(pkReader.GetString(0));
                        }
                    }

                    // Get columns that are auto-generated (SERIAL, BIGSERIAL, identity columns)
                    var autoGeneratedColumnsQuery = $@"
                SELECT column_name
                FROM information_schema.columns
                WHERE table_name = '{tableName}' AND (column_default LIKE 'nextval%' OR is_identity = 'YES')";

                    var autoGeneratedColumns = new HashSet<string>();
                    using (var autoGenCmd = new NpgsqlCommand(autoGeneratedColumnsQuery, newConnection))
                    using (var autoGenReader = autoGenCmd.ExecuteReader())
                    {
                        while (autoGenReader.Read())
                        {
                            autoGeneratedColumns.Add(autoGenReader.GetString(0));
                        }
                    }

                    // Get generated columns (computed columns like password_expires_at)
                    var generatedColumnsQuery = $@"
                SELECT column_name
                FROM information_schema.columns
                WHERE table_name = '{tableName}' AND generation_expression IS NOT NULL";

                    var generatedColumns = new HashSet<string>();
                    using (var genCmd = new NpgsqlCommand(generatedColumnsQuery, newConnection))
                    using (var genReader = genCmd.ExecuteReader())
                    {
                        while (genReader.Read())
                        {
                            generatedColumns.Add(genReader.GetString(0));
                        }
                    }

                    var dataQuery = $"SELECT * FROM {tableName} order by 1 asc";
                    using var dataCmd = new NpgsqlCommand(dataQuery, newConnection);
                    using var dataReader = dataCmd.ExecuteReader();
                    {
                        scriptBuilder.AppendLine("SET session_replication_role = replica;");
                        while (dataReader.Read())
                        {
                            var columnCount = dataReader.FieldCount;
                            var columnNames = new List<string>();
                            var columnValues = new List<string>();
                           
                            for (int i = 0; i < columnCount; i++)
                            {
                                var columnName = dataReader.GetName(i);
                                // Skip auto-generated, computed, and primary key columns (except in the case of "user_roles")
                                if (tableName == "audit_logs")
                                {
                                    if (tableName != "user_roles" && (primaryKeyColumns.Contains(columnName) || autoGeneratedColumns.Contains(columnName) || generatedColumns.Contains(columnName)))
                                    {
                                        continue;
                                    }
                                }

                                //if (tableName != "user_roles" && (primaryKeyColumns.Contains(columnName) || autoGeneratedColumns.Contains(columnName) || generatedColumns.Contains(columnName)))
                                //{
                                //    continue;
                                //}


                                var columnValue = dataReader[i];
                                if (columnValue == DBNull.Value)
                                {
                                    columnValues.Add("NULL");
                                }
                                else if (columnValue is byte[])
                                {
                                    // Convert byte[] to hexadecimal string
                                    var byteArray = (byte[])columnValue;
                                    var hexString = BitConverter.ToString(byteArray).Replace("-", string.Empty);
                                    columnValues.Add($"'\\x{hexString}'");  // PostgreSQL expects \x for bytea data
                                }
                                else if (columnValue is string)
                                {
                                    columnValues.Add($"'{columnValue.ToString()!.Replace("'", "''")}'");
                                }
                                else if (columnValue is bool)
                                {
                                    columnValues.Add((bool)columnValue ? "TRUE" : "FALSE");
                                }
                                else if (columnValue is DateTime)
                                {
                                    columnValues.Add($"'{((DateTime)columnValue).ToString("yyyy-MM-dd HH:mm:ss")}'");
                                }
                                else
                                {
                                    columnValues.Add(columnValue.ToString()!);
                                }

                                columnNames.Add(columnName);
                            }

                            var columnNamesString = string.Join(", ", columnNames);
                            var columnValuesString = string.Join(", ", columnValues);

                            //bool isInsertingIntoAuditLogId = columnNamesString.Split(',')
                            //    .Any(col => col.Trim().Equals("audit_log_id", StringComparison.OrdinalIgnoreCase));

                            //if (isInsertingIntoAuditLogId)
                            //{
                            //    scriptBuilder.AppendLine($"INSERT INTO {tableName} ({columnNamesString}) OVERRIDING SYSTEM VALUE VALUES ({columnValuesString}) ON CONFLICT DO NOTHING;");
                            //}
                            //else
                            //{
                            //    scriptBuilder.AppendLine($"INSERT INTO {tableName} ({columnNamesString}) VALUES ({columnValuesString}) ON CONFLICT DO NOTHING;");
                            //}
                            scriptBuilder.AppendLine($"INSERT INTO {tableName} ({columnNamesString}) OVERRIDING SYSTEM VALUE VALUES ({columnValuesString}) ON CONFLICT DO NOTHING;");
                        }
                        scriptBuilder.AppendLine("SET session_replication_role = DEFAULT;");
                    }
                }
            }
        }
    }
}