using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TodosApplication.DAL;
using TodosApplication.Model;

namespace TodosApplication.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly IDbContext dbContext;

        public TodoController(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAllTodos(){   
            return await dbContext.Todo.Include(t => t.Type).OrderBy(t=>t.Order).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo([FromForm] string data)
        {
            if(data == null)
            {
                return BadRequest("Nem küldtél adatot a szerverre!");
            }

            dynamic json = JsonConvert.DeserializeObject(data);

            string format = "yyyy-MM-dd";          

            string mezo = json["mezo"];
            string deadline = json["deadline"];
            string name = json["name"];
            string details = json["details"];
            
            if(String.IsNullOrEmpty(mezo) || String.IsNullOrEmpty(deadline) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(details))
            {
                return BadRequest("Töltsd ki az összes adatot!");
            }

            if (!DateTime.TryParseExact(deadline, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime deadlientime))
            {
                return BadRequest("Rossz a dátum formátum!");
            }

            var type = await dbContext.TodoTypes.Where(t => t.Name.ToLower().Equals(mezo)).SingleOrDefaultAsync();
            if(type == null)
            {
                return NotFound("Nem található a tábla.");
            }

            var maxseq = dbContext.Todo.Where(t => t.TypeId == type.Id);
            int max = 0;
            if(maxseq.Any())
            {
                max = await maxseq.Select(t=>t.Order).MaxAsync();
            }

            Todo t = new();
            t.Name = name;
            t.Details = details;
            t.TypeId = type.Id;
            t.Type = type;
            t.Order = max + 1;
            t.Deadline = deadlientime;
            dbContext.Todo.Add(t);
            await dbContext.SaveChangesAsync();


            return t ;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Todo>> DeleteTodo(int id)
        {
            Todo t = await dbContext.Todo.Where(a => a.Id == id).SingleOrDefaultAsync();

            if (t != null)
                dbContext.Todo.Remove(t);
            else
            {
                return BadRequest("Nem található a törölni kívánt elem.");
            }
            var todotypes = dbContext.Todo.Where(t => t.TypeId == t.TypeId);
            await ReOrderTodos(todotypes);
            return Ok();

        }

        [HttpPut("toothercolumn")]
        public async Task<ActionResult<Todo>> PutTodoColumn([FromBody] ItemMove data)
        {
            if(data.Id == -1 || String.IsNullOrEmpty(data.DestinationTableName) || data.DestinationIndex == -1)
            {
                return BadRequest("Nem adtál meg elegendő adatot!");
            }
            
            
            var item = dbContext.Todo.Where(t => t.Id == data.Id).SingleOrDefault();
            if(item == null)
            {
                return NotFound("Nem található ilyen Item");
            }
            var type = dbContext.TodoTypes.Where(t => t.Name.ToLower().Equals(data.DestinationTableName.ToLower())).SingleOrDefault();
            if(type == null)
            {
                return NotFound("Nem található ilyen Tábla");
            }

            var originalTypeTodos = dbContext.Todo.Where(t => t.TypeId == item.TypeId && t.Id != item.Id);
            await ReOrderTodos(originalTypeTodos);


            int number = item.Order;
            item.Type = type;
            item.TypeId = type.Id;
            item.Order = data.DestinationIndex;

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("sort")]
        public async Task<ActionResult<Todo>> PutTodoSort([FromBody] ItemMove data)
        {
            if (data.Id == -1 || data.DestinationIndex == -1)
            {
                return BadRequest("Nem adtál meg elegendő adatot!");
            }

            //adatok

            var item = dbContext.Todo.Where(t => t.Id == data.Id).SingleOrDefault();
            if(item == null)
            {
                return NotFound("Nem található ilyen teendő");
            }
            //-adatok

            //újtábla újra rendezése
            var todos = dbContext.Todo.Where(t =>  t.TypeId == item.TypeId).OrderBy(t => t.Order).ToList();

            await dbContext.SaveChangesAsync();
            await ReOrderTodosRowSort(todos, item.Order, data.DestinationIndex);
            
            return Ok();
        }

        public async Task<ActionResult<Todo>> ReOrderTodosRowSort(List<Todo> todos,int srcindex,int destinationindex)
        {
            Todo b = todos[srcindex];
            todos.RemoveAt(srcindex);
            todos.Insert(destinationindex, b);

            int index = 0;
            foreach (var t in todos)
            {
                t.Order = index++;
            }

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        private async Task<ActionResult<Todo>> ReOrderTodos(IQueryable<Todo> todos, int order= 0, Todo todo = null)
        {
            int index = order;
            if(todo == null)
            {
                foreach (var t in todos)
                {                     
                    t.Order = index++;
                }
            }
            else
            {
                foreach (var t in todos)
                {
                    if (t.Id == todo.Id)
                    {
                        continue;
                    }
                    t.Order = ++index;
                }
            }
            

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }

}
