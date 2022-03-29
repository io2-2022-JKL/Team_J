namespace VaccinationSystem.DTO.PatientDTOs
{
    public class TimeSlotFilterRequestDTO
    {
        public string vaccineType { get; set; }
        public string city { get; set; }
        public string vaccinationDayFrom { get; set; }
    }
}
