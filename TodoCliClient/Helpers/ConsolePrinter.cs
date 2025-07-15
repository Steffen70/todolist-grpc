using SwissPension.Todo.Common;

namespace SwissPension.Todo.CliClient.Helpers;

internal static class ConsolePrinter
{
    public static void PrintCreatedList(int listId, string listName)
    {
        var columns = new (object Value, string Header)[]
        {
            (listId, "ListId"),
            (listName, "ListName")
        };

        var columnWidths = CalculateColumnWidths(columns);
        var totalWidth = CalculateTotalTableWidth(columnWidths);

        DrawRule(totalWidth);
        WriteHeaderRow(columns.Select(c => c.Header).ToArray(), columnWidths);
        DrawRule(totalWidth);
        WriteDataRow(columns.Select(c => c.Value).ToArray(), columnWidths);
        DrawRule(totalWidth);
        Console.WriteLine();
    }

    public static void PrintItem(ReadItemTodoResponse item)
    {
        var columns = new (object Value, string Header)[]
        {
            (item.TodoListId, "ListId"),
            (item.ListName, "ListName"),
            (item.Id, "ItemId"),
            (item.ItemName, "ItemName"),
            (item.IsDone, "IsDone")
        };

        var columnWidths = CalculateColumnWidths(columns);
        var totalWidth = CalculateTotalTableWidth(columnWidths);

        DrawRule(totalWidth);
        WriteHeaderRow(columns.Select(c => c.Header).ToArray(), columnWidths);
        DrawRule(totalWidth);
        WriteDataRow(columns.Select(c => c.Value).ToArray(), columnWidths);
        DrawRule(totalWidth);
        Console.WriteLine();
    }

    public static void PrintLists(ListResponse result)
    {
        if (result.Lists.Count == 0)
        {
            Console.WriteLine("No lists found.\n");
            return;
        }

        foreach (var list in result.Lists)
        {
            Console.WriteLine($"List: {list.ListName} (ID: {list.Id})");

            if (list.Items.Count == 0)
            {
                Console.WriteLine("  No items in this list.\n");
                continue;
            }

            // Calculate column widths based on all items in the list
            var allRows = new List<(object Value, string Header)[]>();

            // Add header row for width calculation
            allRows.Add([
                ("ID", "ID"),
                ("Item Name", "Item Name"),
                ("Done?", "Done?")
            ]);

            // Add all data rows for width calculation
            foreach (var item in list.Items)
                allRows.Add([
                    (item.Id, "ID"),
                    (item.ItemName, "Item Name"),
                    (item.IsDone, "Done?")
                ]);

            var columnWidths = CalculateColumnWidthsFromMultipleRows(allRows);
            var totalWidth = CalculateTotalTableWidth(columnWidths);

            DrawRule(totalWidth);
            WriteHeaderRow(["ID", "Item Name", "Done?"], columnWidths);
            DrawRule(totalWidth);

            foreach (var item in list.Items) WriteDataRow([item.Id, item.ItemName, item.IsDone], columnWidths);

            DrawRule(totalWidth);
            Console.WriteLine();
        }
    }

    public static void PrintUpdatedItem(UpdateItemTodoResponse result)
    {
        Console.WriteLine(result is { Id: > 0 } ? $"Item with ID {result.Id} in list {result.TodoListId} updated successfully.\n" : "Update failed: check the provided IDs and try again.\n");
    }

    private static int[] CalculateColumnWidths((object Value, string Header)[] columns)
    {
        var widths = new int[columns.Length];

        for (var i = 0; i < columns.Length; i++)
        {
            var headerLength = columns[i].Header?.Length ?? 0;
            var valueLength = columns[i].Value?.ToString()?.Length ?? 0;

            // Take the maximum of header and value length, with a minimum width of 3
            widths[i] = Math.Max(Math.Max(headerLength, valueLength), 3);
        }

        return widths;
    }

    private static int[] CalculateColumnWidthsFromMultipleRows(List<(object Value, string Header)[]> rows)
    {
        if (rows.Count == 0) return [];

        var columnCount = rows[0].Length;
        var widths = new int[columnCount];

        // Initialize with minimum width
        for (var col = 0; col < columnCount; col++) widths[col] = 3; // Minimum width

        // Check each row to find maximum width needed for each column
        foreach (var row in rows)
            for (var col = 0; col < Math.Min(columnCount, row.Length); col++)
            {
                var headerLength = row[col].Header?.Length ?? 0;
                var valueLength = row[col].Value?.ToString()?.Length ?? 0;
                var maxLength = Math.Max(headerLength, valueLength);

                widths[col] = Math.Max(widths[col], maxLength);
            }

        return widths;
    }

    // Sum of all column widths + separators (| char + space on each side)
    // Each column: width + 2 spaces, plus 1 for each separator |
    // Plus 1 for the leading | and 1 for the trailing |
    private static int CalculateTotalTableWidth(int[] columnWidths) => columnWidths.Sum() + columnWidths.Length * 2 + columnWidths.Length + 1;

    private static void DrawRule(int length) => Console.WriteLine(new string('-', length));

    private static void WriteHeaderRow(string[] headers, int[] columnWidths) => WriteFormattedRow(headers.Cast<object>().ToArray(), columnWidths);

    private static void WriteDataRow(object[] values, int[] columnWidths) => WriteFormattedRow(values, columnWidths);

    private static void WriteFormattedRow(object[] values, int[] columnWidths)
    {
        Console.Write("|");

        for (var i = 0; i < values.Length && i < columnWidths.Length; i++)
        {
            var value = values[i]?.ToString() ?? "";
            var paddedValue = value.PadRight(columnWidths[i]);
            Console.Write($" {paddedValue} |");
        }

        Console.WriteLine();
    }
}
