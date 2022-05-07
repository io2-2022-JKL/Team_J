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

    static validateFirstName(e, setFirstNameError, setFirstNameErrorState) {
        var name = e.target.value;
        const substrings = ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '[', '{', ']', '}', ';', ':', '"', '\\', '|', '<', '>', ',', '.', '/']
        if (name.length > 0 && !substrings.some(c => name.includes(c))) {
            setFirstNameError('')
            setFirstNameErrorState(false)
        }
        else {
            setFirstNameError('Imię nie może zawierać symboli !@#$%^&*()_+=[{]};:"\|<>,./ i musi mieć przynajmniej jeden znak!')
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
            setLastNameError('Nazwisko nie może zawierać symboli !@#$%^&*()_+=[{]};:"|<>,./\\ i musi mieć przynajmniej jeden znak!')
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
}