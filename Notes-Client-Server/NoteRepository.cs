using Microsoft.Data.Sqlite;
public class NoteRepository
{
    private const string ConnectionString = "Data Source=notes.db";

    //skapar tabellen om den saknas
    public void Initialize()
    {
        using (SqliteConnection connection =
            new SqliteConnection(ConnectionString))
        {
            connection.Open();

            string schemaPath =
                Path.Combine(AppContext.BaseDirectory, "schema.sql");

            string sql = File.ReadAllText(schemaPath);

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
    }
}