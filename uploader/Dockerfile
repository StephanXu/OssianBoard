FROM maven:3.6.1-jdk-8-alpine AS MAVEN_BUILD
COPY . . 
RUN mvn clean package 

FROM openjdk:8-jre-alpine3.9
COPY --from=MAVEN_BUILD target/forward-0.0.1-SNAPSHOT.jar /forward.jar
CMD ["java", "-jar", "/forward.jar"]
EXPOSE 8080
