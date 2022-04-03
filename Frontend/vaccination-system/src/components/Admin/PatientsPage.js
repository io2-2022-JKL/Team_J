import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import { DataGrid } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName,randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';

const theme = createTheme();
const columns = [
    {
        field: 'id',
    },
    {
        field: 'PESEL',
        width: 110,
    },
    {
        field: 'firstName',
        headerName: 'Imię'
    },
    {
        field: 'lastName',
        headerName: 'Nazwisko'
    },
    {
        field: 'mail',
        headerName: 'E-Mail',
        width: 125
    },
    {
        field: 'dateOfBirth',
        headerName: 'Data urodzenia'
    },
    {
        field: 'phoneNumber',
        headerName: 'Numer telefonu',
        width: 125
    },
    {
        field: 'active',
        headerName: 'Aktywny',
        type: Boolean
    }
];

let id = randomId();
const createRandomRow = () => {
    id = randomId();
    return {
        id: id,
        PESEL: randomInt(10000000000,100000000000).toString(),
        firstName: randomTraderName().split(' ')[0],
        lastName: randomTraderName().split(' ')[1],
        mail: randomEmail(),
        dateOfBirth: dateFormat(randomDate(new Date(50,1),new Date("1/1/30")),"isoDate").toString(),
        phoneNumber: randomPhoneNumber(),
        active: randomBoolean()
    }
};

export default function PatientsPage() {
    const navigate = useNavigate();
    
    const [rows, setRows] = React.useState(() => [
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
    ]);
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
                            Pacjenci
                        </Typography>
                        <div style={{ height: 250, width: '100%' }}>
                            <DataGrid
                                hideFooter
                                columns={columns}
                                rows={rows}
                            />
                        </div>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => { navigate("/admin") }}
                        >
                            Powrót
                        </Button>
                    </Box>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}