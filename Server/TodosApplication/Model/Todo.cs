using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodosApplication.Model
{
    public class Todo : IEquatable<Todo>
    {
        public int Id { get; set; }
        public int TypeId { get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public DateTime Deadline { get; set; }

        
        public TodoType Type { get; set; }


        public int Order { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Todo);
        }

        public bool Equals(Todo other)
        {
            return other != null &&
                   Id == other.Id &&
                   TypeId == other.TypeId &&
                   Type.Equals(other.Type) &&
                   Details == other.Details && 
                   Deadline == other.Deadline && 
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TypeId, Type, Name , Details ,Deadline);
        }


    }
}
