using SQLite;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

[Table("Settings")]
public class Settings
{
    [PrimaryKey]
    public int Id { get; set; }

    public int MovementType { get; set; }
    public float SmoothMovementSpeed { get; set; }
    public float MenuSound { get; set; }
    public float DungeonSound { get; set; }
    public float SlumsSound { get; set; }
    public float ForestSound { get; set; }
    public float DwarfSound { get; set; }
    public float DragonSound { get; set; }
    public float MonsterSound { get; set; }

    public int Level { get; set; }
    public int STR { get; set; }
    public int CON { get; set; }
    public int DEX { get; set; }
    public int EXP { get; set; }
    public int APP { get; set; }
    public int MAP { get; set; }

    public bool HasDiedOnce { get; set; }
    public bool HasSeenItemOI { get; set; }
    public bool SeatedToggle { get; set; }
}
public class DefaultValueParameter
{
    public int DefaultValueInt { get; set; }
    public float DefaultValueFloat { get; set; }
    public bool DefaultValueBool { get; set; }
}

public class SettingsDatabase : MonoBehaviour
{
    private string path;
    private const string tableName = "Settings";
    private const string databaseFile = "settings.db";
    private SQLiteConnection connection;

    private void Start()
    {
        path = GetDatabasePath();
        if (!DatabaseExists(path))
        {
            CreateDatabase(path);
        }
        connection = new SQLiteConnection(path);
    }

    private string GetDatabasePath()
    {
#if UNITY_EDITOR
        return Path.Combine(Directory.GetParent(Application.dataPath).FullName, databaseFile);
#else
        return Path.Combine(Application.persistentDataPath, databaseFile);
#endif
    }

