using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BuildingMaterialsStoreManagement.Internal.Models
{
    public class Arrival
    {
        public int Id { get; set; }

        public Department Department { get; set; }

        [Required(ErrorMessage = "Дата поступления обязательна для заполнения")]
        public DateTime ArrivalDate { get; set; }
        public string Status { get; set; }
        public List<Item> ArrivalItems { get; set; }
    }
}