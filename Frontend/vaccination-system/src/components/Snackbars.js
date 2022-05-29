import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import LoginHelpers from '../tools/LoginHelpers';

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export function ErrorSnackbar(props) {

    const navigate = useNavigate();

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        if (props.errorState) {
            if (props.error === '401' || props.error === '403') {
                LoginHelpers.logOut();
                navigate('/signin');
            }
        }
        props.setErrorState(false);
    };

    function renderError(param) {
        switch (param) {
            case '400':
                return '400: złe dane';
            case '401':
                return '401: Użytkownik nieuprawniony do przeglądania danych'
            case '403':
                return '403: Użytkownikowi zabroniono dostępu do danych'
            case '404':
                return '404: Nie znaleziono danych'
            case 'ECONNABORTED':
                return 'Przekroczono limit połączenia'
            default:
                return 'Wystąpił błąd!';
        }
    }

    return (
        <Snackbar open={props.errorState} autoHideDuration={2000} onClose={handleClose}>
            <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                {renderError(props.error)}
            </Alert>
        </Snackbar>
    )
}

export function SuccessSnackbar(props) {

    const handleClose2 = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        props.setSuccess(false);
    };

    return (
        <Snackbar open={props.success} autoHideDuration={2000} onClose={handleClose2}>
            <Alert onClose={handleClose2} severity="success" sx={{ width: '100%' }}>
                Pomyślnie wykonano akcję
            </Alert>
        </Snackbar>
    )
}