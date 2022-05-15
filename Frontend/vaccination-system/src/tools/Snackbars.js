import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import * as React from 'react';

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function Snackbars(error, errorState, setErrorState, success, setSuccess) {

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setErrorState(false);
    };

    const handleClose2 = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setSuccess(false);
    };

    function renderError(param) {
        switch (param) {
            case '400':
                return 'Złe dane. Czyżbyś próbował dodać nieistniejącego wirusa?';
            case '401':
                return 'Użytkownik nieuprawniony do dodania szczepionki'
            case '403':
                return 'Użytkownikowi zabroniono dodawania szczepionki'
            case '404':
                return 'Nie znaleziono szczepionki do dodania'
            case 'ECONNABORTED':
                return 'Przekroczono limit połączenia'
            default:
                return 'Wystąpił błąd!';
        }
    }

    return (
        <Box>
            <Snackbar open={errorState} autoHideDuration={6000} onClose={handleClose}>
                <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                    {renderError(error)}
                </Alert>
            </Snackbar>
            <Snackbar open={success} autoHideDuration={6000} onClose={handleClose2}>
                <Alert onClose={handleClose2} severity="success" sx={{ width: '100%' }}>
                    Pomyślnie dodano szczepionkę
                </Alert>
            </Snackbar>
        </Box>
    )
}