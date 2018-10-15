using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
    public class TaskWithSubtreePath
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        public string Path { get; set; }

        [NotMapped]
        private List<int> _idOfParents;

        [NotMapped]
        public List<int> IdOfParents
        {
            get
            {
                if (Path == null)
                {
                    return null;
                }
                if(_idOfParents == null)
                {
                    _idOfParents = new List<int>();

                    var stringids = Path.Split(',').ToList();

                    foreach(var id in stringids)
                    {
                        _idOfParents.Add(int.Parse(id));
                    }
                }

                return _idOfParents;
            }
        }

        
        public  int? ParentTaskId { get; set; }
       


    }
}
