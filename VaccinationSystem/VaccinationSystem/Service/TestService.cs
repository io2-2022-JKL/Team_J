using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.Models;
using VaccinationSystem.Repository;

namespace VaccinationSystem.Service
{
    public class TestService
    {
        private readonly TestRepository _repository;

        public TestService(TestRepository repository)
        {
            _repository = repository;
        }

        public List<CertificatesResponseDTO> Test()
        {
            List<Certificate> certificates = _repository.Test();
            List<CertificatesResponseDTO> result = new List<CertificatesResponseDTO>();

            foreach(Certificate certificate in certificates)
            {
                CertificatesResponseDTO temp = new CertificatesResponseDTO();
                temp.certificateId = Convert.ToString(certificate.Id);
                temp.vaccineType = certificate.Url;
                temp.virusType = Virus.Koronawirus.ToString();

                result.Add(temp);
            }
            return result;
        }
    }
}
