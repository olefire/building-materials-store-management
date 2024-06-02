using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingMaterialsStoreManagement.Internal.Models
{
    public class Department
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Отдел обязателен для заполнения")]
        [StringLength(100, ErrorMessage = "Длина отдела не должна превышать 100 символов")]
        public string Name { get; set; }
        
        public List<Item> Items { get; set; }
    }

}