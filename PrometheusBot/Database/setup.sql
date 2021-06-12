CREATE USER 'PrometheusBot'@'localhost' identified by 'PrometheusBot';
GRANT ALL PRIVILEGES ON PrometheusDB.* TO 'PrometheusBot'@'localhost';
source Database.sql;