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
import { handleBack } from './General';
import { ErrorSnackbar, SuccessSnackbar } from '../Snackbars';

const theme = createTheme();

export default function IncomingAppointment() {
    const navigate = useNavigate();
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [errorMessage, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);
    const [cancelError, setErrorCancel] = React.useState('');
    const [errorCancelState, setErrorCancelState] = React.useState(false);
    const [success, setSuccess] = React.useState(false);

    React.useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            let userID = localStorage.getItem('patientID');
            let [patientData, err] = await getIncomingAppointments(userID);
            if (patientData != null) {
                setData(patientData);
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
                    name="cancelButton"
                    onClick={async () => {
                        let patientID = localStorage.getItem('patientID');
                        let appointmentId = item.appointmentId;
                        const result = window.confirm("Czy na pewno chcesz anulować wizytę?");
                        if (result) {
                            let err = await cancelAppointment(patientID, appointmentId);
                            if (err !== '200') {
                                setErrorCancel(err);
                                setErrorCancelState(true);
                            }
                            else
                                setSuccess(true);
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
                            onClick={() => { handleBack(navigate) }}
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
                        <ErrorSnackbar
                            error={errorMessage}
                            errorState={errorState}
                            setErrorState={setErrorState}
                        />
                        <ErrorSnackbar
                            error={cancelError}
                            errorState={errorCancelState}
                            setErrorState={setErrorCancelState}
                        />
                        <SuccessSnackbar
                            success={success}
                            setSuccess={setSuccess}
                        />
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