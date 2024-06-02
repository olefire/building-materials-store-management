using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuildingMaterialsStoreManagement.Internal.Models;

namespace BuildingMaterialsStoreManagement.Internal.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public Department Department { get; set; }

        [Required(ErrorMessage = "Дата продажи обязательна для заполнения")]
        public DateTime SaleDate { get; set; }
        
        public string Status { get; set; }
        public List<Item> Items { get; set; }
    }
    
}