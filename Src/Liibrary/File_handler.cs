using System.Data;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using Microsoft.Data.Sqlite;


namespace Website_scraper_v1.API
{
    class File_controll
    {
        // just start at the root so it will say permision error if not set will change later to safer default
        private string File_path = "\\";

        public bool User_enviroment_use = true;

        private string User_envirmoment_name = "Test";


        public void Set_path(string New_path)
        {
            File_path = New_path;
        }

        public void Enviroment_name(string name)
        {
            User_envirmoment_name = name;
        }

        public string Get_path()
        {
            return File_path;
        }

        public void Create_file()
        {
            //use stringbuilder to reduce longer merging times for longer file names or path;
            StringBuilder folder_path = new StringBuilder();
            string Enviroment_path = "";
            //checks if using user data;
            if(User_enviroment_use == true)
            {
                Enviroment_path = Get_user_enviroment();
            }
            //split so can seperate just path and sort path from file location;
            string[] folders = File_path.Split("\\");
            int length = folders.Length;
            int max_index = length;
            folders[max_index] = "";
            folder_path.AppendJoin("\\", folders);
            //create everything;
            Console.WriteLine(Enviroment_path + folder_path);
            Directory.CreateDirectory(Enviroment_path+folder_path.ToString());
            File.Create(Enviroment_path+File_path).Close();
        }

        public void Delete_file()// a test funstion not tested and wount fully work it is a placeholder
        {
            //use stringbuilder to reduce longer merging times for longer file names or path;
            StringBuilder folder_path = new StringBuilder();
            string Enviroment_path = "";
            //checks if using user data;
            if(User_enviroment_use == true)
            {
                Enviroment_path = Get_user_enviroment();
            }
            //split so can seperate just path and sort path from file location;
            string[] folders = File_path.Split("\\");
            int length = folders.Length;
            int max_index = length;
            folders[max_index] = "";
            folder_path.AppendJoin("\\", folders);
            //create everything;
            Console.WriteLine(Enviroment_path + folder_path);
            Directory.Delete(Enviroment_path+folder_path.ToString());
            File.Delete(Enviroment_path+File_path);
        }

        public void Write_file(string data)
        {
            string Enviroment_path = "";
            //checks if using user data;
            if(User_enviroment_use == true)
            {
                Enviroment_path = Get_user_enviroment();
            }
            File.WriteAllText(Enviroment_path + File_path, data);
        }

        public string Read_file()
        {
            string Enviroment_path = "";
            //checks if using user data;
            if(User_enviroment_use == true)
            {
                Enviroment_path = Get_user_enviroment();
            }
            string data = File.ReadAllText(Enviroment_path + File_path);
            return data;
        }

        public string Get_user_enviroment()
        {
            string Application_data_location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Application_data_location;
        }
    }

    class Database_sqlite
    {
        private SqliteConnection Connection = new SqliteConnection();

        private bool User_enviroment;

        //private string DB_file_path;

        public void Sqlite_Connect(string DB_path, bool User_enviroment_use = true)
        {
            User_enviroment = User_enviroment_use;
            StringBuilder path = new StringBuilder();
            File_controll file_Controll = new File_controll();
            path.Append("Data Source=");

            if (User_enviroment_use == true)
            {
                path.Append(file_Controll.Get_user_enviroment().Split(":")[1]);
            }
            path.Append(DB_path);
            //DB_file_path = path.ToString();
            Connection = new SqliteConnection(path.ToString());
            Connection.Open();
            Console.WriteLine(path.ToString());
        }

        public int Sqlite_create_table()
        {
            try
            {
                using var command = Connection.CreateCommand();
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

        public int Sqlite_add_item(int id, string name)
        {
            try
            {
                using var command = Connection.CreateCommand();
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

        public int Sqlite_remove_item(int id)
        {
            try
            {
                using var command = Connection.CreateCommand();
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

        public string[] Sqlite_get_items()
        {
            string [] saved_array= [];
            try
            {

                using var command = Connection.CreateCommand();
                command.CommandText = """SELECT * FROM Saved_list""";
                var data = command.ExecuteReader();
                
                while (data.Read())
                {
                    saved_array.Append(data.GetInt32(0).ToString());
                    saved_array.Append(data.GetString(1));
                }

                return saved_array;

            }
            catch
            {
                Console.WriteLine("Error getting items from database table...");
                return saved_array;
            }
        }

    }
}