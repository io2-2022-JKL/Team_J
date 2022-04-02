import '../App.css';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import PatientMainPage from './Patient/PatientMainPage';
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AdminMainPage from './Admin/AdminMainPage';
import PatientsPage from './Admin/PatientsPage';
import DoctorsPage from './Admin/DoctorsPage';
import VaccinationCentersPage from './Admin/VaccinationCentersPage';
import VaccinesPage from './Admin/VaccinesPage';

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path='/signin' element={< LoginPage />} />
          <Route path='/register' element={<RegisterPage />} />
          <Route path='/patient' element={<PatientMainPage />} />
          <Route exact path='/admin' element={<AdminMainPage />} />
          <Route path='/admin/patients' element={<PatientsPage />} />
          <Route path='/admin/doctors' element={<DoctorsPage />} />
          <Route path='/admin/vaccinationCenters' element={<VaccinationCentersPage />} />
          <Route path='/admin/vaccines' element={<VaccinesPage />} />
          <Route path="" element={<Navigate to="/signin" />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
