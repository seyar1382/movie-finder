using System.Data;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using Microsoft.Data.Sqlite;


namespace MovieFinder.API
{
    class FileControl
    {
        // Defaults to the root path to prevent errors
        private string filePath = "\\";

        // A flag to determine if it writes to the local user data 
        public bool userEnviromentUse = true;

        // Gives the name of the local application folder used to store the local user data
        private string userEnvirmomentName = "movie finder";


        //Set the file path
        public void SetPath(string newPath)
        {
            filePath = newPath;
        }

        // Set the name of the local application folder
        public void EnviromentName(string name)
        {
            userEnvirmomentName = name;
        }

        // Get the file path and return the file path
        public string GetPath()
        {
            return filePath;
        }

        //Get the full folder path and create the file at the correct location and the directories needed 
        public void CreateFile()
        {
            //use stringbuilder to reduce longer merging times for longer file names or path;
            StringBuilder folderPath = new StringBuilder();
            string enviromentPath = "";
            //checks if using user data;
            if(userEnviromentUse == true)
            {
                enviromentPath = GetUserEnviroment();
            }
            //Split the file path with \\ to make sure it is in the correct format for file location and combine them together to create the full file path 
            string[] folders = filePath.Split("\\");
            int length = folders.Length;
            int maxIndex = length;
            folders[maxIndex] = "";
            folderPath.AppendJoin("\\", folders);
            //create file and the directories 
            Console.WriteLine(enviromentPath + folderPath);
            Directory.CreateDirectory(enviromentPath+folderPath.ToString());
            File.Create(enviromentPath+filePath).Close();
        }

        public void DeleteFile()// a test funstion not tested and wount fully work it is a placeholder
        {
            //use stringbuilder to reduce longer merging times for longer file names or path;
            StringBuilder folderPath = new StringBuilder();
            string enviromentPath = "";
            //checks if using user data;
            if(userEnviromentUse == true)
            {
                enviromentPath = GetUserEnviroment();
            }
            //split so can seperate just path and sort path from file location;
            string[] folders = filePath.Split("\\");
            int length = folders.Length;
            int maxIndex = length;
            folders[maxIndex] = "";
            folderPath.AppendJoin("\\", folders);
            //create everything;
            Console.WriteLine(enviromentPath + folderPath);
            Directory.Delete(enviromentPath+folderPath.ToString());
            File.Delete(enviromentPath+filePath);
        }

        //Write to the newly created file
        public void WriteFile(string data)
        {
            string enviromentPath = "";
            //checks if using user data;
            if(userEnviromentUse == true)
            {
                enviromentPath = GetUserEnviroment();
            }
            File.WriteAllText(enviromentPath + filePath, data);
        }

        // Reads the newly created file 
        public string ReadFile()
        {
            string enviromentPath = "";
            //checks if using user data;
            if(userEnviromentUse == true)
            {
                enviromentPath = GetUserEnviroment();
            }
            string data = File.ReadAllText(enviromentPath + filePath);
            return data;
        }

        // Get the location of the application data folder for current user
        public string GetUserEnviroment()
        {
            string applicationDataLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return applicationDataLocation;
        }
    }

    class DatabaseSqlite
    {
        private SqliteConnection connection = new SqliteConnection();

        private bool userEnviroment;

        //private string DB_file_path;

        public void SqliteConnect(string dbPath, bool userEnviromentUse = true)
        {
            userEnviroment = userEnviromentUse;
            StringBuilder path = new StringBuilder();
            FileControl fileControl = new FileControl();
            path.Append("Data Source=");

            if (userEnviromentUse == true)
            {
                path.Append(fileControl.GetUserEnviroment().Split(":")[1]);
            }
            path.Append(dbPath);
            //DB_file_path = path.ToString();
            connection = new SqliteConnection(path.ToString());
            connection.Open();
            Console.WriteLine(path.ToString());
        }

        public int SqliteCreateTable()
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = """CREATE TABLE IF NOT EXISTS Saved_list ([id] Integer, [name] Text)""";
                command.ExecuteNonQuery();
                return 0;
            }
            catch
            {
                Console.WriteLine("Error creating database table...");
                return 1;
            }
        }

        public int SqliteAddItem(int id, string name)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = """INSERT INTO Saved_list (id, name) VALUES ($id, $name)""";
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$name", name);
                command.ExecuteNonQuery();
                return 0;
            }
            catch
            {
                Console.WriteLine("Error writing to database table...");
                return 1;
            }
        }

        public int SqliteRemoveItem(int id)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = """DELETE FROM Saved_list WHERE id = $id""";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
                return 0;
            }
            catch
            {
                Console.WriteLine("Error removing item from database table...");
                return 1;
            }
        }

        public string[] SqliteGetItems()
        {
            string [] savedArray= [];
            try
            {

                using var command = connection.CreateCommand();
                command.CommandText = """SELECT * FROM Saved_list""";
                var data = command.ExecuteReader();
                
                while (data.Read())
                {
                    savedArray.Append(data.GetInt32(0).ToString());
                    savedArray.Append(data.GetString(1));
                }

                return savedArray;

            }
            catch
            {
                Console.WriteLine("Error getting items from database table...");
                return savedArray;
            }
        }

    }
}