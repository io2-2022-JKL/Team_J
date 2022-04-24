# Obraz backendu
## Aby utworzyć obraz, trzeba zainstalować Dockera, a następnie:
- otwieramy konsolę, przechodzimy do `Backend/VaccinationSystem/VaccinationSystem/` na branchu `docker-deploy` (różni się od zwykłego `deploy` o `connection string` do bazy danych);
- wpisujemy `docker build -t [nazwa_tworzonego_obrazu] .`;
- uruchamiamy kontener (przed tym należy uruchomić kontener z lokalną bazą danych, opis w `vaccinationsystemdb`):
	- przez aplikację Dockera: images -> utworzony obraz -> run -> optional settings -> w local host wpisać port (np. 5001);
	- przez konsolę: `docker run -p [port_jaki_chcemy]:80 -d [nazwa_utworzonego_obrazu]`;
- w Dockerze w containers/apps widzimy nowy kontener (o losowej nazwie), wchodzimy w niego i widzimy konsolę;
- Backend niemal natychmiast włacza się;
- w przeglądarce wpisujemy `http://localhost:[podany_port]/swagger/index.html` żeby przetestować połączenie - powinien się wyświetlić zwykły swagger;
- odwołujemy się do enpointów poprzez: `http://localhost:5001/[endpoint]` (po HTTP!);
- mamy lokalny backend w dockerze z lokalną bazą danych!  

Możemy też użyć obrazu `docker pull jakubgazewski/io2-2022-j:backend` zamiast samemu go tworzyć lokalnie (bezpieczniej jednak utworzyć).