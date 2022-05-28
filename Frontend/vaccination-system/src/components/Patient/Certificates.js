import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import { getCertificates } from './PatientApi';
import ItemListPageTemplate from '../ItemsListPageTemplate';
import { handleBack } from './General';


function renderRow(props) {
    const { index, style, data } = props;
    const item = data[index];
    return (
        <ListItem style={style} key={index} component="div" disablePadding divider>
            <Grid container direction={"row"} spacing={1}>
                <Grid item xs={3}>
                    <ListItemText primary={"Wirus: " + item.virus} />
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Nazwa szczepionki: " + item.vaccineName} />
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Firma: " + item.vaccineCompany} />
                </Grid>
                <Button
                    onClick={() => {
                        var win = window.open(item.url, '_blank');
                        win.focus();
                    }}
                >
                    Pobierz certyfikat
                </Button>
            </Grid>
        </ListItem>
    );
}

export default function Certificate() {
    const navigate = useNavigate();

    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);

    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
            let userID = localStorage.getItem('userID');
            let [patientData, err] = await getCertificates(userID);
            if (patientData != null)
                setData(patientData);
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
        //ItemListPageTemplate("Twoje certyfikaty", data, renderRow, renderError, errorState, error, handleClose, handleBackClick, loading, setErrorState)
        <ItemListPageTemplate
            title = {"Twoje certyfikaty"}
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