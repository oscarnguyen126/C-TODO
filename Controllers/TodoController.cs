using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos(
            string? search,
            bool? isCompleted,
            int page = 1,
            int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.TodoItems.AsQueryable();

            // Search by title
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Title.Contains(search));
            }

            // Filter by status
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            // Total items
            var totalItems = await query.CountAsync();

            // Pagination
            var items = await query
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                items
            });
        }


        //GET: api/todo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }


        // GET: api/todo/completed
        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetCompleted()
        {
            var items = await _context.TodoItems.Where(t => t.IsCompleted).ToListAsync();
            return Ok(items);
        }

        // POST: api/todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodos), new { id = item.Id }, item);
        }

        // PUT: api/todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TodoItem item)
        {
            if (id != item.Id) return BadRequest();

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
