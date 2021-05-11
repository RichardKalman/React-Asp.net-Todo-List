using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodosApplication.Controllers;
using TodosApplication.DAL;
using TodosApplication.Model;

namespace Tests
{
    [TestClass]
    public class TodoTest
    {
        Mock<IDbContext> dbcontex;
        [TestInitialize]
        public void init()
        {
            var egyType = new TodoType();
            egyType.Id = 1;
            egyType.Name = "Teszt";

            var TypeList = new List<TodoType> { egyType };
            var types = MockDbSet(TypeList);

            var TestTodos = new List<Todo>();

            for (int i = 0; i < 5; i++)
            {
                var t = new Todo();
                t.Deadline = DateTime.Now;
                t.Details = $"{i} Todo";
                t.Id = i + 1;
                t.Name = $"{i} Todo";
                t.Type = egyType;
                t.TypeId = 1;
                t.Order = i + 1;
                TestTodos.Add(t);

            }
            var TestTodosMock = MockDbSet(TestTodos);

            this.dbcontex = new Mock<IDbContext>();
            this.dbcontex.Setup(m => m.TodoTypes).Returns(types.Object);
            this.dbcontex.Setup(m => m.Todo).Returns(TestTodosMock.Object);
        }

        [TestMethod]
        public void TodoToUp()
        {          
            var todoController = new TodoController(this.dbcontex.Object);
            todoController.ReOrderTodosRowSort(this.dbcontex.Object.Todo.ToList(), 4, 1);
            int todoOrder = this.dbcontex.Object.Todo.Where(t => t.Id == 5).Single().Order;
            Assert.AreEqual(1, todoOrder);
        }
        [TestMethod]
        public void TodoToDown()
        {
            var todoController = new TodoController(this.dbcontex.Object);
            todoController.ReOrderTodosRowSort(this.dbcontex.Object.Todo.ToList(), 0, 4);
            int todoOrder = this.dbcontex.Object.Todo.Where(t => t.Id == 1).Single().Order;
            Assert.AreEqual(4, todoOrder);
        }

        Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> list) where T : class, new()
        {
            IQueryable<T> queryableList = list.AsQueryable();
            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
            dbSetMock.Setup(d => d.Add(It.IsAny<T>()));


            return dbSetMock;
        }
    }
}
