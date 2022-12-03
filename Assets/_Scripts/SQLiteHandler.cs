using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

[DefaultExecutionOrder(0)]
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

    public static bool CheckForCreatedWorld()
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "SELECT HasWorld FROM GAME_INFO;";

                int result = -1;
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }

                return result > 0;
            }
        }
    }

    public static int SetHasWorldValue(bool value)
    {
        int parsedBool = value ? 1 : 0;

        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"UPDATE GAME_INFO
                                        SET
                                            HasWorld = @ParsedBool;";

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "ParsedBool",
                    Value = parsedBool
                });

                return command.ExecuteNonQuery();
            }
        }
    }

    public static int GetWorldBlockCount()
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "SELECT BlocksCount FROM GAME_INFO;";

                int result = -1;
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }

                return result;
            }
        }
    }

    public static void SetBlockCount(int value)
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"UPDATE GAME_INFO
                                        SET
                                            BlocksCount = @Value;";

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Value",
                    Value = value
                });

                command.ExecuteNonQuery();
            }
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

    public static Vector3 GetBlock(int blockId)
    {
        Vector3 result = Vector3.zero;

        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                #region X
                command.CommandText = @"SELECT BlockPosX FROM BLOCK 
                                        WHERE BlockID = $idX;";

                command.Parameters.AddWithValue("$idX", blockId);
                int x = 0;
                SqliteDataReader readerX = command.ExecuteReader();
                while (readerX.Read())
                {
                    x = readerX.GetInt32(0);
                }
                readerX.Close();
                #endregion

                #region Y
                command.CommandText = @"SELECT BlockPosY FROM BLOCK 
                                        WHERE BlockID = $idY;";

                command.Parameters.AddWithValue("$idY", blockId);
                int y = 0;
                SqliteDataReader readerY = command.ExecuteReader();
                while (readerY.Read())
                {
                    y = readerY.GetInt32(0);
                }
                readerY.Close();
                #endregion

                #region Z
                command.CommandText = @"SELECT BlockPosZ FROM BLOCK 
                                        WHERE BlockID = $idZ;";

                command.Parameters.AddWithValue("$idZ", blockId);

                int z = 0;
                SqliteDataReader readerZ = command.ExecuteReader();
                while (readerZ.Read())
                {
                    z = readerZ.GetInt32(0);
                }
                readerZ.Close();
                #endregion

                result.Set(x, y, z);
            }
        }

        return result;
    }
}
