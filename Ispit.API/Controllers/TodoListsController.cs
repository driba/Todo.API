using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ispit.API.Data;
using Ispit.API.Models;

namespace Ispit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoLists
        [HttpGet]
        public ActionResult<IEnumerable<Todo>> GetTodoList()
        {
            try
            {
                if (_context.TodoList.ToList() == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Database or table not found!");
                }
                return Ok(_context.TodoList.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            };           
          
        }

        // POST: api/TodoLists
        [HttpPost]
        public ActionResult<Todo> PostTodo(Todo todo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                // za slucaj da nema autoincrementa u tablici, upisuje se u prvi po redu slobodan Id
                var list_count = _context.TodoList.Count();
                if (list_count == 0)
                {
                    todo.Id = 1;
                }

                for (int i = 1; i < list_count + 1; i++)
                {
                    if (!TodoExists(i))
                    {
                        todo.Id = i;
                        break;
                    }

                }

                _context.TodoList.Add(todo);
                _context.SaveChanges();

                return Ok("Zapis kreiran!");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // GET: api/TodoLists/5
        [HttpGet("{id:int}")]
        public ActionResult<Todo> GetTodo(int id)
        {
            try
            {
                var todo = GetTodoById(id);

                if (todo == null)
                {
                    return NotFound("Zapis nije pronađen...");
                }

                return Ok(todo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/TodoLists/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Podaci zapisa nisu validni");
            }

            var result = GetTodoById(id);

            if (result == null)
            {
                return NotFound("Zapis nije pronađen!");
            }

            result.Title = todo.Title;
            result.Description = todo.Description;
            result.IsCompleted = todo.IsCompleted;
            
            _context.SaveChanges();

            return Ok(result);

        }

        

        // DELETE: api/TodoLists/5
        [HttpDelete("{id:int}")]
        public ActionResult DeleteTodo(int id)
        {
            try
            {
                var todo = GetTodoById(id);
                if (todo == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Zapis nije pronađen!");
                }
                _context.TodoList.Remove(todo);
                _context.SaveChanges();

                return Ok("Zapis je izbrisan!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }


        #region Helper methods
        private bool TodoExists(int id)
        {
            return (_context.TodoList?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private Todo? GetTodoById(int id)
        {
            return _context.TodoList.FirstOrDefault(t => t.Id == id);
        }
        #endregion

    }
}
