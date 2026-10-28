using System.ComponentModel.DataAnnotations;

namespace StudentBudgetApp.Models
{
    public class BudgetEntry
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Please enter a valid amount between 0.01 and 100,000.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a type.")]
        public string Type { get; set; } = "Expense"; // "Income" or "Expense"

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty; // e.g., Food, Rent, Tuition

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}