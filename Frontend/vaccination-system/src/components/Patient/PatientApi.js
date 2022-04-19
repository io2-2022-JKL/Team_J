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
import { PlayCircleFilledWhiteRounded } from '@mui/icons-material';
import axios from 'axios';
import LinearProgress from '@mui/material/LinearProgress';
import DataDisplayArray from '../DataDisplayArray';

export async function getFreeTimeSlots() {
    try {
        const { data: response } = await axios.get('https://localhost:5001/admin/patients');
        return response;
    } catch (error) {
        console.error(error.message);
    }
}
