using System.ComponentModel.DataAnnotations;

namespace BuildingMaterialsStoreManagement.Internal.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int RefId { get; set; }

        [Required(ErrorMessage = "Продукт обязателен для заполнения")]
        public Product Product { get; set; }

        [Required(ErrorMessage = "Количество обязательно для заполнения")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int Quantity { get; set; }
    }
}