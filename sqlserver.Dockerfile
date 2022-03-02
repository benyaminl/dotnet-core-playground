# @see https://www.softwaredeveloper.blog/initialize-mssql-in-docker-container
FROM mcr.microsoft.com/mssql/server:2019-latest
USER root
RUN mkdir -p /usr/src/app/
COPY ./SQLDocker/sql-entrypoint.sh /usr/src/app/sql-entrypoint.sh
COPY ./SQLDocker/sql-init.sh /usr/src/app/sql-init.sh
RUN chmod +x /usr/src/app/sql-init.sh
RUN chmod +x /usr/src/app/sql-entrypoint.sh
USER mssql
WORKDIR /usr/src/app/
CMD /bin/bash ./sql-entrypoint.sh