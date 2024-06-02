using System.ComponentModel.DataAnnotations;

namespace BuildingMaterialsStoreManagement.Internal.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Отдел обязателен для заполнения")]
        public Department Department { get; set; }

        [Required(ErrorMessage = "Имя товара обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Длина имени товара не должна превышать 100 символов")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Длина описания товара не должна превышать 500 символов")]
        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public decimal Price { get; set; }
    }
    

}