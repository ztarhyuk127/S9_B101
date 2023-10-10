# 📁포팅 메뉴얼

스프링 게임 서버, 데이터베이스, 게임 웹사이트 구동을 위한 서버 포팅 메뉴얼 입니다.

## 📌기술스택 버전

|기술 스택|버전| 
|:-|---|
|JDK|11|
|Springboot|2.7.1|
|MySQL|8.0.34|
|NginX|Latest|
|Docker|Latest|
|Unity|2022.3.7f1|

## 📌EC2 포트 번호
|기술 스택|포트| 
|:-|---|
|Front|80|
|Back|5000|
|MySQL|3306|

<br/>

# 📌 서버 빌드 및 배포

## 차례
1. Springboot 서버 빌드
2. EC2 도커 세팅
3. HTTPS를 위한 SSL 인증서 발급
4. 도커 Compose 구성 요소 작성
5. 도커 Compose 실행

## 📌 Springboot 서버 빌드
IDE : IntelliJ Ultimate

- #### 프로젝트 Clean
![Alt text](./images/image.png)

- #### 프로젝트 Build
![Alt text](./images/image-1.png)

- #### {Springboot 파일 경로}\build\libs의 jar 파일 확인
![Alt text](./images/image-2.png)

#### 

<br/>

## 📌 EC2 도커 세팅

- #### apt 업데이트 후 https 인증에 필요한 패키지를 설치
```bash
sudo apt-get update
sudo apt-get install ca-certificates curl gnupg
```
####

- #### 도커 오피셜 GPG 키를 받는다
```bash
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg
```
####

- #### Repository 확인
```bash
echo \
  "deb [arch="$(dpkg --print-architecture)" signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  "$(. /etc/os-release && echo "$VERSION_CODENAME")" stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
```
####

- #### 도커 설치
```bash
sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

### ⚠️Permission Denied 발생시

- `/var/run/docker.sock` 파일의 권한을 666 (R+W)으로 변경하여 그룹 내 다른 사용자도 접근 가능하게 변경
    
```bash
sudo chmod 666 /var/run/docker.sock
```
####

<br/>

## 📌 HTTPS를 위한 SSL 인증서 발급

- #### certbot 설치

```bash
sudo snap install certbot --classic # certbot 공식 페이지 권장 설치 방법
```

- #### standalone 방식으로 인증 실행

```bash
sudo certbot certonly --standalone
```
```
ubuntu@xxxxx:~$ sudo certbot certonly --standalone                                   
Saving debug log to /var/log/letsencrypt/letsencrypt.log
Plugins selected: Authenticator standalone, Installer None
Enter email address (used for urgent renewal and security notices)
 (Enter 'c' to cancel): {내 이메일}

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Please read the Terms of Service at
https://letsencrypt.org/documents/LE-SA-v1.2-November-15-2017.pdf. You must
agree in order to register with the ACME server. Do you agree?
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
(Y)es/(N)o: Y <- ACME 약관에 동의하는지 N 선택시 진행불가

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Would you be willing, once your first certificate is successfully issued, to
share your email address with the Electronic Frontier Foundation, a founding
partner of the Let's Encrypt project and the non-profit organization that
develops Certbot? We'd like to send you email about our work encrypting the web,
EFF news, campaigns, and ways to support digital freedom.
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
(Y)es/(N)o: N <- 이메일을 통해 Let's Encrypt 프로젝트 정보를 받아볼지 여부
Please enter the domain name(s) you would like on your certificate (comma and/or
space separated) (Enter 'c' to cancel): {도메인 주소}
Requesting a certificate for xxxx

Successfully received certificate.
Certificate is saved at: /etc/letsencrypt/live/{도메인 주소}/fullchain.pem
Key is saved at:         /etc/letsencrypt/live/{도메인 주소}/privkey.pem
This certificate expires on 2023-12-13.
These files will be updated when the certificate renews.
Certbot has set up a scheduled task to automatically renew this certificate in the background.
```

위와 같은 창이 뜬다면 인증 성공!

- #### 인증 정보 확인
```
sudo certbot certificates
```

![Alt text](./images/image-4.png)

## 📌 도커 Compose 파일 작성

### 파일 구조

```bash
docker # 그냥 폴더
├── docker-compose.yml
├── mysql
│   └── init.sql
├── nginx
│   ├── index.html
│   └── nginx.conf
└── spring
    ├── Dockerfile
    └── gameserver-0.0.1-SNAPSHOT.jar
```

> 1 문단에서 빌드한 spring 서버의 jar파일을 위 파일구조의 spring 폴더 내부에 복사

####

- #### EC2 port 오픈
```
ufw allow 80
ufw allow 443
```

####

- #### nginx/nginx.conf 작성
``` bash
user  nginx; # 프로세스의 실행되는 권한. 보안상 root를 사용하지 않습니다.

worker_processes  auto; # 워커 프로세스의 수, 보통 auto로 사용.

