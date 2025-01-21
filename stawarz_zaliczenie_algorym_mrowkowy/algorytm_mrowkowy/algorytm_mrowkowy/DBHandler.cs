using Microsoft.Data.Sqlite;

namespace algorytm_mrowkowy;

public class DBHandler  {
    public void CreateTable(){
        using (var connection = new SqliteConnection("Data Source=employee.db")) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @" CREATE TABLE Employee (
        Id INTEGER PRIMARY KEY,
        FirstName TEXT NOT NULL,
        LastName TEXT NOT NULL,
        DESIGNATION TEXT NOT NULL )";
            command.ExecuteNonQuery();
        }

    }
}