FROM mysql/mysql-server:latest

# add setup files
ADD ["./SQL", "/docker-entrypoint-initdb.d"]

# environment variables
# - since this a test server, the password is a 'placeholder'
# - as we don't need security for this database
ENV MYSQL_ROOT_PASSWORD="r00tPw0rd!" \
    MYSQL_DATABASE="dev"
