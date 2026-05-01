using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class DoctorSymptom
    {
        //Composite Key
        [Required]
        public int DoctorId { get; set; }
        public Doctor doctor { get; set; }

        [Required]
        public int SymptomId { get; set; }
        public Symptom Symptom { get; set; }
    }
}
