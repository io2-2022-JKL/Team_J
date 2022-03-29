using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccinationSystem.Config;
using VaccinationSystem.Models;

namespace VaccinationSystem.Repository
{
    public class TestRepository
    {
        private readonly VaccinationSystemDbContext _db;

        public TestRepository(VaccinationSystemDbContext db)
        {
            _db = db;
        }

        public List<Certificate> Test()
        {
            return _db.Certificate.ToList();
        }
    }
}
