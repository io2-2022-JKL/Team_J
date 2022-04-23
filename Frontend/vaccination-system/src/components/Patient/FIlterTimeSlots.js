import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';
import { confirm } from "react-confirm-box";
import { HomeOutlined, PlayCircleFilledWhiteRounded } from '@mui/icons-material';
import axios from 'axios';
import LinearProgress from '@mui/material/LinearProgress';
import DataDisplayArray from '../DataDisplayArray';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ImageIcon from '@mui/icons-material/Image';
import WorkIcon from '@mui/icons-material/Work';
import HomeIcon from '@mui/icons-material/Home';
import BeachAccessIcon from '@mui/icons-material/BeachAccess';

const theme = createTheme();

export default function FilterTimeSlots() {

    const navigate = useNavigate();
    const [showDaysList, setShowDaysList] = React.useState(false);

    const handleSubmit = (event) => {
        console.log(localStorage.getItem('userID'))

    };



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
                            //onSubmit={handleSubmit}
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                                //flexDirection: 'row',
                                //alignItems: 'center'
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
                                <List sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}>
                                    <ListItem>
                                        <ListItemAvatar>
                                            <Avatar>
                                                <HomeIcon />
                                            </Avatar>
                                        </ListItemAvatar>
                                        <ListItemText primary="Centrum szczepień 1" secondary="Jan 9, 2014" />
                                    </ListItem>
                                    <ListItem>
                                        <ListItemAvatar>
                                            <Avatar>
                                                <HomeIcon />
                                            </Avatar>
                                        </ListItemAvatar>
                                        <ListItemText primary="Centrum szczepień 1" secondary="Jan 7, 2014" />
                                    </ListItem>
                                </List>
                            </Box>
                        }
                        {!showDaysList && <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { setShowDaysList(true) }}
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