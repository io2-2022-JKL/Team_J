namespace VaccinationSystem.DTO.AdminDTOs
{
    public class EditVaccineRequestDTO // dopisać w dokumentacji
    {
        public string vaccineId { get; set; }
        public string company { get; set; }
        public string name { get; set; }
        public int numberOfDoses { get; set; }
        public int minDaysBetweenDoses { get; set; }
        public int maxDaysBetweenDoses { get; set; }
        public string virus { get; set; }
        public int minPatientAge { get; set; }
        public int maxPatientAge { get; set; }
    }
}
