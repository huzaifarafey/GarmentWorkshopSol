using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models
{
    public class ExpenseCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }   // Rent, Electricity, Tea, Bhada, Transport, Other

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}