using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    /// <summary>
    /// Подразделение
    /// </summary>
    public class PnrDepartment
    {
        public Guid? ID { get; set; }
        public Guid? CFOID { get; set; }
        public string CFOName { get; set; }
        public string Name { get; set; }
        public string Idx { get; set; }
    }
}
