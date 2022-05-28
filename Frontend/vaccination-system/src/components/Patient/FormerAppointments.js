import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getFormerAppointments } from './PatientApi';
import ItemListPageTemplate from '../ItemsListPageTemplate';
import { handleBack } from './General';

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

    const handleBackClick = () => { handleBack(navigate) }

    return (
        //ItemListPageTemplate("Historia szczpień", data, renderRow, renderError, errorState, error, handleClose, handleBackClick, loading)
        <ItemListPageTemplate
            title = {"Historia szczpień"}
            data = {data}
            renderRow = {renderRow}
            errorState = {errorState}
            error = {error}
            handleBack = {handleBackClick}
            loading = {loading}
            setErrorState = {setErrorState}
        />
    );
}