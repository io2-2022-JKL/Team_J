import * as React from 'react';
import { Route } from "react-router-dom";
const PrivateRoute =
    ({ component: Component, ...rest }) => {
        return (
            <Route
                {...rest}
                render={props =>
                    //isAuthenticated() ? (
                    <Component {...props} />
                    //) : (
                    //    <Navigate
                    //        to={{
                    //            pathname: "/signin",
                    //            state: { from: props.location }
                    //        }}
                    //    />
                    //)
                }
            />
        )
    };
export default PrivateRoute;