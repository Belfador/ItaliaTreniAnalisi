using System.ComponentModel.DataAnnotations;

namespace API.Models.Domain
{
    public class Sample
    {
        [Key]
        public int Mm { get; set; }
        public required double Parameter1 { get; set; }
        public required double Parameter2 { get; set; }
        public required double Parameter3 { get; set; }
        public required double Parameter4 { get; set; }
    }
}
