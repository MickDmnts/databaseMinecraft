using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class SQLiteHandler : MonoBehaviour
{
    private static string dbPath = "";

    private void Awake()
    {
        if (dbPath == "")
        {
            dbPath = "URI=file:" + Application.dataPath + "/Dbs/minecraftDb.db";
        }
    }

    public static void CreateNewBlockTable()
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "DROP TABLE IF EXISTS BLOCK;";

                int result = command.ExecuteNonQuery();
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "CREATE TABLE BLOCK (" +
                    "BlockID INTEGER UNIQUE NOT NULL PRIMARY KEY AUTOINCREMENT," +
                    "BlockPosX INTEGER NOT NULL," +
                    "BlockPosY INTEGER NOT NULL," +
                    "BlockPosZ INTEGER NOT NULL," +
                    "Placed    INTEGER NOT NULL" +
                    "          DEFAULT (-1));";

                int result = command.ExecuteNonQuery();
            }
        }
    }

    public static void AddBlockToWorld(int x, int y, int z)
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "INSERT INTO BLOCK (BlockPosX, BlockPosY, BlockPosZ, Placed) " +
                                        "VALUES (@PosX, @PosY, @PosZ, @Placed);";

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "PosX",
                    Value = x
                });

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "PosY",
                    Value = y
                });

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "PosZ",
                    Value = z
                });

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Placed",
                    Value = 1
                });

                int result = command.ExecuteNonQuery();
                Debug.LogFormat("Block creation result {0}", result);
            }
        }
    }

    public static void UpdateBlockInWorld(int x, int y, int z, int blockID)
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"UPDATE BLOCK
                                        SET BlockPosX = $x,
                                            BlockPosY = $y,
                                            BlockPosZ = $z
                                        WHERE BlockID = $id;";

                command.Parameters.AddWithValue("$x", x);
                command.Parameters.AddWithValue("$y", y);
                command.Parameters.AddWithValue("$z", z);
                command.Parameters.AddWithValue("$id", blockID);

                int result = command.ExecuteNonQuery();
                Debug.LogFormat("Updated block with ID {0}: {1}", blockID, result);
            }
        }
    }

    public static void DeleteBlockFromWorld(int blockId)
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"DELETE FROM BLOCK WHERE BlockID = $id";
                command.Parameters.AddWithValue("$id", blockId);

                int result = command.ExecuteNonQuery();
                Debug.LogFormat("Block deletion with id {0}: {1}", blockId, result);
            }
        }
    }
}
