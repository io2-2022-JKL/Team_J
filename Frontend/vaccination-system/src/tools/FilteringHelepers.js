export default class FilteringHelepers {
    static filterFirstName(array, filter) {
        return array.filter((item) => item.firstName.includes(filter));
    };
    static filterLastName(array, filter) {
        return array.filter((item) => item.lastName.includes(filter));
    };
    static filterEmail(array, filter) {
        return array.filter((item) => item.mail.includes(filter));
    };
    static filterPhoneNumber(array, filter) {
        return array.filter((item) => item.phoneNumber.includes(filter));
    };
    static filterPESEL(array, filter) {
        return array.filter((item) => item.pesel.includes(filter));
    };
    static filterId(array, filter) {
        return array.filter((item) => item.id.includes(filter));
    };
    static filterDate(array, filter) {
        return array.filter((item) => {
            if (filter.length !== 10)
                return true;
            else
                return item.dateOfBirth === filter;
        });
    }
    static filterActive(array, filter) {
        return array.filter((item) => {
            if (filter === "aktywny")
                return item.active === true;
            else if (filter === "nieaktywny")
                return item.active === false;
            else
                return true;
        });
    }
}
