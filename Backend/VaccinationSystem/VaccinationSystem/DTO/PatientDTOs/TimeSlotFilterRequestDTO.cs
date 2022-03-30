namespace VaccinationSystem.DTO.PatientDTOs
{
    public class TimeSlotFilterRequestDTO
    {
        public string city { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string virus { get; set; }
    }
}
