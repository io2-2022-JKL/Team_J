import axios from "axios"

export default class LoginHelpers{
    static logOut() {
        delete axios.defaults.headers.common["authorization"]
    }
    static preventGoingBack() {
        window.history.pushState(null, document.title, window.location.href);
        window.addEventListener('popstate', function (event) {
            window.history.pushState(null, document.title, window.location.href);
        });
    }
}