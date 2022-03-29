import '../App.css';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import PatientPage from './Patient/PatientPage';
import { BrowserRouter, Routes, Route } from "react-router-dom";

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path='/' element={< LoginPage />} />
          <Route path='/register' element={<RegisterPage />} />
          <Route path='/patient' element={<PatientPage />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
