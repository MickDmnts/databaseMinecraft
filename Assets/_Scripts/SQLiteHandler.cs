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
            dbPath = "URI=file:" + Application.dataPath + "/Resources/minecraftDb.db";
            Debug.Log(dbPath);


#if UNITY_STANDALONE && !UNITY_EDITOR
            dbPath = "URI=file:" + Application.dataPath + "/minecraftDb.db";
            CreateNeededTables();
            Debug.Log(dbPath);
#endif
        }
    }

    void CreateNeededTables()
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"CREATE TABLE IF NOT EXISTS GAME_INFO (
                                            HasWorld    INTEGER PRIMARY KEY
                                                                UNIQUE
                                                                NOT NULL,
                                            BlocksCount INTEGER DEFAULT (0) 
                                        );";

                int result = command.ExecuteNonQuery();

                command.CommandText = @"INSERT OR IGNORE INTO GAME_INFO (BlocksCount, HasWorld)
                                        VALUES
                                            (0,0);";

                int insertionRes = command.ExecuteNonQuery();
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"CREATE TABLE IF NOT EXISTS BLOCK_TYPES (
                                            BlockType VARCHAR (25) UNIQUE
                                                                   NOT NULL,
                                            TypeInt   INTEGER      UNIQUE,
                                            PRIMARY KEY (
                                                TypeInt
                                            )
                                        );";

                int result = command.ExecuteNonQuery();

                command.CommandText = @"INSERT OR IGNORE INTO BLOCK_TYPES (TypeInt, BlockType)
                                        VALUES
                                            (0, 'Dirt'),
                                            (1, 'Stone');";

                int insertionRes = command.ExecuteNonQuery();
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"CREATE TABLE IF NOT EXISTS BLOCK (
                                            BlockID     INTEGER UNIQUE
                                                                NOT NULL
                                                                PRIMARY KEY AUTOINCREMENT
                                                                DEFAULT (0),
                                            BlockPosX   INTEGER NOT NULL,
                                            BlockPosY   INTEGER NOT NULL,
                                            BlockPosZ   INTEGER NOT NULL,
                                            BlockTypeFK INTEGER REFERENCES BLOCK_TYPES (TypeInt) 
                                        );";

                int result = command.ExecuteNonQuery();
            }
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

    public static void SetWorldBlockCount(int value)
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

                command.CommandText = @"CREATE TABLE BLOCK (
                                            BlockID     INTEGER UNIQUE
                                                                NOT NULL
                                                                PRIMARY KEY AUTOINCREMENT
                                                                DEFAULT (0),
                                            BlockPosX   INTEGER NOT NULL,
                                            BlockPosY   INTEGER NOT NULL,
                                            BlockPosZ   INTEGER NOT NULL,
                                            BlockTypeFK INTEGER REFERENCES BLOCK_TYPES (TypeInt) 
                                        );";

                int result = command.ExecuteNonQuery();
                Debug.Log(result);
            }
        }
    }

    public static void AddBlockToWorld(int x, int y, int z, int blockType, int blockId)
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "INSERT INTO BLOCK (BlockID, BlockPosX, BlockPosY, BlockPosZ, BlockTypeFK) " +
                                        "VALUES (@BlockID, @PosX, @PosY, @PosZ, @BlockTypeFK);";

                command.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "BlockID",
                    Value = blockId
                });

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
                    ParameterName = "BlockTypeFK",
                    Value = blockType
                });

                int result = command.ExecuteNonQuery();
                Debug.LogFormat("Block creation result {0}", result);
            }
        }
    }

    /*public static void UpdateBlockInWorld(int x, int y, int z, int blockID, int blockType)
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
                                            BlockPosZ = $z,
                                            BlockTypeFK = $type
                                        WHERE BlockID = $id;";

                command.Parameters.AddWithValue("$x", x);
                command.Parameters.AddWithValue("$y", y);
                command.Parameters.AddWithValue("$z", z);
                command.Parameters.AddWithValue("$id", blockID);
                command.Parameters.AddWithValue("$type", blockType);

                int result = command.ExecuteNonQuery();
                Debug.LogFormat("Updated block with ID {0}: {1}", blockID, result);
            }
        }
    }*/

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

    public static BlockInfo GetBlockInfo(int blockId)
    {
        BlockInfo blockInfo = new BlockInfo();

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

                #region BLOCK_TYPE
                command.CommandText = @"SELECT BlockTypeFK FROM BLOCK 
                                        WHERE BlockID = $idType;";

                command.Parameters.AddWithValue("$idType", blockId);
                int typeInt = 0;
                SqliteDataReader readerType = command.ExecuteReader();
                while (readerType.Read())
                {
                    typeInt = readerType.GetInt32(0);
                    Debug.Log(typeInt);
                }
                readerType.Close();
                #endregion

                result.Set(x, y, z);
                blockInfo.Position = result;
                blockInfo.BlockType = typeInt;
            }
        }

        return blockInfo;
    }

    public static void CreateNewLocalTables()
    {
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = @"DROP TABLE IF EXISTS BLOCK;";

                int blockRes = command.ExecuteNonQuery();

                command.CommandText = @"DROP TABLE IF EXISTS GAME_INFO;";

                int infoRes = command.ExecuteNonQuery();

                command.CommandText = @"DROP TABLE IF EXISTS BLOCK_TYPES;";

                int typesRes = command.ExecuteNonQuery();
            }
        }
    }
}
