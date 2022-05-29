import { TextField } from "@mui/material"
import React from "react"

export default function DropDownSelect(name, label, options, selectedOption, setOption) {
    const handleChange = (event) => {
        setOption(event.target.value);
    };

    return <TextField
        fullWidth
        id={name}
        select
        label={label}
        name={name}
        value={selectedOption}
        onChange={handleChange}
        SelectProps={{
            native: true,
        }}
    >
        {options.map((option) => (
            <option key={option.value} value={option.value}>
                {option.label}
            </option>
        ))}
    </TextField>
}