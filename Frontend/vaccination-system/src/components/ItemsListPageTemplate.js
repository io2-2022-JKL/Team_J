import * as React from 'react';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { Box, Grid } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { FixedSizeList } from 'react-window';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import SummarizeIcon from '@mui/icons-material/Summarize';
import Avatar from '@mui/material/Avatar';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

const theme = createTheme();


export default function ItemListPageTemplate(title, data, renderRow, renderError, errorState, error, handleSnackBarClose, handleBack, loading) {

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
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
                            <SummarizeIcon />
                        </Avatar>
                        <Typography component="h1" variant='h5'>
                            {title}
                        </Typography>

                        <Box sx={{ marginTop: 2, }} />

                        <FixedSizeList
                            height={Math.min(window.innerHeight - 200, data.length * 100)}
                            width="70%"
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
                            onClick={handleBack}
                        >
                            Powrót
                        </Button>
                        {
                            loading &&
                            (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: blue,
                                        position: 'relative',
                                        alignSelf: 'center',
                                        left: '50%'
                                    }}
                                />
                            )
                        }
                    </Box>
                    <Snackbar open={errorState} autoHideDuration={6000} onClose={handleSnackBarClose}>
                        <Alert onClose={handleSnackBarClose} severity="error" sx={{ width: '100%' }}>
                            {
                                renderError(error)
                            }
                        </Alert>
                    </Snackbar>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}