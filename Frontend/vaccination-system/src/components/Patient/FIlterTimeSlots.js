import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import HomeIcon from '@mui/icons-material/Home';
import { getFreeTimeSlots } from './PatientApi';

const theme = createTheme();

export default function FilterTimeSlots() {

    const navigate = useNavigate();
    const [showDaysList, setShowDaysList] = React.useState(false);
    const [city, setCity] = React.useState();
    const [virus, setVirus] = React.useState();
    const [dateFrom, setDateFrom] = React.useState();
    const [dateTo, setDateTo] = React.useState();

    const [daysInCenters, setDaysInCenters] = React.useState();

    const handleSubmit = (event) => {
        const data = new FormData(event.currentTarget);

        setCity(data.get('cityFilter'))
        setVirus(data.get('virusFilter'))
        setDateFrom(data.get('dateFrom'))
        setDateTo(data.get('dateTo'))
    };

    function getDayFromDate(date) {
        console.log(date.substring(0, 9))
        return date.substring(0, 9)
    }

    function divideCenterIntoDays(center) {
        console.log('divideCenterIntoDays')
        const days = center.reduce((days, item) => {
            const day = getDayFromDate(item.from)
            const group = (days[day] || []);
            group.push(item);
            days[day] = group;
            return days;
        }, {});
        return days;
    }

    const submitSearch = async () => {
        const data = await getFreeTimeSlots(city, dateFrom, dateTo, virus)

        //data.sort((a, b) => (a.vaccinationCenterName > b.vaccinationCenterName) ? 1 : -1)

        const centers = data.reduce((centers, item) => {
            const group = (centers[item.vaccinationCenterName] || []);
            group.push(item);
            centers[item.vaccinationCenterName] = group;
            return centers;
        }, {});

        //console.log(data);
        //console.log(centers);

        let result = []

        for (let c in centers) {
            console.log(centers[c])
            result.push(divideCenterIntoDays(centers[c]))
        }

        /*for (let i = 0; i < Object.keys(centers).length; i++) {
            console.log(Object.keys(centers))
            result.push(divideCenterIntoDays(Object.keys(centers)[i]))
        }*/

        setDaysInCenters(result)

        console.log(
            "days", daysInCenters
        )


        setShowDaysList(true);

    }

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth='lg'>
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
                            <PeopleOutlineIcon />
                        </Avatar>
                        <Typography component="h1" variant='h4'>
                            Terminy szczepień
                        </Typography>
                        <Box
                            component='form'
                            noValidate
                            onChange={handleSubmit}
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                            }}
                        >
                            <Grid container direction={"row"} spacing={1} >
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="idVirus"
                                        label="Wirus"
                                        name="virusFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="city"
                                        label="Miasto"
                                        name="cityFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="dateFrom"
                                        label="Data Od"
                                        name="dateFrom"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="dateTo"
                                        label="Data Do"
                                        name="dateTo"
                                    />
                                </Grid>
                            </Grid>

                        </Box>
                        {showDaysList &&
                            <Box>


                                {daysInCenters.map(dayInCenter => (<div> {dayInCenter.type} </div>))
                                }
                            </Box>
                        }
                        {!showDaysList && <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { submitSearch() }}
                        >
                            Wybierz
                        </Button>}
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { navigate("/patient") }}
                        >
                            Powrót
                        </Button>
                    </Box>
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}