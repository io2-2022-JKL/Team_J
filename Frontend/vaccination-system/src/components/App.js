import '../App.css';
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

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route exact path='/signin' element={< LoginPage />} />
          <Route exact path='/register' element={<RegisterPage />} />
          <Route exact path='/patient' element={<PatientMainPage />} />
          <Route exact path='/admin' element={<AdminMainPage />} />
          <Route exact path='/admin/patients' element={<PatientsPage />} />
          <Route exact path='/admin/doctors' element={<DoctorsPage />} />
          <Route exact path='/admin/vaccinationCenters' element={<VaccinationCentersPage />} />
          <Route exact path='/admin/vaccines' element={<VaccinesPage />} />
          <Route path="" element={<Navigate to="/signin" />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
