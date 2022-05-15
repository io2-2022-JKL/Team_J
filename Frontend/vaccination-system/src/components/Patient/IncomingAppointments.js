import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Box, Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { FixedSizeList } from 'react-window';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import { cancelAppointment, getIncomingAppointments } from './PatientApi';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import SummarizeIcon from '@mui/icons-material/Summarize';
import Avatar from '@mui/material/Avatar';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

function renderError(param) {
    switch (param) {
        case '400':
            return 'Złe dane';
        case '401':
            return 'Użytkownik nieuprawniony do uzyskania przyszłych szczepień'
        case '403':
            return 'Użytkownikowi zabroniono uzyskiwania przyszłych szczepień'
        case '404':
            return 'Nie znaleziono przyszłych szczepień'
        case 'ECONNABORTED':
            return 'Przekroczono limit połączenia'
        default:
            return 'Wystąpił błąd!';
    }
}
function renderCancelError(param) {
    switch (param) {
        case '200':
            return 'Anulowano wizytę'
        case '400':
            return 'Złe dane';
        case '401':
            return 'Użytkownik nieuprawniony do anulowania wizyty'
        case '403':
            return 'Użytkownikowi zabroniono anulować wizytę'
        case '404':
            return 'Nie znaleziono przyszłego szczepienia'
        default:
            return 'Wystąpił błąd!';
    }
}

export default function IncomingAppointment() {
    const navigate = useNavigate();
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [errorMessage, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);
    const [cancelError, setErrorCancel] = React.useState('');
    const [errorCancelState, setErrorCancelState] = React.useState(false);

    React.useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            let userID = localStorage.getItem('userID');
            let [patientData, err] = await getIncomingAppointments(userID);
            if (patientData != null) {
                setData(patientData);
                console.log(patientData)
            }
            else {
                setData([]);
                setError(err);
                setErrorState(true);
            }
            setLoading(false);
        }
        fetchData();
        console.log("run useEffect")
    }, [cancelError]);

    function renderRow(props) {
        const { index, style, data } = props;
        const item = data[index];
        return (
            <ListItem style={style} key={index} component="div" disablePadding divider>
                <Grid container direction={"row"} spacing={1}>
                    <Grid item xs={4}>
                        <ListItemText primary={"Wirus: " + item.vaccineVirus} secondary={"Numer dawki: " + item.whichVaccineDose} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Miasto: " + item.vaccinationCenterCity} secondary={"Ulica: " + item.vaccinationCenterStreet} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Lekarz: " + item.doctorFirstName + " " + item.doctorLastName} secondary={"Data szczepienia: " + item.windowEnd} />
                    </Grid>
                </Grid>
                <Button
                    onClick={async () => {
                        let userID = localStorage.getItem('userID');
                        let appointmentId = item.appointmentId;
                        const result = window.confirm("Czy na pewno chcesz anulować wizytę?", confirmOptionsInPolish);
                        if (result) {
                            console.log("You click yes!");
                            let err = await cancelAppointment(userID, appointmentId);
                            setErrorCancel(err);
                            setErrorCancelState(true);
                            return;
                        }
                        else
                            console.log("You click No!");
                    }}
                >
                    Anuluj wizytę
                </Button>
            </ListItem>
        );
    }

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
        setErrorCancelState(false);
    };
    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
                <CssBaseline>
                    <Box
                        sx={{
                            marginTop: 2,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                        }}
                    >
                        <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                            <SummarizeIcon />
                        </Avatar>
                        <Typography component="h1" variant='h5'>
                            Przyszłe szczepienia
                        </Typography>

                        <Box sx={{ marginTop: 2, }} />

                        <FixedSizeList
                            height={Math.min(window.innerHeight - 200, data.length * 100)}
                            width="70%"
                            itemSize={100}
                            itemCount={data.length}
                            overscanCount={5}
                            itemData={data}
                        >
                            {renderRow}
                        </FixedSizeList>

                        <Button
                            type="submit"
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => { navigate("/patient") }}
                        >
                            Powrót
                        </Button>
                        {
                            loading &&
                            (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: blue,
                                        position: 'relative',
                                        alignSelf: 'center',
                                        left: '50%'
                                    }}
                                />
                            )
                        }
                        <Snackbar open={errorState} autoHideDuration={6000} onClose={handleClose}>
                            <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                                {
                                    renderError(errorMessage)
                                }
                            </Alert>
                        </Snackbar>
                        <Snackbar open={errorCancelState} autoHideDuration={6000} onClose={handleClose2}>
                            <Alert onClose={handleClose2} severity={cancelError === '200' ? "success" : "error"} sx={{ width: '100%' }}>
                                {
                                    renderCancelError(cancelError)
                                }
                            </Alert>
                        </Snackbar>
                    </Box>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}

const confirmOptionsInPolish = {
    labels: {
        confirmable: "Tak",
        cancellable: "Nie"
    }
}