import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getFormerAppointments } from './PatientApi';
import ItemListPageTemplate from '../ItemsListPageTemplate';

function renderRow(props) {
    const { index, style, data } = props;
    const item = data[index];
    return (
        <ListItem style={style} key={index} component="div" disablePadding divider>
            <Grid container direction={"row"} spacing={1}>
                <Grid item xs={3}>
                    <ListItemText primary={"Wirus: " + item.vaccineVirus} secondary={"Numer dawki: " + item.whichVaccineDose} />
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Miasto: " + item.vaccinationCenterCity} secondary={"Ulica: " + item.vaccinationCenterStreet} />
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Lekarz: " + item.doctorFirstName + " " + item.doctorLastName} secondary={"Data szczepienia: " + item.windowEnd} />
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Status: " + item.visitState} />
                </Grid>
            </Grid>
        </ListItem>
    );
}

function renderError(param) {
    switch (param) {
        case '400':
            return 'Złe dane';
        case '401':
            return 'Użytkownik nieuprawniony do uzyskania historii szczepień'
        case '403':
            return 'Użytkownikowi zabroniono uzyskiwania historii szczepień'
        case '404':
            return 'Nie znaleziono historii szczepień'
        case 'ECONNABORTED':
            return 'Przekroczono limit połączenia'
        default:
            return 'Wystąpił błąd!';
    }
}

export default function FormerAppointments() {
    const navigate = useNavigate();
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);

    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
            let userID = localStorage.getItem('userID');
            let [patientData, err] = await getFormerAppointments(userID);
            if (patientData != null) {
                setData(patientData);
            }
            else {
                setError(err);
                setErrorState(true);
            }
            setLoading(false);
        }
        fetchData();
    }, []);

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setErrorState(false);
    };

    const handleBack = () => { navigate("/patient") }

    return (
        ItemListPageTemplate("Historia szczpień", data, renderRow, renderError, errorState, error, handleClose, handleBack, loading)
    );
}