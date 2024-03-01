using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Shared
{ 
    public class BaseEntity
    {
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public bool Deleted { get; set; } = false;
    }
}
