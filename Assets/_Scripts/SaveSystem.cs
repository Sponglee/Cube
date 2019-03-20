using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;




public static class SaveSystem
{
   
   public static void SaveLevel(int index, TowerData saveData, bool BackupSave=false)
    {

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "twrsFolder")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "twrsFolder")); 
        }

        FileStream stream;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.twr",index));

        if(BackupSave)
        {
            FileStream backupStream;
            string bckupPath = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.bak", index));
            backupStream = new FileStream(bckupPath, FileMode.Create);
            formatter.Serialize(backupStream, saveData);
            backupStream.Close();
        }

        stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();

        Debug.Log("SAVED " + path);
    }


    public static TowerData LoadLevel(int index, bool backUp = false)
    {
        string path = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.twr", index));
        string bckupPath = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.bak", index));

        if(backUp)
        {
            //LOAD BACKUP FILE
            if (File.Exists(bckupPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = new FileStream(path, FileMode.Open);

                TowerData data = formatter.Deserialize(stream) as TowerData;

                stream.Close();
                Debug.Log("LOADED BACKUP " + path);
                return data;
            }
            else
                return null;
        }
        //LOAD NORMAL FILE
        else if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            TowerData data = formatter.Deserialize(stream) as TowerData;

            stream.Close();
            Debug.Log("LOADED " + path);
            return data;
            
        }
        else
        {
            Debug.Log("NO SUCH FILE");
            return null;
        }
    }
}
