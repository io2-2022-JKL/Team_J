import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import PatientMainPage from './Patient/PatientMainPage';
import { BrowserRouter, Routes, Route, Navigate, Router, Switch } from "react-router-dom";
import AdminMainPage from './Admin/AdminMainPage';
import PatientsPage from './Admin/PatientsPage';
import DoctorsPage from './Admin/DoctorsPage';
import VaccinationCentersPage from './Admin/VaccinationCentersPage';
import VaccinesPage from './Admin/VaccinesPage';
import PrivateRoute from './PrivateRoute';
import DoctorMainPage from './Doctor/DoctorMainPage';
import EditDoctor from './Admin/EditDoctor';
import FilterTimeSlots from './Patient/FIlterTimeSlots';
import FormerAppointment from './Patient/FormerAppointments';
import IncomingAppointment from './Patient/IncomingAppointments';
import Certificate from './Patient/Certificates';
import AddOrEditVaccine from './Admin/AddOrEditVaccine';
import EditPatient from './Admin/EditPatient';
import AddOrEditVaccinationCenter from './Admin/AddOrEditVaccinationCenter';
import DoctorIncomingAppointments from './Doctor/IncomingAppointments';
import Redirection from './Doctor/Redirection';


function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route exact path='/signin' element={< LoginPage />} />
          <Route exact path='/register' element={<RegisterPage />} />
          <Route exact path='/patient' element={<PatientMainPage />} />
          <Route exact path='/patient/certificates' element={<Certificate />} />
          <Route exact path='/patient/appointments/formerAppointments' element={<FormerAppointment />} />
          <Route exact path='/patient/appointments/incomingAppointments' element={<IncomingAppointment />} />
          <Route exact path='/doctor' element={<DoctorMainPage />} />
          <Route exact path='/doctor/redirection' element={<Redirection />} />
          <Route exact path='/doctor/incomingAppointments' element={<DoctorIncomingAppointments />} />
          <Route exact path='/admin' element={<AdminMainPage />} />
          <Route exact path='/admin/patients' element={<PatientsPage />} />
          <Route exact path='/admin/patients/editPatient' element={<EditPatient />} />
          <Route exact path='/admin/doctors' element={<DoctorsPage />} />
          <Route exact path='/admin/doctors/editDoctor' element={< EditDoctor />} />
          <Route exact path='/admin/vaccinationCenters' element={<VaccinationCentersPage />} />
          <Route exact path='/admin/vaccinationCenters/addVaccinationCenter' element={<AddOrEditVaccinationCenter />} />
          <Route exact path='/admin/vaccinationCenters/editVaccinationCenter' element={<AddOrEditVaccinationCenter />} />
          <Route exact path='/admin/vaccines' element={<VaccinesPage />} />
          <Route exact path='/admin/vaccines/addVaccine' element={<AddOrEditVaccine />} />
          <Route exact path='/admin/vaccines/editVaccine' element={<AddOrEditVaccine />} />
          <Route exact path='/patient/timeSlots' element={<FilterTimeSlots />} />
          <Route path="" element={<Navigate to="/signin" />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
