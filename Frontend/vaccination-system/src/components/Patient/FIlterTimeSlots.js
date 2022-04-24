import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, ListItemButton, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { getFreeTimeSlots } from './PatientApi';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import Divider from '@mui/material/Divider';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import Slide from '@mui/material/Slide';
import Dialog from '@mui/material/Dialog';
import Moment from 'moment';

const theme = createTheme();

export default function FilterTimeSlots() {

    const navigate = useNavigate();
    const [showDaysList, setShowDaysList] = React.useState(false);
    const [city, setCity] = React.useState();
    const [virus, setVirus] = React.useState();
    const [dateFrom, setDateFrom] = React.useState();
    const [dateTo, setDateTo] = React.useState();

    const [daysInCenters, setDaysInCenters] = React.useState();
    const [openDialog, setOpenDialog] = React.useState(false);

    const [dayTimeSlots, setDayTimeSlots] = React.useState();

    const handleDayChoice = (dayInCenter) => {
        setDayTimeSlots(dayInCenter[Object.keys(dayInCenter)[0]])
        setOpenDialog(true);
    };

    const handleClose = () => {
        setOpenDialog(false);
    };


    const handleSubmit = (event) => {
        const data = new FormData(event.currentTarget);

        setCity(data.get('cityFilter'))
        setVirus(data.get('virusFilter'))
        setDateFrom(data.get('dateFrom'))
        setDateTo(data.get('dateTo'))
    };

    function getDayFromDate(date) {
        //console.log(date.substring(0, 10))
        return date.substring(0, 10)
    }

    function divideCenterIntoDays(center) {
        //console.log('divideCenterIntoDays')
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

        //console.log(dateFrom)
        Moment(dateFrom).format('YYYY-MM-DD')
        console.log(Moment(dateFrom).format('YYYY-MM-DD'))

        const centers = data.reduce((centers, item) => {
            const group = (centers[item.vaccinationCenterName] || []);
            group.push(item);
            centers[item.vaccinationCenterName] = group;
            return centers;
        }, {});

        //console.log(data);
        //console.log("centers", centers);

        let centersDays = []

        for (let c in centers) {
            //console.log("centrum", centers[c])
            centersDays.push(divideCenterIntoDays(centers[c]))
        }

        /*for (let i = 0; i < Object.keys(centers).length; i++) {
            console.log(Object.keys(centers))
            result.push(divideCenterIntoDays(Object.keys(centers)[i]))
        }*/

        setDaysInCenters(centersDays)

        //console.log("centers days", centersDays)

        /*for (let r in centersDays) {
            console.log(r)
            console.log("dzień w centrum", centersDays[r])
            console.log(Object.keys(centersDays[r])[0])
            console.log(centersDays[r][Object.keys(centersDays[r])[0]])
            console.log("key", Object.keys(r))
            console.log(Object.keys(r))
        }*/

        setShowDaysList(true);

    }

    function getWeekDayNumberForDate(date) {
        let day = getDayFromDate(date)
        const d = new Date(day.substring(6, 10), day.substring(3, 5) - 1, day.substring(0, 2));
        //console.log(day.substring(6, 10), day.substring(3, 5) - 1, day.substring(0, 2))
        //console.log(d)
        return (d.getDay() - 1) % 7;
    }

    function getWeekDayName(num) {
        const weekdays = ["Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela"];
        return weekdays[num];
    }

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth='lg'>
                <CssBaseline>
                    <Container
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

                                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                                        <DatePicker
                                            id="dateFrom"
                                            name="dateFrom"

                                            label="Data Od"
                                            value={dateFrom}
                                            onChange={(newDate) => {
                                                setDateFrom(newDate);
                                            }}
                                            renderInput={(params) => <TextField {...params} />}
                                        />
                                    </LocalizationProvider>
                                </Grid>
                                <Grid item xs={3}>
                                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                                        <DatePicker
                                            id="dateTo"
                                            name="dateTo"

                                            label="Data Do"
                                            value={dateTo}
                                            onChange={(newDate) => {
                                                setDateTo(newDate);
                                            }}
                                            renderInput={(params) => <TextField {...params} />}
                                        />
                                    </LocalizationProvider>
                                </Grid>
                            </Grid>

                        </Box>
                        {showDaysList &&
                            <Box>
                                <List
                                    sx={{ width: '100%', bgcolor: 'background.paper' }}
                                    component="nav"
                                    aria-labelledby="nested-list-subheader"
                                >
                                    {daysInCenters &&
                                        daysInCenters.map(dayInCenter =>
                                        (<ListItemButton
                                            onClick={() => { handleDayChoice(dayInCenter) }}
                                        >
                                            <ListItemText
                                                primary={dayInCenter[Object.keys(dayInCenter)[0]][0].vaccinationCenterName + ", " +
                                                    dayInCenter[Object.keys(dayInCenter)[0]][0].vaccinationCenterStreet + ", " +
                                                    getWeekDayName(getWeekDayNumberForDate(dayInCenter[Object.keys(dayInCenter)[0]][0].from)) + ", " +
                                                    getDayFromDate(dayInCenter[Object.keys(dayInCenter)[0]][0].from)
                                                }
                                                secondary={"Godziny otwarcia: " +
                                                    dayInCenter[Object.keys(dayInCenter)[0]][0].openingHours
                                                    [getWeekDayNumberForDate(dayInCenter[Object.keys(dayInCenter)[0]][0].from)].from + " - " +
                                                    dayInCenter[Object.keys(dayInCenter)[0]][0].openingHours
                                                    [getWeekDayNumberForDate(dayInCenter[Object.keys(dayInCenter)[0]][0].from)].to
                                                }
                                            />
                                        </ListItemButton>))
                                    }
                                </List>
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
                        {showDaysList && <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { setShowDaysList(false) }}
                        >
                            Wyczyść wyszukiwanie
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
                    </Container>

                    <Dialog
                        fullScreen
                        open={openDialog}
                        onClose={handleClose}
                        TransitionComponent={Transition}
                    >
                        <AppBar sx={{ position: 'relative' }}>
                            <Toolbar>
                                <IconButton
                                    edge="start"
                                    color="inherit"
                                    onClick={handleClose}
                                    aria-label="close"
                                >
                                    <CloseIcon />
                                </IconButton>
                                <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                                    Wybierz termin szczepienia
                                </Typography>
                                <Button autoFocus color="inherit" onClick={handleClose}>
                                    Wróć
                                </Button>
                            </Toolbar>
                        </AppBar>
                        <List>
                            {dayTimeSlots &&
                                dayTimeSlots.map(dayTimeSlot =>
                                    <ListItem button>
                                        <ListItemText primary={dayTimeSlot.doctorFirstName} secondary="" />
                                    </ListItem>
                                )}

                            <ListItem button>
                                <ListItemText primary=" " secondary="" />
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemText
                                    primary=""
                                    secondary=""
                                />
                            </ListItem>
                        </List>
                    </Dialog>
                </CssBaseline>
            </Container >
        </ThemeProvider >

    );
}

const Transition = React.forwardRef(function Transition(props, ref) {
    return <Slide direction="up" ref={ref} {...props} />;
});