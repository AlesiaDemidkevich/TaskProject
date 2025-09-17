using C_Part1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskServiceApi.Storage
{
    public interface ITaskStorage
    {
        void Save(List<TaskItem> tasks);
        List<TaskItem>? Load();
    }
}
