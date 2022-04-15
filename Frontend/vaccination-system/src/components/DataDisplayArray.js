import * as React from 'react';
import Box from '@mui/material/Box';
import { DataGrid } from '@mui/x-data-grid';
import LinearProgress from '@mui/material/LinearProgress';

export default function DataDisplayArray(
    props
) {
    const [pageSize, setPageSize] = React.useState(10);

    return (
        <Box
            sx={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                '& .active.positive': {
                    backgroundColor: 'rgba(157, 255, 118, 0.49)',
                    color: '#1a3e72',
                    fontWeight: '600',
                },
                '& .active.negative': {
                    backgroundColor: '#d47483',
                    color: '#1a3e72',
                    fontWeight: '600',
                },
            }}
        >
            <DataGrid
                onCellEditCommit={props.editCell}
                disableColumnFilter
                autoHeight
                pageSize={pageSize}
                onPageSizeChange={(newPageSize) => setPageSize(newPageSize)}
                rowsPerPageOptions={[5, 10, 15, 20]}
                pagination
                components={{
                    LoadingOverlay: LinearProgress,
                }}
                loading={props.loading}
                columns={props.columns}
                rows={props.filteredRows}
            />
        </Box>
    );
}