    private bool DatabaseExists(string path)
    {
        if (File.Exists(path))
        {
            using var connection = new SQLiteConnection(path);
            var tableQuery = connection.GetTableInfo(tableName);
            var expectedColumns = typeof(Settings).GetProperties().Select(property => property.Name).ToList();

            if (tableQuery.Count != expectedColumns.Count)
            {
                return false;
            }
            var actualColumns = tableQuery.Select(column => column.Name).ToList();

            // Check if all expected columns exist in the table
            if (!expectedColumns.All(column => actualColumns.Contains(column)))
            {
                return false;
            }

            // Check if any additional columns exist in the table
            var additionalColumns = actualColumns.Except(expectedColumns).ToList();
            if (additionalColumns.Any())
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private void CreateDatabase(string path)
    {
        using var connection = new SQLiteConnection(path);

        var tableExists = connection.GetTableInfo(tableName).Any();

        if (tableExists)
        {
            // Retrieve the existing columns in the table
            var existingColumns = connection.GetTableInfo(tableName)
                .Select(column => column.Name)
                .ToList();

            // Get the expected columns based on the Settings class
            var expectedColumns = typeof(Settings).GetProperties()
                .Select(property => property.Name)
                .ToList();

            // Find the missing columns
            var missingColumns = expectedColumns.Except(existingColumns).ToList();

            if (missingColumns.Any())
            {
                // Create a backup table with the existing data
                var backupTableName = $"{tableName}_Backup";
                var backupQuery = $"ALTER TABLE {tableName} RENAME TO {backupTableName}";
                connection.Execute(backupQuery);

                // Create a new table with the updated schema
                connection.CreateTable<Settings>();

                // Transfer data from the backup table to the new table
                var columnsToCopy = existingColumns.Intersect(expectedColumns);
                var columnNames = string.Join(", ", columnsToCopy);
                var transferDataQuery = $"INSERT INTO {tableName} ({columnNames}) SELECT {columnNames} FROM {backupTableName}";
                connection.Execute(transferDataQuery);

                // Drop the backup table
                var dropBackupQuery = $"DROP TABLE IF EXISTS {backupTableName}";
                connection.Execute(dropBackupQuery);

                foreach (var missingColumn in missingColumns)
                {
                    var defaultSetting = GetDefaultSettingValue(missingColumn);
                    if (defaultSetting == null)
                    {
                        continue;
                    }

                    var insertDefaultQuery = $"UPDATE {tableName} SET {missingColumn} = @DefaultValue WHERE {missingColumn} IS NULL";
                    int defaultINT;
                    float defaultFloat;
                    bool defaultBool;

                    if (defaultSetting.DefaultValueInt != 0)
                    {
                        defaultINT = defaultSetting.DefaultValueInt;
                        connection.Execute(insertDefaultQuery, defaultINT);
                    }
                    else if (defaultSetting.DefaultValueFloat != 0.0f)
                    {
                        defaultFloat = defaultSetting.DefaultValueFloat;
                        connection.Execute(insertDefaultQuery, defaultFloat);
                    }
                    else
                    {
                        defaultBool = defaultSetting.DefaultValueBool;
                        connection.Execute(insertDefaultQuery, defaultBool);
                    }
                }
            }
        }
        else
        {
            connection.CreateTable<Settings>();
            InsertDefaultSettings(connection);
        }
    }

    private void InsertDefaultSettings(SQLiteConnection connection)
    {
        var defaultSettings = new Settings
        {
            MovementType = 0,
            SmoothMovementSpeed = 100f,
            MenuSound = 0.25f,
            DungeonSound = 0.5f,
            SlumsSound = 0.05f,
            ForestSound = 0.1f,
            DwarfSound = 0.1f,
            DragonSound = 0.075f,
            MonsterSound = 0.05f,
            Level = 1,
            STR = 1,
            CON = 1,
            DEX = 1,
            EXP = 0,
            APP = 2,
            MAP = 0,
            HasDiedOnce = false,
            HasSeenItemOI = false,
            SeatedToggle = false,
        };
        connection.InsertOrReplace(defaultSettings);

    }

    private DefaultValueParameter GetDefaultSettingValue(string columnName)
    {
        return columnName switch
        {
            nameof(Settings.MovementType) => new DefaultValueParameter { DefaultValueInt = 0 },
            nameof(Settings.SmoothMovementSpeed) => new DefaultValueParameter { DefaultValueFloat = 100f },
            nameof(Settings.MenuSound) => new DefaultValueParameter { DefaultValueFloat = 0.25f },
            nameof(Settings.DungeonSound) => new DefaultValueParameter { DefaultValueFloat = 0.5f },
            nameof(Settings.SlumsSound) => new DefaultValueParameter { DefaultValueFloat = 0.05f },
            nameof(Settings.ForestSound) => new DefaultValueParameter { DefaultValueFloat = 0.1f },
            nameof(Settings.DwarfSound) => new DefaultValueParameter { DefaultValueFloat = 0.1f },
            nameof(Settings.DragonSound) => new DefaultValueParameter { DefaultValueFloat = 0.075f },
            nameof(Settings.MonsterSound) => new DefaultValueParameter { DefaultValueFloat = 0.05f },
            nameof(Settings.Level) => new DefaultValueParameter { DefaultValueInt = 1 },
            nameof(Settings.STR) => new DefaultValueParameter { DefaultValueInt = 1 },
            nameof(Settings.CON) => new DefaultValueParameter { DefaultValueInt = 1 },
            nameof(Settings.DEX) => new DefaultValueParameter { DefaultValueInt = 1 },
            nameof(Settings.EXP) => new DefaultValueParameter { DefaultValueInt = 0 },
            nameof(Settings.APP) => new DefaultValueParameter { DefaultValueInt = 2 },
            nameof(Settings.MAP) => new DefaultValueParameter { DefaultValueInt = 0 },
            nameof(Settings.HasDiedOnce) => new DefaultValueParameter { DefaultValueBool = false },
            nameof(Settings.HasSeenItemOI) => new DefaultValueParameter { DefaultValueBool = false },
            nameof(Settings.SeatedToggle) => new DefaultValueParameter { DefaultValueBool = false },
            _ => null
        };
    }

    public void SaveSettings(bool movementType, float smoothMovementSpeed, float[] soundSettings, int level, int str, int con, int dex, int exp, int app, int map, bool reset, bool died, bool itemoi, bool seatedtoggle)
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.MovementType = movementType ? 1 : 0;
            settings.SmoothMovementSpeed = smoothMovementSpeed;
            settings.MenuSound = soundSettings[0];
            settings.DungeonSound = soundSettings[1];
            settings.SlumsSound = soundSettings[2];
            settings.ForestSound = soundSettings[3];
            settings.DwarfSound = soundSettings[4];
            settings.DragonSound = soundSettings[5];
            settings.MonsterSound = soundSettings[6];
            settings.Level = level;
            settings.STR = str;
            settings.CON = con;
            settings.DEX = dex;
            settings.EXP = exp;
            settings.APP = app;
            if (map > settings.MAP || reset)
            {
                settings.MAP = map;
            }
            settings.HasDiedOnce = died;
            settings.HasSeenItemOI = itemoi;
            settings.SeatedToggle = seatedtoggle;
            connection.Update(settings);
        }
    }

