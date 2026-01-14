using System;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var connStr = "Host=31.220.84.102;Port=5433;Database=daterp;Username=admin-tqd;Password=PgI_hOcs8NkK_Kp9jzLT_kz0;Trust Server Certificate=true";
        try
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT t.""Order"", t.""Content"", t.""Point"", t.""Id""
                        FROM ""ExamTasks"" t
                        JOIN ""ExamProjects"" p ON t.""ProjectId"" = p.""Id""
                        JOIN ""ExamLists"" l ON p.""ExamListId"" = l.""Id"" 
                        WHERE l.""Title"" = 'Word 2019 - Bộ đề 1' 
                          AND p.""Order"" = 1
                        ORDER BY t.""Order"" ASC";

                    using (var reader = cmd.ExecuteReader())
                    {
                        var data = new List<string[]>();
                        // Header
                        data.Add(new[] { "Order", "Content", "Point" });

                        while (reader.Read())
                        {
                            data.Add(new[] {
                                reader[0].ToString(),
                                reader[1].ToString(),
                                reader[2].ToString()
                            });
                        }

                        Console.WriteLine($"Total Tasks: {data.Count - 1}");
                        // Print Markdown Table
                        PrintMarkdownTable(data);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void PrintMarkdownTable(List<string[]> rows)
    {
        if (rows.Count == 0) return;

        var colWidths = new int[rows[0].Length];
        for (int i = 0; i < rows[0].Length; i++)
        {
            colWidths[i] = rows.Max(r => (r[i] ?? "").Length);
        }

        // Header
        Console.Write("| ");
        for (int i = 0; i < rows[0].Length; i++)
        {
            Console.Write((rows[0][i] ?? "").PadRight(colWidths[i]));
            Console.Write(" | ");
        }
        Console.WriteLine();

        // Separator
        Console.Write("| ");
        for (int i = 0; i < rows[0].Length; i++)
        {
            Console.Write(new string('-', colWidths[i]));
            Console.Write(" | ");
        }
        Console.WriteLine();

        // Data
        for (int r = 1; r < rows.Count; r++)
        {
            Console.Write("| ");
            for (int i = 0; i < rows[r].Length; i++)
            {
                Console.Write((rows[r][i] ?? "").PadRight(colWidths[i]));
                Console.Write(" | ");
            }
            Console.WriteLine();
        }
    }
}
