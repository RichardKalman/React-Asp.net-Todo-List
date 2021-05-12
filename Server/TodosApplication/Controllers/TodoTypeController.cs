using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodosApplication.DAL;
using TodosApplication.Model;

namespace TodosApplication.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class TodoTypeController : ControllerBase
    {
        private readonly IDbContext dbContext;

        public TodoTypeController(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoType>>> GetAllTodoType()
        {
            var types = await dbContext.TodoTypes.OrderBy(tt => tt.Order).ToListAsync();
      
            return types;
        }

        [HttpPost]
        public async Task<ActionResult<TodoType>> PostTodoType([FromForm] string data)
        {
            if (data == null)
            {
                return BadRequest("Nem küldtél adatot a szerverre!");
            }

            dynamic json = JsonConvert.DeserializeObject(data);
            string name = json["name"];
            if (String.IsNullOrEmpty(name))
            {
                return BadRequest("Töltsd ki az összes adatot!");
            }

            var maxseq = dbContext.TodoTypes;
            int max = 0;
            if (maxseq.Any())
            {
                max = await maxseq.Select(t => t.Order).MaxAsync();
            }

            TodoType t = new();
            t.Name = name;
            t.Order = max + 1;
            dbContext.TodoTypes.Add(t);
            await dbContext.SaveChangesAsync();


            return t;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<IEnumerable<TodoType>>> DeleteTodoTypes(int id)
        {
            
            TodoType todotype = await dbContext.TodoTypes.Where(a => a.Id == id).SingleOrDefaultAsync();
            if (todotype != null)
                dbContext.TodoTypes.Remove(todotype);
            else
            {
                return NotFound("Nincs ilyen tábla");
            }


            var todos = dbContext.Todo.Where(t => t.TypeId == id).ToArray();
            if (todos.Any())
            {
                for (int i = 0; i < todos.Length; i++)
                {
                    dbContext.Todo.Remove(todos[i]);
                }
            }

            await dbContext.SaveChangesAsync();

            return await ReOrderTodoType();


        }

        [HttpPut("rowSort")]
        public async Task<ActionResult<Todo>> PutTodoTypeSort([FromBody] ItemMove data)
        {

            if (data.Id == -1 || data.DestinationIndex == -1 || data.SourceIndex == -1)
            {
                return BadRequest("Nem adtál meg elegendő adatot!");
            }
            
            var tt = dbContext.TodoTypes.Where(t => t.Id == data.Id).SingleOrDefault();
            if(tt == null)
            {
                return NotFound("Nem található ilyen tábla");
            }
            var todotypes = dbContext.TodoTypes.OrderBy(t => t.Order).ToList();

            await dbContext.SaveChangesAsync();
            await ReOrderTodoTypeRowSort(todotypes, data.SourceIndex, data.DestinationIndex);

            return Ok();
        }

        public async Task<ActionResult<TodoType>> ReOrderTodoTypeRowSort(List<TodoType> todos, int srcindex, int destinationindex)
        {
            TodoType swaptt = todos[srcindex];
            todos.RemoveAt(srcindex);
            todos.Insert(destinationindex, swaptt);         
            

            int index = 1;
            foreach (var t in todos)
            {
                t.Order = index++;
            }

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        private async Task<ActionResult<IEnumerable<TodoType>>> ReOrderTodoType()
        {

            List<TodoType> todos = dbContext.TodoTypes.OrderBy(t => t.Order).ToList();
            int index = 1;
            foreach (var t in todos)
            {
                t.Order = index++;
            }

            await dbContext.SaveChangesAsync();
            return todos;
        }



    }
}
