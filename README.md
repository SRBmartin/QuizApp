# QuizApp
**Project setup:**
* Firstly, position yourself in `./docker/` folder and run `docker-compose up -d`, which is going to launch: MinIO, PostgreSQL, pgAdmin and init script for MinIO.
Run `docker ps` to see if containers are running.
* Position yourself in `./frontend/quizhub` folder and run `npm start`. You should see that the app is started on `http://localhost:3000/`.
* Open `./backend/QuizApp/QuizApp.sln` and start the project (properties.json is defining startup info). You should see opened swagger.

**You can start using the application.**