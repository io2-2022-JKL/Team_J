# Zespół J

## Postać danych przyjmowanych przez backend:
- **id** - postać Guid oddzielona myślnikami, np. `97274004-936b-4ff4-bf3b-bb0e05666e75`, w zapytaniach w body przekazujemy jako `string`;
- **PESEL** - w zapytaniach jako `string`, musi posiadać 11 symboli i musi być poprawnie parsowany przez `long.Parse(pesel)`;
- **FirstName** i **LastName** - `string`, przynajmniej 1 znak, nie może zawierać symboli: `!@#$%^&*()_+=[{]};:"\|<>,./`;
- **Password** - `string`, musi mieć przynajmniej 1 znak;
- **PhoneNumber** - jako `string`, może mieć maksymalnie 15 znaków, w zapytaniach może zawierać spacje, które nie liczą się do limitu znaku, może zaczynać się od '+' (liczy -się do limitu znaków), część poza '+' bez spacji musi się poprawnie konwertować przy użyciu `long.Parse(number)`;
- **Active** - `false` lub `true` (`boolean`);
- **DateOfBirth** - `string`, format `dd-MM-yyyy`, w bazie danych przechowywany jako `yyyy-MM-dd 00:00:00:0000000`;
- **OpeningHours** - lista dokładnie 7 elementów `{from, to}`;
- **From**, **To** - `string`, format `hh:mm`, trzeba pisać `08:00` a nie `8:00`, w bazie danych przechowywane jako `hh:mm:00.0000000`, chyba że jest to godzina `24:00`, którą przedstawiamy jako `23:59:59.9999999`;
- **NumberOfDoses** - musi wynosić przynajmniej 1;
- **MinDaysBetweenDoses** - jeśli jest ujemne, to znaczy, że można od razu przyjąć szczepionkę, w takim wypadku *MaxDaysBetweenDoses* może przyjąć dowolną wartość (też ujemną);
- **MaxDaysBetweenDoses** - jeśli jest ujemne, to znaczy, że nie ma granicy w dniach do której trzeba przyjąć szczepionkę, w takim wypadku *MinDaysBetweenDoses* może przyjąć dowolną wartość (też ujemną); 
(przy czym jeśli obie wartości są nieujemne, to *MinDaysBetweenDoses* nie może być większe niż *MaxDaysBetweenDoses*)
- **MinPatientAge** - jeśli jest ujemne, nie ma minimalnego wieku od którego można przyjąć szczepionkę, w takim wypadku *MaxPatientAge* może przyjąć dowolną wartość (też ujemną);
- **MaxPatientAge** - jeśli jest ujemne, nie ma maksymalnego wieku do którego można przyjąć szczepionkę, w takim wypadku *MinPatientAge* może przyjąć dowolną wartość (też ujemną);  
(przy czym jeśli obie wartości są nieujemne, to *MinPatientAge* nie może być większe niż *MaxPatientAge*)
- `TimeSlot` **from**, **to** - `string`, format `dd-MM-yyyy hh:mm`, w bazie danych przechowywane jako `yyyy-MM-dd hh:mm:00.0000000`, chyba że przedstawiamy godzinę `24:00`, wtedy `yyyy-MM-dd 23:59:59.9999999`;

Jeśli dane podane w `Delete` nie pozwalają na znalezienie rekordu, który ma status `Active == true`, zwracany jest błąd. Wyjątkiem jest `admin/doctors/timeSlots/deleteTimeSlots`, które zwraca błąd tylko, jeśli nie został usunięty żaden z podanych terminów (więc mogą się pojawić złe identyfikatory).

## Postać danych zwracanych przez backend (jeśli coś nie zostało wymienione w poprzednim dziale lub zupełnie się różni):
- **UserType** - `string`, przyjmuje wartości `doctor`, `patient` i `admin`;
- **PhoneNumber** - tak jak w postaci przyjmowanej przez backend, ale bez spacji;
