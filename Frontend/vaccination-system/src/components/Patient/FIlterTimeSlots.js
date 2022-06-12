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
import { bookTimeSlot, getFreeTimeSlots } from './PatientApi';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import Divider from '@mui/material/Divider';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import Slide from '@mui/material/Slide';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Snackbar from '@mui/material/Snackbar';
import { handleBack } from './General';
import DropDownSelect from '../DropDownSelect';
import { virusesEmptyPossible } from '../../api/Viruses';
import { citiesEmptyPossible } from '../../api/Cities';
import WeekDays from '../../tools/WeekDays';

const theme = createTheme();

export default function FilterTimeSlots() {

    const navigate = useNavigate();
    const [showDaysList, setShowDaysList] = React.useState(false);
    const [city, setCity] = React.useState('');
    const [virus, setVirus] = React.useState('');
    const [dateFrom, setDateFrom] = React.useState();
    const [dateTo, setDateTo] = React.useState();
    const [daysInCenters, setDaysInCenters] = React.useState();
    const [openTimeSlotsDialog, setOpenTimeSlotsDialog] = React.useState(false);
    const [openVaccinesDialog, setOpenVaccinesDialog] = React.useState(false);
    const [dayTimeSlots, setDayTimeSlots] = React.useState();
    const [possibleVaccines, setPossibleVaccines] = React.useState([]);
    const [vaccine, setVaccine] = React.useState();
    const [timeSlot, setTimeSlot] = React.useState()
    const [openConfirmDialog, setOpenConfrimDialog] = React.useState(false)
    const [openSnackBar, setOpenSnackBar] = React.useState()
    const [snackBarMessage, setSnackBarMessage] = React.useState("Pomyślnie zapisano na szczepienie")

    function getFilterErrorMessage(errCode) {
        switch (errCode) {
            case '400':
                return 'Nieprawidłowe dane wyszukiwania';
            case '401':
                return 'Użytkownik nieuprawniony do wyszukiwania'
            case '403':
                return 'Użytkownikowi zabroniono wyszukiwania'
            case '404':
                return 'Nie znaleziono dostępnych terminów dla tych danych'
            default:
                return 'Wystąpił błąd!';
        }
    }
    function getBookErrorMessage(errCode) {
        switch (errCode) {
            case '200':
                return 'Udało się zapisać na szczepienie'
            case '400':
                return 'Nieprawidłowe dane';
            case '401':
                return 'Użytkownik nieuprawniony do zapisu na to szczepienie'
            case '403':
                return 'Użytkownikowi zabroniono zapisać się na to szczepienie'
            case '404':
                return 'Nie udało się zapisać na szczepienie'
            default:
                return 'Wystąpił błąd!';
        }
    }

    function handleDayChoice(dayInCenter) {
        setDayTimeSlots(dayInCenter[Object.keys(dayInCenter)[0]].sort(function (a, b) { // non-anonymous as you ordered...
            return b.from < a.from ? 1 : -1
        }))
        setPossibleVaccines(dayInCenter[Object.keys(dayInCenter)[0]][0].availableVaccines);
        setVaccine(dayInCenter[Object.keys(dayInCenter)[0]][0].availableVaccines[0].vaccineId)
        setOpenTimeSlotsDialog(true);
    };

    const handleVaccinesClose = () => {
        setOpenVaccinesDialog(false)
    }

    const handleVaccineChoice = async () => {
        setOpenConfrimDialog(true)
        setOpenVaccinesDialog(false)
    }

    const handleTimeSLotsClose = () => {
        setOpenTimeSlotsDialog(false);
    };

    const handleTimeSlotChoice = (dayTimeSlot) => {
        setTimeSlot(dayTimeSlot)
        setOpenVaccinesDialog(true)
    }

    const handleConfrimClose = () => {
        setOpenConfrimDialog(false)
    }

    const bookTimeSlotTrue = async () => {
        const errCode = await bookTimeSlot(timeSlot.timeSlotId, vaccine)

        setOpenConfrimDialog(false)

        if (errCode != "200") {
            setSnackBarMessage(getBookErrorMessage(errCode))
            setOpenSnackBar(true)
            return
        }

        setSnackBarMessage("Pomyślnie zapisano na szczepienie")
        setOpenSnackBar(true)
    }

    function getDayFromDate(date) {
        return date.substring(0, 10)
    }

    function divideCenterIntoDays(center) {
        const days = center.reduce((days, item) => {
            const day = getDayFromDate(item.from)
            const group = (days[day] || []);
            group.push(item);
            days[day] = group;
            return days;
        }, {});
        return days;
    }

    function getHoursMinsFromDate(date) {
        return date.substring(11, 16)
    }

    const submitSearch = async () => {
        const [data, code] = await getFreeTimeSlots(city, dateFrom, dateTo, virus)

        if (code != "200") {
            setSnackBarMessage(getFilterErrorMessage(code))
            setOpenSnackBar(true)
            return
        }

        const centers = data.reduce((centers, item) => {
            const group = (centers[item.vaccinationCenterName] || []);
            group.push(item);
            centers[item.vaccinationCenterName] = group;
            return centers;
        }, {});

        let centersDays = []

        for (let c in centers) {
            let days = divideCenterIntoDays(centers[c]);
            for (let d in days) {
                centersDays.push({ d: days[d] })
            }
        }

        setDaysInCenters(centersDays)
        setShowDaysList(true);
    }

    function getWeekDayNumberForDate(date) {
        let day = getDayFromDate(date)
        const d = new Date(day.substring(6, 10), day.substring(3, 5) - 1, day.substring(0, 2));
        return (d.getDay() - 1) % 7;
    }

    function getWeekDayName(num) {
        return WeekDays[num];
    }

    const handleChange = (event) => {
        setVaccine(event.target.value);
    };

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
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                            }}
                        >
                            <Grid container direction={"row"} spacing={1} >
                                <Grid item xs={3}>
                                    {DropDownSelect("idVirus", "Wirus", virusesEmptyPossible, virus, setVirus)}
                                </Grid>
                                <Grid item xs={3}>

                                    {DropDownSelect("cityFilter", "Miasto", citiesEmptyPossible, city, setCity)}
                                </Grid>
                                <Grid item xs={3}>

                                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                                        <DatePicker
                                            label="Data od"
                                            views={['year', 'month', 'day']}
                                            inputFormat="dd-MM-yyyy"
                                            mask="__-__-____"
                                            value={dateFrom}
                                            name="dateFromPicker"
                                            onChange={(newDate) => {
                                                setDateFrom(newDate);
                                            }}
                                            renderInput={(params) => <TextField
                                                {...params}
                                                fullWidth
                                                id='dateFrom'
                                                name='dateFrom'
                                            />}
                                        />
                                    </LocalizationProvider>
                                </Grid>
                                <Grid item xs={3}>
                                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                                        <DatePicker
                                            label="Data do"
                                            views={['year', 'month', 'day']}
                                            inputFormat="dd-MM-yyyy"
                                            mask="__-__-____"
                                            value={dateTo}
                                            onChange={(newDate) => {
                                                setDateTo(newDate);
                                            }}
                                            renderInput={(params) => <TextField
                                                {...params}
                                                fullWidth
                                                id='dateTo'
                                                name='dateTo'
                                            />}
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
                                            key={dayInCenter[Object.keys(dayInCenter)[0]][0].timeSlotId}
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
                            name="chooseButton"
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
                            Anuluj wyszukiwanie
                        </Button>}
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { handleBack(navigate) }}
                            name="backButton2"
                        >
                            Powrót
                        </Button>
                    </Container>

                    <Dialog
                        fullScreen
                        open={openTimeSlotsDialog}
                        onClose={handleTimeSLotsClose}
                        TransitionComponent={Transition}
                    >
                        <AppBar sx={{ position: 'relative' }}>
                            <Toolbar>
                                <IconButton
                                    edge="start"
                                    color="inherit"
                                    onClick={handleTimeSLotsClose}
                                    aria-label="close"
                                >
                                    <CloseIcon />
                                </IconButton>
                                <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                                    {"Wybierz termin szczepienia"}
                                </Typography>
                                <Button name="backButton" autoFocus color="inherit" onClick={handleTimeSLotsClose}>
                                    Wróć
                                </Button>
                            </Toolbar>
                        </AppBar>
                        <List>
                            {dayTimeSlots &&
                                dayTimeSlots.map(dayTimeSlot =>
                                    <ListItemButton
                                        onClick={() => { handleTimeSlotChoice(dayTimeSlot) }}
                                        key={dayTimeSlot.id}
                                    >
                                        <ListItemText
                                            primary={"Lekarz " + dayTimeSlot.doctorFirstName + " " + dayTimeSlot.doctorLastName + ", " + getDayFromDate(dayTimeSlot.from) + ", " +
                                                getHoursMinsFromDate(dayTimeSlot.from) + " - " + getHoursMinsFromDate(dayTimeSlot.to)}
                                            secondary="" />
                                    </ListItemButton>
                                )}

                            <ListItem button>
                                <ListItemText primary=" " secondary="" />
                            </ListItem>
                            <Divider />
                        </List>
                    </Dialog>

                    <Dialog
                        fullWidth
                        open={openVaccinesDialog}
                    >
                        <DialogTitle>Wybierz szczepionkę</DialogTitle>
                        <DialogContent>
                            <Box
                                fullWidth
                                noValidate
                                component="form"
                                sx={{
                                    display: 'flex',
                                    flexDirection: 'column',
                                    m: 'auto',
                                    width: 'fit-content',
                                }}
                            >
                                {possibleVaccines && <TextField
                                    fullWidth
                                    id={'vaccines'}
                                    select
                                    label={'Wybierz szczepionkę'}
                                    name={'vaccines'}
                                    value={vaccine}
                                    onChange={handleChange}
                                    SelectProps={{
                                        native: true,
                                    }}
                                >
                                    {possibleVaccines.map((option) => (
                                        <option key={option.vaccineId} value={option.vaccineId}>
                                            {option.name + ", firma: " + option.company}
                                        </option>
                                    ))}
                                </TextField>}
                            </Box>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleVaccinesClose}>Wróc</Button>
                            <Button name="chooseVaccineButton" onClick={handleVaccineChoice}>Wybierz</Button>
                        </DialogActions>
                    </Dialog>

                    {timeSlot && vaccine && <Dialog
                        open={openConfirmDialog}
                        onClose={handleConfrimClose}
                        aria-labelledby="alert-dialog-title"
                        aria-describedby="alert-dialog-description"
                    >
                        <DialogTitle id="alert-dialog-title">
                            {"Potwierdź zapis"}
                        </DialogTitle>
                        <DialogContent>
                            <DialogContentText id="alert-dialog-description">
                                {"Czy na pewno chcesz zapisać się na szczepienie?"}
                            </DialogContentText>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleConfrimClose}>Nie</Button>
                            <Button name="confirmButton" onClick={bookTimeSlotTrue} autoFocus>
                                Tak
                            </Button>
                        </DialogActions>
                    </Dialog>}

                    <Snackbar
                        open={openSnackBar}
                        autoHideDuration={6000}
                        onClose={() => { setOpenSnackBar(false) }}
                        message={snackBarMessage}
                    />
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );

}

const Transition = React.forwardRef(function Transition(props, ref) {
    return <Slide direction="up" ref={ref} {...props} />;
});