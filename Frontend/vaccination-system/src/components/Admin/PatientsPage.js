import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import { DataGrid } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';

const theme = createTheme();

const columns = [
    {
        field: 'id',
        flex: 1
    },
    {
        field: 'PESEL',
        minWidth: 110,
        flex: 0.5,
        editable: true
    },
    {
        field: 'firstName',
        headerName: 'Imię',
        editable: true
    },
    {
        field: 'lastName',
        headerName: 'Nazwisko',
        editable: true
    },
    {
        field: 'mail',
        headerName: 'E-Mail',
        minWidth: 125,
        flex: 0.5,
        editable: true
    },
    {
        field: 'dateOfBirth',
        headerName: 'Data urodzenia',
        flex: 0.5,
        editable: true
    },
    {
        field: 'phoneNumber',
        headerName: 'Numer telefonu',
        width: 125,
        editable: true
    },
    {
        field: 'active',
        headerName: 'Aktywny',
        type: Boolean,
        editable: true
    }
];

let id = randomId();
const createRandomRow = () => {
    id = randomId();
    return {
        id: id,
        PESEL: randomInt(10000000000, 100000000000).toString(),
        firstName: randomTraderName().split(' ')[0],
        lastName: randomTraderName().split(' ')[1],
        mail: randomEmail(),
        dateOfBirth: dateFormat(randomDate(new Date(50, 1), new Date("1/1/30")), "isoDate").toString(),
        phoneNumber: randomPhoneNumber(),
        active: randomBoolean()
    }
};

export default function PatientsPage() {
    const navigate = useNavigate();
    const [pageSize, setPageSize] = React.useState(15);

    const [rows, setRows] = React.useState(() => [
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
                        <div style={{height:400,width: '100%' }}>
                            <DataGrid
                                
                                pageSize={pageSize}
                                onPageSizeChange={(newPageSize) => setPageSize(newPageSize)}
                                rowsPerPageOptions={[5, 10, 15, 20]}
                                pagination
                                onClick={() => console.log('test')}
                                hideFooter
                                columns={columns}
                                rows={rows}
                                disableColumnMenu
                                
                                componentsProps={{
                                    pagination: { classes: null }
                                  }}
                                  
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