using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SavePlayerData(Player player)
    {
        // create binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        // give a name for the file and then create it
        string path = Application.persistentDataPath + "/player.file";
        FileStream stream = new FileStream(path, FileMode.Create);

        // get data from player
        PlayerData data = new PlayerData(player);

        // serialize stream and data into binary file
        formatter.Serialize(stream, data);

        // close file to avoid any bugs
        stream.Close();

        Debug.Log("Saved player data: " + path);
    }

    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/player.file";
        if (File.Exists(path) == true)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file for " + path + " not found!");
            return null;
        }
    }

    public static void SavePlayerStatsData(PlayerStats stats)
    {   
        // ---------------------------------
        // Adjust values if a powerup is active
        if (PowerUp.movementPowerUp == true)
        {
            PowerUp.movementPowerUp = false;
            stats.runSpeed -= PowerUp.movementValue;
        }
        
        if (PowerUp.punchDamagePowerUp == true)
        {
            PowerUp.punchDamagePowerUp = false;
            stats.damage -= (int)PowerUp.punchDamageValue;
        }

        // ---------------------------------

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/playerStats.file";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerStatsData data = new PlayerStatsData(stats);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved player data: " + path);
    }

    public static PlayerStatsData LoadPlayerStatsData()
    {
        string path = Application.persistentDataPath + "/playerStats.file";
        if (File.Exists(path) == true)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerStatsData data = formatter.Deserialize(stream) as PlayerStatsData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file for " + path + " not found!");
            return null;
        }
    }

    public static void DeletePlayerFile()
    {
        string path = Application.persistentDataPath + "/player.file";
        File.Delete(path);
        Debug.Log("Deleted: " + path);
    }

    public static void DeletePlayerStatsFile()
    {
        string path = Application.persistentDataPath + "/playerStats.file";
        File.Delete(path);
        Debug.Log("Deleted: " + path);
    }
}
