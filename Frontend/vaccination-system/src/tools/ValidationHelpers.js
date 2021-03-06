import validator from 'validator';

export default class ValidationHelpers {
    static validateEmail(e, setErrorMessage, setErrorState) {
        var email = e.target.value

        if (validator.isEmail(email)) {
            setErrorMessage('')
            setErrorState(false)
        } else {
            setErrorMessage('Wprowadź poprawny adres email!')
            setErrorState(true)
        }
    }

    static validatePESEL(e, setPeselError, setPeselErrorState) {
        var pesel = e.target.value;
        if (pesel.length === 11 && validator.isNumeric(pesel)) {
            setPeselError('')
            setPeselErrorState(false)
        }
        else {
            setPeselError('Wprowadź poprawny PESEL!')
            setPeselErrorState(true)
        }
    }

    static validateName(e, setFirstNameError, setFirstNameErrorState) {
        var name = e.target.value;
        const substrings = ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '[', '{', ']', '}', ';', ':', '"', '\\', '|', '<', '>', ',', '.', '/']
        if (name.length > 0 && !substrings.some(c => name.includes(c))) {
            setFirstNameError('')
            setFirstNameErrorState(false)
        }
        else {
            setFirstNameError('Pole nie może zawierać symboli !@#$%^&*()_+=[{]};:"\|<>,./ i musi mieć przynajmniej jeden znak!')
            setFirstNameErrorState(true)
        }
    }

    static validateLastName(e, setLastNameError, setLastNameErrorState) {
        var name = e.target.value;
        const substrings = ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '[', '{', ']', '}', ';', ':', '"', '\\', '|', '<', '>', ',', '.', '/']
        if (name.length > 0 && !substrings.some(c => name.includes(c))) {
            setLastNameError('')
            setLastNameErrorState(false)
        }
        else {
            setLastNameError('Pole nie może zawierać symboli !@#$%^&*()_+=[{]};:"|<>,./\\ i musi mieć przynajmniej jeden znak!')
            setLastNameErrorState(true)
        }
    }

    static validatePassword(e, setPasswordError, setPasswordErrorState) {
        var password1 = e.target.value;
        if (password1.length > 0) {
            setPasswordError('')
            setPasswordErrorState(false)
        }
        else {
            setPasswordError('Hasło musi mieć co najmniej 1 znak')
            setPasswordErrorState(true)
        }
    }

    static validateRepeatPassword(e, setPassword2Error, setPassword2ErrorState, password) {
        var password2 = e.target.value;
        if (validator.equals(password2, password)) {
            setPassword2Error('')
            setPassword2ErrorState(false)
        }
        else {
            setPassword2Error('Hasła się nie pokrywają ze sobą')
            setPassword2ErrorState(true)
        }
    }

    static validatePhoneNumber(e, setPhoneNumberError, setPhoneNumberErrorState) {
        var number = e.target.value;
        var phoneNumberWithoutSpaces = number.replace(/ /g, "");
        var regEx = /^[+]?[0-9]*$/
        if (number.length > 0 && phoneNumberWithoutSpaces.length <= 15 && regEx.test(phoneNumberWithoutSpaces)) {
            setPhoneNumberError('');
            setPhoneNumberErrorState(false);
        }
        else {
            setPhoneNumberError('Niepoprawny numer telefonu!');
            setPhoneNumberErrorState(true);
        }
    }

    static validateInt(e, setError, setErrorState) {
        var integer = e.target.value;
        if (validator.isNumeric(integer)) {
            setError('');
            setErrorState(false);
        }
        else {
            setError("Podana wartość nie jest liczbą!");
            setErrorState(true);
        }
    }
    static validateNumberOfDoses(e, setError, setErrorState) {
        var integer = e.target.value;
        if (validator.isNumeric(integer) && Number.parseInt(integer) > 0) {
            setError('');
            setErrorState(false);
        }
        else {
            setError("Minimalna dawka wynosi 1!");
            setErrorState(true);
        }
    }

    static validateDate(e, setError, setErrorState) {
        var date = e.target.value;
        var regEx = /[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]/
        if (regEx.test(date)) {
            setError('');
            setErrorState(false);
        }
        else {
            setError('Niepoprawny format daty! Oczekiwany format: dd-MM-yyyy');
            setErrorState(true);
        }
    }

    static validateOpeningHours(e, setError, setErrorState) {
        var date = e.target.value;
        var regEx = /[0-9][0-9]:[0-9][0-9] - [0-9][0-9]:[0-9][0-9]/
        if (regEx.test(date)) {
            setError('');
            setErrorState(false);
        }
        else {
            setError('Nieprawidłowe dane! Oczekiwany format: hh:mm - hh:mm');
            setErrorState(true);
        }
    }
}