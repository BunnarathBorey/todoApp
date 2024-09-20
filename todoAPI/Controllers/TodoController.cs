using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using todoAPI.Data;
using todoAPI.Models;

namespace todoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        // context variable
        private readonly ApplicationDbContext _context;

        // Declare class constructor
        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/todo
        // This API is used to create a todo
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] TodoModel model)
        {
            model.Created = DateTime.Now;
            _context.todos.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetTodoDetails),
                new { id = model.TodoId },
                model
            );
        }

        // PUT: api/todo/{id}
        // This API is used to update todo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] TodoModel model)
        {
            model.Updated = DateTime.Now;
            // Check if the id matches the model's TodoId
            if (id != model.TodoId)
            {
                return BadRequest("Todo ID mismatch");
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the todo still exists
                if (!_context.todos.Any(e => e.TodoId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw the exception if todo still exists
                }
            }

            return NoContent(); // Returns NoContent when successful
        }

        // DELETE: api/todo/{id}
        // This API is used to delete a todo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            var todo = await _context.todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns NoContent when deleted
        }

        // GET: api/todo
        // This API is used to get all todo data records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoModel>>> GetTodos()
        {
            var todos = await _context.todos.ToListAsync();
            return Ok(todos); // Returns all todos
        }

        // GET: api/todo/{id}
        // This API is used to get todo details
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoModel>> GetTodoDetails(Guid id)
        {
            var todo = await _context.todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo); // Returns the specific todo
        }
    }
}
