using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;




public static class SaveSystem
{
   
   public static void SaveLevel(int index, TowerData saveData)
    {

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "twrsFolder")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "twrsFolder")); 
        }

        FileStream stream;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.twr",index));

        //if (File.Exists(path))
        //{
        //    stream = new FileStream(path, FileMode.Create);
        //}
        //else
        //{
            stream = new FileStream(path, FileMode.Create);
        //}
           

        //TowerData data = new TowerData();


        formatter.Serialize(stream, saveData);
        stream.Close();

        Debug.Log("SAVED " + path);
    }


    public static TowerData LoadLevel(int index)
    {
        string path = Path.Combine(Application.persistentDataPath, "twrsFolder", string.Format("twrs{0}.twr", index));
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            TowerData data = formatter.Deserialize(stream) as TowerData;

            stream.Close();

            return data;
            
        }
        else
        {
            Debug.Log("NO SUCH FILE");
            return null;
        }
    }
}
