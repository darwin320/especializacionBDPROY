using System.ComponentModel.DataAnnotations;

namespace IntroASP.Models.ViewModels
{
    public class BeerViewModel
    {
        [Required]
        [Display(Name = "String")]

        public string Name { get; set; }

        [Required]
        [Display(Name = "Marca")]

        public int BrandId { get; set; }
    }
}
