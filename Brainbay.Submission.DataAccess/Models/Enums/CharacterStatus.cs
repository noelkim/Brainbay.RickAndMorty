using System.ComponentModel.DataAnnotations;

namespace Brainbay.Submission.DataAccess.Models.Enums
{
    public enum CharacterStatus
    {
        [Display(Name ="Alive")]
        Alive,
        [Display(Name = "Dead")]
        Dead,
        [Display(Name = "Unknown")]
        Unknown
    }
}
