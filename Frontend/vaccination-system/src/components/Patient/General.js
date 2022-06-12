export function handleBack(navigate) {
    if (localStorage.getItem('isDoctor') == 'true') navigate('/doctor/redirection', { state: { page: "patient" } })
    else {
        navigate("/patient")
    }
}