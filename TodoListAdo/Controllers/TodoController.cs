using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoListAdo.Depandancies;
using TodoListAdo.Models;

namespace TodoListAdo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodos _todosData;
        public TodoController(ITodos todosData)
        {
            _todosData= todosData;
        }



        [HttpGet("{id:int}",Name= "Get")]
        [Authorize]
        public IActionResult Get(int id) 
        {
            var result = _todosData.DisplayTodos(id);
            return Ok(result);
        }


        [HttpPost("AddTodo")]
        [Authorize]
        public IActionResult AddTodo([FromBody] inputTodoDTO todo)
        {
            bool isAdded = _todosData.AddTodo(todo);
            return CreatedAtRoute("Get", new { id = todo.UserId},todo );
        }


        [HttpPut("{id:int}")]
        [Authorize]
        public IActionResult UpdateTodo(int id,[FromBody] inputTodoDTO todo)
        {
            bool isUpdated= _todosData.UpdateTodo(id,todo);
            return Ok(isUpdated);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult DeleteTodo(int id)
        {
            bool isDeleted = _todosData.DeleteTodo(id);
            return Ok(isDeleted);
        }

    }
}
