FROM adoptopenjdk/openjdk11:alpine-jre

COPY ./gameserver-0.0.1-SNAPSHOT.jar gameserver-0.0.1-SNAPSHOT.jar

EXPOSE 5000

ENTRYPOINT [ "java" ,"-jar", "gameserver-0.0.1-SNAPSHOT.jar"]