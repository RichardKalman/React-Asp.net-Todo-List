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
                return BadRequest();
            }

            dynamic json = JsonConvert.DeserializeObject(data);
            string name = json["name"];
            

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

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<TodoType>>> DeleteTodoTypes([FromForm] string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);
            int id = Convert.ToInt32(json["id"]);
            TodoType todotype = await dbContext.TodoTypes.Where(a => a.Id == id).SingleOrDefaultAsync();

            var todos = dbContext.Todo.Where(t => t.TypeId == id).ToArray();
            if (todos.Any())
            {
                for (int i = 0; i < todos.Length; i++)
                {
                    dbContext.Todo.Remove(todos[i]);
                }
            }


            if (todotype != null)
                dbContext.TodoTypes.Remove(todotype);
            else
            {
                return BadRequest();
            }

            await dbContext.SaveChangesAsync();

            return await ReOrderTodoType();


        }

        [HttpPut("rowSort")]
        public async Task<ActionResult<Todo>> PutTodoSort([FromForm] string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);

            //adatok
            int id = Convert.ToInt32(json["item"].id); ;
            int srcindex = Convert.ToInt32(json["srcindex"]);
            int destinationindex = Convert.ToInt32(json["destinationindex"]);
            //-adatok

            //újtábla újra rendezése
            var todotypes = dbContext.TodoTypes.OrderBy(t => t.Order).ToList();

            await dbContext.SaveChangesAsync();
            await ReOrderTodoTypeRowSort(todotypes, srcindex, destinationindex);

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
