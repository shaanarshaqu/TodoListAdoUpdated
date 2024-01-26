using Microsoft.Data.SqlClient;
using System;
using TodoListAdo.Depandancies;

namespace TodoListAdo.Models
{
    public interface ITodos
    {
        List<TodoDTO> DisplayTodos(int id);
        bool AddTodo(inputTodoDTO todo);
        bool UpdateTodo(int id, inputTodoDTO todo);
        bool DeleteTodo(int id);
    }


    public class Todos: ITodos
    {
        private readonly IConfiguration _configuration;
        private readonly string connection_source;
        public Todos(IConfiguration configuration)
        {
            _configuration=  configuration;
            connection_source = _configuration["ConnectionStrings:ConnectionObj"];
        }



        public List<TodoDTO> DisplayTodos(int id)
        {
            List <TodoDTO> todos = new List <TodoDTO> ();

            using(SqlConnection connect = new SqlConnection(connection_source))
            {
                connect.Open ();
                SqlCommand cmd = new SqlCommand($"select * from TodoList where UserId={id}",connect);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read ())
                {
                    int todoid= int.Parse(dataReader["TodoId"].ToString());
                    string todo = dataReader["Todo"].ToString();
                    int userid = int.Parse(dataReader["UserId"].ToString());

                    todos.Add(new TodoDTO
                    {
                        TodoId = todoid,
                        Todo = todo,
                        UserId = userid
                    });
                }
                return todos;
            }
        }


        public bool AddTodo(inputTodoDTO todo)
        {
            using(SqlConnection connect =new SqlConnection(connection_source))
            {
                connect.Open ();
                SqlCommand cmd = new SqlCommand("insert into TodoList values (@todo,@userid)", connect);
                cmd.Parameters.AddWithValue("@todo", todo.Todo);
                cmd.Parameters.AddWithValue("@userid", todo.UserId);
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        
        
        public bool UpdateTodo(int id, inputTodoDTO todo)
        {
            using(SqlConnection  connection = new SqlConnection(connection_source))
            {
                connection.Open ();
                SqlCommand cmd = new SqlCommand("update TodoList set Todo=@todo where TodoId=@id", connection);
                cmd.Parameters.AddWithValue("@todo", todo.Todo);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery ();
                return true;
            }
        }

        public bool DeleteTodo(int id)
        {
            using(SqlConnection connect = new SqlConnection(connection_source))
            {
                connect.Open ();
                SqlCommand cmd = new SqlCommand($"delete from TodoList where TodoId={id}",connect);
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