events {
    worker_connections  1024;   # 하나의 프로세스가 처리할 수 있는 연결 수.
}

http {

    # spring upstream 설정
    upstream spring {
        server webserver-spring:5000;
    }

    server {    # 웹 사이트 선언, server가 여러개이면 하나의 호스트에 여러 사이트를 서빙할 수 있음. -> 가상 호스트
        listen 80;
        server_name j9b101.p.ssafy.io;

        location / {
            # return 301 https://$host$request_uri;
            return 404;
        }
        
        location /.well-known/acme-challenge/ {
            root /var/www/letsencrypt;
        }
    }

    server {
        listen 443 ssl;
        server_name j9b101.p.ssafy.io;

        location / { # front-end reverse-proxy
            root /var/www; # root 폴더 지정
            index index.html; # 보여줄 html 파일
            try_files $uri $uri/ =404; # 파일이 없으면 404 에러 발생
        }

        location /api { # spinrg api reverse-proxy 
            proxy_pass         http://spring/api;
            proxy_redirect     off;
            proxy_set_header   Host $host;
            proxy_set_header   X-Real-IP $remote_addr;
            proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header   X-Forwarded-Host $server_name;
        }

        ssl_certificate /etc/nginx/certs/j9b101.p.ssafy.io/fullchain1.pem;
        ssl_certificate_key /etc/nginx/certs/j9b101.p.ssafy.io/privkey1.pem;
    }
}
```
####

- #### nginx/index.html 작성
``` html
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>onlyone games</title>
    </head>
    <body>
        <h1>Welcome to Team Onlyone Project!!</h1>
        <a
            href="{구글 드라이브 경로}"
        >
            <button>게임 다운로드!!</button>
        </a>
    </body>
</html>

```
> 게임 파일의 용량이 10GB를 초과하여 구글드라이브를 이용해 게임을 배포하기로 결정함

####

- #### springboot/Dockerfile 작성
``` Dockerfile
FROM adoptopenjdk/openjdk11:alpine-jre

COPY ./gameserver-0.0.1-SNAPSHOT.jar gameserver-0.0.1-SNAPSHOT.jar

EXPOSE 5000

ENTRYPOINT [ "java" ,"-jar", "gameserver-0.0.1-SNAPSHOT.jar"]
```

####

- #### mysql/init.sql 작성
``` SQL
# root 계정 외부 연결 차단 및 서버용 계정 생성
drop user  'root'@'%';

create user 'b101'@'%' identified by '---';
create user 'b101'@'localhost' identified by '---';

grant all privileges on *.* to 'b101'@'%';
grant all privileges on *.* to 'b101'@'localhost';

use onedb;

```

- #### docker-compose.yml 작성
``` yaml
version: "3"

services:
    nginx:
        container_name: nginx
        image: nginx
        ports:
            - "80:80"
            - "443:443"
        expose:
            - 80
            - 443
        volumes:
            - ./nginx/nginx.conf:/etc/nginx/nginx.conf
            - ./nginx/index.html:/var/www/index.html
            - /etc/letsencrypt/archive/:/etc/nginx/certs/
        depends_on:
            - webserver-spring
        networks:
            - game-network

    database-mysql:
        container_name: database-mysql
        image: mysql:8.0.34
        restart: always
        environment:
            MYSQL_ROOT_PASSWORD: 1234
            MYSQL_DATABASE: onedb # create database
            TZ: Asia/Seoul
        ports:
            - 3306:3306
        expose:
            - 3306
            - --character-set-server=utf8mb4
            - --collation-server=utf8mb4_unicode_ci
        volumes:
            # init sql
            - ./mysql/init.sql:/docker-entrypoint-initdb.d/init.sql
        networks:
            - game-network

    webserver-spring:
        container_name: webserver-spring
        # ports:
        #     - 5000:5000
        expose:
            - 5000
        build:
            context: ./spring
            dockerfile: Dockerfile
        restart: always
        environment:
            SPRING_DATASOURCE_URL: jdbc:mysql://database-mysql:3306/onedb?autoReconnect=true&serverTimezone=Asia/Seoul&useUnicode=true&characterEncoding=utf8
            SPRING_DATASOURCE_USERNAME: b101
            SPRING_DATASOURCE_PASSWORD: ---
        networks:
            - game-network
        depends_on:
            # below container must be excuted faster than this
            - database-mysql

networks:
    game-network:
        driver: bridge

```

<br/>

## 📌 도커 Compose 구동
#### 1. docker 폴더로 이동
``` bash
cd docker
```

#### 2. docker-compose.yml 구동
``` bash
docker compose up -d
```
![Alt text](./images/image-3.png)

#### 3. docker 실행 체크
``` bash
docker ps
```
![Alt text](./images/image-5.png)

#### 4. 홈페이지 및 API 접속 확인
![Alt text](./images/image-6.png)
![Alt text](./images/image-7.png)
