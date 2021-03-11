using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Domain.Model
{
    public class InsertResult
    {
        public bool IsSuccess { get; set; }
        public object NewId { get; set; }
    }
}
