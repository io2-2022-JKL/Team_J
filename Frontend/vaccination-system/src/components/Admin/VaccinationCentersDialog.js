import * as React from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';

export default function VaccinationCentersDialog(openDialog) {

    const [centers, setCenters] = React.useState()

    React.useEffect(() => {
        data = getVaccinationCenters()
        set
    });

    return (
        <Dialog
            fullWidth
            open={openDialog}
        >
            <DialogTitle>Wybierz Centrum Szczepień</DialogTitle>
            <DialogContent>
                <Box
                    noValidate
                    component="form"
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        m: 'auto',
                        width: 'fit-content',
                    }}
                >
                    {openDialog && <FormControl sx={{ mt: 2, minWidth: 120 }}>
                        <InputLabel htmlFor="max-width">Wybierz</InputLabel>
                        <Select
                            value={possibleVaccines[0].name}
                            autoFocus
                            onChange={handleVaccineChange}
                            label="maxWidth"
                        >
                            {possibleVaccines && possibleVaccines.map(possibleVaccine =>
                                <MenuItem value={possibleVaccine.name}>{possibleVaccine.name + ", firma: " + possibleVaccine.company}</MenuItem>)}
                        </Select>
                    </FormControl>}
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleVaccinesClose}>Wróc</Button>
                <Button onClick={handleVaccineChoice}>Wybierz</Button>
            </DialogActions>
        </Dialog>
    )
}