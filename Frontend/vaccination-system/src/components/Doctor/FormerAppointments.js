import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { certify, getFormerAppointments } from './DoctorApi';
import ItemListPageTemplate from '../ItemsListPageTemplate';
import { handleBack } from "../Patient/General"
import Button from '@mui/material/Button';

export default function DoctorFormerAppointments() {
    const navigate = useNavigate();
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);
    const [errorCertify, setErrorCertify] = React.useState('');
    const [errorCertifyState, setErrorCertifyState] = React.useState(false);
    const [success, setSuccess] = React.useState(false);

    let userID = localStorage.getItem('doctorID');

    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
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
    }, [errorCertify]);

    function stateToPolish(state) {
        switch (state) {
            case "Finished":
                return "Zakończono"
            case "Cancelled":
                return "Odwołano"
            default:
                return state
        }
    }

    const handleBackClick = () => { { navigate("/doctor/redirection", { state: { page: "doctor" } }) } }
    function renderRow(props) {
        const { index, style, data } = props;
        const item = data[index];

        return (
            <ListItem style={style} key={index} component="div" disablePadding divider>
                <Grid container direction={"row"} spacing={1}>
                    <Grid item xs={4}>
                        <ListItemText primary={"Wirus: " + item.vaccineVirus + ", Szczepionka: " + item.vaccineName} secondary={"Numer dawki: " + item.whichVaccineDose} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Pacjent: " + item.patientFirstName + " " + item.patientLastName} secondary={"Data szczepienia: " + item.to} />
                    </Grid>
                    <Grid item xs={4}>
                        <ListItemText primary={"Status: " + stateToPolish(item.state)} secondary={"Wystawiono certyfikat: " + (item.certifyState === "Certified" ? "Tak" : "Nie")} />
                    </Grid>
                </Grid>
                <Button
                    disabled={item.certifyState === "LastNotCertified" ? false : true}
                    onClick={async () => {
                        let appointmentId = item.appointmentId;
                        const result = window.confirm("Czy na pewno chcesz wystawić certyfikat?", confirmOptionsInPolish);
                        if (result) {
                            console.log("You click yes!");
                            let err = await certify(userID, appointmentId);
                            setErrorCertify(err);
                            if (err !== '200') {
                                setErrorCertifyState(true);
                            }
                            else
                                setSuccess(true);
                            return;
                        }
                        else
                            console.log("You click No!");
                    }}
                >
                    Wystaw certyfikat
                </Button>
            </ListItem>

        );
    }
    return (
        <ItemListPageTemplate
            title={"Historia szczpień"}
            data={data}
            renderRow={renderRow}
            errorState={errorState}
            error={error}
            handleBack={handleBackClick}
            loading={loading}
            setErrorState={setErrorState}
            success={success}
            setSuccess={setSuccess}
            error2={errorCertify}
            errorState2={errorCertifyState}
            setErrorState2={setErrorCertifyState}
        />
    );
}

const confirmOptionsInPolish = {
    labels: {
        confirmable: "Tak",
        cancellable: "Nie"
    }
}