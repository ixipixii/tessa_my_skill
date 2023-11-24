using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.Models
{
    public class PnrFdCardState
    {
        public PnrFdCardState(Guid stateID, string stateName)
        {
            ID = stateID;
            Name = stateName;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}
