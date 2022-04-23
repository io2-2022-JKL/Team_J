import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Box, Grid } from '@mui/material';
import { randomAddress, randomCity, randomCommodity, randomCompanyName, randomDate, randomInt, randomId, randomTraderName } from '@mui/x-data-grid-generator';
import { useNavigate } from 'react-router-dom';
import { FixedSizeList } from 'react-window';
import ListItemButton from '@mui/material/ListItemButton';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import dateFormat from 'dateformat';
import { getCertificates, getFormerAppointments } from './PatientApi';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';

const theme = createTheme();

let id = randomId();
const createRandomRow = () => {
    id = randomId();
    return {
        vaccineName: randomCompanyName(),
        vaccineCompany: randomCompanyName(),
        vaccineVirus: "Koronavirus",
        whichVaccineDose: randomInt(1, 3),
        appointmentId: id,
        windowBegin: dateFormat(randomDate(new Date(50, 1), new Date("1/1/30")), "isoDate").toString(),
        windowEnd: dateFormat(randomDate(new Date(50, 1), new Date("1/1/30")), "isoDate").toString(),
        vaccinationCenterName: randomCommodity(),
        vaccinationCenterCity: randomCity(),
        vaccinationCenterStreet: randomAddress(),
        doctorFirstName: randomTraderName().split(' ')[0],
        doctorLastName: randomTraderName().split(' ')[1],
        visitState: "Finished"
    }
};

function renderRow(props) {
    const { index, style, data } = props;
    const item = data[index];
    return (
        <ListItem style={style} key={index} component="div" disablePadding>
            <Grid container direction={"row"} spacing={1}>
                <Grid item xs={3}>
                    <ListItemText primary={"Wirus: " + item.virus}/>
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Nazwa szczepionki: " + item.vaccineName}/>
                </Grid>
                <Grid item xs={3}>
                    <ListItemText primary={"Firma: " + item.vaccineCompany}/>
                </Grid>
                <Button
                    onClick={()=>{
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
    /*
    const [data, setData] = React.useState(() => [
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
    ]);
    */
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
                <CssBaseline>
                    <Box
                        sx={{
                            marginTop: 8,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                        }}
                    >
                        <Typography component="h1" variant='h5'>
                            Certyfikaty
                        </Typography>
                        <Button
                            type="submit"
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => {
                                setLoading(true);
                                let userID = localStorage.getItem('userID');
                                let patientData = await getCertificates(userID);
                                if(patientData!=null)
                                    setData(patientData);
                                setLoading(false);
                            }}
                        >
                            Pobierz dane
                        </Button>
                        {
                            loading &&
                            (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: blue,
                                        position: 'absolute',
                                        alignSelf: 'center',
                                        bottom: '37%',
                                        left: '50%'
                                    }}
                                />
                            )
                        }
                        <FixedSizeList
                            height={600}
                            width="60%"
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
                    </Box>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}