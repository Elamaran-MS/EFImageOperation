using System.ComponentModel.DataAnnotations;

namespace MyFamily.Models
{
    public class MemberViewModel
    {        
        public int MemberId { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Please Enter Age")]
        public int Age { get; set; }
        [Required(ErrorMessage ="Please Select Image")]
        [Display(Name = "Picture")]
        public IFormFile Image { get; set; }
        public string? ImagePath { get; set; }
    }
}
