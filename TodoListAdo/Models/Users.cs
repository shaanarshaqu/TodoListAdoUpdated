using Microsoft.Data.SqlClient;
using System;
using TodoListAdo.Depandancies;

namespace TodoListAdo.Models
{

    public interface IUsers
    {
        List<UsersDTO> DisplayUsers();
        bool Register(inputUserDTO user);
        UsersDTO Login(inputUserDTO user);

    }
    public class Users: IUsers
    {
        private readonly IConfiguration _configuration;
        private readonly string connection_source;
        public Users(IConfiguration configuration) 
        {
            _configuration= configuration;
            connection_source = _configuration["ConnectionStrings:ConnectionObj"];
        }





        public List<UsersDTO> DisplayUsers()
        {
            List<UsersDTO> users = new List<UsersDTO>();

            using (SqlConnection connect = new SqlConnection(connection_source))
            {
                connect.Open();
                SqlCommand cmd = new SqlCommand("select * from Users", connect);
                SqlDataReader sqlDataReader = cmd.ExecuteReader();
                while(sqlDataReader.Read())
                {
                    int id = int.Parse(sqlDataReader["Id"].ToString());
                    string user = sqlDataReader["UserName"].ToString();
                    string pass = sqlDataReader["Password"].ToString();
                    string role = sqlDataReader["Role"].ToString();

                    users.Add(new UsersDTO
                    {
                        Id = id,
                        UserName= user,
                        Password= pass,
                        Role= role
                    });
                }
                return users;

            }

        }


        public bool Register(inputUserDTO user)
        {
            using(SqlConnection connect = new SqlConnection(connection_source))
            {
                connect.Open();
                SqlCommand cmd = new SqlCommand("insert into Users values(@username,@password,'user')", connect);
                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@username", user.Password);
                cmd.ExecuteNonQuery();
                return true;
            }
        }



        public UsersDTO Login(inputUserDTO user)
        {
            using(SqlConnection  connection = new SqlConnection(connection_source))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select * from Users where UserName=@name and Password=@pass", connection);
                cmd.Parameters.AddWithValue("@name", user.UserName);
                cmd.Parameters.AddWithValue("@pass", user.Password);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                UsersDTO foundeduser=null;
                if (reader.Read())
                {
                    foundeduser = new UsersDTO();
                    foundeduser.Id= int.Parse(reader["Id"].ToString());
                    foundeduser.UserName = reader["UserName"].ToString();
                    foundeduser.Role = reader["Role"].ToString();
                }
                return foundeduser;
            }
        }
    }
}
