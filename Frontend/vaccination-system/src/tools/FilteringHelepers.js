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
    static filterName = (array, filter) => {
        return array.filter((item) => item.name.includes(filter));
    };
    static filterNumberOfDoses = (array, filter) => {
        return array.filter((item) => {
            if(filter === '')
                return true;
            return item.numberOfDoses == filter;
        });
    };
   static filterMinDaysBetweenDoses = (array, filter) => {
        return array.filter((item) => {
            if(filter === '')
                return true;
            return item.minDaysBetweenDoses == filter
        });
    };
    static filterMaxDaysBetweenDoses = (array, filter) => {
        return array.filter((item) => {
            if(filter === '')
                return true;
            return item.maxDaysBetweenDoses == filter
        });
    };
    static filterVirus = (array, filter) => {
        return array.filter((item) => item.virus.includes(filter));
    };
    static filterCompany = (array, filter) => {
        return array.filter((item) => item.company.includes(filter));
    };
    static filterMinPatientAge = (array, filter) => {
        return array.filter((item) => {
            if(filter === '')
                return true;
            return item.minPatientAge == filter
        });
    };
    static filterMaxPatientAge = (array, filter) => {
        return array.filter((item) => {
            if(filter === '')
                return true;
            return item.maxPatientAge == filter
        });
    };
    static filterCity = (array, filter) => {
        return array.filter((item) => item.city.includes(filter));
    };
    static filterStreet = (array, filter) => {
        return array.filter((item) => item.street.includes(filter));
    };
    static filterVirus2 = (array, filter) => {
        return array.map(e => e.vaccines).filter((item) => item.virus.includes(filter));
    };
}
