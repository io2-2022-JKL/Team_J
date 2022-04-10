import * as React from 'react';
import { Navigate, Route } from "react-router-dom";
export default function PrivateRoute({ children, ...rest }) {
    return (
        <Route
            {...rest}
            render={
                ({ location }) => (
                    true
                        ? (
                            children
                        ) : (
                            null
                        ))
            }
        />
    );
}