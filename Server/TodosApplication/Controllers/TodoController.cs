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
                return BadRequest();
            }

            dynamic json = JsonConvert.DeserializeObject(data);

            string format = "yyyy-MM-dd";
            

            string mezo = json["mezo"];
            string deadline = json["deadline"];
            string name = json["name"];
            string details = json["details"];

            if (!DateTime.TryParseExact(deadline, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime deadlientime))
            {
                return BadRequest();
            }

            var type = await dbContext.TodoTypes.Where(t => t.Name.ToLower().Equals(mezo)).SingleAsync();

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

        [HttpDelete]
        public async Task<ActionResult<Todo>> DeleteTodo([FromForm] string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);
            int id = Convert.ToInt32(json["id"]);
            Todo ad = await dbContext.Todo.Where(a => a.Id == id).SingleOrDefaultAsync();

            var todos = dbContext.Todo.Where(t => t.TypeId == ad.TypeId);      


            if (ad != null)
                dbContext.Todo.Remove(ad);
            else
            {
                return BadRequest();
            }

            await ReOrderTodos(todos);

            return Ok();

        }

        [HttpPut("toothercolumn")]
        public async Task<ActionResult<Todo>> PutTodoColumn([FromForm] string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);
            string destinationid = json["destinationid"];

            //adatok
            int id = Convert.ToInt32(json["item"].id);
            int index = Convert.ToInt32(json["index"]);
            var item = dbContext.Todo.Where(t => t.Id == id).SingleOrDefault();
            //-adatok


            var type = dbContext.TodoTypes.Where(t => t.Name.ToLower().Equals(destinationid.ToLower())).SingleOrDefault();

            var originalTypeTodos = dbContext.Todo.Where(t => t.TypeId == item.TypeId && t.Id != item.Id);
            await ReOrderTodos(originalTypeTodos);

            if (type == null)
            {
                return BadRequest();
            }

            int number = item.Order;
            item.Type = type;
            item.TypeId = type.Id;
            item.Order = index;

            await dbContext.SaveChangesAsync();

            //újtábla újra rendezése
            var todos = dbContext.Todo.Where(t => t.Order >= index && t.TypeId==type.Id /*&& t.Id != item.Id*/).OrderBy(t=>t.Order);
            await ReOrderTodos(todos, index,item);

            return Ok();
        }

        [HttpPut("sort")]
        public async Task<ActionResult<Todo>> PutTodoSort([FromForm] string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);

            //adatok
            int id = Convert.ToInt32(json["item"].id);;
            int srcindex = Convert.ToInt32(json["srcindex"]);
            int destinationindex = Convert.ToInt32(json["destinationindex"]);
            var item = dbContext.Todo.Where(t => t.Id == id).SingleOrDefault();
            //-adatok

            //újtábla újra rendezése
            var todos = dbContext.Todo.Where(t =>  t.TypeId == item.TypeId).OrderBy(t => t.Order).ToList();

            await dbContext.SaveChangesAsync();
            await ReOrderTodosRowSort(todos, srcindex, destinationindex);

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
