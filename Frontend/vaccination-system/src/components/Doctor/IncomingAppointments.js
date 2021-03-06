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
import { getAppointmetInfo, getIncomingAppointments } from './DoctorApi';
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

export default function DoctorIncomingAppointment() {
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
            let userID = localStorage.getItem('doctorID');
            let [appointmentsData, err] = await getIncomingAppointments(userID);
            if (appointmentsData != null) {
                setData(appointmentsData);
            }
            else {
                setData([]);
                setError(err);
                setErrorState(true);
            }
            setLoading(false);
        }
        fetchData();
    }, [cancelError]);

    function renderRow(props) {
        const { index, style, data } = props;
        const item = data[index];
        return (
            <ListItem style={style} key={index} component="div" disablePadding divider name="list">
                <Grid container direction={"row"} spacing={1}>
                    <Grid item xs={4}>
                        <ListItemText primary={"Wirus: " + item.vaccineVirus} secondary={"Nazwa szczepionki: " + item.vaccineName} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Imię pacjenta: " + item.patientFirstName} secondary={"Nazwisko pacjenta: " + item.patientLastName} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Początek wizyty: " + item.from} secondary={"Koniec wizyty: " + item.to} />
                    </Grid>
                    <Button
                        name={item.patientFirstName + item.patientLastName}
                        onClick={async () => {
                            const appointmentId = item.appointmentId
                            const [appointmentData, err] = await getAppointmetInfo(localStorage.getItem('doctorID'), item.appointmentId)
                            navigate("/doctor/vaccinate", { state: { appointmentData, appointmentId } })
                        }}
                    >
                        Rozpocznij szczepienie
                    </Button>
                </Grid>
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
                            onClick={() => { navigate("/doctor/redirection", { state: { page: "doctor" } }) }}
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