export function handleBack(navigate) {
    if (localStorage.getItem('isDoctor') == 'true') navigate('/doctor/redirection')
    else {
        navigate("/patient")
    }
}