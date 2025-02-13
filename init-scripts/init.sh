#!/bin/bash -x
set -e
echo "begining of init"


psql -U shorty -d short_urls -a -f /docker-entrypoint-initdb.d/init_db.sql
echo "end of init"