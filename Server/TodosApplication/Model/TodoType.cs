using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodosApplication.Model
{


    public class TodoType : IEquatable<TodoType>
    {
        
        public int Id {  get; set; }
        public string Name { get; set; }

        public int Order { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TodoType);
        }

        public bool Equals(TodoType other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

    }
}
