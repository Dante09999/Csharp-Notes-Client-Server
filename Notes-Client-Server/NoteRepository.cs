using Microsoft.Data.Sqlite;
public class NoteRepository
{
    private readonly string ConnectionString;

    public NoteRepository(string connectionString = "Data Source=notes.db")
    {
        ConnectionString = connectionString;
    }

    // båda ändringarna måste lyckas
    public void UpdateTwo(int firstId, string firstText, int secondId, string secondText)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                "UPDATE Notes SET Text = $text WHERE Id = $id;";

            command.Parameters.AddWithValue("$text", firstText);
            command.Parameters.AddWithValue("$id", firstId);

            if (command.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    "Första anteckningen saknas.");
            }

            command.Parameters["$text"].Value = secondText;
            command.Parameters["$id"].Value = secondId;

            if (command.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    "Andra anteckningen saknas.");
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // hämta en anteckning via id
    public Note? GetById(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Text FROM Notes WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Note
        {
            Id = reader.GetInt32(0),
            Text = reader.GetString(1)
        };
    }

    // ändra en anteckning
    public bool Update(int id, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Anteckningen får inte vara tom.");
        }

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "UPDATE Notes SET Text = $text WHERE Id = $id;";

        command.Parameters.AddWithValue("$text", text);
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() == 1;
    }

    // ta bort en anteckning
    public bool Delete(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        return command.ExecuteNonQuery() == 1;
    }

    // hämta alla anteckningar
    public List<Note> GetAll()
    {
        List<Note> notes = new List<Note>();

        using (SqliteConnection connection =
            new SqliteConnection(ConnectionString))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT Id, Text FROM Notes ORDER BY Id;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notes.Add(new Note
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }

        return notes;
    }

    // skapar en ny anteckning i databasen och returnerar dess id
    public int Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Anteckningen får inte vara tom.");
        }

        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = """
                    INSERT INTO Notes (Text) VALUES ($text);
                    SELECT last_insert_rowid();
                    """;

                command.Parameters.AddWithValue("$text", text);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    // skapar tabellen om den saknas
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