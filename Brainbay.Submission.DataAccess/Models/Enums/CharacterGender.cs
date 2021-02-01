using System.ComponentModel.DataAnnotations;

namespace Brainbay.Submission.DataAccess.Models.Enums
{
    public enum CharacterGender
    {
        [Display(Name = "Female")]
        Female,
        [Display(Name = "Male")]
        Male,
        [Display(Name = "Genderless")]
        Genderless,
        [Display(Name = "Unknown")]
        Unknown
    }
}