    public void LoadSettings(out bool movementType, out float smoothMovementSpeed, out float[] soundSettings, out int level, out int str, out int con, out int dex, out int exp, out int app, out int map, out bool died, out bool itemoi, out bool seatedtoggle)
    {
        movementType = false;
        smoothMovementSpeed = 0f;
        soundSettings = new float[7];
        level = 0;
        str = 0;
        con = 0;
        dex = 0;
        exp = 0;
        app = 0;
        map = 0;
        died = false;
        itemoi = false;
        seatedtoggle = false;

        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            movementType = settings.MovementType == 1;
            smoothMovementSpeed = settings.SmoothMovementSpeed;
            soundSettings[0] = settings.MenuSound;
            soundSettings[1] = settings.DungeonSound;
            soundSettings[2] = settings.SlumsSound;
            soundSettings[3] = settings.ForestSound;
            soundSettings[4] = settings.DwarfSound;
            soundSettings[5] = settings.DragonSound;
            soundSettings[6] = settings.MonsterSound;
            level = settings.Level;
            str = settings.STR;
            con = settings.CON;
            dex = settings.DEX;
            exp = settings.EXP;
            app = settings.APP;
            map = settings.MAP;
            died = settings.HasDiedOnce;
            itemoi = settings.HasSeenItemOI;
            seatedtoggle = settings.SeatedToggle;
        }
    }

    public int GetLevel()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.Level;
        }
        return 0;
    }

    public int GetSTR()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.STR;
        }
        return 0;
    }

    public int GetCON()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.CON;
        }
        return 0;
    }

    public int GetDEX()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.DEX;
        }
        return 0;
    }

    public int GetEXP()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.EXP;
        }
        return 0;
    }
    public int GetAPP()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.APP;
        }
        return 0;
    }
    public int GetMAP()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.MAP;
        }
        return 0;
    }
    public bool GetDied()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.HasDiedOnce;
        }
        return false;
    }
    public bool Getitemoi()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.HasSeenItemOI;
        }
        return false;
    }
    public bool GetSeated()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.SeatedToggle;
        }
        return false;
    }
    public bool GetMovementType()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.MovementType == 1;
        }
        return false;
    }

    public float GetSmoothMovementSpeed()
    {
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            return settings.SmoothMovementSpeed;
        }
        return 0f;
    }

    public float[] GetSoundSettings()
    {
        var soundSettings = new float[7];
        var settings = connection.Table<Settings>().FirstOrDefault();
        if (settings != null)
        {
            soundSettings[0] = settings.MenuSound;
            soundSettings[1] = settings.DungeonSound;
            soundSettings[2] = settings.SlumsSound;
            soundSettings[3] = settings.ForestSound;
            soundSettings[4] = settings.DwarfSound;
            soundSettings[5] = settings.DragonSound;
            soundSettings[6] = settings.MonsterSound;
        }
        return soundSettings;
    }
    public void CloseConcection()
    {
        if (connection != null)
        {
            connection.Close();
            connection = null;
        }
    }
}


