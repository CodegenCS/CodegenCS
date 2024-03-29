﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace CodegenCS.Models.DbSchema.Extractor
{
    public class ExtractWizard
    {
        public DbTypeEnum? DbType { get; set; } = null;
        public string ConnectionString { get; set; } = null;
        public string OutputJsonSchema { 
            get { return _outputJsonSchema; }
            set { /*if (value != null && !System.IO.Path.IsPathRooted(value)) value = System.IO.Path.Combine(Program.GetScriptFolder(), value);*/ _outputJsonSchema = value; } 
        }
        private string _outputJsonSchema = null;


        public enum DbTypeEnum
        {
            MSSQL,
            PostgreSQL
        }
        public ExtractWizard()
        {
        }

        public void Run()
        {
            string[] dbTypes = Enum.GetNames(typeof(DbTypeEnum));
            while (DbType == null)
            {
                Console.WriteLine($"[Choose a Database Type]");
                for (int i = 0; i < dbTypes.Length; i++)
                    Console.WriteLine($"{i + 1}. {dbTypes[i]}");
                Console.Write($"Database Type: ");
                string chosen = Console.ReadLine();
                int chosenInt;
                if (int.TryParse(chosen, out chosenInt) && chosenInt >= 1 && chosenInt <= dbTypes.Length)
                    DbType = (DbTypeEnum)Enum.Parse(typeof(DbTypeEnum), dbTypes[chosenInt - 1]);
            }
            Console.WriteLine($"Database Type is {DbType}");
            Console.WriteLine($"");

            while(string.IsNullOrEmpty(ConnectionString))
            {

                Console.WriteLine($"[Enter the Connection String]");
                switch (DbType.Value)
                {
                    case DbTypeEnum.MSSQL:
                        Console.WriteLine($"Example: Server=MYSERVER; Database=AdventureWorks; Integrated Security=True;");
                        Console.WriteLine($"Example: Server=MYSERVER; Database=AdventureWorks; User Id=myUsername;Password=myPassword");
                        Console.WriteLine($"Example: Server=MYWORKSTATION\\SQLEXPRESS; Database=AdventureWorks; Integrated Security=True;");
                        break;
                    case DbTypeEnum.PostgreSQL:
                        Console.WriteLine($"Example: Host=localhost; Database=Adventureworks; Username=postgres; Password=myPassword");
                        break;
                }

                Console.Write($"Connection String: ");
                ConnectionString = Console.ReadLine();
            }

            while (string.IsNullOrEmpty(OutputJsonSchema))
            {
                Console.WriteLine($"[Enter the Output File]");
                Console.Write($"Output file: [Schema.json]");
                OutputJsonSchema = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(OutputJsonSchema))
                    OutputJsonSchema = "Schema.json";
            }

            DatabaseSchema schema = null;
            switch (DbType.Value)
            {
                case DbTypeEnum.MSSQL:
                    {
                        Func<IDbConnection> connectionFactory = () => new System.Data.SqlClient.SqlConnection(ConnectionString);
                        var reader = new SqlServer.SqlServerSchemaReader(connectionFactory);
                        schema = reader.ExportSchemaToJSON();
                    }
                    break;
                case DbTypeEnum.PostgreSQL:
                    {
                        Func<IDbConnection> connectionFactory = () => new Npgsql.NpgsqlConnection(ConnectionString);
                        var reader = new PostgreSQL.PgsqlSchemaReader(connectionFactory);
                        schema = reader.ExportSchemaToJSON();
                    }
                    break;
            }

            Console.WriteLine($"Saving into {OutputJsonSchema}...");
            File.WriteAllText(OutputJsonSchema, JsonConvert.SerializeObject(schema, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Success!");


            //Console.Write($"Press any key to exit...");
            //Console.ReadKey(true);
            //Console.WriteLine();
        }
    }
}
