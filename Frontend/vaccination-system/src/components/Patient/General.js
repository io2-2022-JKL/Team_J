export function handleBack(navigate) {
    if (localStorage.getItem('isDoctor') == true) navigate('doctor/redirection')
    navigate("/patient")
}